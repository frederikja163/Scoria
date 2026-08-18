namespace Scoria.Layout;

public class LayoutException : Exception
{
    public LayoutException(string message) : base(message) { }
}

// TODO: Fix this ugly vibe coded shit.
public sealed class LayoutCycleException : LayoutException
{
    public LayoutCycleException(IEnumerable<LayoutProperty> cycle): base(GetMessage(cycle))
    {
        
    }

    private static string GetMessage(IEnumerable<LayoutProperty> cycle)
    {
        string chain = string.Join("->", cycle.Take(10));
        return $"Layout cycle detected {chain}";
    }
}
