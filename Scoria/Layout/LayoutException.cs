namespace Scoria.Layout;

/// <summary>
/// Exception thrown when an error occurs during layout resolution.
/// </summary>
public class LayoutException : Exception
{
    /// <summary>
    /// Initializes a new <see cref="LayoutException"/> with the specified message.
    /// </summary>
    /// <param name="message">The error message describing the layout failure.</param>
    public LayoutException(string message) : base(message) { }
}

// TODO: Fix this ugly vibe coded shit.
/// <summary>
/// Exception thrown when a circular dependency is detected during layout resolution.
/// </summary>
public sealed class LayoutCycleException : LayoutException
{
    /// <summary>
    /// Initializes a new <see cref="LayoutCycleException"/> from the detected dependency cycle.
    /// </summary>
    /// <param name="cycle">The sequence of layout properties forming the cycle.</param>
    public LayoutCycleException(IEnumerable<LayoutProperty> cycle): base(GetMessage(cycle))
    {
        
    }

    private static string GetMessage(IEnumerable<LayoutProperty> cycle)
    {
        string chain = string.Join("->", cycle.Take(10));
        return $"Layout cycle detected {chain}";
    }
}
