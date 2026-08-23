namespace Scoria.Events;

/// <summary>Represents a method that handles routed events of type <typeparamref name="T"/>.</summary>
/// <param name="args">The event arguments.</param>
public delegate void EventDelegate<in T>(T args) where T : AnyEventArgs;