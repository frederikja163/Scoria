namespace Scoria.Layout;

public abstract class Size : IReferenceContainer
{
    List<Reference> IReferenceContainer.GetReferences(Property property, Element self) => GetReferences(property, self);
    internal abstract List<Reference> GetReferences(Property property, Element self);
    int IReferenceContainer.Solve(LayoutSolver solver, List<int> dependencies) => Solve(solver, dependencies);
    internal abstract int Solve(LayoutSolver solver, List<int> dependencies);

    private sealed class AbsoluteSize(int value) : Size
    {
        internal override List<Reference> GetReferences(Property property, Element self) => [];

        internal override int Solve(LayoutSolver solver, List<int> dependencies) => value;

        public override string ToString() => $"Absolute({value}";
    }

    public static Size Abs(int value) => new AbsoluteSize(value);

    private sealed class RelativeSize(float percentage, Element? element) : Size
    {
        internal override List<Reference> GetReferences(Property property, Element self) =>
        [
            new (property,
                element ?? self.Parent ??
                throw new Exception("Relative size must either specify an element or have a parent element."))
        ];

        internal override int Solve(LayoutSolver solver, List<int> dependencies) => (int)(percentage * dependencies[0]);

        public override string ToString() => $"Relative({percentage}, {element?.ToString() ?? "Parent"}";
    }

    public static Size Relative(float percentage, Element? element = null) => new RelativeSize(percentage, element);
    public static Size Fill(Element? element = null) => new RelativeSize(1, element);
}