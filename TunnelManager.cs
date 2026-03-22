using System.Diagnostics;
using System.Text.RegularExpressions;
using CloudflareTunnelMonitor.Models;

namespace CloudflareTunnelMonitor;

/// <summary>
/// Manages the cloudflared process for creating Cloudflare tunnels.
/// </summary>
public class TunnelManager : IDisposable
{
    private Process? _process;
    private string _cloudflaredPath;
    private string _localUrl;
    private bool _isRunning;
    private bool _userStopped;
    private DateTime _startTime;
    private DateTime _lastOutputTime;
    private readonly object _lock = new();

    // Regex pattern to match Cloudflare tunnel URLs (more flexible)
    private static readonly Regex TunnelUrlPattern = new(
        @"https?://[a-zA-Z0-9\-.]+\.trycloudflare\.com[a-zA-Z0-9\-._?=/]*",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    // Fallback pattern to catch any URL in output
    private static readonly Regex AnyUrlPattern = new(
        @"(https?://[^\s]+)",
        RegexOptions.Compiled);

    #region Properties

    /// <summary>
    /// Gets whether the tunnel is currently running.
    /// </summary>
    public bool IsRunning
    {
        get
        {
            lock (_lock)
            {
                return _isRunning && _process != null && !_process.HasExited;
            }
        }
    }

    /// <summary>
    /// Gets the current local URL being exposed.
    /// </summary>
    public string CurrentLocalUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the current Cloudflare tunnel URL.
    /// </summary>
    public string CurrentTunnelUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the tunnel uptime, or TimeSpan.Zero if not running.
    /// </summary>
    public TimeSpan Uptime => IsRunning ? DateTime.Now - _startTime : TimeSpan.Zero;

    /// <summary>
    /// Gets when the last output was received from the tunnel process.
    /// </summary>
    public DateTime LastOutputTime => _lastOutputTime;

    #endregion

    #region Events

    /// <summary>
    /// Event raised when output is received from the tunnel process.
    /// </summary>
    public event EventHandler<string>? OutputReceived;

    /// <summary>
    /// Event raised when a new tunnel URL is generated.
    /// </summary>
    public event EventHandler<string>? TunnelUrlReceived;

    /// <summary>
    /// Event raised when the tunnel process exits.
    /// </summary>
    public event EventHandler? ProcessExited;

    /// <summary>
    /// Event raised when the tunnel state changes.
    /// </summary>
    public event EventHandler<TunnelState>? StateChanged;

    #endregion

    public TunnelManager()
    {
        _cloudflaredPath = "cloudflared.exe";
        _localUrl = "http://localhost:8080";
    }

    /// <summary>
    /// Configures the tunnel manager with the specified cloudflared path and local URL.
    /// </summary>
    public void Configure(string cloudflaredPath, string localUrl)
    {
        _cloudflaredPath = cloudflaredPath;
        _localUrl = localUrl;
    }

    /// <summary>
    /// Starts the Cloudflare tunnel.
    /// </summary>
    public async Task<bool> StartAsync()
    {
        // Don't start if already running
        if (IsRunning)
        {
            return true;
        }

        _userStopped = false;
        CurrentTunnelUrl = string.Empty;
        
        try
        {
            // Validate cloudflared path
            var fullPath = GetFullCloudflaredPath();
            if (!File.Exists(fullPath))
            {
                OutputReceived?.Invoke(this, $"Error: cloudflared.exe not found at: {fullPath}");
                return false;
            }

            // Kill any existing process first
            await StopAsync();

            OutputReceived?.Invoke(this, $"Starting tunnel to {_localUrl}...");
            RaiseStateChanged(TunnelState.Starting);

            // Set up process start info
            var startInfo = new ProcessStartInfo
            {
                FileName = fullPath,
                Arguments = $"tunnel --url {_localUrl}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(fullPath) ?? 
                    AppDomain.CurrentDomain.BaseDirectory
            };

            _process = new Process { StartInfo = startInfo };
            
            // Set up event handlers
            _process.OutputDataReceived += OnOutputDataReceived;
            _process.ErrorDataReceived += OnErrorDataReceived;
            _process.EnableRaisingEvents = true;
            _process.Exited += OnProcessExited;

            // Start the process
            _process.Start();
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            _isRunning = true;
            _startTime = DateTime.Now;
            _lastOutputTime = DateTime.Now;
            CurrentLocalUrl = _localUrl;

            OutputReceived?.Invoke(this, $"Tunnel process started (PID: {_process.Id})");
            RaiseStateChanged(TunnelState.Running);

            return true;
        }
        catch (Exception ex)
        {
            OutputReceived?.Invoke(this, $"Error starting tunnel: {ex.Message}");
            RaiseStateChanged(TunnelState.Error);
            return false;
        }
    }

    /// <summary>
    /// Stops the Cloudflare tunnel.
    /// </summary>
    public async Task StopAsync()
    {
        _userStopped = true;

        if (_process == null || _process.HasExited)
        {
            _isRunning = false;
            CurrentTunnelUrl = string.Empty;
            RaiseStateChanged(TunnelState.Stopped);
            return;
        }

        RaiseStateChanged(TunnelState.Stopping);

        try
        {
            // Try graceful shutdown first
            if (!_process.HasExited)
            {
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(5));
            }
        }
        catch
        {
            // Force kill if graceful doesn't work
            try
            {
                _process.Kill(entireProcessTree: true);
            }
            catch { /* Ignore */ }
        }
        finally
        {
            CleanupProcess();
        }
    }

