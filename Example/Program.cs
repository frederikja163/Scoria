using System.Text;
using Scoria;
using Scoria.Drivers;
using Scoria.Events;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

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
