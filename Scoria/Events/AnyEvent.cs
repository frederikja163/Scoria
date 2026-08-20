using Scoria.Elements;

namespace Scoria.Events;

public abstract class AnyEventArgs : EventArgs
{
    public Element Target { get; internal set; } = null!;
    public Element CurrentTarget { get; internal set; } = null!;
    public bool PropagationStopped { get; private set; } = true;

    public void StopPropagation()
    {
        PropagationStopped = true;
    }
}
