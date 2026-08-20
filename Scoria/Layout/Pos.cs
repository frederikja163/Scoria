using System.Globalization;
using Scoria.Elements;

namespace Scoria.Layout;

/// <summary>
/// Describes how the horizontal or vertical position of an element is resolved during layout.
/// </summary>
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

    /// <summary>
    /// Creates a position that is automatically resolved based on the element's layout dependencies.
    /// </summary>
    public static Pos Auto() => new AutoPos();
    
    private sealed class AbsolutePos(int value) : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => [];
        internal override int Resolve(LayoutProperty property, List<int> dependencies) => value;

        public override string ToString() => $"Absolute({value})";
    }

    /// <summary>
    /// Creates an absolute position at the specified pixel offset from the parent's origin.
    /// </summary>
    /// <param name="value">The fixed offset in characters.</param>
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
    
    /// <summary>
    /// Creates a position that is a fractional interpolation between the reference element's position and its opposite edge.
    /// </summary>
    /// <param name="factor">
    /// Interpolation factor: 0 aligns to the start, 1 aligns to the end,
    /// and values between interpolate proportionally.
    /// </param>
    /// <param name="element">
    /// The element to position relative to. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos Relative(float factor, Element? element = null) => new RelativePos(factor, element);
    
    private sealed class CenterPos(Element? element) : RelativePos(0.5f, element)
    {
        public override string ToString() => $"Center({ElementString})";
    }

    /// <summary>
    /// Creates a position that centers the element within the reference element.
    /// Equivalent to <c>Relative(0.5f, element)</c>.
    /// </summary>
    /// <param name="element">
    /// The element to center within. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos Center(Element? element = null) => new CenterPos(element);
    
    private sealed class BeginPos(int offset, Element? element) : RelativePos(0, element)
    {
        protected override int Solve(int size, int refPos, int refSize)
        {
            return base.Solve(size, refPos, refSize) + offset;
        }

        public override string ToString() => $"Begin({ElementString})";
    }
    
    /// <summary>
    /// Creates a position aligned to the start (top or left edge) of the reference element, with an optional offset.
    /// Equivalent to <c>Relative(0, element)</c> plus the offset.
    /// </summary>
    /// <param name="offset">The offset in characters from the start edge.</param>
    /// <param name="element">
    /// The reference element. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos Begin(int offset = 0, Element? element = null) => new BeginPos(offset, element);
    
    private sealed class EndPos(int offset, Element? element) : RelativePos(1, element)
    {
        protected override int Solve(int size, int refPos, int refSize)
        {
            return base.Solve(size, refPos, refSize) - offset;
        }

        public override string ToString() => $"End({ElementString})";
    }

    /// <summary>
    /// Creates a position aligned to the end (bottom or right edge) of the reference element, with an optional offset.
    /// Equivalent to <c>Relative(1, element)</c> minus the offset.
    /// </summary>
    /// <param name="offset">The offset in characters from the end edge.</param>
    /// <param name="element">
    /// The reference element. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos End(int offset = 0, Element? element = null) => new EndPos(offset, element);

    private sealed class BeforePos(int offset, Element? element) : RelativeBase(element)
    {
        protected override int Solve(int size, int refPos, int refSize)
        {
            return refPos - size - offset;
        }

        public override string ToString() => $"Before({offset}, {ElementString})";
    }

    /// <summary>
    /// Creates a position placed before (to the left of or above) the reference element with an optional gap.
    /// </summary>
    /// <param name="offset">The gap in characters between the element's end edge and the reference's start edge.</param>
    /// <param name="element">
    /// The element to position before. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos Before(int offset = 0, Element? element = null) => new BeforePos(offset, element);

    private sealed class AfterPos(int offset, Element? element) : RelativeBase(element)
    {
        protected override int Solve(int size, int refPos, int refSize)
        {
            return refPos + refSize + offset;
        }

        public override string ToString() => $"After({offset}, {ElementString})";
    }

    /// <summary>
    /// Creates a position placed after (to the right of or below) the reference element with an optional gap.
    /// </summary>
    /// <param name="offset">The gap in characters between the reference's end edge and this element's start edge.</param>
    /// <param name="element">
    /// The element to position after. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos After(int offset = 0, Element? element = null) => new AfterPos(offset, element);
}