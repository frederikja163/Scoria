namespace Scoria.Drivers.Providers;

internal sealed class PasteInputProvider : IInputProvider
{
    private const string PasteStart = "\x1b[200~";
    private const string PasteStop = "\x1b[201~";
    
    public bool Enable => true;
    public int Order => 0;

    public void Init()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.BracketedPaste, true);
    }

    public void Restore()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.BracketedPaste, false);
    }

    public EventArgs? HandleInput(string input)
    {
        return input.StartsWith(PasteStart) && input.EndsWith(PasteStop)
            ? new PasteEventArgs(input[PasteStart.Length..^PasteStop.Length])
            : null;
    }
}