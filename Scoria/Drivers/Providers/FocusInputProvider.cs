namespace Scoria.Drivers.Providers;

internal sealed class MatchConstInputProvider : IInputProvider
{
    private readonly string _value;
    private readonly EventArgs _constValue;

    internal MatchConstInputProvider(string value, EventArgs constValue)
    {
        _value = value;
        _constValue = constValue;
    }

    public EventArgs? HandleInput(string input)
    {
        return input == _value ? _constValue : null;
    }
}