using Scoria.Drivers;
using Scoria.Drivers.Providers;
using Scoria.Elements;
using Scoria.Events;
using Scoria.Layout;

namespace Scoria;

public sealed class ApplicationOptions
{
    public Window Window { get; init; } = new Window();
}

public sealed class Application
{
    public static Application? Current { get; private set; }
    
    private readonly ConsoleDriver _driver = new(
        new FocusInputProvider(),
        new KeyInputProvider(),
        new MouseInputProvider(),
        new PasteInputProvider(),
        new ResizeInputProvider());

    public Application(ApplicationOptions options)
    {
        Window = options.Window;
        ActiveElement = Window;
    }

    private void DriverOnOnEvent(AnyEventArgs args)
    {
        args.Target = ActiveElement;
        EventRouter.Dispatch(args);
    }

    public Window Window { get; private set; }
    public Element ActiveElement { get; private set; }
    
    public bool IsRunning { get; private set; }
    
    
    public void Start()
    {
        IsRunning = true;
        Current = this;
        _driver.OnEvent += DriverOnOnEvent;
        while (IsRunning)
        {
            Surface surface = Window.GetSurface(_driver.Width, _driver.Height);
            _driver.Frame(surface);
            _driver.PollInput();
        }

        _driver.OnEvent -= DriverOnOnEvent;
        Current = null;
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public void SetActive(Element element)
    {
        ActiveElement = element;
    }
}