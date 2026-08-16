namespace Scoria.Layout;

public abstract class Size : ILayoutResolver
{
    List<LayoutProperty> ILayoutResolver.GetDependencies(LayoutProperty property) => GetDependencies(property);
    internal abstract List<LayoutProperty> GetDependencies(LayoutProperty property);
    int ILayoutResolver.Resolve(LayoutProperty property, List<int> dependencies) => Resolve(property, dependencies);
    internal abstract int Resolve(LayoutProperty property, List<int> dependencies);

    private sealed class AutoSize() : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => property.Element.ResolveAutoLayoutDependencies(property.Type);

        internal override int Resolve(LayoutProperty property, List<int> dependencies) => property.Element.ResolveAutoLayout(property.Type, dependencies);
    }

    public static Size Auto() => new AutoSize();
    
    private sealed class AbsoluteSize(int value) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => [];

        internal override int Resolve(LayoutProperty property, List<int> dependencies) => value;

        public override string ToString() => $"Absolute({value}";
    }

    public static Size Abs(int value) => new AbsoluteSize(value);

    private sealed class RelativeSize(float percentage, Element? element) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) =>
        [
            property with { Element = element ?? property.Element.Parent ??
                throw new Exception("Relative size must either specify an element or have a parent element.") }
        ];

        internal override int Resolve(LayoutProperty property, List<int> dependencies)
        {
            int refSize = dependencies[0];
            return (int)(percentage * refSize);
        }

        public override string ToString() => $"Relative({percentage}, {element?.ToString() ?? "Parent"}";
    }

    public static Size Relative(float percentage, Element? element = null) => new RelativeSize(percentage, element);
    public static Size Fill(Element? element = null) => new RelativeSize(1, element);

    private sealed class AspectSize(float aspectRatio) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) =>
            [property with { Type = property.Type.OtherAxis() }];

        internal override int Resolve(LayoutProperty property, List<int> dependencies)
        {
            int selfOtherSize = dependencies[0];
            return (int)(aspectRatio * selfOtherSize);
        }

        public override string ToString() => $"Aspect({aspectRatio})";
    }

    public static Size Aspect(float aspectRatio) => new AspectSize(aspectRatio);
    
    private sealed class FitChildrenSize : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) =>
            property.Element.GetChildren().Select(e => property with { Element = e }).ToList();

        internal override int Resolve(LayoutProperty property, List<int> dependencies)
        {
            return dependencies.Sum();
        }

        public override string ToString() => $"FitChildren";
    }

    public static Size FitChildren() => new FitChildrenSize();
}