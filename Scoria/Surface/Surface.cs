namespace Scoria;

/// <summary>
/// Represents a rectangular grid of characters each with an associated <see cref="Style"/>.
/// Supports filling, drawing, alpha blending, and composition of multiple surfaces.
/// </summary>
public sealed class Surface : ISurface
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

    /// <inheritdoc />
    public int Width { get; }
    /// <inheritdoc />
    public int Height { get; }

    /// <inheritdoc />
    public void Write(char c, int x, int y, Style style)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfNegative(y);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, Width);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, Height);
        
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

    /// <inheritdoc />
    public char GetChar(int x, int y)
    {
        return _glyphs[x, y];
    }
    
    /// <inheritdoc />
    public Style GetStyle(int x, int y)
    {
        return _styles[x, y];
    }
}
