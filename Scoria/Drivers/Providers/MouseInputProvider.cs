using Scoria.Events;

namespace Scoria.Drivers.Providers;

internal sealed class MouseInputProvider : IInputProvider
{
    private static int _mouseX = int.MaxValue;
    private static int _mouseY = int.MaxValue;

    public bool Enable => true;
    public int Order => 0;

    public void Init(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.SgrMouse, true);
        driver.Enable(PrivateMode.AnyEventMouse, true);
    }

    public void Restore(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.SgrMouse, false);
        driver.Enable(PrivateMode.AnyEventMouse, false);
    }

    public AnyEventArgs? HandleInput(ref ReadOnlySpan<char> input)
    {
        if (!TryParseSgrMouse(ref input, out int cb, out int cx, out int cy, out bool down))
        {
            return null;
        }

        _mouseX = _mouseX == int.MaxValue ? cx : _mouseX;
        _mouseY = _mouseY == int.MaxValue ? cy : _mouseY;

        if ((cb & 32) == 32)
        {
            AnyEventArgs args = new MouseMoveEventArgs(cx, cy, _mouseX, _mouseY);
            _mouseX = cx;
            _mouseY = cy;
            return args;
        }
        if ((Button)cb is Button.Left or Button.Right or Button.Middle)
        {
            return new MouseButtonEventArgs((Button)cb, cx, cy, down);
        }
        if (cb is 64 or 65)
        {
            return new MouseScrollEventArgs(cx, cy, cb == 65);
        }

        return null;
    }

    private static bool TryParseSgrMouse(ref ReadOnlySpan<char> input, out int cb, out int cx, out int cy, out bool down)
    {
        cb = cx = cy = 0;
        down = false;

        int pos = 0;
        if (!input.TryReadString(ref pos, "\x1b[<"))
        {
            return false;
        }

        if (!input.TryReadNumber(ref pos, out cb) || !input.TryReadString(ref pos, ";"))
        {
            return false;
        }
        if (!input.TryReadNumber(ref pos, out cx) || !input.TryReadString(ref pos, ";"))
        {
            return false;
        }
        if (!input.TryReadNumber(ref pos, out cy) || pos >= input.Length || (input[pos] != 'm' && input[pos] != 'M'))
        {
            return false;
        }

        down = input[pos] == 'M';
        pos++;
        input = input[pos..];
        return true;
    }
}
