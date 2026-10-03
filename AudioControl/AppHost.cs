namespace AudioControl
{
    internal sealed class AppHost : IApplicationHost
    {
        private readonly IApplicationHost uiHost;
        private readonly ISerialDeviceService serialDeviceService;
        private readonly IUsbWatcherService usbWatcherService;
        private readonly IAudioSessionService audioSessionService;
        private readonly ISettingsStore settingsStore;
        private bool started;

        public AppHost(
            IApplicationHost uiHost,
            ISerialDeviceService serialDeviceService,
            IUsbWatcherService usbWatcherService,
            IAudioSessionService audioSessionService,
            ISettingsStore settingsStore)
        {
            this.uiHost = uiHost;
            this.serialDeviceService = serialDeviceService;
            this.usbWatcherService = usbWatcherService;
            this.audioSessionService = audioSessionService;
            this.settingsStore = settingsStore;
        }

        public void Start()
        {
            if (started)
            {
                return;
            }

            uiHost.Start();

            usbWatcherService.Start();
            serialDeviceService.OpenComPort();

            if (serialDeviceService.IsConnected())
            {
                serialDeviceService.SendData("get");
                serialDeviceService.SendData(MapNoiseReductionToCommand(settingsStore.NoiseReduction));
            }

            audioSessionService.StartSessionMonitoring();

            started = true;
        }

        public void Stop()
        {
            if (!started)
            {
                return;
            }

            uiHost.Stop();
            audioSessionService.StopSessionMonitoring();
            usbWatcherService.Stop();
            serialDeviceService.Shutdown();

            settingsStore.SaveNow();

            started = false;
        }

        public void Dispose()
        {
            Stop();
        }

        private static string MapNoiseReductionToCommand(string noiseReduction)
        {
            return noiseReduction switch
            {
                "Off" => "NR=0",
                "Low" => "NR=1",
                "Medium" => "NR=2",
                "High" => "NR=3",
                _ => "NR=3"
            };
        }
    }
}
