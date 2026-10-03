namespace AudioControl
{
    internal sealed class LegacyApplicationHostFactory : IApplicationHostFactory
    {
        public IApplicationHost Create()
        {
            var settings = new SettingsStore();
            var autoStart = new AutoStartService();
            var uiHost = new TrayIconApplicationHost(settings, autoStart);
            var serial = new SerialDeviceService(uiHost);
            var usb = new UsbWatcherService();
            var audioSessions = new AudioSessionService(uiHost);
            var audioBalance = new AudioBalanceService(audioSessions);

            uiHost.Configure(serial, audioSessions, audioBalance);

            return new AppHost(uiHost, serial, usb, audioSessions, settings);
        }
    }
}
