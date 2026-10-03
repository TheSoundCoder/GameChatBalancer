namespace AudioControl
{
    internal interface IOverlayService : IDisposable
    {
        void Show(float gameLevel, float chatLevel);
        void Update(float gameLevel, float chatLevel);
        void Hide();
        void ApplyConfiguration(bool enabled, int durationSeconds, string position, double opacity);
    }
}
