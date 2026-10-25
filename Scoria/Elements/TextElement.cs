using Scoria.Layout;
using Scoria.Rendering;
using Scoria.Styling;

namespace Scoria.Elements;

/// <summary>
/// An element that renders a single line of styled text.
/// </summary>
public sealed class TextElement : Element
{
    /// <summary>The visual style applied to the text.</summary>
    public Style Style { get; set; } = default;
    /// <summary>The text content to render.</summary>
    public string Text { get; set; } = string.Empty;

    // TODO: Text element is missing a lot of features, like text wrapping, proper size detection etc.
    /// <inheritdoc/>
    protected override void Render(ISurface surface)
    {
        for (int i = 0; i < Text.Length; i++)
        {
            surface.Write(Text[i], CalculatedLayout.X + i, CalculatedLayout.Y, Style);
        }
        base.Render(surface);
    }

    protected internal override List<LayoutProperty> ResolveAutoLayoutDependencies(LayoutPropertyType type)
    {
        if (type == LayoutPropertyType.Width)
            return [new LayoutProperty(LayoutPropertyType.Width, Parent!)];
        if (type == LayoutPropertyType.Height)
            return [new(LayoutPropertyType.Width, this)];
        return [];
    }

    protected internal override int ResolveAutoLayout(LayoutPropertyType type, ref ReadOnlySpan<int> dependencies)
    {
        if (type == LayoutPropertyType.Width)
        {
            int value = int.Min(dependencies[0], Text.Length);
            dependencies = dependencies[1..];
            return value;
        }
        if (type == LayoutPropertyType.Height)
        {
            int width = dependencies[0];
            dependencies = dependencies[1..];
            // Integer division with rounding up.
            return width == 0 ? 0 : (Text.Length + width - 1) / width;
        }

        return 0;
    }
}
