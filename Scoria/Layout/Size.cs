using System.Globalization;
using Scoria.Elements;

namespace Scoria.Layout;

/// <summary>
/// Describes how the width or height of an element is resolved during layout.
/// </summary>
public abstract class Size : ILayoutResolver
{
    List<LayoutProperty> ILayoutResolver.GetDependencies(LayoutProperty property) => GetDependencies(property);
    internal abstract List<LayoutProperty> GetDependencies(LayoutProperty property);
    int ILayoutResolver.Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies) => Resolve(property, ref dependencies);
    internal int Resolve(LayoutProperty property, ReadOnlySpan<int> dependencies) => Resolve(property, ref dependencies);

    internal abstract int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies);

    private sealed class AutoSize() : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => property.Element.ResolveAutoLayoutDependencies(property.Type);

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
            => property.Element.ResolveAutoLayout(property.Type, ref dependencies);

        /// <summary>Returns "Auto".</summary>
        public override string ToString() => "Auto";
    }

    /// <summary>
    /// Creates a size that is automatically resolved based on the element's layout dependencies.
    /// </summary>
    public static Size Auto() => new AutoSize();
    
    private sealed class AbsoluteSize(int value) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => [];

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies) => value;

        /// <summary>Returns "Absolute(value)".</summary>
        public override string ToString() => $"Absolute({value})";
    }

    /// <summary>
    /// Creates a size with a fixed value in characters.
    /// </summary>
    /// <param name="value">The fixed size in characters.</param>
    public static Size Abs(int value) => new AbsoluteSize(value);

    private class RelativeSize(float factor, Element? element) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) =>
        [
            property with { Element = element ?? property.Element.Parent ??
                throw new LayoutException("Relative size must either specify an element or have a parent element.") }
        ];

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int refSize = dependencies[0];
            dependencies = dependencies[1..];
            return (int)(factor * refSize);
        }

        protected string ElementString => element?.ToString() ?? "Parent";

        /// <summary>Returns "Relative(factor, reference)".</summary>
        public override string ToString() => $"Relative({factor.ToString(CultureInfo.InvariantCulture)}, {ElementString})";
    }

    /// <summary>
    /// Creates a size that is a fraction of the reference element's corresponding size.
    /// </summary>
    /// <param name="factor">The fraction of the reference size to use (e.g. 0.5 for half).</param>
    /// <param name="element">
    /// The element whose size to scale relative to. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Size Relative(float factor, Element? element = null) => new RelativeSize(factor, element);

    private sealed class FillSize(int margin, Element? element) : RelativeSize(1, element)
    {
        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            return base.Resolve(property, ref dependencies) - margin;
        }

        /// <summary>Returns "Fill(reference)".</summary>
        public override string ToString() => $"Fill({ElementString})";
    }

    /// <summary>
    /// Creates a size that fills the entire reference element along this axis.
    /// Equivalent to <c>Relative(1, element)</c>.
    /// </summary>
    /// <param name="element">
    /// The element to fill. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Size Fill(int margin = 0, Element? element = null) => new FillSize(margin, element);

    private sealed class AspectSize(float aspectRatio) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) =>
            [property with { Type = property.Type.OtherAxis() }];

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int selfOtherSize = dependencies[0];
            dependencies = dependencies[1..];
            return (int)(aspectRatio * selfOtherSize);
        }

        /// <summary>Returns "Aspect(ratio)".</summary>
        public override string ToString() => $"Aspect({aspectRatio.ToString(CultureInfo.InvariantCulture)})";
    }

    /// <summary>
    /// Creates a size derived from the element's size on the other axis, scaled by the given ratio.
    /// </summary>
    /// <param name="aspectRatio">The ratio to multiply the other axis size by.</param>
    public static Size Aspect(float aspectRatio) => new AspectSize(aspectRatio);
    
    private sealed class FitChildrenSize : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property)
        {
            List<LayoutProperty> properties = [];
            foreach (Element child in property.Element.GetChildren())
            {
                properties.Add(new LayoutProperty(property.Type.SameAxisPosition(), child));
                properties.Add(property with { Element = child });
            }

            return properties;
        }

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int min = int.MaxValue;
            int max = int.MinValue;
            foreach (Element child in property.Element.GetChildren())
            {
                int pos = dependencies[0];
                int size = dependencies[1];
                dependencies = dependencies[2..];
                min = int.Min(pos, min);
                max = int.Max(pos + size, max);
            }
            return min == int.MaxValue || max == int.MinValue ? 0 : max - min;
        }

        /// <summary>Returns "FitChildren".</summary>
        public override string ToString() => "FitChildren";
    }

    /// <summary>
    /// Creates a size equal to the bounding box of all direct children along this axis.
    /// </summary>
    public static Size FitChildren() => new FitChildrenSize();

    private sealed class CombinedSize(Size left, Size right, Func<int, int, int> combine, string op) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property)
        {
            List<LayoutProperty> dependencies = left.GetDependencies(property);
            dependencies.AddRange(right.GetDependencies(property));
            return dependencies;
        }

        internal override int Resolve(LayoutProperty property, ref ReadOnlySpan<int> dependencies)
        {
            int leftValue = left.Resolve(property, ref dependencies);
            int rightValue = right.Resolve(property, ref dependencies);
            return combine(leftValue, rightValue);
        }

        /// <summary>Returns a string representation of the combined size expression.</summary>
        public override string ToString() => $"({left} {op} {right})";
    }

    /// <summary>Combines two sizes by adding their resolved values.</summary>
    public static Size operator +(Size left, Size right) => new CombinedSize(left, right, static (a, b) => a + b, "+");

    /// <summary>Combines two sizes by subtracting their resolved values.</summary>
    public static Size operator -(Size left, Size right) => new CombinedSize(left, right, static (a, b) => a - b, "-");

    /// <summary>Combines two sizes by multiplying their resolved values.</summary>
    public static Size operator *(Size left, Size right) => new CombinedSize(left, right, static (a, b) => a * b, "*");

    /// <summary>Combines two sizes by dividing their resolved values.</summary>
    public static Size operator /(Size left, Size right) => new CombinedSize(left, right, static (a, b) => a / b, "/");

    // The scalar overloads below exist so integer literals stay unambiguous when mixed with
    // Pos and Size operands (both have implicit conversions from int).

    /// <summary>Adds a fixed offset in characters to a size.</summary>
    public static Size operator +(Size size, int offset) => size + Size.Abs(offset);

    /// <summary>Adds a fixed offset in characters to a size.</summary>
    public static Size operator +(int offset, Size size) => Size.Abs(offset) + size;

    /// <summary>
    /// Implicitly converts an integer to an absolute size.
    /// </summary>
    /// <param name="value">The fixed size in characters.</param>
    public static implicit operator Size(int value) => Abs(value);

    /// <summary>
    /// Implicitly converts a float to a relative size.
    /// </summary>
    /// <param name="factor">The fraction of the reference size to use.</param>
    public static implicit operator Size(float factor) => Relative(factor);

    /// <summary>
    /// Implicitly converts a double to a relative size.
    /// </summary>
    /// <param name="factor">The fraction of the reference size to use.</param>
    public static implicit operator Size(double factor) => Relative((float)factor);
}