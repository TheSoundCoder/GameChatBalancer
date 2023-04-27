//using Microsoft.VisualBasic;
//using System.Linq.Expressions;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
//using static System.Runtime.InteropServices.JavaScript.JSType;
//using System.Security.Cryptography;
using System.Management;
using System.Windows.Forms;
using static AudioManager.AudioManager;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AudioControl
{
    public partial class Form1 : Form
    {
        //static SerialPort _serialPort;
        //string ComPort = Properties.Settings.Default.ComPort;    //Config

        string GAME = Properties.Settings.Default.GAME;   //List of Games comma separated
        string CHAT = Properties.Settings.Default.CHAT;    //List of Voice apps comma separated

        bool debug = false;
        public object lb_item = null;
        ListBox source_LB = null;

        public Form1()
        {
            InitializeComponent();
            // Handle the ApplicationExit event to know when the application is exiting.
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);
            USBandCOM.HandOverForm(this);
            HandOverForm(this); //AudioManager
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
            cb_invert.Checked = Properties.Settings.Default.Invert;
            if (Properties.Settings.Default.ComPort == "") { Properties.Settings.Default.ComPort = "Auto"; }
            fill_ddl_ComPort();

            USBandCOM.Initialize_USB_Watcher();
            USBandCOM.OpenComPort();
            if (USBandCOM.sp_connected())
            {
                GetVol();
                Send_NoiseReducion_Value();
            }
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
            lb_AudioProcesses.Items.AddRange(GetAudioApplications(false).Split("\r\n").Distinct().ToArray());
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

        private void fill_ddl_ComPort()
        {
            string strOptions = "";
            string strSelection = Properties.Settings.Default.ComPort;
            strOptions += "Auto,";
            strOptions += strSelection + ",";
            foreach (string ComPort in SerialPort.GetPortNames())
            {
                strOptions += ComPort + ",";
            }
            ddl_ComPort.Items.Clear();
            ddl_ComPort.Items.AddRange(strOptions.Split(",").Distinct().ToArray());
            ddl_ComPort.Items.Remove("");
            ddl_ComPort.SelectedItem = Properties.Settings.Default.ComPort;
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
            ddlNoiseReduction.Text = Properties.Settings.Default.NoiseReduction;
        }

        public void SendToLog(string msg)
        {
            textBox1.AppendText(msg + "\r\n");
        }

        public void ConfirmNR()
        {
            cbNR.Checked = true;
        }


        public bool Debug
        {
            get => debug;
            set { debug = value; }
        }

        public string SystrayCom
        {
            get => systrayCom.Text;
            set { systrayCom.Text = value; }
        }

        public void Send_NoiseReducion_Value()
        {
            cbNR.Checked = false;
            USBandCOM.sp_SendData(ddlNoiseReduction.SelectedValue.ToString());
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            USBandCOM.CloseComPort();
            USBandCOM.OpenComPort();
            if (USBandCOM.sp_connected()) { GetVol(); }
        }


        public void controlVolume(float volume)
        {
            //if (Debug) { textBox1.AppendText(volume.ToString() + "\r\n"); }
            if (cb_invert.Checked) { volume = 100 - volume; }
            systrayVolume.Text = volume.ToString();
            trackBar1.Value = (int)volume;
            lbl_absoluteval.Text = volume.ToString();
            if (volume < 50)
            {
                //tune GAME
                SetApplicationVolumeByName(GAME, volume * 2);
                SetApplicationVolumeByName(CHAT, 100);
                lbl_game_vol.Text = (volume * 2).ToString();
                lbl_chat_vol.Text = "100";
            }
            else if (volume > 50)
            {
                //tune CHAT
                SetApplicationVolumeByName(CHAT, 100 - ((volume - 50) * 2));
                SetApplicationVolumeByName(GAME, 100);
                lbl_chat_vol.Text = (100 - ((volume - 50) * 2)).ToString();
                lbl_game_vol.Text = "100";
            }
            else
            {
                //set BOTH to 100
                SetApplicationVolumeByName(GAME, 100);
                SetApplicationVolumeByName(CHAT, 100);
                lbl_game_vol.Text = "100";
                lbl_chat_vol.Text = "100";
            }

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
            USBandCOM.sp_SendData("get");
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
            // When the application is exiting, write the application data to the
            // user file and close it.
            Properties.Settings.Default.GAME = GAME;
            Properties.Settings.Default.CHAT = CHAT;
            Properties.Settings.Default.Save();
            USBandCOM.CloseComPort();
        }

        private void ddl_ComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ComPort = ddl_ComPort.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

        private void btn_ComPort_Refresh_Click(object sender, EventArgs e)
        {
            fill_ddl_ComPort();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ddlNoiseReduction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!ddlNoiseReduction.Enabled) { return; }
            Properties.Settings.Default.NoiseReduction = ddlNoiseReduction.Text;
            //USBandCOM.sp_SendData(message);
            //USBandCOM.CloseComPort();
            if (USBandCOM.sp_connected()) { Send_NoiseReducion_Value(); }
            if (Debug) { SendToLog("Key: " + ddlNoiseReduction.Text); }
            if (Debug) { SendToLog("Value: " + ddlNoiseReduction.SelectedValue.ToString()); }
            if (Debug) { SendToLog("Stored Value: " + Properties.Settings.Default.NoiseReduction); }
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
            Properties.Settings.Default.Invert = cb_invert.Checked;
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