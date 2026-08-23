using Scoria.Elements;

namespace Scoria.Events;

/// <summary>
/// Base class for all routed event arguments. Tracks the event target and supports
/// stopping propagation through the element tree.
/// </summary>
public abstract class AnyEventArgs : EventArgs
{
    /// <summary>Gets the element the event was originally dispatched to.</summary>
    public Element Target { get; internal set; } = null!;
    /// <summary>Gets the element currently handling the event during routing.</summary>
    public Element CurrentTarget { get; internal set; } = null!;
    /// <summary>Gets a value indicating whether propagation has been stopped. Defaults to <see langword="true"/> until dispatch begins.</summary>
    public bool PropagationStopped { get; private set; } = false;

    /// <summary>Prevents the event from being handled by any further elements in the route.</summary>
    public void StopPropagation()
    {
        PropagationStopped = true;
    }
}
