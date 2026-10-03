using System;
using GameChatBalancer.Wpf;

namespace AudioControl
{
    internal sealed class WpfUiCoordinator : IUiCoordinator
    {
        private readonly Func<MainWindow?> getMainWindow;
        private readonly Func<MainWindow> createMainWindow;
        private readonly Action<MainWindow?> setMainWindow;
        private readonly Action hideLegacyHostToTray;

        public WpfUiCoordinator(
            Func<MainWindow?> getMainWindow,
            Func<MainWindow> createMainWindow,
            Action<MainWindow?> setMainWindow,
            Action hideLegacyHostToTray)
        {
            this.getMainWindow = getMainWindow;
            this.createMainWindow = createMainWindow;
            this.setMainWindow = setMainWindow;
            this.hideLegacyHostToTray = hideLegacyHostToTray;
        }

        public void ShowMainWindow(bool showDebugOption)
        {
            var window = getMainWindow();
            if (window == null)
            {
                window = createMainWindow();
                setMainWindow(window);
            }

            window.SetShowDebugOption(showDebugOption);

            window.Show();
            window.WindowState = System.Windows.WindowState.Normal;
            window.Activate();
            hideLegacyHostToTray();
        }

        public void CloseMainWindow()
        {
            var window = getMainWindow();
            if (window == null)
            {
                return;
            }

            window.Close();
            setMainWindow(null);
        }

        public void HideMainWindowToTray()
        {
            getMainWindow()?.Hide();
            hideLegacyHostToTray();
        }

        public void SetHardwareStatus() => getMainWindow()?.SetHardwareStatus();
        public void SetArduinoValue(float value) => getMainWindow()?.SetArduinoValue(value);
        public void SetStartWithWindowsState(bool enabled) => getMainWindow()?.SetStartWithWindowsState(enabled);
        public void SetNoiseReductionSelection(string level) => getMainWindow()?.SetNoiseReductionSelection(level);
        public void SetNoiseReductionApplying(string level) => getMainWindow()?.SetNoiseReductionApplying(level);
        public void SetNoiseReductionConfirmed(string level) => getMainWindow()?.SetNoiseReductionConfirmed(level);
        public void SetNoiseReductionError(string level) => getMainWindow()?.SetNoiseReductionError(level);
        public void SetInvertControlState(bool enabled) => getMainWindow()?.SetInvertControlState(enabled);
        public void RefreshAppAssignments() => getMainWindow()?.RefreshAppAssignments();
        public void RefreshDebugMessages() => getMainWindow()?.RefreshDebugMessages();
    }
}
