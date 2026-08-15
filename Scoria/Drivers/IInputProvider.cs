namespace Scoria.Drivers;

internal interface IInputProvider
{
    public int Order { get; }
    public bool Enable { get; }
    public void Init(IConsoleDriver driver);
    public void Restore(IConsoleDriver driver);
    public EventArgs? HandleInput(ref ReadOnlySpan<char> input);
}