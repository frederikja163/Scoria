using Scoria;
using Scoria.Drivers;

ConsoleDriver.OnEvent += Console.WriteLine;
ConsoleDriver.OnEvent += eventArgs =>
{
    if (eventArgs is MouseButtonEventArgs args && args.Button == Button.Middle)
    {
        Environment.Exit(0);
    }
};

while (true)
{
    ConsoleDriver.PollInput();
    // await ConsoleDriver.PollInputAsync(100);
}
