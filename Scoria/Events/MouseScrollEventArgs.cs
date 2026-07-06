namespace Scoria.Drivers;

/// <summary>Provides data for mouse scroll wheel events.</summary>
public sealed class MouseScrollEventArgs : EventArgs
{
    internal MouseScrollEventArgs(int x, int y, bool down)
    {
        X = x;
        Y = y;
        Down = down;
    }
    
    /// <summary>The X coordinate of the mouse cursor at the time of the scroll.</summary>
    public int X { get; }
    /// <summary>The Y coordinate of the mouse cursor at the time of the scroll.</summary>
    public int Y { get; }
    /// <summary><see langword="true"/> if the wheel was scrolled down; <see langword="false"/> if scrolled up.</summary>
    public bool Down { get; }

    /// <summary>Returns a string representation of the scroll event.</summary>
    /// <returns>"Mouse scroll down" or "Mouse scroll up".</returns>
    public override string ToString()
    {
        string down = Down ? "down" : "up";
        return $"Mouse scroll {down}";
    }
}