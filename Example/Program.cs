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
    new LogInputProvider());

driver.OnEvent += Console.WriteLine;
driver.OnEvent += eventArgs =>
{
    if (eventArgs is MouseButtonEventArgs args && args.Button == Button.Middle)
    {
        Environment.Exit(0);
    }
};

while (true)
{
    driver.PollInput();
    // await ConsoleDriver.PollInputAsync(100);
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