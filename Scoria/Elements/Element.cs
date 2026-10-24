using Scoria.Events;
using Scoria.Layout;

namespace Scoria.Elements;

/// <summary>
/// Base class for all visual elements in the layout tree.
/// </summary>
public abstract class Element
{
    // TODO: Make a way to hide some elements so .Parent skips the element and goes to the next and .GetChildren skips them too.
    private readonly List<Element> _children = new List<Element>();

    /// <summary>Initializes a new instance of the <see cref="Element"/> class.</summary>
    public Element()
    {
        Events = new EventRouter(this);
    }

    protected Pos XDefault { get; set; } = Pos.Center();
    protected Pos YDefault { get; set; } = Pos.Center();
    protected Size WidthDefault { get; set; } = Size.Fill();
    protected Size HeightDefault { get; set; } = Size.Fill();

    /// <summary>Horizontal position of this element within its parent.</summary>
    public Pos X
    {
        get;
        set
        {
            field = value;
            SetNeedsCalculateLayout();
        }
    } = Pos.Auto();
    /// <summary>Vertical position of this element within its parent.</summary>
    public Pos Y
    {
        get;
        set
        {
            field = value;
            SetNeedsCalculateLayout();
        }
    } = Pos.Auto();
    /// <summary>Width of this element.</summary>
    public Size Width
    {
        get;
        set
        {
            field = value;
            SetNeedsCalculateLayout();
        }
    } = Size.Auto();
    /// <summary>Height of this element.</summary>
    public Size Height
    {
        get;
        set
        {
            field = value;
            SetNeedsCalculateLayout();
        }
    } = Size.Auto();

    /// <summary>The parent element in the layout tree, or <see langword="null"/> if this is a root element.</summary>
    public Element? Parent { get; private set; } = null;
    /// <summary>The resolved layout values computed by the layout solver.</summary>
    protected internal CalculatedLayout CalculatedLayout { get; } = new();

    /// <summary>Gets the event router used to subscribe to and dispatch events for this element.</summary>
    public EventRouter Events { get; }

    /// <summary>Flags this element (and its ancestors) as needing layout recalculation.</summary>
    public virtual void SetNeedsCalculateLayout()
    {
        Parent?.SetNeedsCalculateLayout();
    }

    /// <summary>Flags this element (and its ancestors) as needing to be re-rendered.</summary>
    public virtual void SetNeedsRender()
    {
        Parent?.SetNeedsRender();
    }

    /// <summary>
    /// Adds a child element to this element.
    /// </summary>
    /// <param name="element">The element to add as a child. Must not already have a parent.</param>
    /// <exception cref="Exception">Thrown if <paramref name="element"/> already has a parent.</exception>
    public virtual void AddChild(Element element)
    {
        if (element.Parent is not null)
        {
            throw new Exception("Must remove element from parent before it can be added to a new element.");
        }

        if (!SupportsChild(element))
        {
            throw new NotSupportedException($"{this} cannot contain child {element}");
        }

        if (!element.SupportsParent(this))
        {
            throw new NotSupportedException($"{this} is not a valid parent for {element}");
        }

        element.Parent = this;
        _children.Add(element);
    }

    /// <summary>
    /// Removes a child element from this element.
    /// </summary>
    /// <param name="element">The child element to remove.</param>
    public virtual void RemoveChild(Element element)
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
    /// <returns>An IEnumerable of child elements.</returns>
    public IEnumerable<Element> GetChildren()
    {
        return _children;
    }

    protected virtual bool SupportsParent(Element? parent)
    {
        return true;
    }

    protected virtual bool SupportsChild(Element element)
    {
        return true;
    }

    /// <summary>
    /// Renders this element and its children to the given surface.
    /// </summary>
    /// <param name="surface">The surface to render to.</param>
    protected virtual void Render(ISurface surface)
    {
        foreach (Element child in GetChildren())
        {
            child.Render(surface);
        }
    }

    protected internal virtual List<LayoutProperty> ResolveAutoLayoutDependencies(LayoutPropertyType type)
    {
        return GetDefaultLayoutResolver(type).GetDependencies(new LayoutProperty(type, this));
    }

    protected internal virtual int ResolveAutoLayout(LayoutPropertyType type, ref ReadOnlySpan<int> dependencies)
    {
        return GetDefaultLayoutResolver(type).Resolve(new LayoutProperty(type, this), ref dependencies);
    }

    private ILayoutResolver GetDefaultLayoutResolver(LayoutPropertyType propertyType) => propertyType switch
    {
        LayoutPropertyType.X => XDefault,
        LayoutPropertyType.Y => YDefault,
        LayoutPropertyType.Width => WidthDefault,
        LayoutPropertyType.Height => HeightDefault,
        _ => throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, null)
    };
}
