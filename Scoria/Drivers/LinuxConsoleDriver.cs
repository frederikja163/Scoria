using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Scoria.Drivers;

[SupportedOSPlatform("Linux")]
internal sealed class LinuxConsoleDriver : IPlatformDriver
{
    private const int O_RdWr = 2;
    private const int Tcsanow = 0;
    private const int TermiosSize = 60;
    private const int OffsetCC = 17;

    private byte[]? _originalTermios;
    private int _fd = -1;

    [DllImport("libc", SetLastError = true)]
    private static extern int tcgetattr(int fd, IntPtr termios);

    [DllImport("libc", SetLastError = true)]
    private static extern int tcsetattr(int fd, int optionalActions, IntPtr termios);

    [DllImport("libc", SetLastError = true)]
    private static extern void cfmakeraw(IntPtr termios);

    [DllImport("libc", SetLastError = true)]
    private static extern int open(string pathname, int flags);

    [DllImport("libc", SetLastError = true)]
    private static extern int close(int fd);

    [DllImport("libc", SetLastError = true)]
    private static extern int read(int fd, [Out] byte[] buffer, IntPtr count);

    [DllImport("libc", SetLastError = true)]
    private static extern int write(int fd, [In] byte[] buffer, IntPtr count);

    [DllImport("libc", SetLastError = true)]
    private static extern int poll([In, Out] PollFd[] fds, int nfds, int timeout);

    [StructLayout(LayoutKind.Sequential)]
    private struct PollFd
    {
        public int fd;
        public short events;
        public short revents;
    }

    private const short POLLIN = 1;

    public void Init()
    {
        _fd = open("/dev/tty", O_RdWr);
        if (_fd < 0)
            return;

        IntPtr ptr = Marshal.AllocHGlobal(TermiosSize);
        try
        {
            if (tcgetattr(_fd, ptr) != 0)
                return;

            _originalTermios = new byte[TermiosSize];
            Marshal.Copy(ptr, _originalTermios, 0, TermiosSize);

            cfmakeraw(ptr);
            Marshal.WriteByte(ptr, OffsetCC + 6, 1); // VMIN = 1
            Marshal.WriteByte(ptr, OffsetCC + 5, 0); // VTIME = 0

            tcsetattr(_fd, Tcsanow, ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }

    public int PollInput(byte[] buffer, TimeSpan timeout)
    {
        int timeoutMs = (int)timeout.TotalMilliseconds;
        if (timeoutMs >= 0)
        {
            var pfd = new PollFd { fd = _fd, events = POLLIN };
            if (poll(new[] { pfd }, 1, timeoutMs) <= 0)
                return 0;
        }
        return read(_fd, buffer, (IntPtr)buffer.Length);
    }

    public void Write(byte[] buffer)
    {
        write(_fd, buffer, (IntPtr)buffer.Length);
    }

    public void Flush()
    {
    }

    public void Restore()
    {
        if (_originalTermios == null)
            return;

        IntPtr ptr = Marshal.AllocHGlobal(TermiosSize);
        try
        {
            Marshal.Copy(_originalTermios, 0, ptr, TermiosSize);
            tcsetattr(_fd, Tcsanow, ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        _originalTermios = null;
        close(_fd);
        _fd = -1;
    }
}
