namespace Scoria;

/// <summary>
/// Provides a view into a rectangular region of a parent <see cref="ISurface"/>.
/// All operations are offset by the sub-surface's position within the parent.
/// </summary>
public sealed class SubSurface : ISurface
{
    private readonly ISurface _parent;

    internal SubSurface(ISurface parent, int offsetX, int offsetY, int width, int height, Theme theme)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(offsetX);
        ArgumentOutOfRangeException.ThrowIfNegative(offsetY);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(offsetX + width, parent.Width);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(offsetY + height, parent.Height);
        _parent = parent;
        OffsetX = offsetX;
        OffsetY = offsetY;
        Width = width;
        Height = height;
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

    public Theme Theme { get; }

    /// <inheritdoc />
    public void Write(char c, int x, int y, Style style)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, Width);
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, Height);
        ArgumentOutOfRangeException.ThrowIfNegative(y);
        _parent.Write(c, x + OffsetX, y + OffsetY, style);
    }

    /// <inheritdoc />
    public char GetChar(int x, int y)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, Width);
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, Height);
        ArgumentOutOfRangeException.ThrowIfNegative(y);
        return _parent.GetChar(x + OffsetX, y + OffsetY);
    }

    /// <inheritdoc />
    public Style GetStyle(int x, int y)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(x, Width);
        ArgumentOutOfRangeException.ThrowIfNegative(x);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(y, Height);
        ArgumentOutOfRangeException.ThrowIfNegative(y);
        return _parent.GetStyle(x + OffsetX, y + OffsetY);
    }
}
