namespace AudioControl
{
    public interface IAudioBalanceService
    {
        AudioBalanceResult ApplyBalance(string gameApps, string chatApps, float inputVolume, bool invert);
    }
}
