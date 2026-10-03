using Scoria.Drivers;
using Scoria.Drivers.Providers;
using Scoria.Elements;
using Scoria.Events;
using Scoria.Layout;

namespace Scoria;

/// <summary>
/// Options used to configure a new <see cref="Application"/>.
/// </summary>
public sealed class ApplicationOptions
{
    /// <summary>The root window the application renders and routes input to.</summary>
    public Window Window { get; init; } = new Window();
}

/// <summary>
/// Manages the main loop of a terminal application: rendering the window,
/// polling input from the console driver, and dispatching events.
/// </summary>
public sealed class Application
{
    /// <summary>Gets the currently running application, or <see langword="null"/> if none is running.</summary>
    public static Application? Current { get; private set; }

    private readonly ConsoleDriver _driver = new(
        new FocusInputProvider(),
        new KeyInputProvider(),
        new MouseInputProvider(),
        new PasteInputProvider());

    /// <summary>
    /// Initializes a new <see cref="Application"/> with the specified options.
    /// </summary>
    /// <param name="options">The configuration options, including the root window.</param>
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

    /// <summary>Gets the root window of the application.</summary>
    public Window Window { get; private set; }
    /// <summary>Gets or sets the element that receives input events by default.</summary>
    public Element ActiveElement { get; private set; }

    /// <summary>Gets a value indicating whether the application main loop is running.</summary>
    public bool IsRunning { get; private set; }


    /// <summary>
    /// Starts the application main loop, rendering frames and polling input until <see cref="Stop"/> is called.
    /// </summary>
    public void Start()
    {
        IsRunning = true;
        Current = this;
        _driver.OnEvent += DriverOnOnEvent;
        while (IsRunning)
        {
            // TODO: Because resize does not stop the polling we need a timespan here.
            // The correct solution would be for the platform drivers to stop the polling when resize is detected.
            _driver.PollInput(TimeSpan.FromMilliseconds(100));
            Surface surface = Window.GetSurface(_driver.Width, _driver.Height);
            _driver.Frame(surface);
        }

        _driver.OnEvent -= DriverOnOnEvent;
        Current = null;
    }

    /// <summary>Stops the application main loop, causing <see cref="Start"/> to return.</summary>
    public void Stop()
    {
        IsRunning = false;
    }

    /// <summary>Sets the element that receives input events by default.</summary>
    /// <param name="element">The element to make active.</param>
    public void SetActive(Element element)
    {
        ActiveElement = element;
    }
}
