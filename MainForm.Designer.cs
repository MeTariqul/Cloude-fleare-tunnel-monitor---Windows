namespace CloudflareTunnelMonitor
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private TabControl tabControl;
        private TabPage tabDashboard;
        private TabPage tabLogs;
        private TabPage tabSettings;

        // Dashboard controls
        private GroupBox grpTunnelControl;
        private Label lblLocalUrl;
        private TextBox txtLocalUrl;
        private Button btnApplyLocalUrl;
        private Button btnStart;
        private Button btnStop;
        private Label lblStatus;
        private Label lblStatusText;

        private GroupBox grpCurrentUrl;
        private TextBox txtCurrentUrl;
        private Button btnCopyUrl;

        private GroupBox grpNetworkStatus;
        private Label lblNetworkStatus;

        private GroupBox grpStatistics;
        private Label lblUptime;
        private Label lblLastUrlGenerated;

        // Logs controls
        private TextBox txtLog;
        private Button btnClearLog;
        private Button btnSaveLog;
        private CheckBox chkAutoScroll;

        // Settings controls
        private GroupBox grpPaths;
        private Label lblCloudflaredPath;
        private TextBox txtCloudflaredPath;
        private Button btnBrowseCloudflared;
        private Label lblSaveDirectory;
        private TextBox txtSaveDirectory;
        private Button btnBrowseSaveDir;

        private GroupBox grpBehavior;
        private CheckBox chkAutoStart;
        private CheckBox chkAutoReconnect;
        private CheckBox chkMinimizeToTray;
        private Label lblCheckInterval;
        private NumericUpDown numCheckInterval;
        private Label lblPingTestUrl;
        private TextBox txtPingTestUrl;
        private CheckBox chkAutoCopyUrl;

        private GroupBox grpActions;
        private Button btnResetSettings;
        private Button btnOpenSaveDirectory;
        private Button btnViewUrlLog;

        private GroupBox grpAbout;
        private Label lblVersion;
        private LinkLabel lnkGitHub;

        // Status strip
        private StatusStrip statusStrip;
        private ToolStripStatusLabel tsStatusLabel;
        private ToolStripStatusLabel tsUptimeLabel;

        // Notify Icon
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem showToolStripMenuItem;
        private ToolStripMenuItem startTunnelToolStripMenuItem;
        private ToolStripMenuItem stopTunnelToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private TableLayoutPanel dashboardLayout;
        private TableLayoutPanel settingsLayout;
        private TableLayoutPanel logsLayout;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl = new TabControl();
            this.tabDashboard = new TabPage();
            this.tabLogs = new TabPage();
            this.tabSettings = new TabPage();
            this.statusStrip = new StatusStrip();
            this.tsStatusLabel = new ToolStripStatusLabel();
            this.tsUptimeLabel = new ToolStripStatusLabel();
            this.notifyIcon = new NotifyIcon(this.components);
            // Use the application icon for the system tray
            var iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.ico");
            if (File.Exists(iconPath))
            {
                this.notifyIcon.Icon = new Icon(iconPath);
            }
            else if (this.Icon != null)
            {
                this.notifyIcon.Icon = this.Icon;
            }
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            
            // Layout panels for responsive design
            this.dashboardLayout = new TableLayoutPanel();
            this.settingsLayout = new TableLayoutPanel();
            this.logsLayout = new TableLayoutPanel();

            // Dashboard Tab
            this.grpTunnelControl = new GroupBox();
            this.lblLocalUrl = new Label();
            this.txtLocalUrl = new TextBox();
            this.btnApplyLocalUrl = new Button();
            this.btnStart = new Button();
            this.btnStop = new Button();
            this.lblStatusText = new Label();
            this.lblStatus = new Label();

            this.grpCurrentUrl = new GroupBox();
            this.txtCurrentUrl = new TextBox();
            this.btnCopyUrl = new Button();

            this.grpNetworkStatus = new GroupBox();
            this.lblNetworkStatus = new Label();

            this.grpStatistics = new GroupBox();
            this.lblUptime = new Label();
            this.lblLastUrlGenerated = new Label();

            // Logs Tab
            this.txtLog = new TextBox();
            this.btnClearLog = new Button();
            this.btnSaveLog = new Button();
            this.chkAutoScroll = new CheckBox();

            // Settings Tab
            this.grpPaths = new GroupBox();
            this.lblCloudflaredPath = new Label();
            this.txtCloudflaredPath = new TextBox();
            this.btnBrowseCloudflared = new Button();
            this.lblSaveDirectory = new Label();
            this.txtSaveDirectory = new TextBox();
            this.btnBrowseSaveDir = new Button();

            this.grpBehavior = new GroupBox();
            this.chkAutoStart = new CheckBox();
            this.chkAutoReconnect = new CheckBox();
            this.chkMinimizeToTray = new CheckBox();
            this.chkAutoCopyUrl = new CheckBox();
            this.lblCheckInterval = new Label();
            this.numCheckInterval = new NumericUpDown();
            this.lblPingTestUrl = new Label();
            this.txtPingTestUrl = new TextBox();

            this.grpActions = new GroupBox();
            this.btnResetSettings = new Button();
            this.btnOpenSaveDirectory = new Button();
            this.btnViewUrlLog = new Button();

            this.grpAbout = new GroupBox();
            this.lblVersion = new Label();
            this.lnkGitHub = new LinkLabel();

            // Context menu items
            this.showToolStripMenuItem = new ToolStripMenuItem();
            this.startTunnelToolStripMenuItem = new ToolStripMenuItem();
            this.stopTunnelToolStripMenuItem = new ToolStripMenuItem();
            this.exitToolStripMenuItem = new ToolStripMenuItem();

            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabDashboard);
            this.tabControl.Controls.Add(this.tabLogs);
            this.tabControl.Controls.Add(this.tabSettings);
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(800, 450);
            this.tabControl.TabIndex = 0;
            // 
            // Dashboard Layout (2x2 responsive grid)
            // 
            this.dashboardLayout.ColumnCount = 2;
            this.dashboardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.dashboardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            this.dashboardLayout.Dock = DockStyle.Fill;
            this.dashboardLayout.Location = new Point(3, 3);
            this.dashboardLayout.Name = "dashboardLayout";
            this.dashboardLayout.RowCount = 3;
            this.dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
            this.dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            this.dashboardLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.dashboardLayout.Size = new Size(786, 415);
            this.dashboardLayout.TabIndex = 0;
            
            this.dashboardLayout.Controls.Add(this.grpTunnelControl, 0, 0);
            this.dashboardLayout.Controls.Add(this.grpCurrentUrl, 1, 0);
            this.dashboardLayout.Controls.Add(this.grpNetworkStatus, 1, 1);
            this.dashboardLayout.Controls.Add(this.grpStatistics, 0, 2);
            this.dashboardLayout.SetColumnSpan(this.grpStatistics, 2);
            
            this.tabDashboard.Controls.Add(this.dashboardLayout);
            this.tabDashboard.Location = new Point(4, 25);
            this.tabDashboard.Name = "tabDashboard";
            this.tabDashboard.Padding = new Padding(3);
            this.tabDashboard.Size = new Size(792, 421);
            this.tabDashboard.TabIndex = 0;
            this.tabDashboard.Text = "Dashboard";
            this.tabDashboard.UseVisualStyleBackColor = true;
            
            // Logs Layout
            this.logsLayout.ColumnCount = 3;
            this.logsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.logsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            this.logsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.logsLayout.Dock = DockStyle.Fill;
            this.logsLayout.Location = new Point(3, 3);
            this.logsLayout.Name = "logsLayout";
            this.logsLayout.RowCount = 2;
            this.logsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.logsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.logsLayout.Size = new Size(786, 415);
            this.logsLayout.TabIndex = 0;
            
            this.logsLayout.Controls.Add(this.txtLog, 0, 0);
            this.logsLayout.SetColumnSpan(this.txtLog, 3);
            this.logsLayout.Controls.Add(this.btnClearLog, 0, 1);
            this.logsLayout.Controls.Add(this.btnSaveLog, 1, 1);
            this.logsLayout.Controls.Add(this.chkAutoScroll, 2, 1);
            
            this.tabLogs.Controls.Add(this.logsLayout);
            this.tabLogs.Location = new Point(4, 25);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.Padding = new Padding(3);
            this.tabLogs.Size = new Size(792, 421);
            this.tabLogs.TabIndex = 1;
            this.tabLogs.Text = "Logs";
            this.tabLogs.UseVisualStyleBackColor = true;
            
            // Settings Layout (vertical stack)
            this.settingsLayout.ColumnCount = 1;
            this.settingsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.settingsLayout.Dock = DockStyle.Fill;
            this.settingsLayout.Location = new Point(3, 3);
            this.settingsLayout.Name = "settingsLayout";
            this.settingsLayout.RowCount = 4;
            this.settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.settingsLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.settingsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.settingsLayout.Size = new Size(786, 415);
            this.settingsLayout.TabIndex = 0;
            
            this.settingsLayout.Controls.Add(this.grpPaths, 0, 0);
            this.settingsLayout.Controls.Add(this.grpBehavior, 0, 1);
            this.settingsLayout.Controls.Add(this.grpActions, 0, 2);
            this.settingsLayout.Controls.Add(this.grpAbout, 0, 3);
            
            this.tabSettings.Controls.Add(this.settingsLayout);
            this.tabSettings.Location = new Point(4, 25);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Size = new Size(792, 421);
            this.tabSettings.TabIndex = 2;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new ToolStripItem[] {
            this.tsStatusLabel,
            this.tsUptimeLabel});
            this.statusStrip.Location = new Point(0, 428);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(800, 22);
            this.statusStrip.TabIndex = 1;
            // 
            // tsStatusLabel
            // 
            this.tsStatusLabel.Name = "tsStatusLabel";
            this.tsStatusLabel.Size = new Size(39, 17);
            this.tsStatusLabel.Text = "Ready";
            // 
            // tsUptimeLabel
            // 
            this.tsUptimeLabel.Name = "tsUptimeLabel";
            this.tsUptimeLabel.Size = new Size(39, 17);
            this.tsUptimeLabel.Text = "";
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "Cloudflare Tunnel Monitor";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new MouseEventHandler(this.NotifyIcon_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[] {
            this.showToolStripMenuItem,
            this.startTunnelToolStripMenuItem,
            this.stopTunnelToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;

            // Context menu items
            this.showToolStripMenuItem.Text = "Show";
            this.showToolStripMenuItem.Click += new EventHandler(this.ShowToolStripMenuItem_Click);
            
            this.startTunnelToolStripMenuItem.Text = "Start Tunnel";
            this.startTunnelToolStripMenuItem.Click += new EventHandler(this.StartTunnelToolStripMenuItem_Click);
            
            this.stopTunnelToolStripMenuItem.Text = "Stop Tunnel";
            this.stopTunnelToolStripMenuItem.Click += new EventHandler(this.StopTunnelToolStripMenuItem_Click);
            
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new EventHandler(this.ExitToolStripMenuItem_Click);

            // 
            // grpTunnelControl
            // 
            this.grpTunnelControl.Controls.Add(this.lblStatusText);
            this.grpTunnelControl.Controls.Add(this.lblStatus);
            this.grpTunnelControl.Controls.Add(this.lblLocalUrl);
            this.grpTunnelControl.Controls.Add(this.txtLocalUrl);
            this.grpTunnelControl.Controls.Add(this.btnApplyLocalUrl);
            this.grpTunnelControl.Controls.Add(this.btnStart);
            this.grpTunnelControl.Controls.Add(this.btnStop);
            this.grpTunnelControl.Dock = DockStyle.Fill;
            this.grpTunnelControl.Location = new Point(3, 3);
            this.grpTunnelControl.Name = "grpTunnelControl";
            this.grpTunnelControl.Size = new Size(387, 194);
            this.grpTunnelControl.TabIndex = 0;
            this.grpTunnelControl.TabStop = false;
            this.grpTunnelControl.Text = "Tunnel Control";
            // 
            // lblLocalUrl
            // 
            this.lblLocalUrl.AutoSize = true;
            this.lblLocalUrl.Location = new Point(15, 30);
            this.lblLocalUrl.Name = "lblLocalUrl";
            this.lblLocalUrl.Size = new Size(115, 17);
            this.lblLocalUrl.Text = "Local URL to expose:";
            // 
            // txtLocalUrl
            // 
            this.txtLocalUrl.Location = new Point(15, 50);
            this.txtLocalUrl.Name = "txtLocalUrl";
            this.txtLocalUrl.Size = new Size(250, 23);
            this.txtLocalUrl.TabIndex = 1;
            this.txtLocalUrl.Text = "http://localhost:8080";
            // 
            // btnApplyLocalUrl
            // 
            this.btnApplyLocalUrl.Location = new Point(270, 48);
            this.btnApplyLocalUrl.Name = "btnApplyLocalUrl";
            this.btnApplyLocalUrl.Size = new Size(100, 27);
            this.btnApplyLocalUrl.TabIndex = 2;
            this.btnApplyLocalUrl.Text = "Apply & Restart";
            this.btnApplyLocalUrl.UseVisualStyleBackColor = true;
            this.btnApplyLocalUrl.Click += new EventHandler(this.BtnApplyLocalUrl_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new Point(15, 90);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new Size(100, 30);
            this.btnStart.TabIndex = 3;
            this.btnStart.Text = "Start Tunnel";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new EventHandler(this.BtnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new Point(130, 90);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new Size(100, 30);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop Tunnel";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new EventHandler(this.BtnStop_Click);
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = true;
            this.lblStatusText.Location = new Point(15, 135);
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Size = new Size(48, 17);
            this.lblStatusText.Text = "Status:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblStatus.Location = new Point(70, 135);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(54, 17);
            this.lblStatus.Text = "Stopped";
            // 
            // grpCurrentUrl
            // 
            this.grpCurrentUrl.Controls.Add(this.txtCurrentUrl);
            this.grpCurrentUrl.Controls.Add(this.btnCopyUrl);
            this.grpCurrentUrl.Dock = DockStyle.Fill;
            this.grpCurrentUrl.Location = new Point(396, 3);
            this.grpCurrentUrl.Name = "grpCurrentUrl";
            this.grpCurrentUrl.Size = new Size(387, 94);
            this.grpCurrentUrl.TabIndex = 1;
            this.grpCurrentUrl.TabStop = false;
            this.grpCurrentUrl.Text = "Current Tunnel URL";
            // 
            // txtCurrentUrl
            // 
            this.txtCurrentUrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.txtCurrentUrl.Location = new Point(15, 30);
            this.txtCurrentUrl.Name = "txtCurrentUrl";
            this.txtCurrentUrl.ReadOnly = true;
            this.txtCurrentUrl.Size = new Size(275, 23);
            this.txtCurrentUrl.TabIndex = 0;
            // 
            // btnCopyUrl
            // 
            this.btnCopyUrl.Anchor = AnchorStyles.Right;
            this.btnCopyUrl.Location = new Point(300, 28);
            this.btnCopyUrl.Name = "btnCopyUrl";
            this.btnCopyUrl.Size = new Size(70, 27);
            this.btnCopyUrl.TabIndex = 1;
            this.btnCopyUrl.Text = "Copy URL";
            this.btnCopyUrl.UseVisualStyleBackColor = true;
            this.btnCopyUrl.Click += new EventHandler(this.BtnCopyUrl_Click);
            // 
            // grpNetworkStatus
            // 
            this.grpNetworkStatus.Controls.Add(this.lblNetworkStatus);
            this.grpNetworkStatus.Dock = DockStyle.Fill;
            this.grpNetworkStatus.Location = new Point(396, 203);
            this.grpNetworkStatus.Name = "grpNetworkStatus";
            this.grpNetworkStatus.Size = new Size(387, 94);
            this.grpNetworkStatus.TabIndex = 2;
            this.grpNetworkStatus.TabStop = false;
            this.grpNetworkStatus.Text = "Network Status";
            // 
            // lblNetworkStatus
            // 
            this.lblNetworkStatus.AutoSize = true;
            this.lblNetworkStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblNetworkStatus.Location = new Point(15, 35);
            this.lblNetworkStatus.Name = "lblNetworkStatus";
            this.lblNetworkStatus.Size = new Size(43, 17);
            this.lblNetworkStatus.Text = "Online";
            // 
            // grpStatistics
            // 
            this.grpStatistics.Controls.Add(this.lblUptime);
            this.grpStatistics.Controls.Add(this.lblLastUrlGenerated);
            this.grpStatistics.Dock = DockStyle.Fill;
            this.grpStatistics.Location = new Point(3, 303);
            this.grpStatistics.Name = "grpStatistics";
            this.grpStatistics.Size = new Size(780, 109);
            this.grpStatistics.TabIndex = 3;
            this.grpStatistics.TabStop = false;
            this.grpStatistics.Text = "Statistics";
            // 
            // lblUptime
            // 
            this.lblUptime.AutoSize = true;
            this.lblUptime.Location = new Point(15, 35);
            this.lblUptime.Name = "lblUptime";
            this.lblUptime.Size = new Size(102, 17);
            this.lblUptime.Text = "Uptime: --:--:--";
            // 
            // lblLastUrlGenerated
            // 
            this.lblLastUrlGenerated.AutoSize = true;
            this.lblLastUrlGenerated.Location = new Point(15, 60);
            this.lblLastUrlGenerated.Name = "lblLastUrlGenerated";
            this.lblLastUrlGenerated.Size = new Size(180, 17);
            this.lblLastUrlGenerated.Text = "Last URL generated: --:--:--";
            // 
            // tabLogs
            // 
            // 
            // txtLog
            // 
            this.txtLog.Dock = DockStyle.Fill;
            this.txtLog.Font = new Font("Consolas", 9F);
            this.txtLog.Location = new Point(3, 3);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.Size = new Size(780, 353);
            this.txtLog.TabIndex = 0;
            this.txtLog.WordWrap = false;
            // 
            // btnClearLog
            // 
            this.btnClearLog.Location = new Point(3, 362);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new Size(100, 30);
            this.btnClearLog.TabIndex = 1;
            this.btnClearLog.Text = "Clear Log";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new EventHandler(this.BtnClearLog_Click);
            // 
            // btnSaveLog
            // 
            this.btnSaveLog.Location = new Point(109, 362);
            this.btnSaveLog.Name = "btnSaveLog";
            this.btnSaveLog.Size = new Size(100, 30);
            this.btnSaveLog.TabIndex = 2;
            this.btnSaveLog.Text = "Save Log";
            this.btnSaveLog.UseVisualStyleBackColor = true;
            this.btnSaveLog.Click += new EventHandler(this.BtnSaveLog_Click);
            // 
            // chkAutoScroll
            // 
            this.chkAutoScroll.AutoSize = true;
            this.chkAutoScroll.Anchor = AnchorStyles.Right;
            this.chkAutoScroll.Checked = true;
            this.chkAutoScroll.CheckState = CheckState.Checked;
            this.chkAutoScroll.Location = new Point(670, 362);
            this.chkAutoScroll.Name = "chkAutoScroll";
            this.chkAutoScroll.Size = new Size(110, 20);
            this.chkAutoScroll.TabIndex = 3;
            this.chkAutoScroll.Text = "Auto-scroll";
            this.chkAutoScroll.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            // 
            // grpPaths
            // 
            this.grpPaths.Controls.Add(this.lblCloudflaredPath);
            this.grpPaths.Controls.Add(this.txtCloudflaredPath);
            this.grpPaths.Controls.Add(this.btnBrowseCloudflared);
            this.grpPaths.Controls.Add(this.lblSaveDirectory);
            this.grpPaths.Controls.Add(this.txtSaveDirectory);
            this.grpPaths.Controls.Add(this.btnBrowseSaveDir);
            this.grpPaths.Dock = DockStyle.Fill;
            this.grpPaths.Location = new Point(3, 3);
            this.grpPaths.Name = "grpPaths";
            this.grpPaths.Size = new Size(780, 100);
            this.grpPaths.TabIndex = 0;
            this.grpPaths.TabStop = false;
            this.grpPaths.Text = "Paths";
            // 
            // lblCloudflaredPath
            // 
            this.lblCloudflaredPath.AutoSize = true;
            this.lblCloudflaredPath.Location = new Point(15, 30);
            this.lblCloudflaredPath.Name = "lblCloudflaredPath";
            this.lblCloudflaredPath.Size = new Size(105, 17);
            this.lblCloudflaredPath.Text = "cloudflared.exe:";
            // 
            // txtCloudflaredPath
            // 
            this.txtCloudflaredPath.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.txtCloudflaredPath.Location = new Point(130, 27);
            this.txtCloudflaredPath.Name = "txtCloudflaredPath";
            this.txtCloudflaredPath.Size = new Size(510, 23);
            this.txtCloudflaredPath.TabIndex = 1;
            // 
            // btnBrowseCloudflared
            // 
            this.btnBrowseCloudflared.Anchor = AnchorStyles.Right;
            this.btnBrowseCloudflared.Location = new Point(650, 25);
            this.btnBrowseCloudflared.Name = "btnBrowseCloudflared";
            this.btnBrowseCloudflared.Size = new Size(100, 27);
            this.btnBrowseCloudflared.TabIndex = 2;
            this.btnBrowseCloudflared.Text = "Browse...";
            this.btnBrowseCloudflared.UseVisualStyleBackColor = true;
            this.btnBrowseCloudflared.Click += new EventHandler(this.BtnBrowseCloudflared_Click);
            // 
            // lblSaveDirectory
            // 
            this.lblSaveDirectory.AutoSize = true;
            this.lblSaveDirectory.Location = new Point(15, 65);
            this.lblSaveDirectory.Name = "lblSaveDirectory";
            this.lblSaveDirectory.Size = new Size(100, 17);
            this.lblSaveDirectory.Text = "Save Directory:";
            // 
            // txtSaveDirectory
            // 
            this.txtSaveDirectory.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.txtSaveDirectory.Location = new Point(130, 62);
            this.txtSaveDirectory.Name = "txtSaveDirectory";
            this.txtSaveDirectory.Size = new Size(510, 23);
            this.txtSaveDirectory.TabIndex = 3;
            // 
            // btnBrowseSaveDir
            // 
            this.btnBrowseSaveDir.Anchor = AnchorStyles.Right;
            this.btnBrowseSaveDir.Location = new Point(650, 60);
            this.btnBrowseSaveDir.Name = "btnBrowseSaveDir";
            this.btnBrowseSaveDir.Size = new Size(100, 27);
            this.btnBrowseSaveDir.TabIndex = 4;
            this.btnBrowseSaveDir.Text = "Browse...";
            this.btnBrowseSaveDir.UseVisualStyleBackColor = true;
            this.btnBrowseSaveDir.Click += new EventHandler(this.BtnBrowseSaveDir_Click);
            // 
            // grpBehavior
            // 
            this.grpBehavior.Controls.Add(this.chkAutoStart);
            this.grpBehavior.Controls.Add(this.chkAutoReconnect);
            this.grpBehavior.Controls.Add(this.chkMinimizeToTray);
            this.grpBehavior.Controls.Add(this.chkAutoCopyUrl);
            this.grpBehavior.Controls.Add(this.lblCheckInterval);
            this.grpBehavior.Controls.Add(this.numCheckInterval);
            this.grpBehavior.Controls.Add(this.lblPingTestUrl);
            this.grpBehavior.Controls.Add(this.txtPingTestUrl);
            this.grpBehavior.Dock = DockStyle.Fill;
            this.grpBehavior.Location = new Point(3, 109);
            this.grpBehavior.Name = "grpBehavior";
            this.grpBehavior.Size = new Size(780, 120);
            this.grpBehavior.TabIndex = 1;
            this.grpBehavior.TabStop = false;
            this.grpBehavior.Text = "Behavior";
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Location = new Point(15, 30);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new Size(155, 20);
            this.chkAutoStart.TabIndex = 0;
            this.chkAutoStart.Text = "Auto-start with Windows";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            this.chkAutoStart.CheckedChanged += new EventHandler(this.ChkAutoStart_CheckedChanged);
            // 
            // chkAutoReconnect
            // 
            this.chkAutoReconnect.AutoSize = true;
            this.chkAutoReconnect.Checked = true;
            this.chkAutoReconnect.CheckState = CheckState.Checked;
            this.chkAutoReconnect.Location = new Point(15, 55);
            this.chkAutoReconnect.Name = "chkAutoReconnect";
            this.chkAutoReconnect.Size = new Size(135, 20);
            this.chkAutoReconnect.TabIndex = 1;
            this.chkAutoReconnect.Text = "Auto-reconnect on network";
            this.chkAutoReconnect.UseVisualStyleBackColor = true;
            this.chkAutoReconnect.CheckedChanged += new EventHandler(this.Settings_Changed);
            // 
            // chkMinimizeToTray
            // 
            this.chkMinimizeToTray.AutoSize = true;
            this.chkMinimizeToTray.Checked = true;
            this.chkMinimizeToTray.CheckState = CheckState.Checked;
            this.chkMinimizeToTray.Location = new Point(15, 80);
            this.chkMinimizeToTray.Name = "chkMinimizeToTray";
            this.chkMinimizeToTray.Size = new Size(140, 20);
            this.chkMinimizeToTray.TabIndex = 2;
            this.chkMinimizeToTray.Text = "Minimize to system tray";
            this.chkMinimizeToTray.UseVisualStyleBackColor = true;
            this.chkMinimizeToTray.CheckedChanged += new EventHandler(this.Settings_Changed);
            // 
            // chkAutoCopyUrl
            // 
            this.chkAutoCopyUrl.AutoSize = true;
            this.chkAutoCopyUrl.Location = new Point(250, 30);
            this.chkAutoCopyUrl.Name = "chkAutoCopyUrl";
            this.chkAutoCopyUrl.Size = new Size(145, 20);
            this.chkAutoCopyUrl.TabIndex = 3;
            this.chkAutoCopyUrl.Text = "Auto-copy URL when generated";
            this.chkAutoCopyUrl.UseVisualStyleBackColor = true;
            this.chkAutoCopyUrl.CheckedChanged += new EventHandler(this.Settings_Changed);
            // 
            // lblCheckInterval
            // 
            this.lblCheckInterval.AutoSize = true;
            this.lblCheckInterval.Location = new Point(250, 55);
            this.lblCheckInterval.Name = "lblCheckInterval";
            this.lblCheckInterval.Size = new Size(120, 17);
            this.lblCheckInterval.Text = "Check Interval (sec):";
            // 
            // numCheckInterval
            // 
            this.numCheckInterval.Location = new Point(380, 53);
            this.numCheckInterval.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.numCheckInterval.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            this.numCheckInterval.Name = "numCheckInterval";
            this.numCheckInterval.Size = new Size(60, 23);
            this.numCheckInterval.TabIndex = 4;
            this.numCheckInterval.Value = new decimal(new int[] { 60, 0, 0, 0 });
            this.numCheckInterval.ValueChanged += new EventHandler(this.Settings_Changed);
            // 
            // lblPingTestUrl
            // 
            this.lblPingTestUrl.AutoSize = true;
            this.lblPingTestUrl.Location = new Point(250, 80);
            this.lblPingTestUrl.Name = "lblPingTestUrl";
            this.lblPingTestUrl.Size = new Size(100, 17);
            this.lblPingTestUrl.Text = "Ping Test URL/IP:";
            // 
            // txtPingTestUrl
            // 
            this.txtPingTestUrl.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            this.txtPingTestUrl.Location = new Point(380, 77);
            this.txtPingTestUrl.Name = "txtPingTestUrl";
            this.txtPingTestUrl.Size = new Size(120, 23);
            this.txtPingTestUrl.TabIndex = 5;
            // 
            // grpActions
            // 
            this.grpActions.Controls.Add(this.btnResetSettings);
            this.grpActions.Controls.Add(this.btnOpenSaveDirectory);
            this.grpActions.Controls.Add(this.btnViewUrlLog);
            this.grpActions.Dock = DockStyle.Fill;
            this.grpActions.Location = new Point(3, 235);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new Size(780, 70);
            this.grpActions.TabIndex = 2;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "Actions";
            // 
            // btnResetSettings
            // 
            this.btnResetSettings.Location = new Point(15, 25);
            this.btnResetSettings.Name = "btnResetSettings";
            this.btnResetSettings.Size = new Size(140, 30);
            this.btnResetSettings.TabIndex = 0;
            this.btnResetSettings.Text = "Reset to Defaults";
            this.btnResetSettings.UseVisualStyleBackColor = true;
            this.btnResetSettings.Click += new EventHandler(this.BtnResetSettings_Click);
            // 
            // btnOpenSaveDirectory
            // 
            this.btnOpenSaveDirectory.Location = new Point(170, 25);
            this.btnOpenSaveDirectory.Name = "btnOpenSaveDirectory";
            this.btnOpenSaveDirectory.Size = new Size(140, 30);
            this.btnOpenSaveDirectory.TabIndex = 1;
            this.btnOpenSaveDirectory.Text = "Open Save Directory";
            this.btnOpenSaveDirectory.UseVisualStyleBackColor = true;
            this.btnOpenSaveDirectory.Click += new EventHandler(this.BtnOpenSaveDirectory_Click);
            // 
            // btnViewUrlLog
            // 
            this.btnViewUrlLog.Location = new Point(325, 25);
            this.btnViewUrlLog.Name = "btnViewUrlLog";
            this.btnViewUrlLog.Size = new Size(140, 30);
            this.btnViewUrlLog.TabIndex = 2;
            this.btnViewUrlLog.Text = "View URL Log";
            this.btnViewUrlLog.UseVisualStyleBackColor = true;
            this.btnViewUrlLog.Click += new EventHandler(this.BtnViewUrlLog_Click);
            // 
            // grpAbout
            // 
            this.grpAbout.Controls.Add(this.lblVersion);
            this.grpAbout.Controls.Add(this.lnkGitHub);
            this.grpAbout.Dock = DockStyle.Fill;
            this.grpAbout.Location = new Point(3, 311);
            this.grpAbout.Name = "grpAbout";
            this.grpAbout.Size = new Size(780, 101);
            this.grpAbout.TabIndex = 3;
            this.grpAbout.TabStop = false;
            this.grpAbout.Text = "About";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new Point(15, 30);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new Size(250, 17);
            this.lblVersion.Text = "Cloudflare Tunnel Monitor v1.0.0";
            // 
            // lnkGitHub
            // 
            this.lnkGitHub.AutoSize = true;
            this.lnkGitHub.Location = new Point(15, 60);
            this.lnkGitHub.Name = "lnkGitHub";
            this.lnkGitHub.Size = new Size(200, 17);
            this.lnkGitHub.TabIndex = 1;
            this.lnkGitHub.TabStop = true;
            this.lnkGitHub.Text = "Developed by MeTariqul";
            this.lnkGitHub.LinkClicked += new LinkLabelLinkClickedEventHandler(this.LnkGitHub_LinkClicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(96F, 96F);
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.ClientSize = new Size(800, 450);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(700, 400);
            this.MaximizeBox = true;
            this.Name = "MainForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Cloudflare Tunnel Monitor";
            this.dashboardLayout.ResumeLayout(false);
            this.logsLayout.ResumeLayout(false);
            this.logsLayout.PerformLayout();
            this.settingsLayout.ResumeLayout(false);
            this.tabDashboard.ResumeLayout(false);
            this.tabLogs.ResumeLayout(false);
            this.tabLogs.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numCheckInterval)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
