using Scoria.Layout;

namespace Scoria;

public abstract class Element
{
    private readonly List<Element> _children = new List<Element>();
    
    // TODO: Event system
    // TODO: Hierarchical theme system
    // TODO Layout system

    public Pos X { get; set; } = Pos.Center();
    public Pos Y { get; set; } = Pos.Center();
    public Size Width { get; set; } = Size.Fill();
    public Size Height { get; set; } = Size.Fill();
    protected internal CalculatedLayout CalculatedLayout { get; } = new();
    public Element? Parent { get; set; } = null;

    public void AddChild(Element element)
    {
        if (element.Parent is not null)
        {
            throw new Exception("Must remove element from parent before it can be added to a new element.");
        }

        element.Parent = this;
        _children.Add(element);
    }

    public void RemoveChild(Element element)
    {
        _children.Remove(element);
        element.Parent = null;
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
    public string Title { get; set; } = string.Empty;
    
    public override void Render(ISurface surface)
    {
        surface.SubSurface(CalculatedLayout.X, CalculatedLayout.Y, CalculatedLayout.Width, CalculatedLayout.Height).Borders(Title, thin: false);
        base.Render(surface);
    }
}

public sealed class TextElement : Element
{
    public Style Style { get; set; } = default;
    public string Text { get; set; } = string.Empty;
    
    // TODO: Text element is missing a lot of features, like text wrapping, proper size detection etc.
    public override void Render(ISurface surface)
    {
        for (int i = 0; i < Text.Length; i++)
        {
            surface.Write(Text[i], CalculatedLayout.X + i, CalculatedLayout.Y, Style);
        }
        base.Render(surface);
    }
}