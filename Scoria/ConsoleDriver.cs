using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Scoria;

internal static class ConsoleDriver
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    private const int STD_INPUT_HANDLE = -10;

    private const uint ENABLE_LINE_INPUT = 0x0002;
    private const uint ENABLE_ECHO_INPUT = 0x0004;
    private const uint ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200;
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
    
    static ConsoleDriver()
    {
        if (OperatingSystem.IsWindows())
        {
            InitWindows();
        }
    }

    /// <summary>
    /// Creates and returns a new <see cref="FrameDriver"/> for writing a frame to the standard output stream.
    /// </summary>
    public static FrameDriver StartFrame()
    {
        return new FrameDriver(new StreamWriter(Console.OpenStandardOutput()));
    }
}