using System.Text;
using System.Text.RegularExpressions;
using Scoria.Drivers.Providers;

namespace Scoria.Drivers;

internal static class ConsoleDriver
{
    private static readonly string FocusLost = $"\x1b[O";
    private static readonly string FocusGained = $"\x1b[I";
    private static IInputProvider[] _inputProviders =
    [
        new PasteInputProvider(),
        new MatchConstInputProvider(FocusLost, new FocusChangedEventArgs(false)),
        new MatchConstInputProvider(FocusGained, new FocusChangedEventArgs(true)),
        new MouseInputProvider(),
    ];
    internal static event Action<EventArgs>? OnEvent;
    
    private static readonly IPlatformDriver PlatformDriver;
    
    private static readonly StringBuilder Buffer = new StringBuilder();
    private static Style _currentStyle = new Style();
    private static int _width;
    private static int _height;

    private static readonly IReadOnlyDictionary<PrivateMode, bool> PrivateModes =
        new Dictionary<PrivateMode, bool>()
        {
            [PrivateMode.SendEscOnMeta] = true, // TODO
            [PrivateMode.FocusEvents] = true,
            [PrivateMode.BracketedPaste] = true,
            [PrivateMode.ShowCursor] = false,
            [PrivateMode.SgrMouse] = true,
            [PrivateMode.AnyEventMouse] = true,
        };
    private static readonly IReadOnlyDictionary<Mode, bool> Modes =
        new Dictionary<Mode, bool>()
        {
            [Mode.KeyboardAction] = true,
        };
    
