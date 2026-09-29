//using Microsoft.VisualBasic;
//using System.Linq.Expressions;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using Microsoft.Win32;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using System.Security.Cryptography;
using System.Management;
using System.Windows.Forms;
using GameChatBalancer.Wpf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AudioControl
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int VkShift = 0x10;

        //static SerialPort _serialPort;
        //string ComPort = Properties.Settings.Default.ComPort;    //Config

        string GAME;   //List of Games comma separated
        string CHAT;    //List of Voice apps comma separated

        bool debug = false;
        bool initialized = false;
        public object lb_item = null;
        ListBox source_LB = null;
        readonly ISerialDeviceService serialDeviceService;
        readonly IUsbWatcherService usbWatcherService;
        readonly IAudioSessionService audioSessionService;
        readonly IAudioBalanceService audioBalanceService;
        readonly IAutoStartService autoStartService;
        readonly ISettingsStore settingsStore;
        readonly AppAssignmentPathStore appAssignmentPathStore;
        readonly List<string> debugMessages = new();
        readonly object debugMessagesSync = new();
        int audioSessionRefreshQueued;
        MainWindow? wpfMainWindow;
        bool isExiting;

        public Form1()
            : this(null, null, null, null, null, null)
        {
        }

        internal Form1(ISerialDeviceService? serialDeviceService)
            : this(serialDeviceService, null, null, null, null, null)
        {
        }

        internal Form1(ISerialDeviceService? serialDeviceService, IUsbWatcherService? usbWatcherService)
            : this(serialDeviceService, usbWatcherService, null, null, null, null)
        {
        }

        internal Form1(ISerialDeviceService? serialDeviceService, IUsbWatcherService? usbWatcherService, IAudioSessionService? audioSessionService)
            : this(serialDeviceService, usbWatcherService, audioSessionService, null, null, null)
        {
        }

        internal Form1(ISerialDeviceService? serialDeviceService, IUsbWatcherService? usbWatcherService, IAudioSessionService? audioSessionService, ISettingsStore? settingsStore, IAudioBalanceService? audioBalanceService, IAutoStartService? autoStartService)
        {
            InitializeComponent();
            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            SystemEvents.SessionEnding += OnSessionEnding;
            this.serialDeviceService = serialDeviceService ?? new SerialDeviceService(this);
            this.serialDeviceService.ConnectionStatusChanged += SerialDeviceService_ConnectionStatusChanged;
            this.usbWatcherService = usbWatcherService ?? new UsbWatcherService();
            this.audioSessionService = audioSessionService ?? new AudioSessionService(this);
            this.settingsStore = settingsStore ?? new SettingsStore();
            this.audioBalanceService = audioBalanceService ?? new AudioBalanceService(this.audioSessionService);
            this.autoStartService = autoStartService ?? new AutoStartService();
            this.appAssignmentPathStore = new AppAssignmentPathStore();
            GAME = this.settingsStore.Game;
            CHAT = this.settingsStore.Chat;
            this.appAssignmentPathStore.SyncAssignedApps(ParseCsv(GAME), ParseCsv(CHAT));
        }

        private void SerialDeviceService_ConnectionStatusChanged(object? sender, EventArgs e)
        {
            wpfMainWindow?.SetHardwareStatus();
        }

        private void AudioSessionService_AudioSessionsChanged(object? sender, EventArgs e)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            if (debug)
            {
                SendToLog("AudioSessionService: AudioSessionsChanged received.");
            }

            if (System.Threading.Interlocked.Exchange(ref audioSessionRefreshQueued, 1) == 1)
            {
                return;
            }

            BeginInvoke(new Action(HandleAudioSessionsChangedOnUiThread));
        }

        private void HandleAudioSessionsChangedOnUiThread()
        {
            if (IsDisposed)
            {
                return;
            }

            RefreshWpfAssignments(force: true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            /* OK Autodetect Arduino (handshake)
             * OK Control more than one executable (comma separated string and instr check in control function)
             * OK Actively request current volume level from arduino
             * OK Systray icon
             * OK Diconnect of Arduino & Reconnect (poll once per minute)
             * Keep audio objects open for a minute - then close
             */

            //Rectangle workingArea = Screen.GetWorkingArea(this);
            //this.Location = new Point(workingArea.Right - this.Width,
            //                          workingArea.Bottom - this.Height);
            //this.Location.X = Screen.GetWorkingArea().Width - this.Width;
            fill_lb_GAME();
            fill_lb_CHAT();
            fill_lb_AudioProcesses();

            if (string.IsNullOrWhiteSpace(settingsStore.NoiseReduction))
            {
                settingsStore.NoiseReduction = "High";
                settingsStore.SaveNow();
            }

            fill_ddl_NoiseReduction();
            cb_invert.Checked = settingsStore.Invert;
            if (settingsStore.ComPort == "")
            {
                settingsStore.ComPort = "Auto";
                settingsStore.SaveNow();
            }
            SendToLog("Filling ComPortList");
            Fill_ddl_ComPort();

            usbWatcherService.Start();
            serialDeviceService.OpenComPort();
            if (serialDeviceService.IsConnected())
            {
                GetVol();
                Send_NoiseReducion_Value();
            }

            this.Hide();
            trayicon.Visible = true;

            initialized = true;

            audioSessionService.AudioSessionsChanged += AudioSessionService_AudioSessionsChanged;
            audioSessionService.StartSessionMonitoring();
            lastKnownAvailableAppsSnapshot = BuildAvailableAppsSnapshot();
        }

        private string lastKnownAvailableAppsSnapshot = string.Empty;

        private void RefreshWpfAssignments(bool force)
        {
            var currentSnapshot = BuildAvailableAppsSnapshot();
            if (!force && string.Equals(currentSnapshot, lastKnownAvailableAppsSnapshot, StringComparison.Ordinal))
            {
                if (debug)
                {
                    SendToLog("WPF Assignment refresh skipped (snapshot unchanged).");
                }
                return;
            }

            fill_lb_AudioProcesses();
            lastKnownAvailableAppsSnapshot = currentSnapshot;
            wpfMainWindow?.RefreshAppAssignments();

            if (debug)
            {
                SendToLog("WPF Assignment refresh applied.");
            }
        }

        private string BuildAvailableAppsSnapshot()
        {
            var available = GetAvailableAppsForWpf()
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return string.Join("|", available);
        }

        private void fill_lb_GAME()
        {
            lb_GAME.Items.Clear();
            lb_GAME.Items.AddRange(GAME.Split(","));
            lb_GAME.Items.Remove("");
        }

        private void fill_lb_CHAT()
        {
            lb_CHAT.Items.Clear();
            lb_CHAT.Items.AddRange(CHAT.Split(","));
            lb_CHAT.Items.Remove("");
        }

        private void fill_lb_AudioProcesses()
        {
            lb_AudioProcesses.Items.Clear();
            lb_AudioProcesses.Items.AddRange(audioSessionService.GetAudioApplications(false).Split("\r\n").Distinct().ToArray());
            foreach (var item in lb_CHAT.Items)
            {
                lb_AudioProcesses.Items.Remove(item);
            }
            foreach (var item in lb_GAME.Items)
            {
                lb_AudioProcesses.Items.Remove(item);
            }
            lb_AudioProcesses.Items.Remove("Idle");
            lb_AudioProcesses.Items.Remove("");
        }

        private IEnumerable<string> GetAvailableAppsForWpf()
        {
            var gameApps = (settingsStore.Game ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var chatApps = (settingsStore.Chat ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Trim())
                .Where(item => !string.IsNullOrWhiteSpace(item))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

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
            GAME = settingsStore.Game;
            appAssignmentPathStore.SyncAssignedApps(ParseCsv(GAME), ParseCsv(CHAT));
            fill_lb_GAME();
            fill_lb_AudioProcesses();
        }

        private void ApplyChatAppsFromWpf(string csv)
        {
            settingsStore.Chat = csv ?? string.Empty;
            settingsStore.ScheduleSave();
            CHAT = settingsStore.Chat;
            appAssignmentPathStore.SyncAssignedApps(ParseCsv(GAME), ParseCsv(CHAT));
            fill_lb_CHAT();
            fill_lb_AudioProcesses();
        }

        private static IEnumerable<string> ParseCsv(string csv)
        {
            return (csv ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        public void Fill_ddl_ComPort()
        {
            SendToLog("Updating DDL_ComPort");
            string strOptions = "";
            string strSelection = settingsStore.ComPort;
            strOptions += "Auto,";
            strOptions += strSelection + ",";
            foreach (string ComPort in SerialPort.GetPortNames())
            {
                strOptions += ComPort + ",";
            }
            ddl_ComPort.Items.Clear();
            ddl_ComPort.Items.AddRange(strOptions.Split(",").Distinct().ToArray());
            ddl_ComPort.Items.Remove("");
            ddl_ComPort.SelectedItem = strSelection;
        }

        private void fill_ddl_NoiseReduction()
        {
            var items = new BindingList<KeyValuePair<string, string>>();
            items.Add(new KeyValuePair<string, string>("NR=0", "Off"));
            items.Add(new KeyValuePair<string, string>("NR=1", "Low"));
            items.Add(new KeyValuePair<string, string>("NR=2", "Medium"));
            items.Add(new KeyValuePair<string, string>("NR=3", "High"));
            ddlNoiseReduction.DataSource = items;
            ddlNoiseReduction.ValueMember = "Key";
            ddlNoiseReduction.DisplayMember = "Value";
            ddlNoiseReduction.Enabled = true;
            ddlNoiseReduction.Text = settingsStore.NoiseReduction;
        }

        public void SendToLog(string msg)
        {
            if (InvokeRequired)
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    BeginInvoke(new Action<string>(SendToLog), msg);
                }
                return;
            }

            if (debug)
            {
                textBox1.AppendText(msg + "\r\n");
            }

            lock (debugMessagesSync)
            {
                debugMessages.Add($"{DateTime.Now:HH:mm:ss}  {msg}");
                if (debugMessages.Count > 500)
                {
                    debugMessages.RemoveRange(0, debugMessages.Count - 500);
                }
            }

            wpfMainWindow?.RefreshDebugMessages();
        }

        private IEnumerable<string> GetDebugMessagesForWpf()
        {
            lock (debugMessagesSync)
            {
                return debugMessages.AsEnumerable().Reverse().ToArray();
            }
        }

        private void SetDebugModeFromWpf(bool isDebugSectionActive)
        {
            debug = isDebugSectionActive;
            cb_Debug.Checked = isDebugSectionActive;
        }

        public void ConfirmNR()
        {
            if (InvokeRequired)
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    BeginInvoke(new Action(ConfirmNR));
                }
                return;
            }

            cbNR.Checked = true;
            wpfMainWindow?.SetNoiseReductionConfirmed(settingsStore.NoiseReduction);
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Debug
        {
            get => debug;
            set { debug = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Initialized
        {
            get => initialized;
            set { initialized = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SystrayCom
        {
            get => systrayCom.Text;
            set
            {
                if (InvokeRequired)
                {
                    if (!IsDisposed && IsHandleCreated)
                    {
                        BeginInvoke(new Action<string>(v => systrayCom.Text = v), value);
                    }
                    return;
                }

                systrayCom.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Connected
        {
            get => cb_connected.Checked;
            set
            {
                if (InvokeRequired)
                {
                    if (!IsDisposed && IsHandleCreated)
                    {
                        BeginInvoke(new Action<bool>(v => cb_connected.Checked = v), value);
                    }
                    return;
                }

                cb_connected.Checked = value;
            }
        }


        public void Send_NoiseReducion_Value()
        {
            SendToLog("Noise reduction set to: " + ddlNoiseReduction.SelectedValue.ToString());
            cbNR.Checked = false;
            serialDeviceService.SendData(ddlNoiseReduction.SelectedValue.ToString());
        }


        public void controlVolume(float volume)
        {
            if (InvokeRequired)
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    BeginInvoke(new Action<float>(controlVolume), volume);
                }
                return;
            }

            //if (Debug) { textBox1.AppendText(volume.ToString() + "\r\n"); }
            var balance = audioBalanceService.ApplyBalance(GAME, CHAT, volume, cb_invert.Checked);

            systrayVolume.Text = balance.DisplayVolume.ToString();
            trackBar1.Value = (int)balance.DisplayVolume;
            lbl_absoluteval.Text = balance.DisplayVolume.ToString();
            lbl_game_vol.Text = balance.GameVolume.ToString();
            lbl_chat_vol.Text = balance.ChatVolume.ToString();
            wpfMainWindow?.SetArduinoValue(balance.DisplayVolume);

        }

        public static string GetPIDByName(string ProcessName)
        {
            string PID = "#";
            try
            {
                if (Process.GetProcessesByName(ProcessName).Length > 0)
                {
                    foreach (Process _process in Process.GetProcessesByName(ProcessName))
                    {
                        PID += _process.Id.ToString() + "#";
                    }
                    return PID.Substring(1, PID.Length - 2);
                }
            }
            finally { }
            return "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //SetApplicationVolume(int.Parse(textBox2.Text), 50);
            //SetApplicationVolumeByName("Discord", 50);
            foreach (string Port in SerialPort.GetPortNames())
            {
                textBox1.Text += Port.ToString();
            }
        }

        private void GetVol()
        {
            serialDeviceService.SendData("get");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnsureWpfApplicationInitialized();
            var showDebugOption = (GetAsyncKeyState(VkShift) & 0x8000) != 0;

            if (wpfMainWindow == null)
            {
                wpfMainWindow = new MainWindow(
                    () => (serialDeviceService.Connected, serialDeviceService.CurrentPort),
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
                    fill_lb_AudioProcesses,
                    GetDebugMessagesForWpf,
                    SendToLog,
                    SetDebugModeFromWpf,
                    ApplySoftwareBalanceFromWpf,
                    GetStartWithWindowsFromWpf,
                    ApplyStartWithWindowsFromWpf,
                    showDebugOption);
                wpfMainWindow.Closing += WpfMainWindow_Closing;
                wpfMainWindow.Closed += (_, _) => wpfMainWindow = null;
            }
            else
            {
                wpfMainWindow.SetShowDebugOption(showDebugOption);
            }

            wpfMainWindow.SetArduinoValue(trackBar1.Value);
            wpfMainWindow.Show();
            wpfMainWindow.WindowState = System.Windows.WindowState.Normal;
            wpfMainWindow.Activate();

            this.Hide();
            trayicon.Visible = true;
        }

        private bool GetStartWithWindowsFromWpf()
        {
            var enabled = autoStartService.IsEnabled();
            if (settingsStore.StartWithWindows != enabled)
            {
                settingsStore.StartWithWindows = enabled;
                settingsStore.ScheduleSave();
                SendToLog($"Autostart sync: settings adjusted to OS state ({(enabled ? "enabled" : "disabled")}).");
            }

            return enabled;
        }

        private void ApplyStartWithWindowsFromWpf(bool enabled)
        {
            if (!autoStartService.TrySetEnabled(enabled, out var errorMessage))
            {
                var actualState = autoStartService.IsEnabled();
                SendToLog($"Autostart update failed: {errorMessage}");
                SendToLog($"Autostart rollback: keeping OS state ({(actualState ? "enabled" : "disabled")}).");
                settingsStore.StartWithWindows = actualState;
                settingsStore.ScheduleSave();
                wpfMainWindow?.SetStartWithWindowsState(actualState);
                return;
            }

            settingsStore.StartWithWindows = enabled;
            settingsStore.ScheduleSave();
            SendToLog($"Autostart {(enabled ? "enabled" : "disabled")}." );
        }

        private void ApplyNoiseReductionFromWpf(string noiseReduction)
        {
            settingsStore.NoiseReduction = noiseReduction;
            settingsStore.ScheduleSave();

            var command = noiseReduction switch
            {
                "Off" => "NR=0",
                "Low" => "NR=1",
                "Medium" => "NR=2",
                "High" => "NR=3",
                _ => "NR=2"
            };

            SendToLog("Noise reduction set to: " + command);
            if (serialDeviceService.IsConnected())
            {
                serialDeviceService.SendData(command);
            }
            else
            {
                wpfMainWindow?.SetNoiseReductionError(noiseReduction);
            }
        }

        private void ApplyInvertControlFromWpf(bool invert)
        {
            if (InvokeRequired)
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    BeginInvoke(new Action<bool>(ApplyInvertControlFromWpf), invert);
                }
                return;
            }

            if (cb_invert.Checked != invert)
            {
                cb_invert.Checked = invert;
                return;
            }

            settingsStore.Invert = invert;
            settingsStore.ScheduleSave();
        }

        private void ApplySoftwareBalanceFromWpf(float volume)
        {
            if (InvokeRequired)
            {
                if (!IsDisposed && IsHandleCreated)
                {
                    BeginInvoke(new Action<float>(ApplySoftwareBalanceFromWpf), volume);
                }

                return;
            }

            var clampedVolume = Math.Clamp(volume, 0f, 100f);
            var balance = audioBalanceService.ApplyBalance(GAME, CHAT, clampedVolume, invert: false);

            systrayVolume.Text = balance.DisplayVolume.ToString();
            trackBar1.Value = (int)balance.DisplayVolume;
            lbl_absoluteval.Text = balance.DisplayVolume.ToString();
            lbl_game_vol.Text = balance.GameVolume.ToString();
            lbl_chat_vol.Text = balance.ChatVolume.ToString();
            wpfMainWindow?.SetArduinoValue(balance.DisplayVolume);

            if (debug)
            {
                SendToLog($"SoftwareControl: Balance adjusted to {MathF.Round(balance.DisplayVolume):0}.");
            }
        }

        private static void EnsureWpfApplicationInitialized()
        {
            if (System.Windows.Application.Current != null)
            {
                return;
            }

            var wpfApp = new App
            {
                ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown
            };

            wpfApp.InitializeComponent();
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                trayicon.Visible = true;
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isExiting = true;
            if (wpfMainWindow != null)
            {
                wpfMainWindow.Closing -= WpfMainWindow_Closing;
                wpfMainWindow.Close();
            }

            Application.Exit();
        }

        private void WpfMainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (isExiting)
            {
                return;
            }

            e.Cancel = true;
            wpfMainWindow?.Hide();
            this.Hide();
            trayicon.Visible = true;
        }

        private void Settings_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void lb_MouseDown(object sender, MouseEventArgs e)
        {
            lb_item = null;
            ListBox lb = sender as ListBox;
            if (lb.Items.Count == 0)
            {
                return;
            }
            //int index = lb.IndexFromPoint(e.X, e.Y);
            lb.DoDragDrop(lb.SelectedItem.ToString(), DragDropEffects.Move);
            //DragDropEffects dde1 = DoDragDrop(s, DragDropEffects.All);
        }

        private void lb_DragLeave(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
            lb_item = lb.SelectedItem;
            source_LB = lb;
            //lb.Items.Remove(lb.SelectedItem);
            //lb.DoDragDrop(lb_item, DragDropEffects.Move);
        }


        private void lb_DragEnter(object sender, DragEventArgs e)
        {
            if (lb_item != null)
            {
                e.Effect = DragDropEffects.Move;
                ListBox lb = sender as ListBox;
                if (!lb.Items.Contains(lb_item)) { lb.Items.Add(lb_item); }
                //if (source_LB.Name != lb_AudioProcesses.Name) { source_LB.Items.Remove(lb_item); }
                source_LB.Items.Remove(lb_item);
                source_LB = null;
                lb_item = null;

                //store values
                GAME = "";
                foreach (string item in lb_GAME.Items)
                {
                    GAME += item + ",";
                }
                CHAT = "";
                foreach (string item in lb_CHAT.Items)
                {
                    CHAT += item + ",";
                }

                settingsStore.Game = GAME;
                settingsStore.Chat = CHAT;
                settingsStore.ScheduleSave();

                if (Debug) { textBox1.AppendText("GAME: " + GAME + "\r\n"); }
                if (Debug) { textBox1.AppendText("CHAT: " + CHAT + "\r\n"); }
            }
        }

        private void lb_MouseUp(object sender, MouseEventArgs e)
        {
            if (lb_item != null)
            {
                ListBox lb = sender as ListBox;
                lb.Items.Add(lb_item);
                source_LB.Items.Remove(lb_item);
                source_LB = null;
                lb_item = null;
            }
        }

        private void lb_GAME_MouseUp(object sender, MouseEventArgs e)
        {
            //textBox1.AppendText("Mouse up\r\n");
        }

        private void btn_settings_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
            fill_lb_AudioProcesses();
        }

        private void btn_log_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            audioSessionService.AudioSessionsChanged -= AudioSessionService_AudioSessionsChanged;
            audioSessionService.StopSessionMonitoring();

            PersistSettingsImmediate();
            usbWatcherService.Stop();
            serialDeviceService.Shutdown();
            SystemEvents.SessionEnding -= OnSessionEnding;
        }

        private void OnSessionEnding(object? sender, SessionEndingEventArgs e)
        {
            PersistSettingsImmediate();
        }

        private void PersistSettingsImmediate()
        {
            settingsStore.Game = GAME;
            settingsStore.Chat = CHAT;
            settingsStore.SaveNow();
        }

        private void ddl_ComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SendToLog("DDL_ComPort Selection changed");
            settingsStore.ComPort = ddl_ComPort.SelectedItem.ToString();
            settingsStore.SaveNow();
            if (!initialized) { return; }
            //muss hier noch etwas getan werden w?hren eines Updates? (Beim Neubef?llen der DDL?)
            serialDeviceService.CloseComPort();
            serialDeviceService.OpenComPort();
            if (serialDeviceService.IsConnected()) { GetVol(); Send_NoiseReducion_Value(); }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ddlNoiseReduction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!initialized) { return; }
            settingsStore.NoiseReduction = ddlNoiseReduction.Text;
            settingsStore.ScheduleSave();
            //USBandCOM.sp_SendData(message);
            //USBandCOM.CloseComPort();
            if (serialDeviceService.IsConnected()) { Send_NoiseReducion_Value(); }
            SendToLog("Key: " + ddlNoiseReduction.Text);
            SendToLog("Value: " + ddlNoiseReduction.SelectedValue.ToString());
            SendToLog("Stored Value: " + settingsStore.NoiseReduction);
            //SendToLog(ddlNoiseReduction.SelectedValue.ToString());
        }

        private void btn_AudioProcesses_refresh_Click(object sender, EventArgs e)
        {
            fill_lb_AudioProcesses();
        }

        private void cb_Debug_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_Debug.Checked == true) { debug = true; } else { debug = false; }
        }

        private void cb_invert_CheckedChanged(object sender, EventArgs e)
        {
            settingsStore.Invert = cb_invert.Checked;
            settingsStore.ScheduleSave();
            if (cb_invert.Checked == true)
            {
                controlVolume(float.Parse(lbl_absoluteval.Text));
            }
            else
            {
                controlVolume(100 - float.Parse(lbl_absoluteval.Text));
            }
        }
    }
}