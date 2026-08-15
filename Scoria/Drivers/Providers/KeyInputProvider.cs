using Scoria.Events;

namespace Scoria.Drivers.Providers;

internal sealed class KeyInputProvider : IInputProvider
{
    public int Order => 0;
    public bool Enable => true;
    public void Init()
    {
        ConsoleDriver.Write("\x1b[>4;2m");
    }

    public void Restore()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.ApplicationCursorKeys, false);
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.ApplicationKeypad, false);
    }

    public EventArgs? HandleInput(string input)
    {
        (Key? key, char? ch) = HandleEscapeSequence(input);

        if (input.Length == 1)
        {
            key = Key.FromChar(input[0], out ch);
        }

        if (key is not null)
        {
            return new KeyEventArgs(key.Value, ch);
        }

        return null;
    }

    private static (Key? key, char? ch) HandleEscapeSequence(string input)
    {
        if (input.StartsWith("\x1b"))
        {
            return input switch
            {
                "\x1b[A" => (Key.Up, null),
                "\x1b[B" => (Key.Down, null),
                "\x1b[C" => (Key.Right, null),
                "\x1b[D" => (Key.Left, null),
                "\x1b[2\x7E" => (Key.Insert, null),
                "\x1b[3\x7E" => (Key.Delete, null),
                "\x1b[5\x7E" => (Key.PageUp, null),
                "\x1b[6\x7E" => (Key.PageDown, null),
                "\x1b[H" => (Key.Home, null),
                "\x1b[F" => (Key.End, null),
                "\x1bOP" => (Key.F1, null),
                "\x1bOQ" => (Key.F2, null),
                "\x1bOR" => (Key.F3, null),
                "\x1bOS" => (Key.F4, null),
                "\x1b[15\x7E" => (Key.F5, null),
                "\x1b[16\x7E" => (Key.F6, null),
                "\x1b[17\x7E" => (Key.F7, null),
                "\x1b[18\x7E" => (Key.F8, null),
                "\x1b[19\x7E" => (Key.F9, null),
                "\x1b[20\x7E" => (Key.F10, null),
                "\x1b[21\x7E" => (Key.F11, null),
                "\x1b[22\x7E" => (Key.F12, null),
                _ => (null, null),
            };
        }

        return (null, null);
    }
}