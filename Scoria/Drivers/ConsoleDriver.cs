using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Scoria.Drivers.Providers;
using Scoria.Events;

namespace Scoria.Drivers;

internal interface IConsoleDriver
{
    void Write(char value);
    void Write(string value);
    void WriteRaw(int value);
    void Enable(PrivateMode privateMode, bool enable);
}

internal class ConsoleDriver : IConsoleDriver
{
    private readonly IInputProvider[] InputProviders;
    private readonly IPlatformDriver PlatformDriver;
    internal event Action<AnyEventArgs>? OnEvent;


    private readonly StringBuilder Buffer = new StringBuilder();
    private Style _currentStyle = new Style();
    private int _width;
    private int _height;

    internal ConsoleDriver(params IInputProvider[] providers)
    {
        IPlatformDriver CreatePlatform()
        {
            if (OperatingSystem.IsLinux())
            {
                return new LinuxConsoleDriver();
            }

            if (OperatingSystem.IsWindows())
            {
                return new WindowsConsoleDriver();
            }

            throw new PlatformNotSupportedException();
        }

        InputProviders = providers;
        PlatformDriver = CreatePlatform();

        Init();

        _width = Console.WindowWidth;
        _height = Console.WindowHeight;

        AppDomain.CurrentDomain.ProcessExit += (_, _) => { Restore(); };

        Console.CancelKeyPress += (_, e) => { e.Cancel = true; };
    }

    public int Width => _width;
    public int Height => _height;

    internal void Init()
    {
        PlatformDriver.Init();

        foreach (IInputProvider provider in InputProviders)
        {
            if (provider.Enable)
            {
                provider.Init(this);
            }
        }

        Flush();
    }

    internal void Restore()
    {
        foreach (IInputProvider provider in InputProviders)
        {
            if (provider.Enable)
            {
                provider.Restore(this);
            }
        }

        Flush();
        PlatformDriver.Restore();
    }

    internal void PollInput() => PollInput(Timeout.InfiniteTimeSpan);

    internal void PollInput(TimeSpan timeout)
    {
        byte[] bytes = new byte[256];
        int length = PlatformDriver.PollInput(bytes, timeout);
        if (length > 0)
        {
            string input = Encoding.UTF8.GetString(bytes.AsSpan(0, length));
            ReadOnlySpan<char> inp = input;
            while (inp.Length != 0 && HandleInput(ref inp)) ;
        }
    }

    private bool HandleInput(ref ReadOnlySpan<char> inp)
    {
        foreach (IInputProvider inputProvider in InputProviders)
        {
            if (inputProvider.HandleInput(ref inp) is { } args)
            {
                if (args is WindowResizeEventArgs resizeEventArgs)
                {
                    _width = resizeEventArgs.Width;
                    _height = resizeEventArgs.Height;
                }
                OnEvent?.Invoke(args);
                return true;
            }
        }

        return false;
    }

    internal void Frame(Surface surface)
    {
        // TODO: Handle too big frames
        Clear();
        Display(surface);
        Flush();
    }

    private void Display(Surface surface)
    {
        // TODO: Make system to only write surface difference.
        int width = Math.Min(surface.Width, _width);
        int height = Math.Min(surface.Height, _height);
        SelectGraphicsRendition(GraphicsRendition.Reset);
        _currentStyle = new Style();
        for (int y = 0; y < height; y++)
        {
            if (y != 0)
            {
                NextLine();
            }

            for (int x = 0; x < width; x++)
            {
                ApplyStyle(surface.GetStyle(x, y));
                Write(surface.GetChar(x, y));
            }
        }
    }

    private void Flush()
    {
        byte[] data = Encoding.UTF8.GetBytes(Buffer.ToString());
        PlatformDriver.Write(data);
        Buffer.Clear();
    }

    private void Clear()
    {
        ControlSequenceIntroducer('J', 2);
        ControlSequenceIntroducer('H');
        ControlSequenceIntroducer('J', 3);
    }

    private void NextLine()
    {
        Escape();
        Write('E');
    }

    private void ApplyStyle(Style style)
    {
        if (_currentStyle == style)
            return;

        ApplyAttribute(StyleAttributes.Bold, GraphicsRendition.BoldOn, GraphicsRendition.BoldOff);
        ApplyAttribute(StyleAttributes.Italic, GraphicsRendition.ItalicOn, GraphicsRendition.ItalicOff);
        ApplyAttribute(StyleAttributes.Underline, GraphicsRendition.UnderlineOn, GraphicsRendition.UnderlineOff);
        ApplyAttribute(StyleAttributes.Strikethrough, GraphicsRendition.StrikethroughOn,
            GraphicsRendition.StrikethroughOff);
        ApplyAttribute(StyleAttributes.Blink, GraphicsRendition.BlinkOn, GraphicsRendition.BlinkOff);
        ApplyAttribute(StyleAttributes.DoubleUnderline, GraphicsRendition.DoubleUnderlineOn,
            GraphicsRendition.DoubleUnderlineOff);
        ApplyAttribute(StyleAttributes.Overline, GraphicsRendition.OverlinedOn, GraphicsRendition.OverlinedOff);

        if (style.ForegroundRed != _currentStyle.ForegroundRed ||
            style.ForegroundGreen != _currentStyle.ForegroundGreen ||
            style.ForegroundBlue != _currentStyle.ForegroundBlue)
        {
            SelectGraphicsRendition(GraphicsRendition.Foreground, 2,
                style.ForegroundRed, style.ForegroundGreen, style.ForegroundBlue);
            _currentStyle.ForegroundRed = style.ForegroundRed;
            _currentStyle.ForegroundGreen = style.ForegroundGreen;
            _currentStyle.ForegroundBlue = style.ForegroundBlue;
        }

        if (style.BackgroundRed != _currentStyle.BackgroundRed ||
            style.BackgroundGreen != _currentStyle.BackgroundGreen ||
            style.BackgroundBlue != _currentStyle.BackgroundBlue)
        {
            SelectGraphicsRendition(GraphicsRendition.Background, 2,
                style.BackgroundRed, style.BackgroundGreen, style.BackgroundBlue);
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

    void IConsoleDriver.Write(char value) => Write(value);
    internal void Write(char value) => Buffer.Append(value);
    void IConsoleDriver.Write(string value) => Write(value);
    internal void Write(string value) => Buffer.Append(value);
    void IConsoleDriver.WriteRaw(int value) => WriteRaw(value);

    internal void WriteRaw(int value) => Buffer.Append((char)value);
    private void Escape() => WriteRaw(0x1b);

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

    void IConsoleDriver.Enable(PrivateMode feature, bool enable) => Enable(feature, enable);
    internal void Enable(PrivateMode feature, bool enable)
    {
        Escape();
        Write('[');
        Write('?');
        Write(((int)feature).ToString());
        Write(enable ? 'h' : 'l');
    }

    internal void Enable(Mode feature, bool enable)
    {
        Escape();
        Write('[');
        Write(((int)feature).ToString());
        Write(enable ? 'h' : 'l');
    }
}

internal enum Mode
    {
        KeyboardAction = 2,     // Keyboard Action Mode (AM)
        Insert = 4,             // Insert Mode (IRM)
        SendReceive = 12,       // Send/receive (SRM)
        AutomaticNewline = 20,  // Automatic Newline (LNM)
    }

    internal enum PrivateMode
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
        WindowResize = 2048,            // Report text-area size changes. Sends CSI 48;height;width;height_px;width_px t immediately and on resize.
    }

    internal enum GraphicsRendition
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
