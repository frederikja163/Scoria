namespace Scoria.Layout;

internal enum Property
{
    X,
    Y,
    Width,
    Height,
}

internal static class PropertyExtensions
{
    extension(Property property)
    {
        public Property SameAxisSize() => property switch
        {
            Property.X => Property.Width,
            Property.Y => Property.Height,
            Property.Width => Property.Width,
            Property.Height => Property.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(property), property, null)
        };
        public Property SameAxisPosition() => property switch
        {
            Property.X => Property.X,
            Property.Y => Property.Y,
            Property.Width => Property.X,
            Property.Height => Property.Y,
            _ => throw new ArgumentOutOfRangeException(nameof(property), property, null)
        };
    }
}

internal interface IReferenceContainer
{
    internal List<Reference> GetReferences(Property property, Element parent);

    internal int Solve(LayoutSolver layoutSolver, List<int> dependencies);
}

internal sealed record Reference(Property Property, Element Element) : IEquatable<Reference>
{
    internal IReferenceContainer GetProperty() => Property switch
    {
        Property.X => Element.X,
        Property.Y => Element.Y,
        Property.Width => Element.Width,
        Property.Height => Element.Height,
        _ => throw new ArgumentOutOfRangeException()
    };
}