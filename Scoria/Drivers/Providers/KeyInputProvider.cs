using Scoria.Events;

namespace Scoria.Drivers.Providers;

internal sealed class KeyInputProvider : IInputProvider
{
    private static readonly (string Sequence, Key Key)[] EscapeSequences =
    {
        ("\x1b[A", Key.Up),
        ("\x1b[B", Key.Down),
        ("\x1b[C", Key.Right),
        ("\x1b[D", Key.Left),
        ("\x1bOA", Key.Up),
        ("\x1bOB", Key.Down),
        ("\x1bOC", Key.Right),
        ("\x1bOD", Key.Left),
        ("\x1b[2~", Key.Insert),
        ("\x1b[3~", Key.Delete),
        ("\x1b[5~", Key.PageUp),
        ("\x1b[6~", Key.PageDown),
        ("\x1b[H", Key.Home),
        ("\x1b[F", Key.End),
        ("\x1bOP", Key.F1),
        ("\x1bOQ", Key.F2),
        ("\x1bOR", Key.F3),
        ("\x1bOS", Key.F4),
        ("\x1b[15~", Key.F5),
        ("\x1b[16~", Key.F6),
        ("\x1b[17~", Key.F7),
        ("\x1b[18~", Key.F8),
        ("\x1b[19~", Key.F9),
        ("\x1b[20~", Key.F10),
        ("\x1b[21~", Key.F11),
        ("\x1b[22~", Key.F12),
    };

    public int Order => 0;
    public bool Enable => true;

    public void Init(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.ApplicationCursorKeys, true);
        driver.Enable(PrivateMode.ApplicationKeypad, true);
    }

    public void Restore(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.ApplicationCursorKeys, false);
        driver.Enable(PrivateMode.ApplicationKeypad, false);
    }

    public EventArgs? HandleInput(ref ReadOnlySpan<char> input)
    {
        if (input.Length == 1)
        {
            Key key = Key.FromChar(input[0], out char? ch);
            if (key == Key.Unicode && ToCtrlKey(input[0]) is { } ctrlKey)
            {
                key = ctrlKey;
                ch = null;
            }
            input = input[1..];
            return new KeyEventArgs(key, ch);
        }

        if (input.StartsWith("\x1b") && input.Length == 2)
        {
            Key key = Key.FromChar(input[1], out char? ch) | Key.Alt;
            input = input[2..];
            return new KeyEventArgs(key, ch);
        }

        foreach ((string sequence, Key key) in EscapeSequences)
        {
            int pos = 0;
            if (input.TryReadString(ref pos, sequence))
            {
                input = input[pos..];
                return new KeyEventArgs(key, null);
            }
        }

        return null;
    }

    private static Key? ToCtrlKey(char c) => c switch
    {
        '\x00' => Key.Space | Key.Ctrl,
        >= '\x01' and <= '\x1A' => (Key)(c - '\x01' + (int)Key.A) | Key.Ctrl,
        _ => null,
    };
}
