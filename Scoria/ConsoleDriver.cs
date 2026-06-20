using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Scoria;

internal static class ConsoleDriver
{
    private static readonly StringBuilder _buffer = new StringBuilder();
    private static readonly StreamWriter _writer;
    private static Style _currentStyle = new Style();
    private static int _width;
    private static int _height;

    static ConsoleDriver()
    {
        if (OperatingSystem.IsWindows())
        {
            InitWindows();
        }
        _writer = new StreamWriter(Console.OpenStandardOutput());
        _width = Console.BufferWidth;
        _height = Console.BufferHeight;
    }

    internal static void Frame(Surface surface)
    {
        Clear();
        Display(surface);
        Flush();
    }
    
    private static void Display(Surface surface)
    {
        SelectGraphicsRendition(GraphicsRendition.Reset);
        _currentStyle = new Style();
        for (int y = 0; y < _height; y++)
        {
            if (y != 0)
            {
                NextLine();
            }
            for (int x = 0; x < _width; x++)
            {
                ApplyStyle(surface.GetStyle(x, y));
                Write(surface.GetChar(x, y));
            }
        }
    }

    private static void Flush()
    {
        _writer.Write(_buffer);
        _writer.Flush();
        _buffer.Clear();
    }
    
    private static void Clear()
    {
        Escape();
        Write('c');
    }

    private static void NextLine()
    {
        Escape();
        Write('E');
    }

    private static void ApplyStyle(Style style)
    {
        if (_currentStyle == style)
            return;

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
            _currentStyle.ForegroundRed = style.ForegroundRed;
            _currentStyle.ForegroundGreen = style.ForegroundGreen;
            _currentStyle.ForegroundBlue = style.ForegroundBlue;
        }
        if (style.BackgroundRed != _currentStyle.BackgroundRed ||
            style.BackgroundGreen != _currentStyle.BackgroundGreen ||
            style.BackgroundBlue != _currentStyle.BackgroundBlue)
        {
            SelectGraphicsRendition(GraphicsRendition.Background, 2, style.BackgroundRed, style.BackgroundGreen, style.BackgroundBlue);
            _currentStyle.BackgroundRed = style.BackgroundRed;
            _currentStyle.BackgroundGreen = style.BackgroundGreen;
            _currentStyle.BackgroundBlue = style.BackgroundBlue;
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

    private static void Write(char value)
    {
        _buffer.Append(value);
    }

    private static void Write(string value)
    {
        _buffer.Append(value);
    }

    private static void WriteRaw(int value)
    {
        _buffer.Append((char)value);
    }

    private static void Escape()
    {
        WriteRaw(0x1b);
    }

    private static void ControlSequenceIntroducer(char command, params IEnumerable<int> args)
    {
        Escape();
        Write('[');
        Write(string.Join(';', args));
        Write(command);
    }

    private static void SelectGraphicsRendition(GraphicsRendition rendition, params IEnumerable<int> codes)
    {
        ControlSequenceIntroducer('m', codes.Prepend((char)rendition));
    }

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

    private const int STD_INPUT_HANDLE = -10;

    private const uint ENABLE_LINE_INPUT = 0x0002;
    private const uint ENABLE_ECHO_INPUT = 0x0004;
    private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;

    [DllImport("kernel32.dll")]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [SupportedOSPlatform("Windows")]
    private static void InitWindows()
    {
        IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);

        GetConsoleMode(handle, out uint mode);

        mode &= ~ENABLE_LINE_INPUT;
        mode &= ~ENABLE_ECHO_INPUT;
        mode |= ENABLE_VIRTUAL_TERMINAL_INPUT;

        SetConsoleMode(handle, mode);
    }
}
