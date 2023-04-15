namespace AudioControl
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
            btn_ComPort_Refresh = new Button();
            ddl_ComPort = new ComboBox();
            lbl_ComPort = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            tabPage1 = new TabPage();
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
            systrayCom.BorderStyle = BorderStyle.None;
            systrayCom.Name = "systrayCom";
            systrayCom.ReadOnly = true;
            systrayCom.Size = new Size(100, 16);
            // 
            // systrayVolume
            // 
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
            tabPage2.Controls.Add(btn_ComPort_Refresh);
            tabPage2.Controls.Add(ddl_ComPort);
            tabPage2.Controls.Add(lbl_ComPort);
            tabPage2.Controls.Add(textBox1);
            tabPage2.Controls.Add(button1);
            tabPage2.Location = new Point(4, 5);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(508, 251);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // btn_ComPort_Refresh
            // 
            btn_ComPort_Refresh.BackgroundImage = (Image)resources.GetObject("btn_ComPort_Refresh.BackgroundImage");
            btn_ComPort_Refresh.BackgroundImageLayout = ImageLayout.Stretch;
            btn_ComPort_Refresh.Location = new Point(194, 216);
            btn_ComPort_Refresh.Name = "btn_ComPort_Refresh";
            btn_ComPort_Refresh.Size = new Size(29, 27);
            btn_ComPort_Refresh.TabIndex = 4;
            btn_ComPort_Refresh.UseVisualStyleBackColor = true;
            btn_ComPort_Refresh.Click += btn_ComPort_Refresh_Click;
            // 
            // ddl_ComPort
            // 
            ddl_ComPort.FormattingEnabled = true;
            ddl_ComPort.Location = new Point(67, 220);
            ddl_ComPort.Name = "ddl_ComPort";
            ddl_ComPort.Size = new Size(121, 23);
            ddl_ComPort.Sorted = true;
            ddl_ComPort.TabIndex = 3;
            ddl_ComPort.SelectedIndexChanged += ddl_ComPort_SelectedIndexChanged;
            // 
            // lbl_ComPort
            // 
            lbl_ComPort.AutoSize = true;
            lbl_ComPort.Location = new Point(6, 223);
            lbl_ComPort.Name = "lbl_ComPort";
            lbl_ComPort.Size = new Size(55, 15);
            lbl_ComPort.TabIndex = 2;
            lbl_ComPort.Text = "ComPort";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 6);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(496, 157);
            textBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(192, 169);
            button1.Name = "button1";
            button1.Size = new Size(115, 31);
            button1.TabIndex = 1;
            button1.Text = "(Re-) Connect";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btn_connect_Click;
            // 
            // tabPage1
            // 
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
            tabPage1.Size = new Size(508, 251);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
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
            lbl_absoluteval.Location = new Point(234, 197);
            lbl_absoluteval.Name = "lbl_absoluteval";
            lbl_absoluteval.Size = new Size(19, 15);
            lbl_absoluteval.TabIndex = 12;
            lbl_absoluteval.Text = "99";
            lbl_absoluteval.TextAlign = ContentAlignment.MiddleCenter;
            lbl_absoluteval.Click += label1_Click;
            // 
            // trackBar1
            // 
            trackBar1.Enabled = false;
            trackBar1.Location = new Point(87, 167);
            trackBar1.Maximum = 100;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(324, 45);
            trackBar1.TabIndex = 11;
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
            lbl_HideTabs.Location = new Point(59, -5);
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
            tabControl1.Size = new Size(516, 260);
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
            lbl_vertical_line.Size = new Size(1, 250);
            lbl_vertical_line.TabIndex = 10;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(593, 274);
            Controls.Add(lbl_HideTabs);
            Controls.Add(lbl_vertical_line);
            Controls.Add(btn_settings);
            Controls.Add(btn_log);
            Controls.Add(tabControl1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            ShowInTaskbar = false;
            Text = "AudioControl - Settings";
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
        private Button button1;
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
        private Button btn_ComPort_Refresh;
        private TrackBar trackBar1;
        private Label lbl_chat_vol;
        private Label lbl_game_vol;
        private Label lbl_absoluteval;
    }
}