    static ConsoleDriver()
    {
        if (OperatingSystem.IsWindows())
        {
            PlatformDriver = new WindowsConsoleDriver();
        }
        else if (OperatingSystem.IsLinux())
        {
            PlatformDriver = new LinuxConsoleDriver();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }

        Init();
        
        _width = Console.WindowWidth;
        _height = Console.WindowHeight;

        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            Restore();
        };

        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = false;
            Restore();
        };
    }

    internal static void Init()
    {
        PlatformDriver.Init();

        foreach ((PrivateMode feature, var value) in PrivateModes)
        {
            Enable(feature, value);
        }
        foreach ((Mode feature, var value) in Modes)
        {
            Enable(feature, value);
        }
        Enable((PrivateMode)9001, true);
        Flush();
    }

    internal static void Restore()
    {
        foreach ((PrivateMode feature, var value) in PrivateModes)
        {
            Enable(feature, !value);
        }
        foreach ((Mode feature, var value) in Modes)
        {
            Enable(feature, !value);
        }
        Flush();

        PlatformDriver.Restore();
    }

    internal static void PollInput() => PollInput(Timeout.InfiniteTimeSpan);

    internal static void PollInput(TimeSpan timeout)
    {
        byte[] bytes = new byte[256];
        int length = PlatformDriver.PollInput(bytes, timeout);
        if (length > 0)
        {
            string input = Encoding.UTF8.GetString(bytes.AsSpan(0, length));
            HandleInput(input);
        }
    }

    private static void HandleInput(string input)
    {
        foreach (IInputProvider inputProvider in _inputProviders)
        {
            if (inputProvider.HandleInput(input) is {} args)
            {
                OnEvent?.Invoke(args);
            }
        }

        input = string.Join(' ', input.Select(c => char.IsLetterOrDigit(c) ? c.ToString() : ((int)c).ToString("X2")));
        Console.WriteLine($"Unrecognized input: '{input}'");
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
        byte[] data = Encoding.UTF8.GetBytes(Buffer.ToString());
        PlatformDriver.Write(data);
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

    private static void Enable(PrivateMode feature, bool enable)
    {
        Escape();
        Write('[');
        Write('?');
        Write(((int)feature).ToString());
        Write(enable ? 'h' : 'l');
    }

    private static void Enable(Mode feature, bool enable)
    {
        Escape();
        Write('[');
        Write(((int)feature).ToString());
        Write(enable ? 'h' : 'l');
    }

    private enum Mode
    {
        KeyboardAction = 2,     // Keyboard Action Mode (AM)
        Insert = 4,             // Insert Mode (IRM)
        SendReceive = 12,       // Send/receive (SRM)
        AutomaticNewline = 20,  // Automatic Newline (LNM)
    }

    private enum PrivateMode
    {
        ApplicationCursorKeys = 1,      // Application Cursor Keys (DECCKM)
        DesignateUSASCII = 2,           // Designate USASCII for character sets G0-G3 (DECANM), and set VT100 mode
        Column132Mode = 3,              // 132 Column Mode (DECCOLM)
        SmoothScroll = 4,               // Smooth (Slow) Scroll (DECSCLM)
        ReverseVideo = 5,               // Reverse Video (DECSCNM)
        OriginMode = 6,                 // Origin Mode (DECOM)
        WraparoundMode = 7,             // Wraparound Mode (DECAWM)
        AutoRepeatKeys = 8,             // Auto-repeat Keys (DECARM)
        SendMouseOnPress = 9,           // Send Mouse X & Y on button press. See the section Mouse Tracking.
        ShowToolbar = 10,               // Show toolbar (rxvt)
        StartBlinkingCursor = 12,       // Start Blinking Cursor (att610)
        PrintFormFeed = 18,             // Print form feed (DECPFF)
        PrintExtentFullScreen = 19,     // Set print extent to full screen (DECPEX)
        ShowCursor = 25,                // Show Cursor (DECTCEM). `Enable(ShowCursor, false)` hides it.
        ShowScrollbar = 30,             // Show scrollbar (rxvt)
        EnableFontShifting = 35,        // Enable font-shifting functions (rxvt)
        EnterTektronixMode = 38,        // Enter Tektronix Mode (DECTEK)
        Allow80To132Mode = 40,          // Allow 80 → 132 Mode
        MoreFix = 41,                   // more(1) fix (see curses resource)
        EnableNationReplacementChars = 42, // Enable Nation Replacement Character sets (DECNRCM)
        MarginBell = 44,                // Turn On Margin Bell
        ReverseWraparound = 45,         // Reverse-wraparound Mode
        StartLogging = 46,              // Start Logging (normally disabled by a compile-time option)
        UseAlternateScreenBuffer = 47,  // Use Alternate Screen Buffer (unless disabled by the titeInhibit resource)
        ApplicationKeypad = 66,         // Application keypad (DECNKM)
        BackarrowKeyBackspace = 67,     // Backarrow key sends backspace (DECBKM)
        SendMouseOnPressRelease = 1000, // Send Mouse X & Y on button press and release. See the section Mouse Tracking.
        HiliteMouseTracking = 1001,     // Use Hilite Mouse Tracking
        ButtonEventMouse = 1002,        // Reports mouse button presses and drags. Use with SgrMouse for extended coordinates. Also known as Cell Motion Mouse Tracking.
        AnyEventMouse = 1003,           // Reports all mouse motion events (not just clicks). Use sparingly — very noisy. Also known as All Motion Mouse Tracking.
        FocusEvents = 1004,            // Sends CSI I / CSI O sequences when the terminal gains/loses focus.
        SgrMouse = 1006,               // Enables SGR-encoded mouse events (coordinates > 223 supported). Usually combined with ButtonEventMouse.
        ScrollToBottomTtyOutput = 1010, // Scroll to bottom on tty output (rxvt)
        ScrollToBottomKeyPress = 1011,  // Scroll to bottom on key press (rxvt)
        EnableSpecialModifiers = 1035,  // Enable special modifiers for Alt and NumLock keys
        SendEscOnMeta = 1036,           // Send ESC when Meta modifies a key (enables the metaSendsEscape resource)
        SendDelFromDelete = 1037,       // Send DEL from the editing-keypad Delete key
        UseAlternateScreenBuffer2 = 1047, // Use Alternate Screen Buffer (unless disabled by the titeInhibit resource)
        SaveCursor = 1048,              // Save cursor as in DECSC (unless disabled by the titeInhibit resource)
        AlternateScreen = 1049,         // Save cursor as in DECSC and use Alternate Screen Buffer, clearing it first (unless disabled by the titeInhibit resource). This combines the effects of 1047 and 1048.
        SetSunFunctionKeyMode = 1051,   // Set Sun function-key mode
        SetHPFunctionKeyMode = 1052,    // Set HP function-key mode
        SetSCOFunctionKeyMode = 1053,   // Set SCO function-key mode
        SetLegacyKeyboardEmulation = 1060, // Set legacy keyboard emulation (X11R6)
        SetSunPCKeyboardEmulation = 1061, // Set Sun/PC keyboard emulation of VT220 keyboard
        BracketedPaste = 2004,          // Set bracketed paste mode. Wraps pasted text in CSI 200~ / CSI 201~ so the app can distinguish paste from keystrokes.
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

}
