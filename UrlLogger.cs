namespace CloudflareTunnelMonitor;

/// <summary>
/// Handles logging of tunnel URLs to a text file.
/// </summary>
public class UrlLogger : IDisposable
{
    private readonly string _logFilePath;
    private readonly object _lock = new();
    private readonly StreamWriter? _writer;
    private bool _disposed;

    /// <summary>
    /// Creates a new UrlLogger instance.
    /// </summary>
    /// <param name="logFilePath">Full path to the log file.</param>
    public UrlLogger(string logFilePath)
    {
        _logFilePath = logFilePath;
        
        // Ensure directory exists
        var directory = Path.GetDirectoryName(_logFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // Create/append to the log file with shared read/write access
        // Use FileStream with proper sharing mode to allow multiple instances
        var fileStream = new FileStream(
            _logFilePath, 
            FileMode.OpenOrCreate, 
            FileAccess.ReadWrite, 
            FileShare.ReadWrite);
        
        // Seek to end for append mode
        fileStream.Seek(0, SeekOrigin.End);
        
        _writer = new StreamWriter(fileStream)
        {
            AutoFlush = true
        };
    }

    /// <summary>
    /// Logs a tunnel URL to the file.
    /// </summary>
    /// <param name="url">The tunnel URL to log.</param>
    public void LogUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        lock (_lock)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"{timestamp} - {url}";
                
                _writer?.WriteLine(logEntry);
            }
            catch (Exception)
            {
                // Ignore write errors
            }
        }
    }

    /// <summary>
    /// Gets all logged URLs.
    /// </summary>
    /// <returns>List of URL entries.</returns>
    public List<string> GetLoggedUrls()
    {
        var urls = new List<string>();
        
        lock (_lock)
        {
            try
            {
                if (File.Exists(_logFilePath))
                {
                    using var fs = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(fs);
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        urls.Add(line);
                    }
                }
            }
            catch (Exception)
            {
                // Ignore read errors
            }
        }
        
        return urls;
    }

    /// <summary>
    /// Clears all logged URLs.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            try
            {
                _writer?.Flush();
                // Use FileStream with shared access to clear the file
                using var fs = new FileStream(_logFilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
            }
            catch (Exception)
            {
                // Ignore errors
            }
        }
    }

    /// <summary>
    /// Gets the path to the log file.
    /// </summary>
    public string LogFilePath => _logFilePath;

    /// <summary>
    /// Opens the containing folder in File Explorer.
    /// </summary>
    public void OpenContainingFolder()
    {
        try
        {
            var directory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{_logFilePath}\"");
            }
        }
        catch (Exception)
        {
            // Ignore errors
        }
    }

    /// <summary>
    /// Opens the log file in the default text editor.
    /// </summary>
    public void OpenLogFile()
    {
        try
        {
            if (File.Exists(_logFilePath))
            {
                var processInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _logFilePath,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(processInfo);
            }
        }
        catch (Exception)
        {
            // Ignore errors
        }
    }

    #region IDisposable

    public void Dispose()
    {
        if (_disposed) return;
        
        lock (_lock)
        {
            try
            {
                _writer?.Flush();
                _writer?.Dispose();
            }
            catch { /* Ignore */ }
        }
        
        _disposed = true;
    }

    #endregion
}
