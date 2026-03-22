using CloudflareTunnelMonitor.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace CloudflareTunnelMonitor;

/// <summary>
/// Main form for the Cloudflare Tunnel Monitor application.
/// </summary>
public partial class MainForm : Form
{
    #region Private Fields

    private readonly SettingsManager _settingsManager;
    private readonly TunnelManager _tunnelManager;
    private readonly NetworkMonitor _networkMonitor;
    private UrlLogger _urlLogger;
    private Timer? _healthCheckTimer;
    private Timer? _uptimeTimer;
    private DateTime _lastUrlGeneratedTime;
    private bool _isClosing;
    private bool _minimizeToTray;

    #endregion

    #region Constructor

    public MainForm()
    {
        InitializeComponent();

        // Initialize managers
        _settingsManager = new SettingsManager();
        _tunnelManager = new TunnelManager();
        _networkMonitor = new NetworkMonitor();
        
        // Initialize URL logger
        _urlLogger = new UrlLogger(_settingsManager.GetUrlLogPath());

        // Set up event handlers
        SetupEventHandlers();

        // Load settings
        LoadSettings();

        // Start network monitoring
        _networkMonitor.StartMonitoring();

        // Initialize UI state
        UpdateStatus("Ready");
        UpdateNetworkStatus();
    }

    #endregion

    #region Event Handlers Setup

    private void SetupEventHandlers()
    {
        // Tunnel manager events
        _tunnelManager.OutputReceived += OnTunnelOutputReceived;
        _tunnelManager.TunnelUrlReceived += OnTunnelUrlReceived;
        _tunnelManager.StateChanged += OnTunnelStateChanged;

        // Network monitor events
        _networkMonitor.NetworkStatusChanged += OnNetworkStatusChanged;

        // Settings changed event
        _settingsManager.SettingsChanged += OnSettingsChanged;
    }

    #endregion

    #region Form Events

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        
        // Signal that form is loaded for thread-safe UI updates
        _networkMonitor.SetFormLoaded(true);

        // Initialize health check timer
        _healthCheckTimer = new Timer();
        _healthCheckTimer.Interval = _settingsManager.CurrentSettings.CheckIntervalSeconds * 1000;
        _healthCheckTimer.Tick += OnHealthCheckTimerTick;
        _healthCheckTimer.Start();

        // Initialize uptime timer (updates every second)
        _uptimeTimer = new Timer();
        _uptimeTimer.Interval = 1000;
        _uptimeTimer.Tick += OnUptimeTimerTick;
        _uptimeTimer.Start();

