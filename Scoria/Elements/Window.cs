using Scoria.Layout;

namespace Scoria.Elements;

public sealed class Window : Element
{
    private bool NeedsCalculateLayout { get; set; }
    private bool NeedsRender { get; set; }

    private Surface _surface;

    public Window()
    {
        _surface = new Surface(0, 0, Theme);
    }

    public Theme Theme { get; set; } = Theme.Default;
    
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

    public override void SetNeedsCalculateLayout()
    {
        NeedsCalculateLayout = true;
    }

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