using Scoria.Events;

namespace Scoria.Drivers.Providers;

internal sealed class FocusInputProvider : IInputProvider
{
    private const string FocusLost = $"\x1b[O";
    private const string FocusGained = $"\x1b[I";

    public int Order => 0;
    public bool Enable => true;
    public void Init(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.FocusEvents, true);
    }

    public void Restore(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.FocusEvents, false);
    }

    public AnyEventArgs? HandleInput(ref ReadOnlySpan<char> input)
    {
        int pos = 0;
        if (input.TryReadString(ref pos, FocusLost))
        {
            input = input[pos..];
            return new FocusChangedEventArgs(false);
        }

        pos = 0;
        if (input.TryReadString(ref pos, FocusGained))
        {
            input = input[pos..];
            return new FocusChangedEventArgs(true);
        }
        return null;
    }
}