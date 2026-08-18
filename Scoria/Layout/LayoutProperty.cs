namespace Scoria.Layout;

/// <summary>
/// Identifies a layout property on an element (position or size along an axis).
/// </summary>
public enum LayoutPropertyType
{
    /// <summary>Horizontal position.</summary>
    X,
    /// <summary>Vertical position.</summary>
    Y,
    /// <summary>Width.</summary>
    Width,
    /// <summary>Height.</summary>
    Height,
}

internal interface ILayoutResolver
{
    internal List<LayoutProperty> GetDependencies(LayoutProperty layoutProperty);

    internal int Resolve(LayoutProperty layoutProperty, List<int> dependencies);
}

/// <summary>
/// Represents a reference to a specific layout property (position or size) on an element.
/// </summary>
public sealed record LayoutProperty(LayoutPropertyType Type, Element Element)
{
    internal ILayoutResolver GetProperty() => Type switch
    {
        LayoutPropertyType.X => Element.X,
        LayoutPropertyType.Y => Element.Y,
        LayoutPropertyType.Width => Element.Width,
        LayoutPropertyType.Height => Element.Height,
        _ => throw new ArgumentOutOfRangeException()
    };

    /// <summary>Returns a string representation of this layout property reference.</summary>
    public override string ToString()
    {
        return $"{Element}.{Type}";
    }
}

/// <summary>
/// Extension methods for mapping between related layout property types.
/// </summary>
public static class LayoutPropertyTypeExtensions
{
    extension(LayoutPropertyType layoutPropertyType)
    {
        /// <summary>
        /// Returns the size property on the same axis (e.g. <see cref="LayoutPropertyType.X"/> returns <see cref="LayoutPropertyType.Width"/>).
        /// </summary>
        public LayoutPropertyType SameAxisSize() => layoutPropertyType switch
        {
            LayoutPropertyType.X => LayoutPropertyType.Width,
            LayoutPropertyType.Y => LayoutPropertyType.Height,
            LayoutPropertyType.Width => LayoutPropertyType.Width,
            LayoutPropertyType.Height => LayoutPropertyType.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(layoutPropertyType), layoutPropertyType, null)
        };
        /// <summary>
        /// Returns the position property on the same axis (e.g. <see cref="LayoutPropertyType.Width"/> returns <see cref="LayoutPropertyType.X"/>).
        /// </summary>
        public LayoutPropertyType SameAxisPosition() => layoutPropertyType switch
        {
            LayoutPropertyType.X => LayoutPropertyType.X,
            LayoutPropertyType.Y => LayoutPropertyType.Y,
            LayoutPropertyType.Width => LayoutPropertyType.X,
            LayoutPropertyType.Height => LayoutPropertyType.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(layoutPropertyType), layoutPropertyType, null)
        };
        /// <summary>
        /// Returns the corresponding property on the other axis (e.g. <see cref="LayoutPropertyType.X"/> returns <see cref="LayoutPropertyType.Y"/>).
        /// </summary>
        public LayoutPropertyType OtherAxis() => layoutPropertyType switch
        {
            LayoutPropertyType.X => LayoutPropertyType.Y,
            LayoutPropertyType.Y => LayoutPropertyType.X,
            LayoutPropertyType.Width => LayoutPropertyType.Height,
            LayoutPropertyType.Height => LayoutPropertyType.Width,
            _ => throw new ArgumentOutOfRangeException(nameof(layoutPropertyType), layoutPropertyType, null)
        };
    }
}