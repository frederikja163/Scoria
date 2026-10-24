using System.Globalization;
using System.Runtime.CompilerServices;
using Scoria.Elements;

namespace Scoria.Layout;

/// <summary>
/// Describes how the horizontal or vertical position of an element is resolved during layout.
/// </summary>
public abstract class Pos : ILayoutResolver
{
    List<LayoutProperty> ILayoutResolver.GetDependencies(LayoutProperty property) => GetDependencies(property);
    internal abstract List<LayoutProperty> GetDependencies(LayoutProperty property);
    int ILayoutResolver.Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies) => Resolve(property, ref dependencies);
    internal int Resolve(LayoutProperty property, ReadOnlySpan<int> dependencies) => Resolve(property, ref dependencies);

    internal abstract int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies);

    private sealed class AutoPos() : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => property.Element!.ResolveAutoLayoutDependencies(property.Type);

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
            => property.Element!.ResolveAutoLayout(property.Type, ref dependencies);

        /// <summary>Returns "Auto".</summary>
        public override string ToString() => "Auto";
    }

    /// <summary>
    /// Creates a position that is automatically resolved based on the element's layout dependencies.
    /// </summary>
    public static Pos Auto() => new AutoPos();

    private sealed class AbsolutePos(int value) : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => [];
        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies) => value;

        /// <summary>Returns "Absolute(value)".</summary>
        public override string ToString() => $"Absolute({value})";
    }

    /// <summary>
    /// Creates an absolute position at the specified pixel offset from the parent's origin.
    /// </summary>
    /// <param name="value">The fixed offset in characters.</param>
    public static Pos Abs(int value) => new AbsolutePos(value);

    private sealed class RelativePos(float factor, Element? element, string func = "Relative") : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property)
        {
            Element reference = element ?? property.Element?.Parent ??
                throw new LayoutException($"Pos.{func} must either specify an element or have a parent element.");
            return
            [
                property with { Type = property.Type.SameAxisSize() },
                property with { Element = reference },
                new(property.Type.SameAxisSize(), reference)
            ];
        }

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int size = dependencies[0];
            int refPos = dependencies[1];
            int refSize = dependencies[2];
            dependencies = dependencies[3..];
            return (int)((refSize - size) * factor) + refPos;
        }

        /// <summary>Returns "{func}(factor, reference)".</summary>
        public override string ToString() => $"{func}({factor.ToString(CultureInfo.InvariantCulture)}, {element?.ToString() ?? "Parent"})";
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

    /// <summary>
    /// Creates a position that centers the element within the reference element.
    /// Equivalent to <c>Relative(0.5f, element)</c>.
    /// </summary>
    /// <param name="element">
    /// The element to center within. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos Center(Element? element = null) => new RelativePos(0.5f, element, "Center");
    
    private sealed class BeginPos(Element? element) : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property)
        {
            Element reference = element ?? property.Element?.Parent ??
                throw new LayoutException("Pos.Begin must either specify an element or have a parent element.");
            return [property with { Element = reference }];
        }

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int value = dependencies[0];
            dependencies = dependencies[1..];
            return value;
        }
        
        /// <summary>Returns "Begin(reference)".</summary>
        public override string ToString() => $"Begin({element?.ToString() ?? "Parent"})";
    }
    
    /// <summary>
    /// Creates a position aligned to the start (top or left edge) of the reference element.
    /// </summary>
    /// <param name="element">
    /// The reference element. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos Begin(Element? element = null) => new BeginPos(element);

    /// <summary>
    /// Creates a position aligned to the end (bottom or right edge) of the reference element.
    /// Equivalent to <c>Relative(1, element)</c>.
    /// </summary>
    /// <param name="element">
    /// The reference element. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos End(Element? element = null) => new RelativePos(1, element, "End");

    private sealed class BeforePos(Element? element) : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property)
        {
            Element reference = element ?? property.Element?.Parent ??
                throw new LayoutException("Pos.Before must either specify an element or have a parent element.");
            return
            [
                property with { Element = reference },
                property with { Type = property.Type.SameAxisSize() }
            ];
        }

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int refPos = dependencies[0];
            int selfSize = dependencies[1];
            dependencies = dependencies[2..];
            return refPos - selfSize;
        }
        
        /// <summary>Returns "Before(reference)".</summary>
        public override string ToString() => $"Before({element?.ToString() ?? "Parent"})";
    }

    /// <summary>
    /// Creates a position placed before (to the left of or above) the reference element.
    /// </summary>
    /// <param name="element">
    /// The reference element. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos Before(Element? element = null) => new BeforePos(element);

    private sealed class AfterPos(Element? element) : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property)
        {
            Element reference = element ?? property.Element?.Parent ??
                throw new LayoutException("Pos.After must either specify an element or have a parent element.");
            return
            [
                property with { Element = reference },
                new(property.Type.SameAxisSize(), reference)
            ];
        }

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int refPos = dependencies[0];
            int refSize = dependencies[1];
            dependencies = dependencies[2..];
            return refPos + refSize;
        }
        
        /// <summary>Returns "After(reference)".</summary>
        public override string ToString() => $"After({element?.ToString() ?? "Parent"})";
    }

    /// <summary>
    /// Creates a position placed after (to the right of or below) the reference element.
    /// </summary>
    /// <param name="element">
    /// The reference element. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Pos After(Element? element = null) => new AfterPos(element);

    private sealed class CombinedPos(ILayoutResolver left, ILayoutResolver right, Func<int, int, int> combine, string str) : Pos
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property)
        {
            List<LayoutProperty> dependencies = new(left.GetDependencies(property));
            dependencies.AddRange(right.GetDependencies(property));
            return dependencies;
        }

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int leftValue = left.Resolve(property, ref dependencies);
            int rightValue = right.Resolve(property, ref dependencies);
            return combine(leftValue, rightValue);
        }

        /// <summary>Returns a string representation of the combined position expression.</summary>
        public override string ToString() => string.Format(str, left, right);
    }

    /// <summary>Combines two positions by adding their resolved values.</summary>
    [OverloadResolutionPriority(2)]
    public static Pos operator +(Pos left, Pos right) => new CombinedPos(left, right, static (a, b) => a + b, "({0} + {1})");

    /// <summary>Combines two positions by subtracting their resolved values.</summary>
    [OverloadResolutionPriority(2)]
    public static Pos operator -(Pos left, Pos right) => new CombinedPos(left, right, static (a, b) => a - b, "({0} - {1})");

    /// <summary>Combines two positions by multiplying their resolved values.</summary>
    [OverloadResolutionPriority(2)]
    public static Pos operator *(Pos left, Pos right) => new CombinedPos(left, right, static (a, b) => a * b, "({0} * {1})");

    /// <summary>Combines two positions by dividing their resolved values.</summary>
    [OverloadResolutionPriority(2)]
    public static Pos operator /(Pos left, Pos right) => new CombinedPos(left, right, static (a, b) => a / b, "({0} / {1})");

    /// <summary>Returns a position that resolves to the smaller of the two positions.</summary>
    /// <param name="left">The first position to compare.</param>
    /// <param name="right">The second position to compare.</param>
    public static Pos Min(Pos left, Pos right) => new CombinedPos(left, right, static (a, b) => int.Min(a, b), "Min({0}, {1})");

    /// <summary>Returns a position that resolves to the larger of the two positions.</summary>
    /// <param name="left">The first position to compare.</param>
    /// <param name="right">The second position to compare.</param>
    public static Pos Max(Pos left, Pos right) => new CombinedPos(left, right, static (a, b) => int.Max(a, b), "Max({0}, {1})");

    /// <summary>
    /// Displaces a position by an offset along the same axis.
    /// </summary>
    /// <param name="position">The position to displace.</param>
    /// <param name="offset">The size to shift the position by.</param>
    [OverloadResolutionPriority(1)]
    public static Pos operator +(Pos position, Size offset) => new CombinedPos(position, offset, static (a, b) => a + b, "({0} + {1})");

    /// <summary>
    /// Displaces a position by an offset along the same axis. The sum is commutative.
    /// </summary>
    /// <param name="offset">The size to shift the position by.</param>
    /// <param name="position">The position to displace.</param>
    [OverloadResolutionPriority(1)]
    public static Pos operator +(Size offset, Pos position) => new CombinedPos(offset, position, static (a, b) => a + b, "({0} + {1})");

    /// <summary>
    /// Displaces a position backwards by an offset along the same axis.
    /// </summary>
    /// <param name="position">The position to displace.</param>
    /// <param name="offset">The size to shift the position by.</param>
    [OverloadResolutionPriority(1)]
    public static Pos operator -(Pos position, Size offset) => new CombinedPos(position, offset, static (a, b) => a - b, "({0} - {1})");

    /// <summary>
    /// Implicitly converts an integer to an absolute position.
    /// </summary>
    /// <param name="value">The fixed offset in characters.</param>
    public static implicit operator Pos(int value) => Abs(value);

    /// <summary>
    /// Implicitly converts a float to a relative position.
    /// </summary>
    /// <param name="factor">The interpolation factor to use.</param>
    public static implicit operator Pos(float factor) => Relative(factor);

    /// <summary>
    /// Implicitly converts a double to a relative position.
    /// </summary>
    /// <param name="factor">The interpolation factor to use.</param>
    public static implicit operator Pos(double factor) => Relative((float)factor);
}