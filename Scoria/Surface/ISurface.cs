namespace Scoria;

/// <summary>
/// Defines the contract for a rectangular grid of characters each with an associated <see cref="Style"/>.
/// </summary>
public interface ISurface
{
    /// <summary>Gets the width of the surface in character cells.</summary>
    public int Width { get; }
    /// <summary>Gets the height of the surface in character cells.</summary>
    public int Height { get; }

    /// <summary>Writes a single character at the specified position with the given style.
    /// If the style's <see cref="Style.Alpha"/> is less than 255, the background color is alpha-blended with the existing cell.</summary>
    /// <param name="c">The character to write.</param>
    /// <param name="x">The X coordinate of the cell.</param>
    /// <param name="y">The Y coordinate of the cell.</param>
    /// <param name="style">The style to apply to the cell.</param>
    public void Write(char c, int x, int y, Style style);

    /// <summary>Gets the character at the specified position.</summary>
    /// <param name="x">The X coordinate of the cell.</param>
    /// <param name="y">The Y coordinate of the cell.</param>
    /// <returns>The character at the specified position.</returns>
    public char GetChar(int x, int y);

    /// <summary>Gets the style at the specified position.</summary>
    /// <param name="x">The X coordinate of the cell.</param>
    /// <param name="y">The Y coordinate of the cell.</param>
    /// <returns>The style at the specified position.</returns>
    public Style GetStyle(int x, int y);
}

/// <summary>
/// Provides extension methods for <see cref="ISurface"/>.
/// </summary>
public static class SurfaceExtensions
{
    /// <summary>Creates a <see cref="SubSurface"/> view into a rectangular region of this surface.</summary>
    /// <param name="surface">The parent surface.</param>
    /// <param name="offsetX">The X offset of the sub-surface within the parent.</param>
    /// <param name="offsetY">The Y offset of the sub-surface within the parent.</param>
    /// <param name="width">The width of the sub-surface in character cells.</param>
    /// <param name="height">The height of the sub-surface in character cells.</param>
    /// <returns>A <see cref="SubSurface"/> that maps to the specified region of the parent surface.</returns>
    public static SubSurface SubSurface(this ISurface surface, int offsetX, int offsetY, int width, int height)
    {
        return new SubSurface(surface, offsetX, offsetY, width, height);
    }

    /// <summary>Creates a <see cref="SubSurface"/> view into a region of this surface specified by <see cref="Range"/> values.</summary>
    /// <param name="surface">The parent surface.</param>
    /// <param name="x">The range of columns to include.</param>
    /// <param name="y">The range of rows to include.</param>
    /// <returns>A <see cref="SubSurface"/> that maps to the specified range of the parent surface.</returns>
    public static SubSurface SubSurface(this ISurface surface, Range x, Range y)
    {
        (int offsetX, int width) = x.GetOffsetAndLength(surface.Width);
        (int offsetY, int height) = y.GetOffsetAndLength(surface.Height);
        return new SubSurface(surface, offsetX, offsetY, width, height);
    }

    /// <summary>Fills a rectangular region of the surface with the specified character and style.</summary>
    /// <param name="surface">The surface to fill.</param>
    /// <param name="c">The character to fill with.</param>
    /// <param name="style">The style to apply to every cell in the region.</param>
    public static void Fill(this ISurface surface, char c, Style style)
    {
        for (int x = 0; x < surface.Width; x++)
        {
            for (int y = 0; y < surface.Height; y++)
            {
                surface.Write(c, x, y, style);
            }
        }
    }

    /// <summary>Writes another surface onto this surface at the specified offset.
    /// Characters and styles from the source surface are composited onto this surface. Alpha blending is applied per-cell.</summary>
    /// <param name="target">The surface to write to.</param>
    /// <param name="source">The target surface to write from.</param>
    /// <param name="xOffset">The X offset on the target surface where the source will be placed.</param>
    /// <param name="yOffset">The Y offset on the target surface where the source will be placed.</param>
    public static void Write(this ISurface target, ISurface source, int xOffset, int yOffset)
    {
        int w = Math.Min(xOffset + source.Width, target.Width);
        int h = Math.Min(yOffset + source.Height, target.Height);

        for (int x = Math.Max(xOffset, 0); x < w; x++)
        {
            for (int y = Math.Max(yOffset, 0); y < h; y++)
            {
                target.Write(source.GetChar(x - xOffset, y - yOffset), x, y, source.GetStyle(x - xOffset, y - yOffset));
            }
        }
    }

    public static bool Contains(this ISurface surface, int x, int y)
    {
        return (uint)x < surface.Width && (uint)y < surface.Height;
    }

    public static void Borders(this ISurface surface, string title = "", bool thin = false, Style? style = null)
    {
        char borderCharacter = thin ? Scoria.Borders.ThinBorderCharacter : Scoria.Borders.ThickBorderCharacter;
        style ??= Theme.CurrentTheme.Border;

        for (int x = 0; x < surface.Width; x++)
        {
            surface.Write(borderCharacter, x, 0, style.Value);
            surface.Write(borderCharacter, x, surface.Height - 1, style.Value);
        }
        for (int y = 0; y < surface.Height; y++)
        {
            surface.Write(borderCharacter, 0, y, style.Value);
            surface.Write(borderCharacter, surface.Width - 1, y, style.Value);
        }

        if (!string.IsNullOrEmpty(title))
        {
            Theme.CurrentTheme.Borders.WriteTitle(surface, title);
        }
    }

    public static void ExpandBorders(this ISurface surface)
    {
        Theme.CurrentTheme.Borders.ExpandBorders(surface);
    }
}
