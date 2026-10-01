namespace AudioControl
{
    public interface ISettingsStore
    {
        string Game { get; set; }
        string Chat { get; set; }
        string ComPort { get; set; }
        string NoiseReduction { get; set; }
        bool Invert { get; set; }
        bool StartWithWindows { get; set; }
        bool OverlayEnabled { get; set; }
        int OverlayDurationSeconds { get; set; }
        string OverlayPosition { get; set; }
        double OverlayOpacity { get; set; }

        void ScheduleSave();
        void SaveNow();
    }
}
