namespace AudioControl
{
    public class AudioBalanceService : IAudioBalanceService
    {
        private readonly IAudioSessionService audioSessionService;

        public AudioBalanceService(IAudioSessionService audioSessionService)
        {
            this.audioSessionService = audioSessionService;
        }

        public AudioBalanceResult ApplyBalance(string gameApps, string chatApps, float inputVolume, bool invert)
        {
            var volume = invert ? 100 - inputVolume : inputVolume;

            float gameVolume;
            float chatVolume;

            if (volume < 50)
            {
                gameVolume = volume * 2;
                chatVolume = 100;
            }
            else if (volume > 50)
            {
                gameVolume = 100;
                chatVolume = 100 - ((volume - 50) * 2);
            }
            else
            {
                gameVolume = 100;
                chatVolume = 100;
            }

            audioSessionService.SetApplicationVolumeByName(gameApps, gameVolume);
            audioSessionService.SetApplicationVolumeByName(chatApps, chatVolume);

            return new AudioBalanceResult
            {
                DisplayVolume = volume,
                GameVolume = gameVolume,
                ChatVolume = chatVolume
            };
        }
    }
}
