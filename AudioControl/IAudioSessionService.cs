namespace AudioControl
{
    public interface IAudioSessionService
    {
        string GetAudioApplications(bool includingPid);
        void SetApplicationVolumeByName(string appName, float level);
    }
}
