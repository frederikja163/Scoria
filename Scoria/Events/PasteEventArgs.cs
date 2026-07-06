namespace Scoria.Drivers;

public sealed class PasteEventArgs : EventArgs
{
    internal PasteEventArgs(string text)
    {
        Text = text;
    }
    
    public string Text { get; }

    public override string ToString()
    {
        return $"Pasted '{Text}'";
    }
}