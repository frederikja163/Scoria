namespace Scoria.Drivers.Providers;

internal sealed class PasteInputProvider : IInputProvider
{
    private const string PasteStart = "\x1b[200~";
    private const string PasteStop = "\x1b[201~";
    
    public EventArgs? HandleInput(string input)
    {
        return input.StartsWith(PasteStart) && input.EndsWith(PasteStop)
            ? new PasteEventArgs(input[PasteStart.Length..^PasteStop.Length])
            : null;
    }
}