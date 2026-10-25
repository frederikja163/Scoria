using Scoria.Styling;

namespace Scoria.Rendering;

/// <summary>
/// Provides a view into a rectangular region of a parent <see cref="ISurface"/>.
/// All operations are offset by the sub-surface's position within the parent.
/// </summary>
public sealed class SubSurface : ISurface
{
    private readonly ISurface _parent;

    internal SubSurface(ISurface parent, int offsetX, int offsetY, int width, int height, Theme theme)
    {
        _parent = parent;
        OffsetX = Math.Clamp(offsetX, 0, parent.Width - 1);
        OffsetY = Math.Clamp(offsetY, 0, parent.Height - 1);
        Width = Math.Clamp(width, 0, Math.Max(0, parent.Width - OffsetX));
        Height = Math.Clamp(height, 0, Math.Max(0, parent.Height - OffsetY));
        Theme = theme;
    }

    /// <summary>Gets the X offset of this sub-surface within the parent surface.</summary>
    public int OffsetX { get; }
    /// <summary>Gets the Y offset of this sub-surface within the parent surface.</summary>
    public int OffsetY { get; }
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
        _parent.Write(c, x + OffsetX, y + OffsetY, style);
    }

    /// <inheritdoc />
    public char GetChar(int x, int y)
    {
        if (Width <= 0 || Height <= 0)
            return (char)0;
        x = Math.Clamp(x, 0, Width - 1);
        y = Math.Clamp(y, 0, Height - 1);
        return _parent.GetChar(x + OffsetX, y + OffsetY);
    }

    /// <inheritdoc />
    public Style GetStyle(int x, int y)
    {
        if (Width <= 0 || Height <= 0)
            return Style.Default;
        x = Math.Clamp(x, 0, Width - 1);
        y = Math.Clamp(y, 0, Height - 1);
        return _parent.GetStyle(x + OffsetX, y + OffsetY);
    }
}
