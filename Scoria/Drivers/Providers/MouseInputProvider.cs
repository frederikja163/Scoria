using System.Text.RegularExpressions;

namespace Scoria.Drivers.Providers;

internal sealed class MouseInputProvider : IInputProvider
{
    private static readonly Regex MouseInputRegex = new Regex(@"\x1b\[<(?<cb>\d+);(?<cx>\d+);(?<cy>\d+)(?<ev>[mM])", RegexOptions.Compiled);
    private static int _mouseX = int.MaxValue;
    private static int _mouseY = int.MaxValue;
    
    public bool Enable => true;
    public int Order => 0;

    public void Init()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.SgrMouse, true);
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.AnyEventMouse, true);
    }

    public void Restore()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.SgrMouse, false);
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.AnyEventMouse, false);
    }

    public EventArgs? HandleInput(string input)
    {
        Match mouseMatch = MouseInputRegex.Match(input);
        if (mouseMatch.Success)
        {
            GroupCollection collection = mouseMatch.Groups;
            int cb = int.Parse(collection["cb"].ValueSpan);
            int cx = int.Parse(collection["cx"].ValueSpan);
            int cy = int.Parse(collection["cy"].ValueSpan);
            _mouseX = _mouseX == int.MaxValue ? cx : _mouseX;
            _mouseY = _mouseY == int.MaxValue ? cy : _mouseY;
            bool down = collection["ev"].Value == "M";
            
            if ((cb & 32) == 32)
            {
                EventArgs args = new MouseMoveEventArgs(cx, cy, _mouseX, _mouseY);
                _mouseX = cx;
                _mouseY = cy;
                return args;
            }
            if ((Button)cb is Button.Left or Button.Right or Button.Middle)
            {
                return new MouseButtonEventArgs((Button)cb, _mouseX, _mouseY, down);
            }
            if (cb is 64 or 65)
            {
                return new MouseScrollEventArgs(_mouseX, _mouseY, cb == 65);
            }
        }

        return null;
    }
}