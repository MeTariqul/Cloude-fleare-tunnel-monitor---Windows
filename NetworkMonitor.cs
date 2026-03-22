using System.Net.NetworkInformation;

namespace CloudflareTunnelMonitor;

/// <summary>
/// Monitors network availability and connectivity.
/// </summary>
public class NetworkMonitor : IDisposable
{
    private readonly Ping _ping;
    private string _pingTestUrl;
    private int _pingTimeout;
    private bool _isMonitoring;
    private bool _lastKnownStatus;

    /// <summary>
    /// Event raised when network availability changes.
    /// </summary>
    public event EventHandler<bool>? NetworkStatusChanged;

    /// <summary>
    /// Gets whether the network is currently available.
    /// </summary>
    public bool IsNetworkAvailable { get; private set; }

    public NetworkMonitor()
    {
        _ping = new Ping();
        _pingTestUrl = "1.1.1.1";
        _pingTimeout = 2000; // 2 seconds
        _isMonitoring = false;
        _lastKnownStatus = false;
        
        // Initial check
        IsNetworkAvailable = CheckInternetConnectivity();
    }

    /// <summary>
    /// Configures the network monitor with the specified ping test URL.
    /// </summary>
    public void Configure(string pingTestUrl, int pingTimeout = 2000)
    {
        _pingTestUrl = pingTestUrl;
        _pingTimeout = pingTimeout;
    }

    private System.Threading.Timer? _livePingTimer;
    
    /// <summary>
    /// Starts monitoring network changes.
    /// </summary>
    public void StartMonitoring()
    {
        if (_isMonitoring) return;

        try
        {
            // Register for network change notifications
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
            _isMonitoring = true;
            
            // Start live ping monitoring every 5 seconds
            _livePingTimer = new System.Threading.Timer(LivePingCallback, null, 0, 5000);
            
            // Initial status
            IsNetworkAvailable = CheckInternetConnectivity();
            _lastKnownStatus = IsNetworkAvailable;
        }
        catch (Exception)
        {
            // Some systems may not support network change notifications
            _isMonitoring = false;
        }
    }
    
    private bool _formLoaded;
    
    /// <summary>
    /// Sets whether the form has been loaded (for thread-safe UI updates).
    /// </summary>
    public void SetFormLoaded(bool loaded)
    {
        _formLoaded = loaded;
    }
    
    /// <summary>
    /// Live ping callback for continuous network monitoring.
    /// </summary>
    private void LivePingCallback(object? state)
    {
        if (!_formLoaded) return; // Skip if form not loaded yet
        
        var wasAvailable = IsNetworkAvailable;
        IsNetworkAvailable = CheckInternetConnectivity();
        
        // Only trigger event if status changed
        if (IsNetworkAvailable != wasAvailable)
        {
            NetworkStatusChanged?.Invoke(this, IsNetworkAvailable);
        }
    }

    /// <summary>
    /// Stops monitoring network changes.
    /// </summary>
    public void StopMonitoring()
    {
        if (!_isMonitoring) return;

        try
        {
            NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;
        }
        catch { /* Ignore */ }
        
        _isMonitoring = false;
    }

    /// <summary>
    /// Checks if internet is available by pinging a known address.
    /// </summary>
    /// <returns>True if internet is reachable, false otherwise.</returns>
    public bool IsInternetAvailable()
    {
        return CheckInternetConnectivity();
    }

    /// <summary>
    /// Performs the actual internet connectivity check.
    /// </summary>
    private bool CheckInternetConnectivity()
    {
        try
        {
            // First check basic network availability
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return false;
            }

            // Try to ping
            var reply = _ping.Send(_pingTestUrl, _pingTimeout);
            return reply.Status == IPStatus.Success;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Handles network availability change events.
    /// </summary>
    private void OnNetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
    {
        var wasAvailable = IsNetworkAvailable;
        IsNetworkAvailable = e.IsAvailable;

        // If the change is from unavailable to available, verify with actual ping
        // because network can be "available" but not actually connected to internet
        if (!wasAvailable && IsNetworkAvailable)
        {
            // Give the network a moment to stabilize
            Task.Delay(500).ContinueWith(_ =>
            {
                IsNetworkAvailable = CheckInternetConnectivity();
                
                if (IsNetworkAvailable != _lastKnownStatus)
                {
                    var previousStatus = _lastKnownStatus;
                    _lastKnownStatus = IsNetworkAvailable;
                    NetworkStatusChanged?.Invoke(this, IsNetworkAvailable);
                }
            });
        }
        else
        {
            // Network became unavailable
            _lastKnownStatus = IsNetworkAvailable;
            NetworkStatusChanged?.Invoke(this, IsNetworkAvailable);
        }
    }

    #region IDisposable

    private bool _disposed;

    public void Dispose()
    {
        if (_disposed) return;

        _livePingTimer?.Dispose();
        StopMonitoring();
        _ping.Dispose();
        _disposed = true;
    }

    #endregion
}
