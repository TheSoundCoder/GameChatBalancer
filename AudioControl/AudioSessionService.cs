namespace AudioControl
{
    public class AudioSessionService : IAudioSessionService
    {
        public AudioSessionService(Form1 form)
        {
            global::AudioManager.AudioManager.HandOverForm(form);
        }

        public string GetAudioApplications(bool includingPid)
        {
            return global::AudioManager.AudioManager.GetAudioApplications(includingPid);
        }

        public void SetApplicationVolumeByName(string appName, float level)
        {
            global::AudioManager.AudioManager.SetApplicationVolumeByName(appName, level);
        }
    }
}
