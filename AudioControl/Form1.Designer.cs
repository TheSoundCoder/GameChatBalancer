﻿namespace AudioControl
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Settings = new ContextMenuStrip(components);
            systrayCom = new ToolStripTextBox();
            systrayVolume = new ToolStripTextBox();
            toolStripSeparator1 = new ToolStripSeparator();
            openToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            trayicon = new NotifyIcon(components);
            tabPage2 = new TabPage();
            cb_Debug = new CheckBox();
            textBox1 = new TextBox();
            ddl_ComPort = new ComboBox();
            lbl_ComPort = new Label();
            tabPage1 = new TabPage();
            cb_connected = new CheckBox();
            cb_invert = new CheckBox();
            lbl_Hardware = new Label();
            btn_AudioProcesses_refresh = new Button();
            label1 = new Label();
            cbNR = new CheckBox();
            lblNoiseReduction = new Label();
            ddlNoiseReduction = new ComboBox();
            lbl_chat_vol = new Label();
            lbl_game_vol = new Label();
            lbl_absoluteval = new Label();
            trackBar1 = new TrackBar();
            lb_CHAT = new ListBox();
            lb_AudioProcesses = new ListBox();
            pb_Chat = new PictureBox();
            lb_GAME = new ListBox();
            pb_Game = new PictureBox();
            lbl_HideTabs = new Label();
            tabControl1 = new TabControl();
            btn_log = new Button();
            btn_settings = new Button();
            lbl_vertical_line = new Label();
            Settings.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pb_Chat).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pb_Game).BeginInit();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // Settings
            // 
            Settings.Items.AddRange(new ToolStripItem[] { systrayCom, systrayVolume, toolStripSeparator1, openToolStripMenuItem, closeToolStripMenuItem });
            Settings.Name = "contextMenuStrip1";
            Settings.Size = new Size(161, 90);
            Settings.Opening += Settings_Opening;
            // 
            // systrayCom
            // 
            systrayCom.BackColor = SystemColors.ControlLightLight;
            systrayCom.BorderStyle = BorderStyle.None;
            systrayCom.Name = "systrayCom";
            systrayCom.ReadOnly = true;
            systrayCom.Size = new Size(100, 16);
            systrayCom.Text = "Not connected";
            // 
            // systrayVolume
            // 
            systrayVolume.BackColor = SystemColors.ControlLightLight;
            systrayVolume.BorderStyle = BorderStyle.None;
            systrayVolume.Name = "systrayVolume";
            systrayVolume.ReadOnly = true;
            systrayVolume.Size = new Size(100, 16);
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(157, 6);
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(160, 22);
            openToolStripMenuItem.Text = "Show";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(160, 22);
            closeToolStripMenuItem.Text = "Exit";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // trayicon
            // 
            trayicon.ContextMenuStrip = Settings;
            trayicon.Icon = (Icon)resources.GetObject("trayicon.Icon");
            trayicon.Text = "AudioControl";
            trayicon.Visible = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(cb_Debug);
            tabPage2.Controls.Add(textBox1);
            tabPage2.Location = new Point(4, 5);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(508, 338);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // cb_Debug
            // 
            cb_Debug.AutoSize = true;
            cb_Debug.Location = new Point(6, 6);
            cb_Debug.Name = "cb_Debug";
            cb_Debug.RightToLeft = RightToLeft.Yes;
            cb_Debug.Size = new Size(61, 19);
            cb_Debug.TabIndex = 2;
            cb_Debug.Text = "Debug";
            cb_Debug.UseVisualStyleBackColor = true;
            cb_Debug.CheckedChanged += cb_Debug_CheckedChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 31);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(496, 301);
            textBox1.TabIndex = 0;
            // 
            // ddl_ComPort
            // 
            ddl_ComPort.FormattingEnabled = true;
            ddl_ComPort.Location = new Point(106, 269);
            ddl_ComPort.Name = "ddl_ComPort";
            ddl_ComPort.Size = new Size(74, 23);
            ddl_ComPort.Sorted = true;
            ddl_ComPort.TabIndex = 3;
            ddl_ComPort.SelectedIndexChanged += ddl_ComPort_SelectedIndexChanged;
            // 
            // lbl_ComPort
            // 
            lbl_ComPort.AutoSize = true;
            lbl_ComPort.Location = new Point(6, 272);
            lbl_ComPort.Name = "lbl_ComPort";
            lbl_ComPort.Size = new Size(55, 15);
            lbl_ComPort.TabIndex = 2;
            lbl_ComPort.Text = "ComPort";
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(cb_connected);
            tabPage1.Controls.Add(cb_invert);
            tabPage1.Controls.Add(lbl_Hardware);
            tabPage1.Controls.Add(btn_AudioProcesses_refresh);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(cbNR);
            tabPage1.Controls.Add(ddl_ComPort);
            tabPage1.Controls.Add(lblNoiseReduction);
            tabPage1.Controls.Add(lbl_ComPort);
            tabPage1.Controls.Add(ddlNoiseReduction);
            tabPage1.Controls.Add(lbl_chat_vol);
            tabPage1.Controls.Add(lbl_game_vol);
            tabPage1.Controls.Add(lbl_absoluteval);
            tabPage1.Controls.Add(trackBar1);
            tabPage1.Controls.Add(lb_CHAT);
            tabPage1.Controls.Add(lb_AudioProcesses);
            tabPage1.Controls.Add(pb_Chat);
            tabPage1.Controls.Add(lb_GAME);
            tabPage1.Controls.Add(pb_Game);
            tabPage1.Location = new Point(4, 5);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(508, 338);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // cb_connected
            // 
            cb_connected.AutoSize = true;
            cb_connected.Enabled = false;
            cb_connected.Location = new Point(185, 271);
            cb_connected.Name = "cb_connected";
            cb_connected.Size = new Size(84, 19);
            cb_connected.TabIndex = 21;
            cb_connected.Text = "Connected";
            cb_connected.UseVisualStyleBackColor = true;
            // 
            // cb_invert
            // 
            cb_invert.AutoSize = true;
            cb_invert.Location = new Point(406, 269);
            cb_invert.Name = "cb_invert";
            cb_invert.Size = new Size(56, 19);
            cb_invert.TabIndex = 20;
            cb_invert.Text = "Invert";
            cb_invert.UseVisualStyleBackColor = true;
            cb_invert.CheckedChanged += cb_invert_CheckedChanged;
            // 
            // lbl_Hardware
            // 
            lbl_Hardware.AutoSize = true;
            lbl_Hardware.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            lbl_Hardware.Location = new Point(6, 228);
            lbl_Hardware.Name = "lbl_Hardware";
            lbl_Hardware.Size = new Size(185, 25);
            lbl_Hardware.TabIndex = 19;
            lbl_Hardware.Text = "Hardware properties";
            // 
            // btn_AudioProcesses_refresh
            // 
            btn_AudioProcesses_refresh.BackgroundImage = (Image)resources.GetObject("btn_AudioProcesses_refresh.BackgroundImage");
            btn_AudioProcesses_refresh.BackgroundImageLayout = ImageLayout.Stretch;
            btn_AudioProcesses_refresh.Location = new Point(229, 101);
            btn_AudioProcesses_refresh.Name = "btn_AudioProcesses_refresh";
            btn_AudioProcesses_refresh.Size = new Size(29, 29);
            btn_AudioProcesses_refresh.TabIndex = 18;
            btn_AudioProcesses_refresh.UseVisualStyleBackColor = true;
            btn_AudioProcesses_refresh.Click += btn_AudioProcesses_refresh_Click;
            // 
            // label1
            // 
            label1.BackColor = SystemColors.MenuText;
            label1.Location = new Point(1, 218);
            label1.Name = "label1";
            label1.Size = new Size(501, 1);
            label1.TabIndex = 12;
            // 
            // cbNR
            // 
            cbNR.AutoSize = true;
            cbNR.Enabled = false;
            cbNR.Location = new Point(185, 308);
            cbNR.Name = "cbNR";
            cbNR.Size = new Size(100, 19);
            cbNR.TabIndex = 17;
            cbNR.Text = "NR confirmed";
            cbNR.UseVisualStyleBackColor = true;
            // 
            // lblNoiseReduction
            // 
            lblNoiseReduction.AutoSize = true;
            lblNoiseReduction.Location = new Point(6, 309);
            lblNoiseReduction.Name = "lblNoiseReduction";
            lblNoiseReduction.Size = new Size(94, 15);
            lblNoiseReduction.TabIndex = 16;
            lblNoiseReduction.Text = "Noise Reduction";
            // 
            // ddlNoiseReduction
            // 
            ddlNoiseReduction.Enabled = false;
            ddlNoiseReduction.FormattingEnabled = true;
            ddlNoiseReduction.Items.AddRange(new object[] { "Off", "Low", "High" });
            ddlNoiseReduction.Location = new Point(106, 306);
            ddlNoiseReduction.Name = "ddlNoiseReduction";
            ddlNoiseReduction.Size = new Size(74, 23);
            ddlNoiseReduction.TabIndex = 15;
            ddlNoiseReduction.Text = "High";
            ddlNoiseReduction.SelectedIndexChanged += ddlNoiseReduction_SelectedIndexChanged;
            // 
            // lbl_chat_vol
            // 
            lbl_chat_vol.AutoSize = true;
            lbl_chat_vol.Location = new Point(119, 122);
            lbl_chat_vol.Name = "lbl_chat_vol";
            lbl_chat_vol.Size = new Size(19, 15);
            lbl_chat_vol.TabIndex = 14;
            lbl_chat_vol.Text = "99";
            lbl_chat_vol.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lbl_game_vol
            // 
            lbl_game_vol.AutoSize = true;
            lbl_game_vol.Location = new Point(348, 122);
            lbl_game_vol.Name = "lbl_game_vol";
            lbl_game_vol.Size = new Size(19, 15);
            lbl_game_vol.TabIndex = 13;
            lbl_game_vol.Text = "99";
            lbl_game_vol.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lbl_absoluteval
            // 
            lbl_absoluteval.AutoSize = true;
            lbl_absoluteval.Location = new Point(234, 185);
            lbl_absoluteval.Name = "lbl_absoluteval";
            lbl_absoluteval.Size = new Size(0, 15);
            lbl_absoluteval.TabIndex = 12;
            lbl_absoluteval.TextAlign = ContentAlignment.MiddleCenter;
            lbl_absoluteval.Click += label1_Click;
            // 
            // trackBar1
            // 
            trackBar1.Enabled = false;
            trackBar1.Location = new Point(81, 154);
            trackBar1.Maximum = 100;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(324, 45);
            trackBar1.TabIndex = 11;
            trackBar1.Value = 50;
            // 
            // lb_CHAT
            // 
            lb_CHAT.AllowDrop = true;
            lb_CHAT.FormattingEnabled = true;
            lb_CHAT.ItemHeight = 15;
            lb_CHAT.Location = new Point(31, 6);
            lb_CHAT.Name = "lb_CHAT";
            lb_CHAT.Size = new Size(120, 94);
            lb_CHAT.Sorted = true;
            lb_CHAT.TabIndex = 3;
            lb_CHAT.DragEnter += lb_DragEnter;
            lb_CHAT.DragLeave += lb_DragLeave;
            lb_CHAT.MouseDown += lb_MouseDown;
            lb_CHAT.MouseUp += lb_MouseUp;
            // 
            // lb_AudioProcesses
            // 
            lb_AudioProcesses.AllowDrop = true;
            lb_AudioProcesses.FormattingEnabled = true;
            lb_AudioProcesses.ItemHeight = 15;
            lb_AudioProcesses.Location = new Point(185, 6);
            lb_AudioProcesses.Name = "lb_AudioProcesses";
            lb_AudioProcesses.Size = new Size(120, 94);
            lb_AudioProcesses.Sorted = true;
            lb_AudioProcesses.TabIndex = 2;
            lb_AudioProcesses.DragEnter += lb_DragEnter;
            lb_AudioProcesses.DragLeave += lb_DragLeave;
            lb_AudioProcesses.MouseDown += lb_MouseDown;
            lb_AudioProcesses.MouseUp += lb_MouseUp;
            // 
            // pb_Chat
            // 
            pb_Chat.Image = Properties.Resources.ico_chat;
            pb_Chat.Location = new Point(66, 106);
            pb_Chat.Name = "pb_Chat";
            pb_Chat.Size = new Size(47, 42);
            pb_Chat.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_Chat.TabIndex = 6;
            pb_Chat.TabStop = false;
            // 
            // lb_GAME
            // 
            lb_GAME.AllowDrop = true;
            lb_GAME.FormattingEnabled = true;
            lb_GAME.ItemHeight = 15;
            lb_GAME.Location = new Point(340, 6);
            lb_GAME.Name = "lb_GAME";
            lb_GAME.Size = new Size(120, 94);
            lb_GAME.Sorted = true;
            lb_GAME.TabIndex = 4;
            lb_GAME.DragEnter += lb_DragEnter;
            lb_GAME.DragLeave += lb_DragLeave;
            lb_GAME.MouseDown += lb_MouseDown;
            lb_GAME.MouseUp += lb_GAME_MouseUp;
            // 
            // pb_Game
            // 
            pb_Game.Image = Properties.Resources.ico_game;
            pb_Game.Location = new Point(373, 106);
            pb_Game.Name = "pb_Game";
            pb_Game.Size = new Size(51, 42);
            pb_Game.SizeMode = PictureBoxSizeMode.StretchImage;
            pb_Game.TabIndex = 5;
            pb_Game.TabStop = false;
            // 
            // lbl_HideTabs
            // 
            lbl_HideTabs.Location = new Point(73, -5);
            lbl_HideTabs.Name = "lbl_HideTabs";
            lbl_HideTabs.Size = new Size(280, 23);
            lbl_HideTabs.TabIndex = 11;
            // 
            // tabControl1
            // 
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.Location = new Point(73, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(516, 347);
            tabControl1.TabIndex = 7;
            // 
            // btn_log
            // 
            btn_log.FlatStyle = FlatStyle.Flat;
            btn_log.Image = (Image)resources.GetObject("btn_log.Image");
            btn_log.Location = new Point(12, 89);
            btn_log.Name = "btn_log";
            btn_log.Size = new Size(55, 55);
            btn_log.TabIndex = 7;
            btn_log.UseVisualStyleBackColor = true;
            btn_log.Click += btn_log_Click;
            // 
            // btn_settings
            // 
            btn_settings.FlatStyle = FlatStyle.Flat;
            btn_settings.Image = (Image)resources.GetObject("btn_settings.Image");
            btn_settings.Location = new Point(12, 23);
            btn_settings.Name = "btn_settings";
            btn_settings.Size = new Size(55, 55);
            btn_settings.TabIndex = 9;
            btn_settings.UseVisualStyleBackColor = true;
            btn_settings.Click += btn_settings_Click;
            // 
            // lbl_vertical_line
            // 
            lbl_vertical_line.BackColor = SystemColors.MenuText;
            lbl_vertical_line.Location = new Point(73, 18);
            lbl_vertical_line.Name = "lbl_vertical_line";
            lbl_vertical_line.Size = new Size(1, 350);
            lbl_vertical_line.TabIndex = 10;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(593, 356);
            Controls.Add(lbl_HideTabs);
            Controls.Add(lbl_vertical_line);
            Controls.Add(btn_settings);
            Controls.Add(btn_log);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            ShowInTaskbar = false;
            Text = "GameChat Balancer - Settings";
            WindowState = FormWindowState.Minimized;
            Load += Form1_Load;
            Move += Form1_Move;
            Settings.ResumeLayout(false);
            Settings.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pb_Chat).EndInit();
            ((System.ComponentModel.ISupportInitialize)pb_Game).EndInit();
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ContextMenuStrip Settings;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripTextBox systrayCom;
        private NotifyIcon trayicon;
        private ToolStripTextBox systrayVolume;
        private ToolStripSeparator toolStripSeparator1;
        private TabPage tabPage2;
        private TextBox textBox1;
        private TabPage tabPage1;
        private ListBox lb_CHAT;
        private ListBox lb_AudioProcesses;
        private PictureBox pb_Chat;
        private ListBox lb_GAME;
        private PictureBox pb_Game;
        private TabControl tabControl1;
        private Button btn_log;
        private Button btn_settings;
        private Label lbl_vertical_line;
        private Label lbl_HideTabs;
        private ComboBox ddl_ComPort;
        private Label lbl_ComPort;
        private TrackBar trackBar1;
        private Label lbl_chat_vol;
        private Label lbl_game_vol;
        private Label lbl_absoluteval;
        private Label lblNoiseReduction;
        private ComboBox ddlNoiseReduction;
        private CheckBox cbNR;
        private Button btn_AudioProcesses_refresh;
        private Label label1;
        private CheckBox cb_Debug;
        private Label lbl_Hardware;
        private CheckBox cb_invert;
        private CheckBox cb_connected;
    }
}