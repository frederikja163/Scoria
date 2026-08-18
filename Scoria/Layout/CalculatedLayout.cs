namespace Scoria.Layout;

/// <summary>
/// Holds the final resolved position and size of an element after layout calculation.
/// </summary>
public sealed class CalculatedLayout
{
    /// <summary>The resolved horizontal position.</summary>
    public int X { get; private set; }
    /// <summary>The resolved vertical position.</summary>
    public int Y { get; private set; }
    /// <summary>The resolved width.</summary>
    public int Width { get; private set; }
    /// <summary>The resolved height.</summary>
    public int Height { get; private set; }

    internal void SetProperty(LayoutPropertyType layoutPropertyType, int value)
    {
        switch (layoutPropertyType)
        {
            case LayoutPropertyType.X:
                X = value;
                break;
            case LayoutPropertyType.Y:
                Y = value;
                break;
            case LayoutPropertyType.Width:
                Width = value;
                break;
            case LayoutPropertyType.Height:
                Height = value;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}