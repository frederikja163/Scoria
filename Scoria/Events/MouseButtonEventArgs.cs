namespace Scoria.Drivers;

/// <summary>Provides data for mouse button press and release events.</summary>
public sealed class MouseButtonEventArgs : EventArgs
{
    internal MouseButtonEventArgs(Button button, int x, int y, bool down)
    {
        Button = button;
        X = x;
        Y = y;
        Down = down;
    }

    /// <summary>The mouse button that was pressed or released.</summary>
    public Button Button { get; }
    /// <summary>The X coordinate of the mouse cursor at the time of the event.</summary>
    public int X { get; }
    /// <summary>The Y coordinate of the mouse cursor at the time of the event.</summary>
    public int Y { get; }
    /// <summary><see langword="true"/> if the button was pressed; <see langword="false"/> if released.</summary>
    public bool Down { get; }

    /// <summary>Returns a string representation of the mouse button event.</summary>
    /// <returns>A string describing the button, action, and cursor position.</returns>
    public override string ToString()
    {
        string down = Down ? "down" : "up";
        return $"Mouse {Button} {down} ({X}, {Y})";
    }
}