namespace Scoria.Events;

/// <summary>Provides data for terminal bracket-paste events.</summary>
public sealed class PasteEventArgs : EventArgs
{
    internal PasteEventArgs(string text)
    {
        Text = text;
    }
    
    /// <summary>The text that was pasted into the terminal.</summary>
    public string Text { get; }

    /// <summary>Returns a string representation of the paste event.</summary>
    /// <returns>A string containing the pasted text.</returns>
    public override string ToString()
    {
        return $"Pasted '{Text}'";
    }
}