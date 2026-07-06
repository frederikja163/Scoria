namespace Scoria.Drivers;

internal interface IPlatformDriver
{
    void Init();
    void Restore();
    int PollInput(byte[] buffer, TimeSpan timeout);
    void Write(byte[] buffer);
}
