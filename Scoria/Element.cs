using Scoria.Events;
using Scoria.Layout;

namespace Scoria;

/// <summary>
/// Base class for all visual elements in the layout tree.
/// </summary>
public abstract class Element
{
    private readonly List<Element> _children = new List<Element>();

    public Element()
    {
        Events = new EventRouter(this);
    }

    // TODO: Event system
    // TODO: Hierarchical theme system
    // TODO Layout system

    /// <summary>Horizontal position of this element within its parent.</summary>
    public Pos X { get; set; } = Pos.Auto();
    /// <summary>Vertical position of this element within its parent.</summary>
    public Pos Y { get; set; } = Pos.Auto();
    /// <summary>Width of this element.</summary>
    public Size Width { get; set; } = Size.Auto();
    /// <summary>Height of this element.</summary>
    public Size Height { get; set; } = Size.Auto();
    /// <summary>The parent element in the layout tree, or <see langword="null"/> if this is a root element.</summary>
    public Element? Parent { get; set; } = null;
    /// <summary>The resolved layout values computed by the layout solver.</summary>
    protected internal CalculatedLayout CalculatedLayout { get; } = new();

    public EventRouter Events { get; }

    /// <summary>
    /// Adds a child element to this element.
    /// </summary>
    /// <param name="element">The element to add as a child. Must not already have a parent.</param>
    /// <exception cref="Exception">Thrown if <paramref name="element"/> already has a parent.</exception>
    public void AddChild(Element element)
    {
        if (element.Parent is not null)
        {
            throw new Exception("Must remove element from parent before it can be added to a new element.");
        }

        element.Parent = this;
        _children.Add(element);
    }

    /// <summary>
    /// Removes a child element from this element.
    /// </summary>
    /// <param name="element">The child element to remove.</param>
    public void RemoveChild(Element element)
    {
        _children.Remove(element);
        element.Parent = null;
    }

    /// <summary>
    /// Gets a child element by index.
    /// </summary>
    /// <param name="index">The zero-based index of the child element to retrieve.</param>
    /// <returns>The child element at the specified index.</returns>
    public Element GetChild(Index index)
    {
        return _children[index];
    }

    /// <summary>
    /// Gets all child elements of this element.
    /// </summary>
    /// <returns>An enumerable of child elements.</returns>
    public IEnumerable<Element> GetChildren()
    {
        return _children;
    }
    
    /// <summary>
    /// Renders this element and its children to the given surface.
    /// </summary>
    /// <param name="surface">The surface to render to.</param>
    public virtual void Render(ISurface surface)
    {
        foreach (Element child in GetChildren())
        {
            child.Render(surface);
        }
    }

    protected internal virtual List<LayoutProperty> ResolveAutoLayoutDependencies(LayoutPropertyType layoutPropertyType)
    {
        return [];
    }

    protected internal virtual int ResolveAutoLayout(LayoutPropertyType propertyType, List<int> dependencies)
    {
        return 0;
    }
}

/// <summary>
/// A container element that renders a bordered panel with an optional title.
/// </summary>
public sealed class PanelElement : Element
{
    /// <summary>The title displayed in the panel border.</summary>
    public string Title { get; set; } = string.Empty;

    public bool ThinBorders = false;
    
    /// <inheritdoc/>
    public override void Render(ISurface surface)
    {
        surface.SubSurface(CalculatedLayout.X, CalculatedLayout.Y, CalculatedLayout.Width, CalculatedLayout.Height).Borders(Title, thin: ThinBorders);
        base.Render(surface);
    }
}

/// <summary>
/// An element that renders a single line of styled text.
/// </summary>
public sealed class TextElement : Element
{
    /// <summary>The visual style applied to the text.</summary>
    public Style Style { get; set; } = default;
    /// <summary>The text content to render.</summary>
    public string Text { get; set; } = string.Empty;
    
    // TODO: Text element is missing a lot of features, like text wrapping, proper size detection etc.
    /// <inheritdoc/>
    public override void Render(ISurface surface)
    {
        for (int i = 0; i < Text.Length; i++)
        {
            surface.Write(Text[i], CalculatedLayout.X + i, CalculatedLayout.Y, Style);
        }
        base.Render(surface);
    }

    protected internal override List<LayoutProperty> ResolveAutoLayoutDependencies(LayoutPropertyType layoutPropertyType)
    {
        if (layoutPropertyType == LayoutPropertyType.Width)
            return [new LayoutProperty(LayoutPropertyType.Width, Parent)];
        if (layoutPropertyType == LayoutPropertyType.Height)
            return [new(LayoutPropertyType.Width, this)];
        return [];
    }

    protected internal override int ResolveAutoLayout(LayoutPropertyType propertyType, List<int> dependencies)
    {
        if (propertyType == LayoutPropertyType.Width)
            return int.Min(dependencies[0], Text.Length);
        if (propertyType == LayoutPropertyType.Height)
        {
            int width = dependencies[0];
            // Integer division with rounding up.
            return (Text.Length + width - 1) / width;
        }

        return 0;
    }
}