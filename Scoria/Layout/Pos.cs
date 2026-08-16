using System.Net.Cache;

namespace Scoria.Layout;

public abstract class Pos : IReferenceContainer
{
    List<Reference> IReferenceContainer.GetReferences(Property property, Element self) => GetReferences(property, self);
    internal abstract List<Reference> GetReferences(Property property, Element self);
    int IReferenceContainer.Solve(LayoutSolver solver, List<int> dependencies) => Solve(solver, dependencies);
    internal abstract int Solve(LayoutSolver solver, List<int> dependencies);

    private sealed class AbsolutePos(int value) : Pos
    {
        internal override List<Reference> GetReferences(Property property, Element self) => [];
        internal override int Solve(LayoutSolver solver, List<int> dependencies) => value;

        public override string ToString() => $"Absolute({value})";
    }

    public static Pos Abs(int value) => new AbsolutePos(value);
    
    private sealed class RelativePos(float percentage, Element? element) : Pos
    {
        internal override List<Reference> GetReferences(Property property, Element self)
        {
            Element reference = element ?? self.Parent ??
                throw new Exception("Relative position must either specify an element or have a parent element.");
            return
            [
                new(property.SameAxisSize(), self),
                new(property, reference),
                new(property.SameAxisSize(), reference)
            ];
        }

        internal override int Solve(LayoutSolver solver, List<int> dependencies)
        {
            int size = dependencies[0];
            int refPos = dependencies[1];
            int refSize = dependencies[2];
            return (int)((refSize - size) * percentage) + refPos;
        }

        public override string ToString() => $"Relative({percentage}, {element?.ToString() ?? "Parent"})";
    }

    public static Pos Relative(float percentage, Element? element = null) => new RelativePos(percentage, element);
    public static Pos Center(Element? element = null) => new RelativePos(0.5f, element);
}