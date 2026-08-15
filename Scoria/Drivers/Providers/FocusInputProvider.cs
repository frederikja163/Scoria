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

    public EventArgs? HandleInput(ref ReadOnlySpan<char> input)
    {
        if (input.StartsWith(FocusLost))
        {
            input = input[FocusLost.Length..];
            return new FocusChangedEventArgs(false);
        }

        if (input.StartsWith(FocusGained))
        {
            input = input[FocusGained.Length..];
            return new FocusChangedEventArgs(true);
        }
        return null;
    }
}