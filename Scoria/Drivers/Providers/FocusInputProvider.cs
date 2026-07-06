namespace Scoria.Drivers.Providers;

internal sealed class FocusInputProvider : IInputProvider
{
    private const string FocusLost = $"\x1b[O";
    private const string FocusGained = $"\x1b[I";

    public bool Enable => true;
    public int Order => 0;

    public void Init()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.FocusEvents, true);
    }

    public void Restore()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.FocusEvents, false);
    }

    public EventArgs? HandleInput(string input)
    {
        return input switch
        {
            FocusLost => new FocusChangedEventArgs(false),
            FocusGained => new FocusChangedEventArgs(true),
            _ => null,
        };
    }
}