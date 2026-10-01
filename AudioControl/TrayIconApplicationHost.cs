using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using GameChatBalancer.Wpf;

namespace AudioControl
{
    internal sealed class TrayIconApplicationHost : IApplicationHost, ISerialHostBridge
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int VkShift = 0x10;
        private const int WmHotkey = 0x0312;
        private const int HotkeyIdDecrease = 0x5201;
        private const int HotkeyIdIncrease = 0x5202;
        private const int HotkeyIdCenter = 0x5203;
        private const uint ModControl = 0x0002;
        private const uint ModShift = 0x0004;
        private const uint VkLeft = 0x25;
        private const uint VkRight = 0x27;
        private const uint VkDown = 0x28;

        private readonly ISettingsStore settingsStore;
        private readonly IAutoStartService autoStartService;
        private readonly AppAssignmentPathStore appAssignmentPathStore;
        private readonly List<string> debugMessages = new();
        private readonly object debugMessagesSync = new();

        private ISerialDeviceService? serialDeviceService;
        private IAudioSessionService? audioSessionService;
        private IAudioBalanceService? audioBalanceService;
        private ITrayHostService? trayHostService;
        private IUiCoordinator? uiCoordinator;
        private readonly IOverlayService overlayService;
        private NotifyIcon? trayIcon;
        private Icon? trayIconResource;
        private HotkeyMessageWindow? hotkeyWindow;
        private bool hotkeysRegistered;
        private MainWindow? wpfMainWindow;
        private SynchronizationContext? uiContext;
        private int uiThreadId;
        private bool debug;
        private bool initialized;
        private bool connected;
        private string currentPort = "disconnected";
        private float lastInputVolume = 50f;
        private float displayVolume = 50f;
        private float gameVolume = 100f;
        private float chatVolume = 100f;
        private bool started;
        private bool overlayPrimed;

        public TrayIconApplicationHost(ISettingsStore settingsStore, IAutoStartService autoStartService)
        {
            this.settingsStore = settingsStore;
            this.autoStartService = autoStartService;
            appAssignmentPathStore = new AppAssignmentPathStore();
            overlayService = new WpfOverlayService(TimeSpan.FromSeconds(3), Log);
        }

        public bool DebugEnabled => debug;
        public bool IsDispatchRequired => uiThreadId != 0 && Environment.CurrentManagedThreadId != uiThreadId;
        public bool CanDispatch => started && uiContext != null && uiThreadId != 0;

        public void Configure(
            ISerialDeviceService serialDeviceService,
            IAudioSessionService audioSessionService,
            IAudioBalanceService audioBalanceService)
        {
            this.serialDeviceService = serialDeviceService;
            this.audioSessionService = audioSessionService;
            this.audioBalanceService = audioBalanceService;

            this.serialDeviceService.ConnectionStatusChanged += SerialDeviceService_ConnectionStatusChanged;
            this.audioSessionService.AudioSessionsChanged += AudioSessionService_AudioSessionsChanged;
        }

        public void Start()
        {
            if (started)
            {
                return;
            }

            uiContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();
            uiThreadId = Environment.CurrentManagedThreadId;
            connected = serialDeviceService?.IsConnected() == true;
            currentPort = serialDeviceService?.CurrentPort ?? "disconnected";

            trayIcon = new NotifyIcon
            {
                Icon = LoadTrayIcon() ?? SystemIcons.Application,
                Text = "GameChatBalancer",
                Visible = true
            };
            trayIcon.MouseUp += TrayIcon_MouseUp;

            uiCoordinator = new WpfUiCoordinator(
                getMainWindow: () => wpfMainWindow,
                createMainWindow: CreateMainWindow,
                setMainWindow: window => wpfMainWindow = window,
                hideLegacyHostToTray: () =>
                {
                    if (trayIcon != null)
                    {
                        trayIcon.Visible = true;
                    }
                });

            trayHostService = new TrayHostService(
                openAppAction: ShowMainWindowFromTray,
                exitAppAction: ExitFromTray,
                startWithWindowsProvider: GetStartWithWindowsFromWpf,
                startWithWindowsSetter: ApplyStartWithWindowsFromWpf,
                noiseReductionProvider: () => settingsStore.NoiseReduction,
                noiseReductionSetter: ApplyNoiseReductionFromWpf,
                invertControlProvider: () => settingsStore.Invert,
                invertControlSetter: ApplyInvertControlFromWpf,
                connectedProvider: () => connected,
                currentPortProvider: () => currentPort,
                balanceProvider: () => (displayVolume, gameVolume, chatVolume));

            trayHostService.RefreshState();
            hotkeyWindow = new HotkeyMessageWindow(HandleHotkeyMessage);
            UpdateSoftwareControlHotkeys(connected);
            ApplyOverlaySettings();
            started = true;
        }

