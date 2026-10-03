using Scoria.Layout;

namespace Scoria.Elements;

/// <summary>
/// The root element of an application. Renders a themed background with a border and
/// optional title, and owns the backing surface that is presented to the console.
/// </summary>
public sealed class Window : Element
{
    private bool NeedsCalculateLayout { get; set; }
    private bool NeedsRender { get; set; }

    private Surface _surface;

    /// <summary>Initializes a new empty <see cref="Window"/> using the default theme.</summary>
    public Window()
    {
        _surface = new Surface(0, 0, Theme);
        XDefault = Pos.Abs(0);
        YDefault = Pos.Abs(0);
    }

    /// <summary>Gets or sets the theme used to render this window and inherited by its surface.</summary>
    public Theme Theme
    {
        get;
        set
        {
            field = value;
            SetNeedsRender();
        }
    } = Theme.Default;

    /// <summary>Gets or sets the title displayed in the window's top border.</summary>
    public string Title
    {
        get;
        set
        {
            field = value;
            SetNeedsRender();
        }
    } = "";

    protected override bool SupportsParent(Element? parent)
    {
        return false;
    }

    /// <summary>Flags the window for layout recalculation and re-render on the next frame.</summary>
    public override void SetNeedsCalculateLayout()
    {
        NeedsCalculateLayout = true;
        SetNeedsRender();
    }

    /// <summary>Flags the window for re-render on the next frame.</summary>
    public override void SetNeedsRender()
    {
        NeedsRender = true;
    }

    internal Surface GetSurface(int width, int height)
    {
        if (_surface.Width != width)
        {
            Width = Size.Abs(width);
        }

        if (_surface.Height != height)
        {
            Height = Size.Abs(height);
        }

        if (NeedsCalculateLayout)
        {
            NeedsCalculateLayout = false;
            LayoutSolver.Solve(this, true);
        }

        if (NeedsRender)
        {
            NeedsRender = false;
            _surface = new Surface(width, height, Theme);
            Render(_surface);
        }

        return _surface;
    }

    protected override void Render(ISurface surface)
    {
        surface.Fill(' ', Theme.Background);
        surface.Borders(Title, false);
        base.Render(surface);
        surface.ExpandBorders();
    }
}
