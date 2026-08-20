using Scoria.Events;

namespace Scoria.Drivers.Providers;

internal sealed class ResizeInputProvider : IInputProvider
{
    public int Order => 0;
    public bool Enable => true;

    public void Init(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.WindowResize, true);
    }

    public void Restore(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.WindowResize, false);
    }

    public AnyEventArgs? HandleInput(ref ReadOnlySpan<char> input)
    {
        if (!TryParseResize(ref input, out int width, out int height))
        {
            return null;
        }

        return new WindowResizeEventArgs(width, height);
    }

    private static bool TryParseResize(ref ReadOnlySpan<char> input, out int width, out int height)
    {
        width = height = 0;

        int pos = 0;
        if (!input.TryReadString(ref pos, "\x1b[") || !input.TryReadNumber(ref pos, out int type) || type != 8 && type != 48)
        {
            return false;
        }

        if (!input.TryReadString(ref pos, ";") || !input.TryReadNumber(ref pos, out height))
        {
            return false;
        }

        if (!input.TryReadString(ref pos, ";") || !input.TryReadNumber(ref pos, out width))
        {
            return false;
        }

        while (pos < input.Length && input[pos] != 't')
        {
            pos++;
        }
        if (pos >= input.Length)
        {
            return false;
        }

        input = input[(pos + 1)..];
        return true;
    }
}
