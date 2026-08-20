using Scoria.Events;

namespace Scoria.Drivers.Providers;

internal sealed class PasteInputProvider : IInputProvider
{
    private const string PasteStart = "\x1b[200~";
    private const string PasteStop = "\x1b[201~";

    public bool Enable => true;
    public int Order => 0;

    public void Init(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.BracketedPaste, true);
    }

    public void Restore(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.BracketedPaste, false);
    }

    public AnyEventArgs? HandleInput(ref ReadOnlySpan<char> input)
    {
        int pos = 0;
        if (!input.TryReadString(ref pos, PasteStart))
        {
            return null;
        }

        int stopIndex = input[pos..].IndexOf(PasteStop);
        if (stopIndex < 0)
        {
            return null;
        }
        stopIndex += pos;

        string text = input[pos..stopIndex].ToString();
        input = input[(stopIndex + PasteStop.Length)..];
        return new PasteEventArgs(text);
    }
}
