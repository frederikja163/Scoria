using Scoria.Styling;

namespace Scoria.Rendering;

/// <summary>
/// Represents a rectangular grid of characters each with an associated <see cref="Style"/>.
/// Supports filling, drawing, alpha blending, and composition of multiple surfaces.
/// </summary>
public sealed class Surface : ISurface
{
    private readonly char[,] _glyphs;
    private readonly Style[,] _styles;

    /// <summary>Initializes a new <see cref="Surface"/> with the specified dimensions and theme.</summary>
    /// <param name="width">The width of the surface in character cells.</param>
    /// <param name="height">The height of the surface in character cells.</param>
    /// <param name="theme">The theme associated with the surface.</param>
    public Surface(int width, int height, Theme theme)
    {
        Width = Math.Max(0, width);
        Height = Math.Max(0, height);
        _glyphs = new char[Width, Height];
        _styles = new Style[Width, Height];
        Theme = theme;
    }

    /// <inheritdoc />
    public int Width { get; }
    /// <inheritdoc />
    public int Height { get; }

    /// <inheritdoc />
    public Theme Theme { get; }

    /// <inheritdoc />
    public void Write(char c, int x, int y, Style style)
    {
        if (Width <= 0 || Height <= 0)
            return;
        x = Math.Clamp(x, 0, Width - 1);
        y = Math.Clamp(y, 0, Height - 1);

        _glyphs[x, y] = c;
        if (style.Alpha != byte.MaxValue)
        {
            float blendNew = style.Alpha / 255f;
            float blendOld = 1 - blendNew;
            Style oldStyle = _styles[x, y];
            style.Background = style.Background * blendNew + oldStyle.Background * blendOld;
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
