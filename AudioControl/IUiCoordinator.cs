namespace AudioControl
{
    internal interface IUiCoordinator
    {
        void ShowMainWindow(bool showDebugOption);
        void CloseMainWindow();
        void HideMainWindowToTray();
        void SetHardwareStatus();
        void SetArduinoValue(float value);
        void SetStartWithWindowsState(bool enabled);
        void SetNoiseReductionSelection(string level);
        void SetNoiseReductionApplying(string level);
        void SetNoiseReductionConfirmed(string level);
        void SetNoiseReductionError(string level);
        void SetInvertControlState(bool enabled);
        void RefreshAppAssignments();
        void RefreshDebugMessages();
    }
}
