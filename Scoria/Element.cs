namespace Scoria;

public abstract class Element
{
    private readonly List<Element> _children = new List<Element>();
    
    // TODO: Event system
    // TODO: Hierarchical theme system
    // TODO Layout system
    
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public void AddChild(Element element)
    {
        _children.Add(element);
    }

    public void RemoveChild(Element element)
    {
        _children.Remove(element);
    }

    public Element GetChild(Index index)
    {
        return _children[index];
    }

    public IEnumerable<Element> GetChildren()
    {
        return _children;
    }
    
    public virtual void Render(ISurface surface)
    {
        foreach (Element child in GetChildren())
        {
            child.Render(surface);
        }
    }
}

public sealed class PanelElement : Element
{
    public string Title { get; set; }
    
    public override void Render(ISurface surface)
    {
        surface.SubSurface(X, Y, Width, Height).Borders(Title, thin: false);
        base.Render(surface);
    }
}

public sealed class TextElement : Element
{
    public Style Style { get; set; }
    public string Text { get; set; }
    
    public override void Render(ISurface surface)
    {
        base.Render(surface);
        for (int i = 0; i < Text.Length; i++)
        {
            surface.Write(Text[i], X + i, Y, Style);
        }
    }
}