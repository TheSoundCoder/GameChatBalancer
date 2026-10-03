namespace AudioControl
{
    public class SettingsStore : ISettingsStore
    {
        private readonly object sync = new();
        private System.Threading.Timer? saveTimer;
        private const int SaveDelayMs = 1000;

        public string Game
        {
            get => Properties.Settings.Default.GAME;
            set => Properties.Settings.Default.GAME = value;
        }

        public string Chat
        {
            get => Properties.Settings.Default.CHAT;
            set => Properties.Settings.Default.CHAT = value;
        }

        public string ComPort
        {
            get => Properties.Settings.Default.ComPort;
            set => Properties.Settings.Default.ComPort = value;
        }

        public string NoiseReduction
        {
            get => Properties.Settings.Default.NoiseReduction;
            set => Properties.Settings.Default.NoiseReduction = value;
        }

        public bool Invert
        {
            get => Properties.Settings.Default.Invert;
            set => Properties.Settings.Default.Invert = value;
        }

        public bool StartWithWindows
        {
            get => Properties.Settings.Default.StartWithWindows;
            set => Properties.Settings.Default.StartWithWindows = value;
        }

        public bool OverlayEnabled
        {
            get => Properties.Settings.Default.OverlayEnabled;
            set => Properties.Settings.Default.OverlayEnabled = value;
        }

        public int OverlayDurationSeconds
        {
            get => Properties.Settings.Default.OverlayDurationSeconds;
            set => Properties.Settings.Default.OverlayDurationSeconds = value;
        }

        public string OverlayPosition
        {
            get => Properties.Settings.Default.OverlayPosition;
            set => Properties.Settings.Default.OverlayPosition = value;
        }

        public double OverlayOpacity
        {
            get => Properties.Settings.Default.OverlayOpacity;
            set => Properties.Settings.Default.OverlayOpacity = value;
        }

        public void ScheduleSave()
        {
            lock (sync)
            {
                saveTimer ??= new System.Threading.Timer(_ => SaveInternal(), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                saveTimer.Change(SaveDelayMs, System.Threading.Timeout.Infinite);
            }
        }

        public void SaveNow()
        {
            lock (sync)
            {
                saveTimer?.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                SaveInternal();
            }
        }

        private static void SaveInternal()
        {
            try
            {
                Properties.Settings.Default.Save();
            }
            catch
            {
            }
        }

    }
}
