using System.Text;

namespace Scoria;

internal class FrameDriver : IDisposable
{
    private enum GraphicsRendition
    {
        Reset = 0,
        BoldOn = 1,
        BoldOff = 22,
        ItalicOn = 3,
        ItalicOff = 23,
        UnderlineOn = 4,
        UnderlineOff = 24,
        BlinkOn = 5,
        BlinkOff = 25,
        StrikethroughOn = 9,
        StrikethroughOff = 29,
        DoubleUnderlineOn = 21,
        DoubleUnderlineOff = 24,
        FramedOn = 51,
        FramedOff = 54,
        EncircledOn = 52,
        EncircledOff = 54,
        OverlinedOn = 53,
        OverlinedOff = 55,
        Foreground = 38,
        Background = 48
    }
    
    private readonly StreamWriter _writer;
    private readonly bool _shouldDispose;
    private Style _currentStyle = new Style();
    private int _x = 0;
    private int _y = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameDriver"/> class with the specified output writer.
    /// </summary>
    /// <param name="writer">The <see cref="StreamWriter"/> to write terminal output to.</param>
    /// <param name="shouldDispose">Whether to dispose <paramref name="writer"/> when this driver is disposed. Defaults to <see langword="true"/>.</param>
    public FrameDriver(StreamWriter writer, bool shouldDispose = true)
    {
        _writer = writer;
        _shouldDispose = shouldDispose;
    }

    /// <summary>
    /// Clears the entire terminal screen.
    /// </summary>
    public void Clear()
    {
        Escape();
        Write('c');
    }

    /// <summary>
    /// Writes a single character to the terminal output.
    /// </summary>
    /// <param name="value">The character to write.</param>
    public void Write(char value)
    {
        _writer.Write(value);
    }

    private void Write(int value)
    {
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a string to the terminal output.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public void Write(string value)
    {
        _writer.Write(value);
    }

    /// <summary>
    /// Applies the specified <see cref="Style"/> to subsequent output, transitioning only the attributes and colors that have changed.
    /// </summary>
    /// <param name="style">The style to apply.</param>
    public void ApplyStyle(Style style)
    {
        ApplyAttribute(StyleAttributes.Bold, GraphicsRendition.BoldOn, GraphicsRendition.BoldOff);
        ApplyAttribute(StyleAttributes.Italic, GraphicsRendition.ItalicOn, GraphicsRendition.ItalicOff);
        ApplyAttribute(StyleAttributes.Underline, GraphicsRendition.UnderlineOn, GraphicsRendition.UnderlineOff);
        ApplyAttribute(StyleAttributes.Strikethrough, GraphicsRendition.StrikethroughOn, GraphicsRendition.StrikethroughOff);
        ApplyAttribute(StyleAttributes.Blink, GraphicsRendition.BlinkOn, GraphicsRendition.BlinkOff);
        ApplyAttribute(StyleAttributes.DoubleUnderline, GraphicsRendition.DoubleUnderlineOn, GraphicsRendition.DoubleUnderlineOff);
        ApplyAttribute(StyleAttributes.Overline, GraphicsRendition.OverlinedOn, GraphicsRendition.OverlinedOff);

        if (style.ForegroundRed != _currentStyle.ForegroundRed ||
            style.ForegroundGreen != _currentStyle.ForegroundGreen ||
            style.ForegroundBlue != _currentStyle.ForegroundBlue)
        {
            SelectGraphicsRendition(GraphicsRendition.Foreground, 2, style.ForegroundRed, style.ForegroundGreen, style.ForegroundBlue);
            style.ForegroundRed = _currentStyle.ForegroundRed;
            style.ForegroundGreen = _currentStyle.ForegroundGreen;
            style.ForegroundBlue = _currentStyle.ForegroundBlue;
        }
        if (style.BackgroundRed != _currentStyle.BackgroundRed ||
            style.BackgroundGreen != _currentStyle.BackgroundGreen ||
            style.BackgroundBlue != _currentStyle.BackgroundBlue)
        {
            SelectGraphicsRendition(GraphicsRendition.Background, 2, style.BackgroundRed, style.BackgroundGreen, style.BackgroundBlue);
            style.BackgroundRed = _currentStyle.BackgroundRed;
            style.BackgroundGreen = _currentStyle.BackgroundGreen;
            style.BackgroundBlue = _currentStyle.BackgroundBlue;
        }
        
        void ApplyAttribute(StyleAttributes styleAttribute, GraphicsRendition on, GraphicsRendition off)
        {
            if (style.StyleAttributes.HasFlag(styleAttribute) != _currentStyle.StyleAttributes.HasFlag(styleAttribute))
            {
                SelectGraphicsRendition(style.StyleAttributes.HasFlag(styleAttribute) ? on : off);
                _currentStyle.StyleAttributes ^= styleAttribute;
            }
        }
    }

    /// <summary>
    /// Moves the cursor to the specified position (1-based coordinates).
    /// </summary>
    /// <param name="x">The 1-based column position.</param>
    /// <param name="y">The 1-based row position.</param>
    public void MoveTo(int x, int y)
    {
        if (_x != x || _y != y)
        {
            ControlSequenceIntroducer('H', x, y);
            _x = x;
            _y = y;
        }
    }

    private void Escape()
    {
        WriteRaw(0x1b);
    }

    private void ControlSequenceIntroducer(char command, params IEnumerable<int> args)
    {
        Escape();
        Write('[');
        Write(string.Join(';', args));
        Write(command);
    }

    private void SelectGraphicsRendition(GraphicsRendition rendition, params IEnumerable<int> codes)
    {
        ControlSequenceIntroducer('m', codes.Prepend((char)rendition));
    }
    
    private void WriteRaw(int value)
    {
        _writer.Write((char)value);
    }

    /// <summary>
    /// Flushes all buffered output and disposes the underlying writer if <c>shouldDispose</c> was set.
    /// </summary>
    public void Dispose()
    {
        _writer.Flush();
        if (_shouldDispose)
            _writer.Dispose();
    }
}