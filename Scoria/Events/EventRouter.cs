using Scoria.Elements;

namespace Scoria.Events;
using EventMap = Dictionary<Type, Delegate?>;

public enum EventPhase
{
    Capture,
    Target,
    Bubble,
}

public sealed class EventRouter(Element element)
{
    private readonly Element _element;
    private Dictionary<EventPhase, EventMap> _allEvents = Enum.GetValues<EventPhase>().ToDictionary(p => p, _ => new EventMap());

    public void Add<T>(EventDelegate<T> eventHandler, EventPhase phase = EventPhase.Target) where T : AnyEventArgs
    {
        EventMap map = _allEvents[phase];
        map[typeof(T)] = Delegate.Combine(map.GetValueOrDefault(typeof(T)), eventHandler);
    }
    
    public void Remove<T>(EventDelegate<T> eventHandler, EventPhase phase = EventPhase.Target) where T : AnyEventArgs
    {
        EventMap map = _allEvents[phase];
        map[typeof(T)] = Delegate.Remove(map.GetValueOrDefault(typeof(T)), eventHandler);
    }

    private void Dispatch<T>(T eventArgs, EventPhase phase) where T : AnyEventArgs
    {
        eventArgs.CurrentTarget = _element;
        EventDelegate<T>? ev = (EventDelegate<T>?)_allEvents[phase].GetValueOrDefault(typeof(T));
        ev?.Invoke(eventArgs);
        EventDelegate<T>? anyEv = (EventDelegate<T>?)_allEvents[phase].GetValueOrDefault(typeof(AnyEventArgs));
        anyEv?.Invoke(eventArgs);
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
