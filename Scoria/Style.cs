namespace Scoria;

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
public struct Style()
{
    /// <summary>Red component of the foreground (text) color (0-255). Defaults to 255.</summary>
    public byte ForegroundRed { get; set; } = 255;
    /// <summary>Green component of the foreground (text) color (0-255). Defaults to 255.</summary>
    public byte ForegroundGreen { get; set; } = 255;
    /// <summary>Blue component of the foreground (text) color (0-255). Defaults to 255.</summary>
    public byte ForegroundBlue { get; set; } = 255;
    /// <summary>Red component of the background color (0-255). Defaults to 0.</summary>
    public byte BackgroundRed { get; set; }
    /// <summary>Green component of the background color (0-255). Defaults to 0.</summary>
    public byte BackgroundGreen { get; set; }
    /// <summary>Blue component of the background color (0-255). Defaults to 0.</summary>
    public byte BackgroundBlue { get; set; }
    /// <summary>Alpha (opacity) of the background color (0-255). Defaults to 0.</summary>
    public byte BackgroundAlpha { get; set; }
    /// <summary>Text style attributes (bold, italic, underline, etc.).</summary>
    public StyleAttributes StyleAttributes { get; set; }

    /// <summary>
    /// Initializes a new <see cref="Style"/> with the specified foreground and background colors and optional style attributes.
    /// </summary>
    /// <param name="foregroundRed">Red component of the foreground color (0-255).</param>
    /// <param name="foregroundGreen">Green component of the foreground color (0-255).</param>
    /// <param name="foregroundBlue">Blue component of the foreground color (0-255).</param>
    /// <param name="backgroundRed">Red component of the background color (0-255).</param>
    /// <param name="backgroundGreen">Green component of the background color (0-255).</param>
    /// <param name="backgroundBlue">Blue component of the background color (0-255).</param>
    /// <param name="backgroundAlpha">Alpha (opacity) of the background color (0-255).</param>
    /// <param name="styleAttributes">Text style attributes. Defaults to <see cref="StyleAttributes.None"/>.</param>
    public Style(byte foregroundRed, byte foregroundGreen, byte foregroundBlue,
        byte backgroundRed = 0, byte backgroundGreen = 0, byte backgroundBlue = 0, byte backgroundAlpha = 0,
        StyleAttributes styleAttributes = StyleAttributes.None) : this()
    {
        ForegroundRed = foregroundRed;
        ForegroundGreen = foregroundGreen;
        ForegroundBlue = foregroundBlue;
        BackgroundRed = backgroundRed;
        BackgroundGreen = backgroundGreen;
        BackgroundBlue = backgroundBlue;
        BackgroundAlpha = backgroundAlpha;
        StyleAttributes = styleAttributes;
    }

    /// <summary>
    /// Initializes a new <see cref="Style"/> with the specified foreground color and optional style attributes, using a transparent background.
    /// </summary>
    /// <param name="foregroundRed">Red component of the foreground color (0-255).</param>
    /// <param name="foregroundGreen">Green component of the foreground color (0-255).</param>
    /// <param name="foregroundBlue">Blue component of the foreground color (0-255).</param>
    /// <param name="styleAttributes">Text style attributes. Defaults to <see cref="StyleAttributes.None"/>.</param>
    public Style(byte foregroundRed, byte foregroundGreen, byte foregroundBlue,
        StyleAttributes styleAttributes = StyleAttributes.None) : this()
    {
        ForegroundRed = foregroundRed;
        ForegroundGreen = foregroundGreen;
        ForegroundBlue = foregroundBlue;
        StyleAttributes = styleAttributes;
    }

    /// <summary>
    /// Initializes a new <see cref="Style"/> with the specified style attributes, using default white foreground and transparent background.
    /// </summary>
    /// <param name="styleAttributes">Text style attributes to apply.</param>
    public Style(StyleAttributes styleAttributes) : this()
    {
        StyleAttributes = styleAttributes;
    }
}