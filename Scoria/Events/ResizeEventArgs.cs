namespace Scoria.Events;

/// <summary>Provides data for terminal text-area resize events.</summary>
public sealed class ResizeEventArgs : AnyEventArgs
{
    internal ResizeEventArgs(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>The width of the terminal text area in character cells.</summary>
    public int Width { get; }
    /// <summary>The height of the terminal text area in character cells.</summary>
    public int Height { get; }

    /// <summary>Returns a string representation of the resize event.</summary>
    /// <returns>A string containing the new text-area dimensions.</returns>
    public override string ToString()
    {
        return $"Resized to {Width}x{Height}";
    }
}
