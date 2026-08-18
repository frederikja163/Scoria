namespace Scoria.Layout;

public enum LayoutPropertyType
{
    X,
    Y,
    Width,
    Height,
}

internal interface ILayoutResolver
{
    internal List<LayoutProperty> GetDependencies(LayoutProperty layoutProperty);

    internal int Resolve(LayoutProperty layoutProperty, List<int> dependencies);
}

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

    public override string ToString()
    {
        return $"{Element}.{Type}";
    }
}

public static class LayoutPropertyTypeExtensions
{
    extension(LayoutPropertyType layoutPropertyType)
    {
        public LayoutPropertyType SameAxisSize() => layoutPropertyType switch
        {
            LayoutPropertyType.X => LayoutPropertyType.Width,
            LayoutPropertyType.Y => LayoutPropertyType.Height,
            LayoutPropertyType.Width => LayoutPropertyType.Width,
            LayoutPropertyType.Height => LayoutPropertyType.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(layoutPropertyType), layoutPropertyType, null)
        };
        public LayoutPropertyType SameAxisPosition() => layoutPropertyType switch
        {
            LayoutPropertyType.X => LayoutPropertyType.X,
            LayoutPropertyType.Y => LayoutPropertyType.Y,
            LayoutPropertyType.Width => LayoutPropertyType.X,
            LayoutPropertyType.Height => LayoutPropertyType.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(layoutPropertyType), layoutPropertyType, null)
        };
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