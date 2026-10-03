using System.Drawing;
using System.Windows.Forms;
using GameChatBalancer.Wpf;

namespace AudioControl
{
    internal sealed class TrayHostService : ITrayHostService
    {
        private readonly Action openAppAction;
        private readonly Action exitAppAction;
        private readonly Func<bool> startWithWindowsProvider;
        private readonly Action<bool> startWithWindowsSetter;
        private readonly Func<bool> invertControlProvider;
        private readonly Action<bool> invertControlSetter;
        private readonly Func<bool> connectedProvider;
        private readonly Func<string> currentPortProvider;
        private readonly Func<(float Display, float Game, float Chat)> balanceProvider;

        private SystrayPopupWindow? popupWindow;
        private bool isCreatingPopupWindow;

        public TrayHostService(
            Action openAppAction,
            Action exitAppAction,
            Func<bool> startWithWindowsProvider,
            Action<bool> startWithWindowsSetter,
            Func<bool> invertControlProvider,
            Action<bool> invertControlSetter,
            Func<bool> connectedProvider,
            Func<string> currentPortProvider,
            Func<(float Display, float Game, float Chat)> balanceProvider)
        {
            this.openAppAction = openAppAction;
            this.exitAppAction = exitAppAction;
            this.startWithWindowsProvider = startWithWindowsProvider;
            this.startWithWindowsSetter = startWithWindowsSetter;
            this.invertControlProvider = invertControlProvider;
            this.invertControlSetter = invertControlSetter;
            this.connectedProvider = connectedProvider;
            this.currentPortProvider = currentPortProvider;
            this.balanceProvider = balanceProvider;
        }

        public void HandleTrayMouseUp(MouseButtons button, Point anchorPoint)
        {
            if (button != MouseButtons.Right)
            {
                return;
            }

            ShowPopup(anchorPoint);
        }

        public void RefreshState()
        {
            EnsurePopupWindow();
            if (popupWindow == null)
            {
                return;
            }

            popupWindow.SetConnectedState(connectedProvider(), currentPortProvider());
            popupWindow.SetStartWithWindowsState(startWithWindowsProvider());
            popupWindow.SetInvertControlState(invertControlProvider());

            var balance = balanceProvider();
            popupWindow.SetBalance(balance.Display, balance.Game, balance.Chat);
        }

        public void SetConnectedState(bool connected, string? currentPort)
        {
            EnsurePopupWindow();
            popupWindow?.SetConnectedState(connected, currentPort);
        }

        public void SetInvertControlState(bool invert)
        {
            EnsurePopupWindow();
            popupWindow?.SetInvertControlState(invert);
        }

        public void SetBalance(float displayVolume, float gamePercent, float chatPercent)
        {
            EnsurePopupWindow();
            popupWindow?.SetBalance(displayVolume, gamePercent, chatPercent);
        }

        public void Close()
        {
            if (popupWindow == null)
            {
                return;
            }

            popupWindow.Close();
            popupWindow = null;
        }

        private void ShowPopup(Point anchorPoint)
        {
            EnsurePopupWindow();
            if (popupWindow == null)
            {
                return;
            }

            RefreshState();

            var screen = Screen.FromPoint(anchorPoint);
            var workingArea = screen.WorkingArea;
            popupWindow.Left = workingArea.Right - popupWindow.Width - 12;
            popupWindow.Top = workingArea.Bottom - popupWindow.Height - 12;

            if (!popupWindow.IsVisible)
            {
                popupWindow.Show();
            }
            else
            {
                popupWindow.Activate();
            }
        }

        private void EnsurePopupWindow()
        {
            if (popupWindow != null || isCreatingPopupWindow)
            {
                return;
            }

            isCreatingPopupWindow = true;
            try
            {
                popupWindow = new SystrayPopupWindow(
                    openAppAction,
                    exitAppAction,
                    startWithWindowsProvider,
                    startWithWindowsSetter,
                    invertControlProvider,
                    invertControlSetter);

                popupWindow.Closed += (_, _) => popupWindow = null;
            }
            finally
            {
                isCreatingPopupWindow = false;
            }
        }
    }
}
