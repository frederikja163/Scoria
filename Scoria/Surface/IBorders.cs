namespace Scoria;

/// <summary>
/// Defines how borders are drawn and expanded on a surface, as used by a <see cref="Theme"/>.
/// </summary>
public interface IBorders
{
    /// <summary>Replaces placeholder border cells on the surface with the appropriate joined border glyphs.</summary>
    /// <param name="surface">The surface containing border cells to expand.</param>
    public void ExpandBorders(ISurface surface);
    /// <summary>Writes a title text onto the top border of the surface.</summary>
    /// <param name="surface">The surface whose top border receives the title.</param>
    /// <param name="title">The title text to write.</param>
    public void WriteTitle(ISurface surface, string title);
}