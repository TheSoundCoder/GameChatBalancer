namespace AudioControl
{
    public interface IAudioSessionService
    {
        event EventHandler? AudioSessionsChanged;

        string GetAudioApplications(bool includingPid);
        void SetApplicationVolumeByName(string appName, float level);
        void StartSessionMonitoring();
        void StopSessionMonitoring();
    }
}