        public void Stop()
        {
            if (!started)
            {
                return;
            }

            if (serialDeviceService != null)
            {
                serialDeviceService.ConnectionStatusChanged -= SerialDeviceService_ConnectionStatusChanged;
            }

            if (audioSessionService != null)
            {
                audioSessionService.AudioSessionsChanged -= AudioSessionService_AudioSessionsChanged;
            }

            trayHostService?.Close();
            trayHostService = null;

            if (trayIcon != null)
            {
                trayIcon.MouseUp -= TrayIcon_MouseUp;
                trayIcon.Visible = false;
                trayIcon.Dispose();
                trayIcon = null;
            }

            if (trayIconResource != null)
            {
                trayIconResource.Dispose();
                trayIconResource = null;
            }

            if (wpfMainWindow != null)
            {
                wpfMainWindow.Close();
                wpfMainWindow = null;
            }

            UnregisterSoftwareControlHotkeys();
            hotkeyWindow?.Dispose();
            hotkeyWindow = null;
            overlayService.Hide();
            overlayService.Dispose();

            uiContext = null;
            uiThreadId = 0;
            uiCoordinator = null;
            started = false;
        }

        public void Dispose()
        {
            Stop();
        }

        public void Log(string message)
        {
            lock (debugMessagesSync)
            {
                debugMessages.Add($"{System.DateTime.Now:HH:mm:ss}  {message}");
                if (debugMessages.Count > 500)
                {
                    debugMessages.RemoveRange(0, debugMessages.Count - 500);
                }
            }

            uiCoordinator?.RefreshDebugMessages();
        }

        public void Dispatch(Action action)
        {
            if (action == null)
            {
                return;
            }

            if (uiContext == null)
            {
                action();
                return;
            }

            uiContext.Post(_ => action(), null);
        }

        public void SetSystrayCom(string value)
        {
            currentPort = string.IsNullOrWhiteSpace(value) ? "disconnected" : value;
            Dispatch(() => trayHostService?.SetConnectedState(connected, currentPort));
        }

        public void SetConnected(bool connected)
        {
            this.connected = connected;
            Dispatch(() =>
            {
                UpdateSoftwareControlHotkeys(this.connected);
                trayHostService?.SetConnectedState(this.connected, currentPort);
                uiCoordinator?.SetHardwareStatus();
            });
        }

        public void SetInitialized(bool initialized)
        {
            this.initialized = initialized;
        }

        public void RefreshComPortOptions()
        {
            // Kein WinForms-Combobox-Host mehr vorhanden.
        }

        public void SendNoiseReductionValue()
        {
            if (serialDeviceService?.IsConnected() != true)
            {
                return;
            }

            serialDeviceService.SendData(MapNoiseReductionToCommand(settingsStore.NoiseReduction));
        }

        public void ConfirmNoiseReduction()
        {
            Dispatch(() => uiCoordinator?.SetNoiseReductionConfirmed(settingsStore.NoiseReduction));
        }

        public void ApplyHardwareVolume(float volume)
        {
            Dispatch(() => ApplyBalance(volume));
        }

        private void SerialDeviceService_ConnectionStatusChanged(object? sender, EventArgs e)
        {
            Dispatch(() =>
            {
                connected = serialDeviceService?.IsConnected() == true;
                currentPort = serialDeviceService?.CurrentPort ?? "disconnected";
                UpdateSoftwareControlHotkeys(connected);
                trayHostService?.SetConnectedState(connected, currentPort);
                uiCoordinator?.SetHardwareStatus();

                if (connected)
                {
                    trayHostService?.SetNoiseReductionState(settingsStore.NoiseReduction);
                    trayHostService?.SetInvertControlState(settingsStore.Invert);
                }
            });
        }

        private void AudioSessionService_AudioSessionsChanged(object? sender, EventArgs e)
        {
            Dispatch(() => uiCoordinator?.RefreshAppAssignments());
        }

