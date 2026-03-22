using System.Text.Json.Serialization;

namespace CloudflareTunnelMonitor.Models;

/// <summary>
/// Settings model class for storing application configuration.
/// </summary>
public class Settings
{
    /// <summary>
    /// Path to cloudflared.exe. Default is cloudflared.exe in application folder.
    /// </summary>
    [JsonPropertyName("cloudflaredPath")]
    public string CloudflaredPath { get; set; } = "cloudflared.exe";

    /// <summary>
    /// Local URL to expose via the tunnel. Default is http://localhost:8080.
    /// </summary>
    [JsonPropertyName("localUrl")]
    public string LocalUrl { get; set; } = "http://localhost:8080";

    /// <summary>
    /// Directory to save tunnel URL logs. Default is Documents\TunnelLogs.
    /// </summary>
    [JsonPropertyName("saveDirectory")]
    public string SaveDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Whether to auto-start the application with Windows.
    /// </summary>
    [JsonPropertyName("autoStartWithWindows")]
    public bool AutoStartWithWindows { get; set; } = false;

    /// <summary>
    /// Whether to auto-reconnect when network becomes available.
    /// </summary>
    [JsonPropertyName("autoReconnect")]
    public bool AutoReconnect { get; set; } = true;

    /// <summary>
    /// Whether to minimize to system tray instead of closing.
    /// </summary>
    [JsonPropertyName("minimizeToTray")]
    public bool MinimizeToTray { get; set; } = true;

    /// <summary>
    /// Health check interval in seconds.
    /// </summary>
    [JsonPropertyName("checkIntervalSeconds")]
    public int CheckIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// URL or IP address to ping for internet connectivity check.
    /// </summary>
    [JsonPropertyName("pingTestUrl")]
    public string PingTestUrl { get; set; } = "1.1.1.1";

    /// <summary>
    /// Name of the URL log file.
    /// </summary>
    [JsonPropertyName("urlLogFileName")]
    public string UrlLogFileName { get; set; } = "tunnel_urls.txt";

    /// <summary>
    /// Whether to copy URL automatically when generated.
    /// </summary>
    [JsonPropertyName("autoCopyUrl")]
    public bool AutoCopyUrl { get; set; } = false;

    /// <summary>
    /// Creates a copy of the default settings.
    /// </summary>
    public static Settings GetDefaults()
    {
        return new Settings
        {
            CloudflaredPath = "cloudflared.exe",
            LocalUrl = "http://localhost:8080",
            SaveDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "TunnelLogs"),
            AutoStartWithWindows = false,
            AutoReconnect = true,
            MinimizeToTray = true,
            CheckIntervalSeconds = 60,
            PingTestUrl = "1.1.1.1",
            UrlLogFileName = "tunnel_urls.txt",
            AutoCopyUrl = false
        };
    }
}

/// <summary>
/// Enum representing the tunnel status.
/// </summary>
public enum TunnelState
{
    Stopped,
    Starting,
    Running,
    Stopping,
    Error
}
