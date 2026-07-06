namespace Scoria.Drivers.Providers;

internal interface ISettingProvider
{
    public int Order { get; }
    public bool Enable { get; }
    public void Init();
    public void Restore();
}