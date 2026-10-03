namespace AudioControl
{
    public class AudioSessionService : IAudioSessionService
    {
        public event EventHandler? AudioSessionsChanged;

        public AudioSessionService(IDiagnosticsSink diagnosticsSink)
        {
            global::AudioManager.AudioManager.HandOverDiagnosticsSink(diagnosticsSink);
            global::AudioManager.AudioManager.AudioSessionsChanged += (_, _) => AudioSessionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public string GetAudioApplications(bool includingPid)
        {
            return global::AudioManager.AudioManager.GetAudioApplications(includingPid);
        }

        public void SetApplicationVolumeByName(string appName, float level)
        {
            global::AudioManager.AudioManager.SetApplicationVolumeByName(appName, level);
        }

        public void StartSessionMonitoring()
        {
            global::AudioManager.AudioManager.StartAudioSessionMonitoring();
        }

        public void StopSessionMonitoring()
        {
            global::AudioManager.AudioManager.StopAudioSessionMonitoring();
        }
    }
}