    /// <summary>
    /// Restarts the tunnel with the current local URL.
    /// </summary>
    public async Task<bool> RestartAsync()
    {
        await StopAsync();
        
        // Wait a moment for the process to fully exit
        await Task.Delay(1000);
        
        return await StartAsync();
    }

    /// <summary>
    /// Restarts the tunnel with a new local URL.
    /// </summary>
    public async Task<bool> RestartWithUrlAsync(string newLocalUrl)
    {
        _localUrl = newLocalUrl;
        return await RestartAsync();
    }

    /// <summary>
    /// Marks the tunnel as intentionally stopped by the user.
    /// </summary>
    public void MarkUserStopped()
    {
        _userStopped = true;
    }

    /// <summary>
    /// Clears the user stopped flag (for auto-restart scenarios).
    /// </summary>
    public void ClearUserStopped()
    {
        _userStopped = false;
    }

    /// <summary>
    /// Gets whether the user intentionally stopped the tunnel.
    /// </summary>
    public bool WasUserStopped => _userStopped;

    #region Private Methods

    private string GetFullCloudflaredPath()
    {
        // If path is just a filename, check in multiple locations
        if (!Path.IsPathRooted(_cloudflaredPath))
        {
            // Check application directory (where exe is running from)
            var appDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _cloudflaredPath);
            if (File.Exists(appDirPath))
            {
                return appDirPath;
            }
            
            // Check current working directory
            var cwdPath = Path.Combine(Directory.GetCurrentDirectory(), _cloudflaredPath);
            if (File.Exists(cwdPath))
            {
                return cwdPath;
            }
            
            // Check project folder
            var projectDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", _cloudflaredPath);
            if (File.Exists(projectDirPath))
            {
                return Path.GetFullPath(projectDirPath);
            }
        }
        
        return _cloudflaredPath;
    }

    private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data)) return;

        _lastOutputTime = DateTime.Now;
        ProcessOutputLine(e.Data);
        OutputReceived?.Invoke(this, e.Data);
    }

    private void ProcessOutputLine(string line)
    {
        // Check for tunnel URL - try specific pattern first
        var urlMatch = TunnelUrlPattern.Match(line);
        string? foundUrl = null;
        
        if (urlMatch.Success)
        {
            foundUrl = urlMatch.Value;
        }
        else
        {
            // Try fallback pattern for any HTTP URL
            var fallbackMatch = AnyUrlPattern.Match(line);
            if (fallbackMatch.Success)
            {
                var potentialUrl = fallbackMatch.Value;
                // Only accept if it looks like a tunnel URL
                if (potentialUrl.Contains("trycloudflare") || 
                    potentialUrl.Contains("cloudflared"))
                {
                    foundUrl = potentialUrl;
                }
            }
        }
        
        // If we found a URL and it's different from current
        if (!string.IsNullOrEmpty(foundUrl) && foundUrl != CurrentTunnelUrl)
        {
            CurrentTunnelUrl = foundUrl;
            TunnelUrlReceived?.Invoke(this, foundUrl);
        }
    }

    private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data)) return;

        _lastOutputTime = DateTime.Now;
        
        // Also check for URLs in stderr
        ProcessOutputLine(e.Data);
        
        // Check if this is actually an error or just INFO from cloudflared
        // cloudflared outputs INFO to stderr, only show ERROR for actual errors
        var prefix = e.Data.Contains(" ERR ") || e.Data.Contains(" error") || e.Data.Contains("Error") ? "[ERROR]" : "[INFO]";
        OutputReceived?.Invoke(this, $"{prefix} {e.Data}");
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        _isRunning = false;
        
        if (!_userStopped)
        {
            OutputReceived?.Invoke(this, "Tunnel process exited unexpectedly");
            RaiseStateChanged(TunnelState.Error);
        }
        else
        {
            RaiseStateChanged(TunnelState.Stopped);
        }

        ProcessExited?.Invoke(this, EventArgs.Empty);
        CleanupProcess();
    }

    private void CleanupProcess()
    {
        lock (_lock)
        {
            if (_process != null)
            {
                try
                {
                    _process.OutputDataReceived -= OnOutputDataReceived;
                    _process.ErrorDataReceived -= OnErrorDataReceived;
                    _process.Exited -= OnProcessExited;
                    
                    if (!_process.HasExited)
                    {
                        _process.Kill(entireProcessTree: true);
                    }
                }
                catch { /* Ignore cleanup errors */ }
                
                _process.Dispose();
                _process = null;
            }

            _isRunning = false;
            CurrentTunnelUrl = string.Empty;
        }
    }

    private void RaiseStateChanged(TunnelState state)
    {
        StateChanged?.Invoke(this, state);
    }

    #endregion

    #region IDisposable

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed) return;
        
        StopAsync().Wait(TimeSpan.FromSeconds(5));
        _disposed = true;
    }

    #endregion
}
