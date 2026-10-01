using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AudioControl
{
    internal sealed class SoftwareControlHotkeyService : ISoftwareControlHotkeyService
    {
        private const int WmHotkey = 0x0312;
        private const int HotkeyIdDecrease = 0x5201;
        private const int HotkeyIdIncrease = 0x5202;
        private const int HotkeyIdCenter = 0x5203;
        private const uint ModControl = 0x0002;
        private const uint ModShift = 0x0004;
        private const uint VkLeft = 0x25;
        private const uint VkRight = 0x27;
        private const uint VkDown = 0x28;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private readonly Form hostForm;
        private readonly Func<float> currentValueProvider;
        private readonly Action<float> applyBalanceAction;
        private readonly Action<string> logAction;

        private bool hotkeysRegistered;

        public SoftwareControlHotkeyService(
            Form hostForm,
            Func<float> currentValueProvider,
            Action<float> applyBalanceAction,
            Action<string> logAction)
        {
            this.hostForm = hostForm;
            this.currentValueProvider = currentValueProvider;
            this.applyBalanceAction = applyBalanceAction;
            this.logAction = logAction;

            this.hostForm.HandleCreated += HostForm_HandleCreated;
            this.hostForm.HandleDestroyed += HostForm_HandleDestroyed;
        }

        public void UpdateConnectionState(bool connected)
        {
            if (connected)
            {
                UnregisterHotkeys();
                return;
            }

            RegisterHotkeys();
        }

        public bool TryHandleMessage(ref Message m)
        {
            if (m.Msg != WmHotkey)
            {
                return false;
            }

            var hotkeyId = m.WParam.ToInt32();
            if (hotkeyId == HotkeyIdDecrease)
            {
                ApplyStep(-5f);
                return true;
            }

            if (hotkeyId == HotkeyIdIncrease)
            {
                ApplyStep(5f);
                return true;
            }

            if (hotkeyId == HotkeyIdCenter)
            {
                ApplyCenter();
                return true;
            }

            return false;
        }

        public void Dispose()
        {
            hostForm.HandleCreated -= HostForm_HandleCreated;
            hostForm.HandleDestroyed -= HostForm_HandleDestroyed;
            UnregisterHotkeys();
        }

        private void HostForm_HandleCreated(object? sender, EventArgs e)
        {
            // no-op: registration is controlled by UpdateConnectionState calls
        }

        private void HostForm_HandleDestroyed(object? sender, EventArgs e)
        {
            hotkeysRegistered = false;
        }

        private void RegisterHotkeys()
        {
            if (hotkeysRegistered || !hostForm.IsHandleCreated)
            {
                return;
            }

            var modifiers = ModControl | ModShift;
            var decreaseRegistered = RegisterHotKey(hostForm.Handle, HotkeyIdDecrease, modifiers, VkLeft);
            var increaseRegistered = RegisterHotKey(hostForm.Handle, HotkeyIdIncrease, modifiers, VkRight);
            var centerRegistered = RegisterHotKey(hostForm.Handle, HotkeyIdCenter, modifiers, VkDown);

            if (decreaseRegistered && increaseRegistered && centerRegistered)
            {
                hotkeysRegistered = true;
                logAction("SoftwareControl: Startup-Hotkeys registriert (Ctrl+Shift+Left/Right/Down).");
                return;
            }

            if (decreaseRegistered)
            {
                UnregisterHotKey(hostForm.Handle, HotkeyIdDecrease);
            }

            if (increaseRegistered)
            {
                UnregisterHotKey(hostForm.Handle, HotkeyIdIncrease);
            }

            if (centerRegistered)
            {
                UnregisterHotKey(hostForm.Handle, HotkeyIdCenter);
            }

            logAction($"SoftwareControl: Startup-Hotkey-Registrierung fehlgeschlagen (Win32={Marshal.GetLastWin32Error()}).");
        }

        private void UnregisterHotkeys()
        {
            if (!hotkeysRegistered || !hostForm.IsHandleCreated)
            {
                hotkeysRegistered = false;
                return;
            }

            UnregisterHotKey(hostForm.Handle, HotkeyIdDecrease);
            UnregisterHotKey(hostForm.Handle, HotkeyIdIncrease);
            UnregisterHotKey(hostForm.Handle, HotkeyIdCenter);
            hotkeysRegistered = false;
            logAction("SoftwareControl: Startup-Hotkeys deregistriert.");
        }

        private void ApplyStep(float delta)
        {
            var current = Math.Clamp(currentValueProvider(), 0f, 100f);
            var isOnFivePercentGrid = Math.Abs(current % 5f) < 0.01f || Math.Abs((current % 5f) - 5f) < 0.01f;

            float nextValue;
            if (delta < 0f)
            {
                nextValue = isOnFivePercentGrid
                    ? current - 5f
                    : MathF.Floor(current / 5f) * 5f;
            }
            else
            {
                nextValue = isOnFivePercentGrid
                    ? current + 5f
                    : MathF.Ceiling(current / 5f) * 5f;
            }

            nextValue = Math.Clamp(nextValue, 0f, 100f);
            if (Math.Abs(nextValue - current) < 0.01f)
            {
                return;
            }

            applyBalanceAction(nextValue);
        }

        private void ApplyCenter()
        {
            const float centerValue = 50f;
            var current = Math.Clamp(currentValueProvider(), 0f, 100f);
            if (Math.Abs(current - centerValue) < 0.01f)
            {
                return;
            }

            applyBalanceAction(centerValue);
        }
    }
}
