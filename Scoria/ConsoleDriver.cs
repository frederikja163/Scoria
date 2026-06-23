using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;

namespace Scoria;

internal static class ConsoleDriver
{
    internal static event Action<int, int, int, bool> OnMouseEvent;
    
    private static readonly StringBuilder Buffer = new StringBuilder();
    private static readonly Stream StdOut;
    private static readonly Stream StdIn;
    private static Style _currentStyle = new Style();
    private static int _width;
    private static int _height;
    private static readonly Regex MouseInputRegex = new Regex(@"\[<(?<cb>\d+);(?<cx>\d+);(?<cy>\d+)(?<ev>[mM])", RegexOptions.Compiled);
    private static readonly Regex ArrowKeysInputRegex = new Regex(@"ESC\[(?<key>[A-D])", RegexOptions.Compiled);

    static ConsoleDriver()
    {
        if (OperatingSystem.IsWindows())
        {
            InitWindows();
        }
        StdOut = Console.OpenStandardOutput();
        StdIn = Console.OpenStandardInput();

        Enable(TerminalFeature.SgrMouse, true);
        Enable(TerminalFeature.AnyEventMouse, true);
        Flush();
        
        _width = Console.WindowWidth;
        _height = Console.WindowHeight;
    }

    internal static void PollInput()
    {
        Span<byte> bytes = stackalloc byte[256];
        int length = StdIn.Read(bytes);
        if (length > 0)
        {
            string input = Encoding.UTF8.GetString(bytes[..length]);
            HandleInput(input);
        }
    }

    private static void HandleInput(string input)
    {
        Match mouseMatch = MouseInputRegex.Match(input);
        if (mouseMatch.Success)
        {
            GroupCollection collection = mouseMatch.Groups;
            (string cb, string cx, string cy, string release) =
                (collection["cb"].Value, collection["cx"].Value, collection["cy"].Value, collection["ev"].Value);
            Console.WriteLine($"Mouse button [{release}] at ({cx}, {cy})");
            return;
        }

        Match arrowKeysMatch = ArrowKeysInputRegex.Match(input);
        if (arrowKeysMatch.Success)
        {
            GroupCollection collection = arrowKeysMatch.Groups;
            Key key = arrowKeysMatch.Groups["key"].Value switch
            {
                "A" => Key.Left,
                "B" => Key.Down,
                "C" => Key.Right,
                "D" => Key.Up,
                _ => throw new UnreachableException(),
            };
        }

        if (input.Length == 1)
        {
            Console.WriteLine($"Key pressed {input}");
            return;
        }
        
        Console.WriteLine($"Unrecognized input: '\\E{input[1..]}'");
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
        StdOut.Write(Encoding.UTF8.GetBytes(Buffer.ToString()));
        StdOut.Flush();
        Buffer.Clear();
    }
    
    private static void Clear()
    {
        ControlSequenceIntroducer('J', 2);
        ControlSequenceIntroducer('H');
        ControlSequenceIntroducer('J', 3);
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

        if (style.ForegroundRed   != _currentStyle.ForegroundRed   ||
            style.ForegroundGreen != _currentStyle.ForegroundGreen ||
            style.ForegroundBlue  != _currentStyle.ForegroundBlue)
        {
            SelectGraphicsRendition(GraphicsRendition.Foreground, 2,
                style.ForegroundRed, style.ForegroundGreen, style.ForegroundBlue);
            _currentStyle.ForegroundRed   = style.ForegroundRed;
            _currentStyle.ForegroundGreen = style.ForegroundGreen;
            _currentStyle.ForegroundBlue  = style.ForegroundBlue;
        }
        if (style.BackgroundRed   != _currentStyle.BackgroundRed   ||
            style.BackgroundGreen != _currentStyle.BackgroundGreen ||
            style.BackgroundBlue  != _currentStyle.BackgroundBlue)
        {
            SelectGraphicsRendition(GraphicsRendition.Background, 2,
                style.BackgroundRed, style.BackgroundGreen, style.BackgroundBlue);
            _currentStyle.BackgroundRed   = style.BackgroundRed;
            _currentStyle.BackgroundGreen = style.BackgroundGreen;
            _currentStyle.BackgroundBlue  = style.BackgroundBlue;
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

    private static void Write(char value)    => Buffer.Append(value);
    private static void Write(string value)  => Buffer.Append(value);
    private static void WriteRaw(int value)  => Buffer.Append((char)value);
    private static void Escape()             => WriteRaw(0x1b);

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

    private static void Enable(TerminalFeature feature, bool enable)
    {
        Escape();
        Write('[');
        Write('?');
        Write(((int)feature).ToString());
        Write(enable ? 'h' : 'l');
    }

    private enum TerminalFeature
    {
        ShowCursor = 25,            // Shows/hides the cursor. `Enable(ShowCursor, false)` hides it.
        AlternateScreen = 1049,     // Switches to/from the alternate screen buffer. Used for full-screen TUI apps.
        SgrMouse = 1006,           // Enables SGR-encoded mouse events (coordinates > 223 supported). Usually combined with ButtonEventMouse.
        ButtonEventMouse = 1002,   // Reports mouse button presses and drags. Use with SgrMouse for extended coordinates.
        AnyEventMouse = 1003,      // Reports all mouse motion events (not just clicks). Use sparingly — very noisy.
        FocusEvents = 1004,        // Sends CSI I / CSI O sequences when the terminal gains/loses focus.
        ApplicationCursorKeys = 1, // Switches cursor keys to application mode (sends CSI O A..D instead of CSI [ A..D).
        BracketedPaste = 2004,     // Wraps pasted text in CSI 200~ / CSI 201~ so the app can distinguish paste from keystrokes.
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

    // ---- Windows P/Invoke ----

    private const int StdInputHandle  = -10;
    private const int StdOutputHandle = -11;

    private const uint EnableLineInput                 = 0x0002;
    private const uint EnableEchoInput                 = 0x0004;
    private const uint EnableVirtualTerminalInput     = 0x0200;

    private static uint _originalInputMode;
    private static uint _originalOutputMode;

    [DllImport("kernel32.dll")]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [SupportedOSPlatform("Windows")]
    private static void InitWindows()
    {
        IntPtr inHandle  = GetStdHandle(StdInputHandle);

        GetConsoleMode(inHandle,  out _originalInputMode);

        uint inMode = _originalInputMode;
        inMode &= ~EnableLineInput;
        inMode &= ~EnableEchoInput;
        inMode |= EnableVirtualTerminalInput;
        SetConsoleMode(inHandle, inMode);;
    }

    private static void RestoreConsole()
    {
        IntPtr inHandle  = GetStdHandle(StdInputHandle);
        IntPtr outHandle = GetStdHandle(StdOutputHandle);
        SetConsoleMode(inHandle,  _originalInputMode);
        SetConsoleMode(outHandle, _originalOutputMode);
    }
}
