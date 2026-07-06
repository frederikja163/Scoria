namespace Scoria.Drivers.Providers;

internal interface IInputProvider : ISettingProvider
{
    public EventArgs? HandleInput(string input);
}