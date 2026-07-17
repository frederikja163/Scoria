namespace Scoria.Events;

/// <summary>Provides data for terminal focus-gained and focus-lost events.</summary>
public sealed class FocusChangedEventArgs : EventArgs
{
    internal FocusChangedEventArgs(bool focused)
    {
        Focused = focused;
    }

    /// <summary>Indicates whether the terminal has gained (<see langword="true"/>) or lost (<see langword="false"/>) focus.</summary>
    public bool Focused { get; }

    /// <summary>Returns a string representation of the focus change.</summary>
    /// <returns>"Focus Gained" or "Focus Lost".</returns>
    public override string ToString()
    {
        string focused = Focused ? "Gained" : "Lost";
        return $"Focus {focused}";
    }
}