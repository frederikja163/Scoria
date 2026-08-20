namespace Scoria.Events;

public delegate void EventDelegate<in T>(T args) where T : AnyEventArgs;