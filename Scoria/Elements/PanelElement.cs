using Scoria.Layout;

namespace Scoria.Elements;

/// <summary>
/// A container element that renders a bordered panel with an optional title.
/// </summary>
public sealed class PanelElement : Element
{
    /// <summary>The title displayed in the panel border.</summary>
    public string Title { get; set; } = string.Empty;

    public BorderStyle BorderStyle = BorderStyle.None;

    /// <inheritdoc/>
    protected override void Render(ISurface surface)
    {
        SubSurface subSurface = surface.SubSurface(CalculatedLayout.X, CalculatedLayout.Y, CalculatedLayout.Width, CalculatedLayout.Height);
        switch (BorderStyle)
        {
            case BorderStyle.None:
                break;
            case BorderStyle.Thin:
                subSurface.Borders(Title, true);
                break;
            case BorderStyle.Thick:
                subSurface.Borders(Title, false);
                break;
            default:
                break;
        }
        base.Render(surface);
    }
}

