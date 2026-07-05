using Scoria;

ConsoleDriver.OnMouseMove += Console.WriteLine;
ConsoleDriver.OnMouseButton += Console.WriteLine;
ConsoleDriver.OnMouseButton += eventArgs =>
{
    if (eventArgs.Button == Button.Middle && eventArgs.Down)
    {
        Environment.Exit(0);
    }
};

while (true)
{
    ConsoleDriver.PollInput();
    // await ConsoleDriver.PollInputAsync(100);
}
