using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Scoria.Styling;

/// <summary>
/// Flags that define text style attributes such as bold, italic, and underline.
/// </summary>
[Flags]
public enum StyleAttributes : byte
{
    /// <summary>No style attributes.</summary>
    None = 0,
    /// <summary>Bold (increased intensity) text.</summary>
    Bold = 1 << 0,
    /// <summary>Italic text.</summary>
    Italic = 1 << 1,
    /// <summary>Underlined text.</summary>
    Underline = 1 << 2,
    /// <summary>Strikethrough text.</summary>
    Strikethrough = 1 << 3,
    /// <summary>Blinking text.</summary>
    Blink = 1 << 4,
    /// <summary>Double-underlined text.</summary>
    DoubleUnderline = 1 << 5,
    /// <summary>Overlined text.</summary>
    Overline = 1 << 6,
}

/// <summary>
/// Defines the visual style for terminal output, including foreground and background colors and text attributes.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public struct Style() : IEquatable<Style>
{
    public Color Foreground { get; set; }
    public Color Background { get; set; }

    /// <summary>Alpha (opacity) of the background color (0-255). Defaults to 255 (fully opaque).</summary>
    public byte Alpha { get; set; } = 255;
    /// <summary>Text style attributes (bold, italic, underline, etc.).</summary>
    public StyleAttributes StyleAttributes { get; set; }

    /// <summary>
    /// Initializes a new <see cref="Style"/> with the specified style attributes, using default white foreground and transparent background.
    /// </summary>
    /// <param name="styleAttributes">Text style attributes to apply.</param>
    public Style(StyleAttributes styleAttributes) : this(Color.White, styleAttributes)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="Style"/> with the specified foreground color and optional style attributes, using a transparent background.
    /// </summary>
    /// <param name="styleAttributes">Text style attributes. Defaults to <see cref="StyleAttributes.None"/>.</param>
    public Style(Color foreground, StyleAttributes styleAttributes = StyleAttributes.None) : this(foreground, Color.Black, 255, styleAttributes)
    {
    }

    /// <summary>
    /// Initializes a new <see cref="Style"/> with the specified foreground and background colors and optional style attributes.
    /// </summary>
    /// <param name="alpha">Alpha (opacity) of the background color (0-255).</param>
    /// <param name="styleAttributes">Text style attributes. Defaults to <see cref="StyleAttributes.None"/>.</param>
    public Style(Color foreground, Color background, byte alpha = 255,
        StyleAttributes styleAttributes = StyleAttributes.None) : this()
    {
        Foreground = foreground;
        Background = background;
        Alpha = alpha;
        StyleAttributes = styleAttributes;
    }

    /// <summary>Determines whether two <see cref="Style"/> instances are equal.</summary>
    /// <param name="left">The first style to compare.</param>
    /// <param name="right">The second style to compare.</param>
    /// <returns><see langword="true"/> if the styles are equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Style left, Style right)
    {
        return Unsafe.BitCast<Style, long>(left) == Unsafe.BitCast<Style, long>(right);
    }

    /// <summary>Determines whether two <see cref="Style"/> instances are not equal.</summary>
    /// <param name="left">The first style to compare.</param>
    /// <param name="right">The second style to compare.</param>
    /// <returns><see langword="true"/> if the styles are not equal; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Style left, Style right)
    {
        return Unsafe.BitCast<Style, long>(left) != Unsafe.BitCast<Style, long>(right);
    }

    /// <summary>Indicates whether the current style is equal to another style.</summary>
    /// <param name="other">A style to compare with this style.</param>
    /// <returns><see langword="true"/> if the current style is equal to the <paramref name="other"/> parameter; otherwise <see langword="false"/>.</returns>
    public readonly bool Equals(Style other)
    {
        return this == other;
    }

    /// <summary>Indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Style"/> and equal to this instance; otherwise <see langword="false"/>.</returns>
    public readonly override bool Equals(object? obj)
    {
        return obj is Style other && Equals(other);
    }

    /// <summary>Returns the hash code for this style.</summary>
    /// <returns>A 32-bit signed integer hash code built from all color channels and style attributes.</returns>
    public readonly override int GetHashCode()
    {
        return HashCode.Combine(Foreground, Background, Alpha, (int)StyleAttributes);
    }
}
