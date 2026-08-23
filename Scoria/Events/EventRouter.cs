using Scoria.Elements;

namespace Scoria.Events;
using EventMap = Dictionary<Type, Delegate?>;

/// <summary>
/// The phase of event routing in the element tree.
/// </summary>
public enum EventPhase
{
    /// <summary>The event is traveling down from the root towards the target.</summary>
    Capture,
    /// <summary>The event has reached the target element.</summary>
    Target,
    /// <summary>The event is traveling back up from the target towards the root.</summary>
    Bubble,
}

/// <summary>
/// Manages event handler subscriptions for a single element and dispatches routed events
/// through capture, target, and bubble phases.
/// </summary>
public sealed class EventRouter(Element element)
{
    private readonly Element _element;
    private Dictionary<EventPhase, EventMap> _allEvents = Enum.GetValues<EventPhase>().ToDictionary(p => p, _ => new EventMap());

    /// <summary>Adds an event handler for events of type <typeparamref name="T"/>.</summary>
    /// <param name="eventHandler">The handler to add.</param>
    /// <param name="phase">The routing phase in which the handler runs. Defaults to <see cref="Events.EventPhase.Target"/>.</param>
    public void Add<T>(EventDelegate<T> eventHandler, EventPhase phase = EventPhase.Target) where T : AnyEventArgs
    {
        EventMap map = _allEvents[phase];
        map[typeof(T)] = Delegate.Combine(map.GetValueOrDefault(typeof(T)), eventHandler);
    }

    /// <summary>Removes a previously added event handler for events of type <typeparamref name="T"/>.</summary>
    /// <param name="eventHandler">The handler to remove.</param>
    /// <param name="phase">The routing phase the handler was added to. Defaults to <see cref="Events.EventPhase.Target"/>.</param>
    public void Remove<T>(EventDelegate<T> eventHandler, EventPhase phase = EventPhase.Target) where T : AnyEventArgs
    {
        EventMap map = _allEvents[phase];
        map[typeof(T)] = Delegate.Remove(map.GetValueOrDefault(typeof(T)), eventHandler);
    }

    private void Dispatch<T>(T eventArgs, EventPhase phase) where T : AnyEventArgs
    {
        eventArgs.CurrentTarget = _element;
        _allEvents[phase].GetValueOrDefault(eventArgs.GetType())?.DynamicInvoke(eventArgs);
        _allEvents[phase].GetValueOrDefault(typeof(AnyEventArgs))?.DynamicInvoke(eventArgs);
    }

    internal static void Dispatch(AnyEventArgs eventArgs)
    {
        List<Element> ancestors = GetAncestors(eventArgs.Target).ToList();

        foreach (Element element in ancestors.AsEnumerable().Reverse())
        {
            element.Events.Dispatch(eventArgs, EventPhase.Capture);
            if (eventArgs.PropagationStopped)
                return;
        }

        eventArgs.Target.Events.Dispatch(eventArgs, EventPhase.Target);
        if (eventArgs.PropagationStopped)
            return;
        
        foreach (Element element in ancestors)
        {
            element.Events.Dispatch(eventArgs, EventPhase.Bubble);
            if (eventArgs.PropagationStopped)
                return;
        }
    }

    private static IEnumerable<Element> GetAncestors(Element target)
    {
        while (target.Parent is {} parent)
        {
            yield return parent;
            target = parent;
        }
    }
}
