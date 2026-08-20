using Scoria.Layout;

namespace Scoria.Elements;

public sealed class Window : Element
{
    private bool NeedsCalculateLayout { get; set; }
    private bool NeedsRender { get; set; }

    internal Surface Surface = new Surface(0, 0);

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
        if (Surface.Width != width)
        {
            Width = Size.Abs(width);
        }

        if (Surface.Height != height)
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
            Surface = new Surface(width, height);
            Render(Surface);
        }

        return Surface;
    }

    protected override void Render(ISurface surface)
    {
        surface.Fill(' ', new Style(255, 255, 255, 20, 20, 30));
        surface.Borders(Title, false);
        base.Render(surface);
        surface.ExpandBorders();
    }
}