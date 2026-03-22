using System.Text.Json;
using CloudflareTunnelMonitor.Models;
using Microsoft.Win32;

namespace CloudflareTunnelMonitor;

/// <summary>
/// Manages application settings using JSON file storage.
/// </summary>
public class SettingsManager
{
    private readonly string _settingsPath;
    private readonly string _errorLogPath;
    private Settings _settings;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Event raised when settings are changed.
    /// </summary>
    public event EventHandler<Settings>? SettingsChanged;

    public SettingsManager()
    {
        // Set up paths
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CloudflareTunnelMonitor");
        
        Directory.CreateDirectory(appDataPath);
        
        _settingsPath = Path.Combine(appDataPath, "settings.json");
        _errorLogPath = Path.Combine(appDataPath, "error.log");
        
        // Load settings
        _settings = Load();
    }

    /// <summary>
    /// Gets the current settings.
    /// </summary>
    public Settings CurrentSettings => _settings;

    /// <summary>
    /// Loads settings from the JSON file, or creates defaults if not found.
    /// </summary>
    public Settings Load()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var loaded = JsonSerializer.Deserialize<Settings>(json, JsonOptions);
                if (loaded != null)
                {
                    _settings = loaded;
                    
                    // Ensure save directory exists
                    if (!string.IsNullOrEmpty(_settings.SaveDirectory) && 
                        !Directory.Exists(_settings.SaveDirectory))
                    {
                        Directory.CreateDirectory(_settings.SaveDirectory);
                    }
                    
                    return _settings;
                }
            }
        }
        catch (Exception ex)
        {
            LogError("Failed to load settings", ex);
        }

        // Return defaults if loading fails
        _settings = Settings.GetDefaults();
        
        // Ensure save directory exists
        if (!Directory.Exists(_settings.SaveDirectory))
        {
            Directory.CreateDirectory(_settings.SaveDirectory);
        }
        
        // Save defaults
        Save(_settings);
        
        return _settings;
    }

    /// <summary>
    /// Saves the settings to the JSON file.
    /// </summary>
    public void Save(Settings? settings = null)
    {
        if (settings != null)
        {
            _settings = settings;
        }

        try
        {
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Ensure save directory exists
            if (!string.IsNullOrEmpty(_settings.SaveDirectory) && 
                !Directory.Exists(_settings.SaveDirectory))
            {
                Directory.CreateDirectory(_settings.SaveDirectory);
            }

            var json = JsonSerializer.Serialize(_settings, JsonOptions);
            File.WriteAllText(_settingsPath, json);
            
            // Notify listeners
            SettingsChanged?.Invoke(this, _settings);
        }
        catch (Exception ex)
        {
            LogError("Failed to save settings", ex);
            throw;
        }
    }

    /// <summary>
    /// Resets settings to defaults.
    /// </summary>
    public void ResetToDefaults()
    {
        var defaults = Settings.GetDefaults();
        
        // Preserve the auto-start setting from current settings
        // (we don't want to change that on reset)
        defaults.AutoStartWithWindows = _settings.AutoStartWithWindows;
        
        Save(defaults);
    }

    /// <summary>
    /// Updates the auto-start with Windows setting and registry.
    /// </summary>
    public void SetAutoStart(bool enable)
    {
        try
        {
            const string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            const string valueName = "CloudflareTunnelMonitor";
            
            using var key = Registry.CurrentUser.OpenSubKey(keyPath, true);
            
            if (key != null)
            {
                if (enable)
                {
                    // Add the registry value
                    var exePath = Environment.ProcessPath ?? 
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CloudflareTunnelMonitor.exe");
                    key.SetValue(valueName, $"\"{exePath}\"");
                }
                else
                {
                    // Remove the registry value
                    key.DeleteValue(valueName, false);
                }
            }

            _settings.AutoStartWithWindows = enable;
            Save(_settings);
        }
        catch (Exception ex)
        {
            LogError("Failed to set auto-start", ex);
            throw;
        }
    }

    /// <summary>
    /// Checks if auto-start is enabled in the registry.
    /// </summary>
    public bool IsAutoStartEnabled()
    {
        try
        {
            const string keyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            const string valueName = "CloudflareTunnelMonitor";
            
            using var key = Registry.CurrentUser.OpenSubKey(keyPath, false);
            return key?.GetValue(valueName) != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Logs an error message to the error log file.
    /// </summary>
    public void LogError(string message, Exception? ex = null)
    {
        try
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            if (ex != null)
            {
                logMessage += $"\n  Exception: {ex.GetType().Name}: {ex.Message}";
                logMessage += $"\n  Stack Trace: {ex.StackTrace}";
            }
            logMessage += Environment.NewLine;
            
            File.AppendAllText(_errorLogPath, logMessage);
        }
        catch
        {
            // Ignore logging errors
        }
    }

    /// <summary>
    /// Gets the full path to the URL log file.
    /// </summary>
    public string GetUrlLogPath()
    {
        var directory = _settings.SaveDirectory;
        if (string.IsNullOrEmpty(directory))
        {
            directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "TunnelLogs");
        }
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        return Path.Combine(directory, _settings.UrlLogFileName);
    }
}
