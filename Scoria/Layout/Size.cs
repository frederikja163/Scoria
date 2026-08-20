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
    int ILayoutResolver.Resolve(LayoutProperty property, List<int> dependencies) => Resolve(property, dependencies);
    internal abstract int Resolve(LayoutProperty property, List<int> dependencies);

    private sealed class AutoSize() : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => property.Element.ResolveAutoLayoutDependencies(property.Type);

        internal override int Resolve(LayoutProperty property, List<int> dependencies) => property.Element.ResolveAutoLayout(property.Type, dependencies);

        public override string ToString() => "Auto";
    }

    /// <summary>
    /// Creates a size that is automatically resolved based on the element's layout dependencies.
    /// </summary>
    public static Size Auto() => new AutoSize();
    
    private sealed class AbsoluteSize(int value) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) => [];

        internal override int Resolve(LayoutProperty property, List<int> dependencies) => value;

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

        internal override int Resolve(LayoutProperty property, List<int> dependencies)
        {
            int refSize = dependencies[0];
            return (int)(factor * refSize);
        }

        protected string ElementString => element?.ToString() ?? "Parent";

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

    private sealed class FillSize(Element? element) : RelativeSize(1, element)
    {
        public override string ToString() => $"Fill({ElementString})";
    }

    /// <summary>
    /// Creates a size that fills the entire reference element along this axis.
    /// Equivalent to <c>Relative(1, element)</c>.
    /// </summary>
    /// <param name="element">
    /// The element to fill. If <see langword="null"/>, the parent element is used.
    /// </param>
    public static Size Fill(Element? element = null) => new FillSize(element);

    private sealed class AspectSize(float aspectRatio) : Size
    {
        internal override List<LayoutProperty> GetDependencies(LayoutProperty property) =>
            [property with { Type = property.Type.OtherAxis() }];

        internal override int Resolve(LayoutProperty property, List<int> dependencies)
        {
            int selfOtherSize = dependencies[0];
            return (int)(aspectRatio * selfOtherSize);
        }

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

        internal override int Resolve(LayoutProperty property, List<int> dependencies)
        {
            if (dependencies.Count == 0) return 0;
            int min = int.MaxValue;
            int max = int.MinValue;
            for (int i = 0; i < dependencies.Count; i++)
            {
                int pos = dependencies[i++];
                int size = dependencies[i];
                min = int.Min(pos, min);
                max = int.Max(pos + size, max);
            }
            return min == int.MaxValue || max == int.MaxValue ? 0 : max - min;
        }

        public override string ToString() => "FitChildren";
    }

    /// <summary>
    /// Creates a size equal to the bounding box of all direct children along this axis.
    /// </summary>
    public static Size FitChildren() => new FitChildrenSize();
}