        UpdateStatus("Ready - Click Start to begin");
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_minimizeToTray && !_isClosing && e.CloseReason == CloseReason.UserClosing)
        {
            // Minimize to tray instead of closing
            e.Cancel = true;
            Hide();
            notifyIcon.ShowBalloonTip(3000, "Cloudflare Tunnel Monitor", 
                "Application is still running in the system tray.", ToolTipIcon.Info);
            return;
        }

        // Actually closing - cleanup
        _isClosing = true;
        
        // Stop timers
        _healthCheckTimer?.Stop();
        _healthCheckTimer?.Dispose();
        _uptimeTimer?.Stop();
        _uptimeTimer?.Dispose();

        // Stop tunnel if running
        if (_tunnelManager.IsRunning)
        {
            _tunnelManager.StopAsync().Wait(TimeSpan.FromSeconds(5));
        }

        // Cleanup resources
        _tunnelManager.Dispose();
        _networkMonitor.Dispose();
        _urlLogger.Dispose();

        base.OnFormClosing(e);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        // Handle minimize to tray
        if (WindowState == FormWindowState.Minimized && _minimizeToTray)
        {
            Hide();
            notifyIcon.ShowBalloonTip(2000, "Cloudflare Tunnel Monitor",
                "Application minimized to tray.", ToolTipIcon.Info);
        }
    }

    #endregion

    #region Settings

    private void LoadSettings()
    {
        var settings = _settingsManager.Load();

        // Update UI with settings
        txtLocalUrl.Text = settings.LocalUrl;
        txtCloudflaredPath.Text = settings.CloudflaredPath;
        txtSaveDirectory.Text = settings.SaveDirectory;
        txtPingTestUrl.Text = settings.PingTestUrl;
        
        chkAutoStart.Checked = _settingsManager.IsAutoStartEnabled();
        chkAutoReconnect.Checked = settings.AutoReconnect;
        chkMinimizeToTray.Checked = settings.MinimizeToTray;
        chkAutoCopyUrl.Checked = settings.AutoCopyUrl;
        
        numCheckInterval.Value = settings.CheckIntervalSeconds;

        // Configure components
        _tunnelManager.Configure(settings.CloudflaredPath, settings.LocalUrl);
        _networkMonitor.Configure(settings.PingTestUrl);
        
        _minimizeToTray = settings.MinimizeToTray;
    }

    private void SaveSettings()
    {
        var settings = _settingsManager.CurrentSettings;
        
        settings.LocalUrl = txtLocalUrl.Text;
        settings.CloudflaredPath = txtCloudflaredPath.Text;
        settings.SaveDirectory = txtSaveDirectory.Text;
        settings.PingTestUrl = txtPingTestUrl.Text;
        settings.AutoStartWithWindows = chkAutoStart.Checked;
        settings.AutoReconnect = chkAutoReconnect.Checked;
        settings.MinimizeToTray = chkMinimizeToTray.Checked;
        settings.AutoCopyUrl = chkAutoCopyUrl.Checked;
        settings.CheckIntervalSeconds = (int)numCheckInterval.Value;
        
        _settingsManager.Save(settings);
    }

    private void OnSettingsChanged(object? sender, Settings settings)
    {
        // Update health check interval
        if (_healthCheckTimer != null)
        {
            _healthCheckTimer.Interval = settings.CheckIntervalSeconds * 1000;
        }

        // Update minimize to tray
        _minimizeToTray = settings.MinimizeToTray;
    }

    #endregion

    #region Tunnel Events

    private void OnTunnelOutputReceived(object? sender, string output)
    {
        // Use BeginInvoke to update UI from background thread
        BeginInvoke(new Action(() =>
        {
            AppendLog(output);
        }));
    }

    private void OnTunnelUrlReceived(object? sender, string url)
    {
        BeginInvoke(new Action(() =>
        {
            txtCurrentUrl.Text = url;
            _lastUrlGeneratedTime = DateTime.Now;
            lblLastUrlGenerated.Text = $"Last URL generated: {_lastUrlGeneratedTime:HH:mm:ss}";
            
            // Log URL
            _urlLogger.LogUrl(url);
            
            // Auto-copy if enabled
            if (_settingsManager.CurrentSettings.AutoCopyUrl)
            {
                try
                {
                    Clipboard.SetText(url);
                    UpdateStatus($"URL copied to clipboard: {url}");
                }
                catch { /* Ignore clipboard errors */ }
            }

            // Show notification
            notifyIcon.ShowBalloonTip(3000, "New Tunnel URL", url, ToolTipIcon.Info);
        }));
    }

    private void OnTunnelStateChanged(object? sender, TunnelState state)
    {
        BeginInvoke(new Action(() =>
        {
            UpdateTunnelStatus(state);
        }));
    }

    #endregion

    #region Network Events

    private void OnNetworkStatusChanged(object? sender, bool isAvailable)
    {
        BeginInvoke(new Action(() =>
        {
            UpdateNetworkStatus();

            if (isAvailable)
            {
                // Network restored - regenerate tunnel URL
                if (_settingsManager.CurrentSettings.AutoReconnect && 
                    !_tunnelManager.IsRunning && !_tunnelManager.WasUserStopped)
                {
                    UpdateStatus("Network restored - restarting tunnel...");
                    _ = StartTunnelAsync();
                }
            }
            else
            {
                // Network lost - clear the URL since it's no longer valid
                txtCurrentUrl.Text = "";
                UpdateStatus("Network lost - tunnel URL cleared");
            }
        }));
    }

    #endregion

    #region Timer Events

    private void OnHealthCheckTimerTick(object? sender, EventArgs e)
    {
        // Check if tunnel should be running but isn't
        if (!_tunnelManager.IsRunning && !_tunnelManager.WasUserStopped)
        {
            // Try to restart
            UpdateStatus("Tunnel not running - attempting to restart...");
            _ = StartTunnelAsync();
        }

        // Check if tunnel is running but stalled (no output for 2 minutes)
        if (_tunnelManager.IsRunning)
        {
            var timeSinceLastOutput = DateTime.Now - _tunnelManager.LastOutputTime;
            if (timeSinceLastOutput.TotalMinutes > 2)
            {
                UpdateStatus("No output for 2 minutes - restarting tunnel...");
                _ = _tunnelManager.RestartAsync();
            }
        }

        // Update network status
        UpdateNetworkStatus();
    }

    private void OnUptimeTimerTick(object? sender, EventArgs e)
    {
        if (_tunnelManager.IsRunning)
        {
            lblUptime.Text = $"Uptime: {_tunnelManager.Uptime:hh\\:mm\\:ss}";
        }
    }

    #endregion

    #region UI Update Methods

    private void UpdateStatus(string message)
    {
        tsStatusLabel.Text = message;
    }

    private void UpdateNetworkStatus()
    {
        var isAvailable = _networkMonitor.IsInternetAvailable();
        
        lblNetworkStatus.Text = isAvailable ? "Online" : "Offline";
        lblNetworkStatus.ForeColor = isAvailable ? Color.Green : Color.Red;
    }

    private void UpdateTunnelStatus(TunnelState state)
    {
        switch (state)
        {
            case TunnelState.Stopped:
                lblStatus.Text = "Stopped";
                lblStatus.ForeColor = Color.Gray;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                btnApplyLocalUrl.Enabled = true;
                txtLocalUrl.Enabled = true;
                lblUptime.Text = "Uptime: --:--:--";
                break;

            case TunnelState.Starting:
                lblStatus.Text = "Starting...";
                lblStatus.ForeColor = Color.Orange;
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                btnApplyLocalUrl.Enabled = false;
                txtLocalUrl.Enabled = false;
                break;

            case TunnelState.Running:
                lblStatus.Text = "Running";
                lblStatus.ForeColor = Color.Green;
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnApplyLocalUrl.Enabled = false;
                txtLocalUrl.Enabled = false;
                break;

            case TunnelState.Stopping:
                lblStatus.Text = "Stopping...";
                lblStatus.ForeColor = Color.Orange;
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                break;

            case TunnelState.Error:
                lblStatus.Text = "Error";
                lblStatus.ForeColor = Color.Red;
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                btnApplyLocalUrl.Enabled = true;
                txtLocalUrl.Enabled = true;
                break;
        }
    }

    private void AppendLog(string line)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        txtLog.AppendText($"[{timestamp}] {line}{Environment.NewLine}");
        
        // Auto-scroll to bottom if enabled
        if (chkAutoScroll.Checked)
        {
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
    }

    #endregion

    #region Button Click Handlers - Dashboard

    private async void BtnStart_Click(object sender, EventArgs e)
    {
        await StartTunnelAsync();
    }

    private async void BtnStop_Click(object sender, EventArgs e)
    {
        await StopTunnelAsync();
    }

    private async void BtnApplyLocalUrl_Click(object sender, EventArgs e)
    {
        var newUrl = txtLocalUrl.Text.Trim();
        
        // Validate URL
        if (!IsValidUrl(newUrl))
        {
            MessageBox.Show("Please enter a valid HTTP/HTTPS URL with a port (e.g., http://localhost:8080)",
                "Invalid URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Save settings
        SaveSettings();
        
        // Configure tunnel manager
        _tunnelManager.Configure(_settingsManager.CurrentSettings.CloudflaredPath, newUrl);
        
        // If tunnel is running, restart with new URL
        if (_tunnelManager.IsRunning)
        {
            await _tunnelManager.RestartWithUrlAsync(newUrl);
            UpdateStatus($"Tunnel restarted with new URL: {newUrl}");
        }
        else
        {
            UpdateStatus($"URL updated to {newUrl} - Click Start to begin");
        }
    }

    private void BtnCopyUrl_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtCurrentUrl.Text))
        {
            MessageBox.Show("No tunnel URL available yet. Start a tunnel and wait for URL to be generated.", 
                "No URL", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        
        try
        {
            Clipboard.SetText(txtCurrentUrl.Text);
            UpdateStatus("URL copied to clipboard");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to copy: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task StartTunnelAsync()
    {
        try
        {
            btnStart.Enabled = false;
            
            // Configure with current settings
            _tunnelManager.Configure(
                _settingsManager.CurrentSettings.CloudflaredPath,
                txtLocalUrl.Text.Trim());
            
            // Clear user stopped flag
            _tunnelManager.ClearUserStopped();
            
            var success = await _tunnelManager.StartAsync();
            
            if (success)
            {
                UpdateStatus("Tunnel started");
            }
            else
            {
                UpdateStatus("Failed to start tunnel");
                btnStart.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error: {ex.Message}");
            btnStart.Enabled = true;
            _settingsManager.LogError("Failed to start tunnel", ex);
        }
    }

    private async Task StopTunnelAsync()
    {
        try
        {
            btnStop.Enabled = false;
            _tunnelManager.MarkUserStopped();
            await _tunnelManager.StopAsync();
            UpdateStatus("Tunnel stopped");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error: {ex.Message}");
            _settingsManager.LogError("Failed to stop tunnel", ex);
        }
    }

    #endregion

    #region Button Click Handlers - Logs

    private void BtnClearLog_Click(object sender, EventArgs e)
    {
        txtLog.Clear();
        UpdateStatus("Log cleared");
    }

    private void BtnSaveLog_Click(object sender, EventArgs e)
    {
        using var saveDialog = new SaveFileDialog
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = ".txt",
            FileName = $"cloudflared_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
        };

        if (saveDialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                File.WriteAllText(saveDialog.FileName, txtLog.Text);
                UpdateStatus($"Log saved to {saveDialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    #endregion

    #region Button Click Handlers - Settings

    private void BtnBrowseCloudflared_Click(object sender, EventArgs e)
    {
        using var openDialog = new OpenFileDialog
        {
            Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*",
            Title = "Select cloudflared.exe"
        };

        if (openDialog.ShowDialog() == DialogResult.OK)
        {
            txtCloudflaredPath.Text = openDialog.FileName;
            SaveSettings();
            UpdateStatus($"Cloudflared path set to: {openDialog.FileName}");
        }
    }

    private void BtnBrowseSaveDir_Click(object sender, EventArgs e)
    {
        using var folderDialog = new FolderBrowserDialog
        {
            Description = "Select directory for URL logs",
            UseDescriptionForTitle = true
        };

        if (folderDialog.ShowDialog() == DialogResult.OK)
        {
            txtSaveDirectory.Text = folderDialog.SelectedPath;
            SaveSettings();
            
            // Reinitialize URL logger with new path
            _urlLogger.Dispose();
            _urlLogger = new UrlLogger(_settingsManager.GetUrlLogPath());
            
            UpdateStatus($"Save directory set to: {folderDialog.SelectedPath}");
        }
    }

    private void BtnResetSettings_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to reset all settings to defaults?",
            "Reset Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            _settingsManager.ResetToDefaults();
            LoadSettings();
            UpdateStatus("Settings reset to defaults");
        }
    }

    private void BtnOpenSaveDirectory_Click(object sender, EventArgs e)
    {
        _urlLogger.OpenContainingFolder();
    }

    private void BtnViewUrlLog_Click(object sender, EventArgs e)
    {
        _urlLogger.OpenLogFile();
    }

    private void LnkGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/MeTariqul",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open link: {ex.Message}", "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ChkAutoStart_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            _settingsManager.SetAutoStart(chkAutoStart.Checked);
            UpdateStatus(chkAutoStart.Checked ? 
                "Auto-start enabled" : "Auto-start disabled");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update auto-start: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            chkAutoStart.Checked = !chkAutoStart.Checked;
        }
    }

    private void Settings_Changed(object sender, EventArgs e)
    {
        SaveSettings();
    }

    #endregion

    #region System Tray

    private void NotifyIcon_MouseDoubleClick(object? sender, MouseEventArgs e)
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
    }

    private void ShowToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        Show();
        WindowState = FormWindowState.Normal;
        Activate();
    }

    private async void StartTunnelToolStripMenuItem_Click(object sender, EventArgs e)
    {
        await StartTunnelAsync();
    }

    private async void StopTunnelToolStripMenuItem_Click(object sender, EventArgs e)
    {
        await StopTunnelAsync();
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _isClosing = true;
        Close();
    }

    #endregion

    #region Helper Methods

    private bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        // Check if it's a valid HTTP/HTTPS URL with port
        var pattern = @"^https?://[^:]+:\d+$";
        return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
    }

    #endregion
}
