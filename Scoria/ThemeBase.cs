using System.Runtime.CompilerServices;

namespace Scoria;

public abstract class ThemeBase<T> : ICloneable
    where T : ThemeBase<T>
{
    private readonly Dictionary<string, object> _map = [];

    protected ThemeBase()
    {

    }

    protected ThemeBase(ThemeBase<T> original)
    {
        foreach ((string key, object value) in original._map)
        {
            _map[key] = value is ICloneable cloneable ? cloneable.Clone() : value;
        }
    }

    internal TValue Get<TValue>(TValue defaultValue, bool addIfNotFound = false, [CallerMemberName] string memberName = "")
        where TValue : notnull
    {
        return Get(() => defaultValue, addIfNotFound, memberName);
    }

    internal TValue Get<TValue>(Func<TValue> defaultFactory, bool addIfNotFound = false, [CallerMemberName] string memberName = "")
        where TValue : notnull
    {
        return Get($"{nameof(Scoria)}.{memberName}", defaultFactory, addIfNotFound);
    }

    public TValue Get<TValue>(string name, TValue defaultValue, bool addIfNotFound = false)
        where TValue : notnull
    {
        return Get(name, () => defaultValue, addIfNotFound);
    }

    public TValue Get<TValue>(string name, Func<TValue> defaultFactory, bool addIfNotFound = false)
        where TValue : notnull
    {
        if (_map.TryGetValue(GetFullName<TValue>(name), out object? value) && value is TValue typedValue)
        {
            return typedValue;
        }
        TValue defaultValue = defaultFactory();
        if (addIfNotFound)
            _map[GetFullName<TValue>(name)] = defaultValue;
        return defaultValue;
    }

    internal void Set<TValue>(TValue value, [CallerMemberName] string memberName = "")
        where TValue : notnull
    {
        Set($"{nameof(Scoria)}.{memberName}", value);
    }

    public void Set<TValue>(string name, TValue value)
        where TValue : notnull
    {
        _map[GetFullName<TValue>(name)] = value;
    }

    private string GetFullName<TValue>(string name)
    {
        return typeof(TValue).FullName + " " + name;
    }

    public abstract T Clone();

    object ICloneable.Clone()
    {
        return Clone();
    }
}

