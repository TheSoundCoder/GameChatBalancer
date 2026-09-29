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

        void ScheduleSave();
        void SaveNow();
    }
}
