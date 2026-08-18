using System.Globalization;

namespace Scoria.Layout;

public abstract class Pos : ILayoutResolver
{
    List<LayoutProperty> ILayoutResolver.GetDependencies(LayoutProperty property) => GetDependencies(property);
    internal abstract List<LayoutProperty> GetDependencies(LayoutProperty property);
    int ILayoutResolver.Resolve(LayoutProperty property, List<int> dependencies) => Resolve(property, dependencies);
    internal abstract int Resolve(LayoutProperty property, List<int> dependencies);


    private sealed class AutoPos() : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => property.Element.ResolveAutoLayoutDependencies(property.Type);

        internal override int Resolve(LayoutProperty property, List<int> dependencies) => property.Element.ResolveAutoLayout(property.Type, dependencies);

        public override string ToString() => "Auto";
    }

    public static Pos Auto() => new AutoPos();
    
    private sealed class AbsolutePos(int value) : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => [];
        internal override int Resolve(LayoutProperty property, List<int> dependencies) => value;

        public override string ToString() => $"Absolute({value})";
    }

    public static Pos Abs(int value) => new AbsolutePos(value);

    private abstract class RelativeBase(Element? element) : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property)
        {
            Element reference = element ?? property.Element.Parent ??
                throw new LayoutException("Relative position must either specify an element or have a parent element.");
            return
            [
                property with { Type = property.Type.SameAxisSize() },
                property with { Element = reference },
                new(property.Type.SameAxisSize(), reference)
            ];
        }

        internal override int Resolve(LayoutProperty property, List<int> dependencies)
        {
            int size = dependencies[0];
            int refPos = dependencies[1];
            int refSize = dependencies[2];
            return Solve(size, refPos, refSize);
        }

        protected abstract int Solve(int size, int refPos, int refSize);

        protected string ElementString => element?.ToString() ?? "Parent";
    }

    private class RelativePos(float factor, Element? element) : RelativeBase(element)
    {
        protected override int Solve(int size, int refPos, int refSize)
        {
            return (int)((refSize - size) * factor) + refPos;
        }

        public override string ToString() => $"Relative({factor.ToString(CultureInfo.InvariantCulture)}, {ElementString})";
    }
    
    public static Pos Relative(float factor, Element? element = null) => new RelativePos(factor, element);
    
    private sealed class CenterPos(Element? element) : RelativePos(0.5f, element)
    {
        public override string ToString() => $"Center({ElementString})";
    }

    public static Pos Center(Element? element = null) => new CenterPos(element);
    
    private sealed class BeginPos(Element? element) : RelativePos(0, element)
    {
        public override string ToString() => $"Begin({ElementString})";
    }
    
    public static Pos Begin(Element? element = null) => new BeginPos(element);
    
    private sealed class EndPos(Element? element) : RelativePos(1, element)
    {
        public override string ToString() => $"End({ElementString})";
    }

    public static Pos End(Element? element = null) => new EndPos(element);

    private sealed class BeforePos(int value, Element? element) : RelativeBase(element)
    {
        protected override int Solve(int size, int refPos, int refSize)
        {
            return refPos - size - value;
        }

        public override string ToString() => $"Before({value}, {ElementString})";
    }

    public static Pos Before(int value = 0, Element? element = null) => new BeforePos(value, element);

    private sealed class AfterPos(int value, Element? element) : RelativeBase(element)
    {
        protected override int Solve(int size, int refPos, int refSize)
        {
            return refPos + refSize + value;
        }

        public override string ToString() => $"After({value}, {ElementString})";
    }

    public static Pos After(int value = 0, Element? element = null) => new AfterPos(value, element);
}