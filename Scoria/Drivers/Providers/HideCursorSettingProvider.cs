using Scoria.Events;

namespace Scoria.Drivers.Providers;

internal sealed class HideCursorSettingProvider : IInputProvider
{
    public int Order => int.MinValue;
    public bool Enable => true;

    public void Init(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.ShowCursor, false);
    }

    public void Restore(IConsoleDriver driver)
    {
        driver.Enable(PrivateMode.ShowCursor, true);
    }

    public AnyEventArgs? HandleInput(ref ReadOnlySpan<char> input)
    {
        return null;
    }
}
