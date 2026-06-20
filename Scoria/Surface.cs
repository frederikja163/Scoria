namespace Scoria;

/// <summary>
/// Represents a rectangular grid of characters each with an associated <see cref="Style"/>.
/// Supports filling, drawing, alpha blending, and composition of multiple surfaces.
/// </summary>
public sealed class Surface
{
    private char[,] _glyphs;
    private Style[,] _styles;

    /// <summary>Initializes a new <see cref="Surface"/> with the specified dimensions.</summary>
    /// <param name="width">The width of the surface in character cells.</param>
    /// <param name="height">The height of the surface in character cells.</param>
    public Surface(int width, int height)
    {
        Width = width;
        Height = height;
        _glyphs = new char[width, height];
        _styles = new Style[width, height];
    }

    /// <summary>Gets the width of the surface in character cells.</summary>
    public int Width { get; }

    /// <summary>Gets the height of the surface in character cells.</summary>
    public int Height { get; }

    /// <summary>Fills the entire surface with the specified character and style.</summary>
    /// <param name="c">The character to fill with.</param>
    /// <param name="style">The style to apply to every cell.</param>
    public void Fill(char c, Style style)
    {
        Fill(c, 0, 0, Width, Height, style);
    }

    /// <summary>Fills a rectangular region of the surface with the specified character and style.</summary>
    /// <param name="c">The character to fill with.</param>
    /// <param name="xMin">The minimum X coordinate (inclusive) of the region.</param>
    /// <param name="yMin">The minimum Y coordinate (inclusive) of the region.</param>
    /// <param name="xMax">The maximum X coordinate (exclusive) of the region.</param>
    /// <param name="yMax">The maximum Y coordinate (exclusive) of the region.</param>
    /// <param name="style">The style to apply to every cell in the region.</param>
    public void Fill(char c, int xMin, int yMin, int xMax, int yMax, Style style)
    {
        int width = Math.Min(xMax, Width);
        int height = Math.Min(yMax, Height);
        xMin = Math.Max(0, xMin);
        yMin = Math.Max(0, yMin);

        for (int xOffset = 0; xOffset < width; xOffset++)
        {
            for (int yOffset = 0; yOffset < height; yOffset++)
            {
                Write(c, xMin + xOffset, yMin + yOffset, style);
            }
        }
    }

    /// <summary>Writes a single character at the specified position with the given style.
    /// If the style's <see cref="Style.Alpha"/> is less than 255, the background color is alpha-blended with the existing cell.</summary>
    /// <param name="c">The character to write.</param>
    /// <param name="x">The X coordinate of the cell.</param>
    /// <param name="y">The Y coordinate of the cell.</param>
    /// <param name="style">The style to apply to the cell.</param>
    public void Write(char c, int x, int y, Style style)
    {
        _glyphs[x, y] = c;
        if (style.Alpha != byte.MaxValue)
        {
            float blendNew = style.Alpha / 255f;
            float blendOld = 1 - blendNew;
            Style oldStyle = _styles[x, y];
            style.BackgroundRed = (byte)(style.BackgroundRed * blendNew + oldStyle.BackgroundRed * blendOld);
            style.BackgroundBlue = (byte)(style.BackgroundBlue * blendNew + oldStyle.BackgroundBlue * blendOld);
            style.BackgroundGreen = (byte)(style.BackgroundGreen * blendNew + oldStyle.BackgroundGreen * blendOld);
        }
        _styles[x, y] = style;
    }

    /// <summary>Writes another surface onto this surface at the specified offset.
    /// Characters and styles from the source surface are composited onto this surface. Alpha blending is applied per-cell.</summary>
    /// <param name="surface">The source surface to copy from.</param>
    /// <param name="xOffset">The X offset on this surface where the source will be placed.</param>
    /// <param name="yOffset">The Y offset on this surface where the source will be placed.</param>
    public void Write(Surface surface, int xOffset, int yOffset)
    {
        int w = Math.Min(xOffset + surface.Width, Width);
        int h = Math.Min(yOffset + surface.Height, Height);

        for (int x = Math.Max(xOffset, 0); x < w; x++)
        {
            for (int y = Math.Max(yOffset, 0); y < h; y++)
            {
                Write(surface.GetChar(x - xOffset, y - yOffset), x, y, surface.GetStyle(x - xOffset, y - yOffset));
            }
        }
    }

    /// <summary>Gets the character at the specified position.</summary>
    /// <param name="x">The X coordinate of the cell.</param>
    /// <param name="y">The Y coordinate of the cell.</param>
    /// <returns>The character at the specified position.</returns>
    public char GetChar(int x, int y)
    {
        return _glyphs[x, y];
    }

    /// <summary>Gets the style at the specified position.</summary>
    /// <param name="x">The X coordinate of the cell.</param>
    /// <param name="y">The Y coordinate of the cell.</param>
    /// <returns>The style at the specified position.</returns>
    public Style GetStyle(int x, int y)
    {
        return _styles[x, y];
    }
}