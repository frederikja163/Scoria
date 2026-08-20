using Scoria.Drivers;
using Scoria.Elements;
using Scoria.Events;

namespace Scoria;

public sealed class ApplicationOptions
{
    public Window Window { get; init; } = new Window();
}

public sealed class Application
{
    public static Application? Current { get; private set; }
    
    private readonly ConsoleDriver _driver = new();

    public Application(ApplicationOptions options)
    {
        Window = options.Window;
        ActiveElement = Window;
        _driver.OnEvent += DriverOnOnEvent;
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

        while (IsRunning)
        {
            Surface surface = new Surface(_driver.Width, _driver.Height);
            Window.Render(surface);
            _driver.Frame(surface);
            _driver.PollInput();
        }
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