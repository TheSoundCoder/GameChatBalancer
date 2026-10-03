namespace AudioControl
{
    public interface IAutoStartService
    {
        bool IsEnabled();
        bool TrySetEnabled(bool enabled, out string? errorMessage);
    }
}
