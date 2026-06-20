using Scoria;

Style style1 = new Style(80, 20, 250, StyleAttributes.None);
Style style2 = new Style(255, 20, 250, StyleAttributes.Bold | StyleAttributes.DoubleUnderline);

using (FrameDriver driver = ConsoleDriver.StartFrame())
{
    driver.Clear();
    driver.ApplyStyle(style1);
    driver.Write("blue");
    driver.ApplyStyle(style2);
    driver.MoveTo(20, 30);
    driver.Write("magenta");
}

Console.Read();

using (FrameDriver driver = ConsoleDriver.StartFrame())
{
    driver.Clear();
}

Console.ReadKey();