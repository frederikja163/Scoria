namespace Scoria.Events;

/// <summary>Provides data for mouse move events.</summary>
public sealed class MouseMoveEventArgs : EventArgs
{
    internal MouseMoveEventArgs(int x, int y, int prevX, int prevY)
    {
        X = x;
        Y = y;
        PrevX = prevX;
        PrevY = prevY;
    }

    /// <summary>The current X coordinate of the mouse cursor.</summary>
    public int X { get; }
    /// <summary>The current Y coordinate of the mouse cursor.</summary>
    public int Y { get; }
    /// <summary>The previous X coordinate of the mouse cursor.</summary>
    public int PrevX { get; }
    /// <summary>The previous Y coordinate of the mouse cursor.</summary>
    public int PrevY { get; }
    /// <summary>The horizontal distance moved since the last event.</summary>
    public int DeltaX => X - PrevX;
    /// <summary>The vertical distance moved since the last event.</summary>
    public int DeltaY => Y - PrevY;

    /// <summary>Returns a string representation of the mouse move event.</summary>
    /// <returns>A string showing the previous and current cursor positions.</returns>
    public override string ToString()
    {
        return $"Mouse move ({PrevX}, {PrevY}) -> ({X}, {Y})";
    }
}