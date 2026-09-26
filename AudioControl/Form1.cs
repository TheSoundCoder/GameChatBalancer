//using Microsoft.VisualBasic;
//using System.Linq.Expressions;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using Microsoft.Win32;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using System.Security.Cryptography;
using System.Management;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AudioControl
{
    public partial class Form1 : Form
    {
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
        readonly ISettingsStore settingsStore;

        public Form1()
            : this(null, null, null, null, null)
        {
        }

        internal Form1(ISerialDeviceService? serialDeviceService)
            : this(serialDeviceService, null, null, null, null)
        {
        }

        internal Form1(ISerialDeviceService? serialDeviceService, IUsbWatcherService? usbWatcherService)
            : this(serialDeviceService, usbWatcherService, null, null, null)
        {
        }

        internal Form1(ISerialDeviceService? serialDeviceService, IUsbWatcherService? usbWatcherService, IAudioSessionService? audioSessionService)
            : this(serialDeviceService, usbWatcherService, audioSessionService, null, null)
        {
        }

        internal Form1(ISerialDeviceService? serialDeviceService, IUsbWatcherService? usbWatcherService, IAudioSessionService? audioSessionService, ISettingsStore? settingsStore, IAudioBalanceService? audioBalanceService)
        {
            InitializeComponent();
            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            SystemEvents.SessionEnding += OnSessionEnding;
            this.serialDeviceService = serialDeviceService ?? new SerialDeviceService(this);
            this.usbWatcherService = usbWatcherService ?? new UsbWatcherService();
            this.audioSessionService = audioSessionService ?? new AudioSessionService(this);
            this.settingsStore = settingsStore ?? new SettingsStore();
            this.audioBalanceService = audioBalanceService ?? new AudioBalanceService(this.audioSessionService);
            GAME = this.settingsStore.Game;
            CHAT = this.settingsStore.Chat;
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

            initialized = true;
        }

        private void fill_lb_GAME()
        {
            lb_GAME.Items.AddRange(GAME.Split(","));
            lb_GAME.Items.Remove("");
        }

        private void fill_lb_CHAT()
        {
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

            if (debug) { textBox1.AppendText(msg + "\r\n"); }
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
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.TopMost = true;
            fill_lb_AudioProcesses();
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
            Application.Exit();
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