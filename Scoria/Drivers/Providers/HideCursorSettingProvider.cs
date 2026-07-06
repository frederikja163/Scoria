namespace Scoria.Drivers.Providers;

internal sealed class HideCursorSettingProvider : ISettingProvider
{
    public int Order => 0;
    public bool Enable => true;
    public void Init()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.ShowCursor, false);
    }

    public void Restore()
    {
        ConsoleDriver.Enable(ConsoleDriver.PrivateMode.ShowCursor, true);
    }
}