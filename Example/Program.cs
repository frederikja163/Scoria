using System.Text;
using Scoria;
using Scoria.Drivers;
using Scoria.Drivers.Providers;
using Scoria.Events;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

ConsoleDriver driver = new ConsoleDriver(
    new FocusInputProvider(),
    new KeyInputProvider(),
    new MouseInputProvider(),
    new PasteInputProvider(),
    new ResizeInputProvider()
    // new LogInputProvider(),
);

// driver.OnEvent += Console.WriteLine;
driver.OnEvent += eventArgs =>
{
    if (eventArgs is MouseButtonEventArgs args && args.Button == Button.Middle)
    {
        Environment.Exit(0);
    }
};

Surface surface = new Surface(driver.Width, driver.Height);
surface.Fill(' ', new Style(255, 255, 255, 30, 30, 40));

PanelElement panel = new PanelElement
{
    X = 4,
    Y = 2,
    Width = 36,
    Height = 8,
    Title = "Panel",
};
panel.AddChild(new TextElement
{
    X = 6,
    Y = 4,
    Text = "Hello, Scoria!",
    Style = new Style { ForegroundRed = 120, ForegroundGreen = 255, ForegroundBlue = 150 },
});
panel.AddChild(new TextElement
{
    X = 6,
    Y = 5,
    Text = "Rendered from elements",
    Style = new Style(StyleAttributes.Bold) { ForegroundRed = 255, ForegroundGreen = 200, ForegroundBlue = 100 },
});

while (true)
{
    panel.Render(surface);
    surface.ExpandBorders();
    driver.Frame(surface);
    driver.PollInput();
}


class LogInputProvider : IInputProvider
{
    public int Order => int.MinValue;
    public bool Enable => true;
    public void Init(IConsoleDriver driver)
    {
        
    }

    public void Restore(IConsoleDriver driver)
    {
    }

    public EventArgs? HandleInput(ref ReadOnlySpan<char> input)
    {
        Console.WriteLine($"Unmatched input ({input.Length} chars): {Format(input)}");
        input = ReadOnlySpan<char>.Empty;
        return null;
    }

    private static string Format(ReadOnlySpan<char> input)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in input)
        {
            sb.Append(char.IsControl(c) ? $"\\x{(int)c:X2}" : c.ToString());
            sb.Append(" ");
        }
        return sb.ToString();
    }
}