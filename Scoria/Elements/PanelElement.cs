using Scoria.Layout;

namespace Scoria.Elements;

/// <summary>
/// A container element that renders a bordered panel with an optional title.
/// </summary>
public sealed class PanelElement : Element
{
    /// <summary>The title displayed in the panel border.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>When <see langword="true"/>, the panel is drawn with thin (single-line) borders instead of thick ones.</summary>
    public bool ThinBorders = false;

    /// <inheritdoc/>
    protected override void Render(ISurface surface)
    {
        surface.SubSurface(CalculatedLayout.X, CalculatedLayout.Y, CalculatedLayout.Width, CalculatedLayout.Height).Borders(Title, thin: ThinBorders);
        base.Render(surface);
    }
}

