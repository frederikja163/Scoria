using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Scoria.Drivers;

[SupportedOSPlatform("Windows")]
internal sealed class WindowsConsoleDriver : IPlatformDriver
{
    private const int StdInputHandle = -10;
    private const int StdOutputHandle = -11;

    [Flags]
    private enum ConsoleInputModes : uint
    {
        ProcessedInput = 0x0001,
        LineInput = 0x0002,
        EchoInput = 0x0004,
        WindowInput = 0x0008,
        MouseInput = 0x0010,
        InsertMode = 0x0020,
        QuickEditMode = 0x0040,
        ExtendedFlags = 0x0080,
        AutoPosition = 0x0100,
        VirtualTerminalInput = 0x0200,
    }

    private ConsoleInputModes _originalInputMode;
    private uint _originalOutputMode;
    private readonly Stream _stdout = Console.OpenStandardOutput();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [DllImport("kernel32.dll")]
    private static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);

    [DllImport("kernel32.dll")]
    private static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, out int lpNumberOfBytesRead, IntPtr lpOverlapped);

    public void Init()
    {
        IntPtr inHandle = GetStdHandle(StdInputHandle);

        GetConsoleMode(inHandle, out uint originalMode);
        _originalInputMode = (ConsoleInputModes)originalMode;

        ConsoleInputModes inMode = _originalInputMode;
        inMode &= ~ConsoleInputModes.LineInput;
        inMode &= ~ConsoleInputModes.EchoInput;
        inMode |= ConsoleInputModes.VirtualTerminalInput;
        SetConsoleMode(inHandle, (uint)inMode);

        IntPtr outHandle = GetStdHandle(StdOutputHandle);
        GetConsoleMode(outHandle, out _originalOutputMode);
    }

    public int PollInput(byte[] buffer, TimeSpan timeout)
    {
        IntPtr handle = GetStdHandle(StdInputHandle);
        int timeoutMs = timeout.TotalMilliseconds < 0 ? -1 : (int)timeout.TotalMilliseconds;

        if (WaitForSingleObject(handle, timeoutMs) == 0)
        {
            ReadFile(handle, buffer, buffer.Length, out int bytesRead, IntPtr.Zero);
            return bytesRead;
        }
        return 0;
    }

    public void Write(byte[] buffer)
    {
        _stdout.Write(buffer, 0, buffer.Length);
    }

    public void Flush()
    {
        _stdout.Flush();
    }

    public void Restore()
    {
        IntPtr inHandle = GetStdHandle(StdInputHandle);
        IntPtr outHandle = GetStdHandle(StdOutputHandle);
        SetConsoleMode(inHandle, (uint)_originalInputMode);
        SetConsoleMode(outHandle, _originalOutputMode);
    }
}