        private void TrayIcon_MouseUp(object? sender, MouseEventArgs e)
        {
            trayHostService?.HandleTrayMouseUp(e.Button, Control.MousePosition);
        }

        private void ShowMainWindowFromTray()
        {
            var showDebugOption = (GetAsyncKeyState(VkShift) & 0x8000) != 0;
            uiCoordinator?.ShowMainWindow(showDebugOption);
        }

        private void ExitFromTray()
        {
            Application.ExitThread();
        }

        private MainWindow CreateMainWindow()
        {
            var window = new MainWindow(
                () => (connected, currentPort),
                () => settingsStore.NoiseReduction,
                ApplyNoiseReductionFromWpf,
                () => settingsStore.Invert,
                ApplyInvertControlFromWpf,
                () => settingsStore.Game,
                ApplyGameAppsFromWpf,
                () => settingsStore.Chat,
                ApplyChatAppsFromWpf,
                GetAvailableAppsForWpf,
                ResolveAssignedAppPathForWpf,
                () => uiCoordinator?.RefreshAppAssignments(),
                GetDebugMessagesForWpf,
                Log,
                SetDebugModeFromWpf,
                ApplySoftwareBalanceFromWpf,
                GetStartWithWindowsFromWpf,
                ApplyStartWithWindowsFromWpf,
                () => settingsStore.OverlayEnabled,
                ApplyOverlayEnabledFromWpf,
                () => Math.Clamp((int)Math.Round(settingsStore.OverlayDurationSeconds * 1000.0), 500, 10000),
                ApplyOverlayDurationFromWpf,
                () => settingsStore.OverlayPosition,
                ApplyOverlayPositionFromWpf,
                () => settingsStore.OverlayOpacity,
                ApplyOverlayOpacityFromWpf,
                showDebugOption: false);

            window.Closed += (_, _) => wpfMainWindow = null;
            return window;
        }

