namespace Scoria.Drivers.Providers;

internal interface IInputProvider
{
    public EventArgs? HandleInput(string input);
}