        private IEnumerable<string> GetAvailableAppsForWpf()
        {
            if (audioSessionService == null)
            {
                return Enumerable.Empty<string>();
            }

            var gameApps = ParseCsv(settingsStore.Game).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var chatApps = ParseCsv(settingsStore.Chat).ToHashSet(StringComparer.OrdinalIgnoreCase);

            return audioSessionService.GetAudioApplications(false)
                .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .Where(item => !item.Equals("Idle", StringComparison.OrdinalIgnoreCase))
                .Where(item => !gameApps.Contains(item) && !chatApps.Contains(item))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private string? ResolveAssignedAppPathForWpf(string appName)
        {
            return appAssignmentPathStore.GetPath(appName);
        }

        private void ApplyGameAppsFromWpf(string csv)
        {
            settingsStore.Game = csv ?? string.Empty;
            settingsStore.ScheduleSave();
            appAssignmentPathStore.SyncAssignedApps(ParseCsv(settingsStore.Game), ParseCsv(settingsStore.Chat));
            uiCoordinator?.RefreshAppAssignments();
        }

        private void ApplyChatAppsFromWpf(string csv)
        {
            settingsStore.Chat = csv ?? string.Empty;
            settingsStore.ScheduleSave();
            appAssignmentPathStore.SyncAssignedApps(ParseCsv(settingsStore.Game), ParseCsv(settingsStore.Chat));
            uiCoordinator?.RefreshAppAssignments();
        }

        private void ApplyNoiseReductionFromWpf(string level)
        {
            settingsStore.NoiseReduction = string.IsNullOrWhiteSpace(level) ? "High" : level;
            settingsStore.ScheduleSave();

            trayHostService?.SetNoiseReductionState(settingsStore.NoiseReduction);
            uiCoordinator?.SetNoiseReductionSelection(settingsStore.NoiseReduction);
            uiCoordinator?.SetNoiseReductionApplying(settingsStore.NoiseReduction);

            if (serialDeviceService?.IsConnected() == true)
            {
                serialDeviceService.SendData(MapNoiseReductionToCommand(settingsStore.NoiseReduction));
            }
            else
            {
                uiCoordinator?.SetNoiseReductionError(settingsStore.NoiseReduction);
            }
        }

        private void ApplyInvertControlFromWpf(bool invert)
        {
            settingsStore.Invert = invert;
            settingsStore.ScheduleSave();
            trayHostService?.SetInvertControlState(invert);
            uiCoordinator?.SetInvertControlState(invert);
            ApplyBalance(lastInputVolume);
        }

        private void ApplySoftwareBalanceFromWpf(float value)
        {
            ApplyBalance(value);
        }

        private bool GetStartWithWindowsFromWpf()
        {
            var enabled = autoStartService.IsEnabled();
            settingsStore.StartWithWindows = enabled;
            return enabled;
        }

        private void ApplyStartWithWindowsFromWpf(bool enabled)
        {
            if (autoStartService.TrySetEnabled(enabled, out var error))
            {
                settingsStore.StartWithWindows = enabled;
                settingsStore.ScheduleSave();
                uiCoordinator?.SetStartWithWindowsState(enabled);
                trayHostService?.RefreshState();
                return;
            }

            if (!string.IsNullOrWhiteSpace(error))
            {
                Log($"Start with Windows: {error}");
            }

            uiCoordinator?.SetStartWithWindowsState(autoStartService.IsEnabled());
            trayHostService?.RefreshState();
        }

        private void ApplyOverlayEnabledFromWpf(bool enabled)
        {
            settingsStore.OverlayEnabled = enabled;
            settingsStore.ScheduleSave();
            ApplyOverlaySettings();
        }

        private void ApplyOverlayDurationFromWpf(int durationMs)
        {
            var seconds = Math.Clamp((int)Math.Round(durationMs / 1000.0), 1, 10);
            settingsStore.OverlayDurationSeconds = seconds;
            settingsStore.ScheduleSave();
            ApplyOverlaySettings();
        }

        private void ApplyOverlayPositionFromWpf(string position)
        {
            settingsStore.OverlayPosition = NormalizeOverlayPosition(position);
            settingsStore.ScheduleSave();
            ApplyOverlaySettings();
        }

        private void ApplyOverlayOpacityFromWpf(double opacity)
        {
            settingsStore.OverlayOpacity = Math.Clamp(opacity, 0.2, 1.0);
            settingsStore.ScheduleSave();
            ApplyOverlaySettings();
        }

        private void SetDebugModeFromWpf(bool isDebugSectionActive)
        {
            debug = isDebugSectionActive;
        }

        private IEnumerable<string> GetDebugMessagesForWpf()
        {
            lock (debugMessagesSync)
            {
                return debugMessages.AsEnumerable().Reverse().ToArray();
            }
        }

        private void ApplyBalance(float inputVolume)
        {
            if (audioBalanceService == null)
            {
                return;
            }

            lastInputVolume = inputVolume;
            var result = audioBalanceService.ApplyBalance(settingsStore.Game, settingsStore.Chat, inputVolume, settingsStore.Invert);
            displayVolume = result.DisplayVolume;
            gameVolume = result.GameVolume;
            chatVolume = result.ChatVolume;

            trayHostService?.SetBalance(displayVolume, gameVolume, chatVolume);
            uiCoordinator?.SetArduinoValue(displayVolume);
            ApplyOverlaySettings();

            if (!overlayPrimed)
            {
                overlayPrimed = true;
                return;
            }

            overlayService.Update(gameVolume, chatVolume);
        }

        private void ApplyOverlaySettings()
        {
            overlayService.ApplyConfiguration(
                settingsStore.OverlayEnabled,
                settingsStore.OverlayDurationSeconds,
                NormalizeOverlayPosition(settingsStore.OverlayPosition),
                settingsStore.OverlayOpacity);
        }

        private static string NormalizeOverlayPosition(string? position)
        {
            var normalized = string.IsNullOrWhiteSpace(position) ? "BottomCenter" : position.Trim();
            return normalized switch
            {
                "TopLeft" => "TopLeft",
                "TopCenter" => "TopCenter",
                "TopRight" => "TopRight",
                "CenterLeft" => "CenterLeft",
                "Center" => "Center",
                "CenterRight" => "CenterRight",
                "BottomLeft" => "BottomLeft",
                "BottomCenter" => "BottomCenter",
                "BottomRight" => "BottomRight",
                _ => "BottomCenter"
            };
        }

        private void UpdateSoftwareControlHotkeys(bool isConnected)
        {
            if (isConnected)
            {
                UnregisterSoftwareControlHotkeys();
                return;
            }

            RegisterSoftwareControlHotkeys();
        }

        private void RegisterSoftwareControlHotkeys()
        {
            if (hotkeysRegistered || hotkeyWindow == null)
            {
                return;
            }

            var handle = hotkeyWindow.Handle;
            if (handle == IntPtr.Zero)
            {
                return;
            }

            var modifiers = ModControl | ModShift;
            var decreaseRegistered = RegisterHotKey(handle, HotkeyIdDecrease, modifiers, VkLeft);
            var increaseRegistered = RegisterHotKey(handle, HotkeyIdIncrease, modifiers, VkRight);
            var centerRegistered = RegisterHotKey(handle, HotkeyIdCenter, modifiers, VkDown);

            if (decreaseRegistered && increaseRegistered && centerRegistered)
            {
                hotkeysRegistered = true;
                Log("SoftwareControl: Startup-Hotkeys registriert (Ctrl+Shift+Left/Right/Down).");
                return;
            }

            if (decreaseRegistered)
            {
                UnregisterHotKey(handle, HotkeyIdDecrease);
            }

            if (increaseRegistered)
            {
                UnregisterHotKey(handle, HotkeyIdIncrease);
            }

            if (centerRegistered)
            {
                UnregisterHotKey(handle, HotkeyIdCenter);
            }

            Log($"SoftwareControl: Startup-Hotkey-Registrierung fehlgeschlagen (Win32={Marshal.GetLastWin32Error()}).");
        }

        private void UnregisterSoftwareControlHotkeys()
        {
            if (!hotkeysRegistered || hotkeyWindow == null)
            {
                hotkeysRegistered = false;
                return;
            }

            var handle = hotkeyWindow.Handle;
            if (handle != IntPtr.Zero)
            {
                UnregisterHotKey(handle, HotkeyIdDecrease);
                UnregisterHotKey(handle, HotkeyIdIncrease);
                UnregisterHotKey(handle, HotkeyIdCenter);
            }

            hotkeysRegistered = false;
            Log("SoftwareControl: Startup-Hotkeys deregistriert.");
        }

        private void HandleHotkeyMessage(Message message)
        {
            if (message.Msg != WmHotkey)
            {
                return;
            }

            var hotkeyId = message.WParam.ToInt32();
            if (hotkeyId == HotkeyIdDecrease)
            {
                ApplyStep(-5f);
                return;
            }

            if (hotkeyId == HotkeyIdIncrease)
            {
                ApplyStep(5f);
                return;
            }

            if (hotkeyId == HotkeyIdCenter)
            {
                ApplyCenter();
            }
        }

        private void ApplyStep(float delta)
        {
            var current = Math.Clamp(displayVolume, 0f, 100f);
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

            ApplyBalance(nextValue);
        }

        private void ApplyCenter()
        {
            const float centerValue = 50f;
            var current = Math.Clamp(displayVolume, 0f, 100f);
            if (Math.Abs(current - centerValue) < 0.01f)
            {
                return;
            }

            ApplyBalance(centerValue);
        }

        private static IEnumerable<string> ParseCsv(string csv)
        {
            return (csv ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        private static string MapNoiseReductionToCommand(string noiseReduction)
        {
            return noiseReduction switch
            {
                "Off" => "NR=0",
                "Low" => "NR=1",
                "Medium" => "NR=2",
                "High" => "NR=3",
                _ => "NR=2"
            };
        }

        private Icon? LoadTrayIcon()
        {
            try
            {
                var pngPath = Path.Combine(AppContext.BaseDirectory, "Resources", "GCB_icon.png");
                if (!File.Exists(pngPath))
                {
                    return null;
                }

                using var bitmap = new Bitmap(pngPath);
                var hIcon = bitmap.GetHicon();
                try
                {
                    trayIconResource = (Icon)Icon.FromHandle(hIcon).Clone();
                    return trayIconResource;
                }
                finally
                {
                    DestroyIcon(hIcon);
                }
            }
            catch
            {
                return null;
            }
        }

        private sealed class HotkeyMessageWindow : NativeWindow, IDisposable
        {
            private readonly Action<Message> messageHandler;

            public HotkeyMessageWindow(Action<Message> messageHandler)
            {
                this.messageHandler = messageHandler;
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                messageHandler(m);
                base.WndProc(ref m);
            }

            public void Dispose()
            {
                if (Handle != IntPtr.Zero)
                {
                    DestroyHandle();
                }
            }
        }
    }
}
