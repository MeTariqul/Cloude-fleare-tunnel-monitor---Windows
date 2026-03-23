# Cloudflare Tunnel Monitor

A powerful Windows desktop application to manage Cloudflare tunnels (cloudflared) with real-time monitoring, automatic reconnection, and comprehensive logging capabilities.

## 👨‍💻 Developed by MeTariqul

GitHub: [https://github.com/MeTariqul](https://github.com/MeTariqul)

---

## Table of Contents

1. [Features](#features)
2. [Requirements](#requirements)
3. [Installation](#installation)
4. [Quick Start Guide](#quick-start-guide)
5. [User Manual](#user-manual)
   - [Dashboard Tab](#dashboard-tab)
   - [Logs Tab](#logs-tab)
   - [Settings Tab](#settings-tab)
6. [Configuration Options](#configuration-options)
7. [System Tray Features](#system-tray-features)
8. [Auto-Start with Windows](#auto-start-with-windows)
9. [File Locations](#file-locations)
10. [Troubleshooting](#troubleshooting)
11. [Building from Source](#building-from-source)
12. [Technology Stack](#technology-stack)
13. [License](#license)

---

## Features

### Core Features
- **Tunnel Management**: Start, stop, and restart Cloudflare tunnels with a single click
- **Real-time Output**: View live tunnel process output in the log viewer with color-coded messages
- **Automatic URL Detection**: Automatically detects and displays the tunnel URL when created
- **URL Logging**: Saves all generated tunnel URLs to a timestamped log file

### Network Monitoring
- **Live Ping Monitoring**: Pings 1.1.1.1 every 5 seconds to check connectivity
- **Network Loss Detection**: Automatically clears URL when network connection is lost
- **Auto URL Regeneration**: Automatically regenerates tunnel URL when network is restored
- **Visual Status Indicators**: Clear visual feedback for online/offline status

### Health & Reliability
- **Health Checks**: Periodic health checks with configurable intervals
- **Auto-Restart Policy**: Automatic tunnel restart on failure
- **Error Logging**: Comprehensive error logging for troubleshooting

### User Experience
- **System Tray Integration**: Minimize to tray, balloon notifications, quick actions menu
- **Persistent Settings**: Settings saved as JSON in user's AppData folder
- **Auto-Start Option**: Start automatically with Windows login
- **Minimize to Tray**: Option to hide to system tray when closed
- **URL Auto-Copy**: Automatically copy generated URL to clipboard

---

## Requirements

### Software Requirements
- **Operating System**: Windows 10 or Windows 11
- **No .NET runtime required** (self-contained version includes .NET)
- **cloudflared.exe** - Already included in the release package

### Hardware Requirements
- **Processor**: 1 GHz or faster
- **Memory**: 512 MB RAM minimum
- **Disk Space**: 300 MB for single-file application + space for logs

---

## Installation

### Method 1: Pre-built Executable (Recommended)

1. Download the latest release from the `publish-single` folder
2. Copy both files to your desired location:
   - `CloudflareTunnelMonitor.exe` (the main application)
   - `cloudflared.exe` (Cloudflare's tunnel client)
3. Run `CloudflareTunnelMonitor.exe`

### Method 2: Build from Source

#### Prerequisites
- Install [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Download `cloudflared.exe` from [GitHub releases](https://github.com/cloudflare/cloudflared/releases)

#### Build Commands

```powershell
# Navigate to project directory
cd CloudflareTunnelMonitor

# Debug build
dotnet build

# Run the application
dotnet run

# Release build
dotnet build -c Release

# Self-contained single-file publish (includes .NET runtime)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish-single
```

---

## Quick Start Guide

### First Time Setup

1. **Launch the Application**
   - Double-click `CloudflareTunnelMonitor.exe`
   - The application will open to the Settings tab

2. **Configure cloudflared.exe**
   - Click "Browse" next to "cloudflared.exe Path"
   - Select your `cloudflared.exe` file (or keep the default if it's in the same folder)

3. **Set Local URL**
   - Enter the local service URL you want to expose (e.g., `http://localhost:8080`)
   - This is typically your web application's address

4. **Apply Settings**
   - Click "Apply & Restart" button
   - The application will switch to the Dashboard tab

5. **Start the Tunnel**
   - Click "Start Tunnel" button
   - Wait a few seconds for the tunnel to establish
   - The tunnel URL will appear in the "Tunnel URL" field

6. **Use Your Tunnel URL**
   - Copy the URL and share it or use it as needed
   - The URL is automatically logged to a file

---

## User Manual

### Dashboard Tab

The Dashboard provides an overview of your tunnel status and quick actions.

#### Status Panel
- **Tunnel Status**: Shows Running/Stopped indicator
- **Network Status**: Shows Online/Offline with live ping result
- **Tunnel URL**: Displays the current public tunnel URL (click to copy)
- **Uptime**: Shows how long the tunnel has been running

#### Control Buttons
- **Start Tunnel**: Starts the cloudflared process
- **Stop Tunnel**: Stops the cloudflared process
- **Restart Tunnel**: Stops and starts the tunnel (useful after configuration changes)

#### Quick Actions
- **View URL Log**: Opens the log file containing all generated URLs
- **Open Save Directory**: Opens the folder where logs are stored

### Logs Tab

The Logs tab displays real-time output from the cloudflared process.

#### Features
- **Live Output**: Shows stdout and stderr in real-time
- **Color Coding**: 
  - `[INFO]` messages in default color
  - `[ERROR]` messages highlighted
- **Auto-scroll**: Automatically scrolls to show new messages
- **Manual Scroll**: You can scroll up to review previous output

#### Controls
- **Clear**: Clears the log display (doesn't delete the log file)
- **Copy**: Copies selected log content to clipboard

### Settings Tab

Configure all application settings here.

#### Path Settings
| Setting | Description | Default |
|---------|-------------|---------|
| cloudflared.exe | Path to cloudflared executable | `cloudflared.exe` (in app folder) |

#### Connection Settings
| Setting | Description | Default |
|---------|-------------|---------|
| Local URL | Local service to expose | `http://localhost:8080` |

#### Storage Settings
| Setting | Description | Default |
|---------|-------------|---------|
| Save Directory | Where to save URL logs | `Documents\TunnelLogs` |

#### Behavior Settings
| Setting | Description | Default |
|---------|-------------|---------|
| Auto-start | Start with Windows | Disabled |
| Auto-reconnect | Reconnect when network returns | Enabled |
| Minimize to tray | Hide to tray on close | Enabled |
| Auto-copy URL | Copy URL to clipboard when generated | Disabled |

#### Health Check Settings
| Setting | Description | Default |
|---------|-------------|---------|
| Check Interval | Health check interval in seconds | 60 |
| Ping Test URL | URL/IP for connectivity check | `1.1.1.1` |

#### Action Buttons
- **Browse**: Select cloudflared.exe file
- **Apply & Restart**: Save settings and restart tunnel
- **Reset Settings**: Reset all settings to defaults
- **Open Save Directory**: Open the log directory in Explorer

---

## Configuration Options

### Settings File Format

Settings are stored in JSON format at:
```
%AppData%\CloudflareTunnelMonitor\settings.json
```

### Example Settings File
```json
{
  "cloudflaredPath": "cloudflared.exe",
  "localUrl": "http://localhost:8080",
  "saveDirectory": "C:\\Users\\Username\\Documents\\TunnelLogs",
  "autoStart": false,
  "autoReconnect": true,
  "minimizeToTray": true,
  "checkInterval": 60,
  "pingTestUrl": "1.1.1.1",
  "autoCopyUrl": false
}
```

---

## System Tray Features

When minimized, the application runs in the system tray.

### Tray Icon
- Shows application icon in the system tray
- Icon changes based on tunnel status (optional enhancement)

### Tray Menu (Right-click)
- **Show**: Restore the application window
- **Start Tunnel**: Start the tunnel without opening the app
- **Stop Tunnel**: Stop the tunnel without opening the app
- **Exit**: Close the application completely

### Notifications
- Balloon notifications for:
  - Tunnel started
  - Tunnel stopped
  - Tunnel URL generated
  - Network connection lost/restored
  - Errors occurred

---

## Auto-Start with Windows

### Enable Auto-Start
1. Go to Settings tab
2. Check "Start with Windows" checkbox
3. Click "Apply & Restart"

### How It Works
The application adds a registry entry at:
```
HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run
```

Value: `CloudflareTunnelMonitor` pointing to the executable path

### Disable Auto-Start
Uncheck the "Start with Windows" option in Settings.

---

## File Locations

### Application Files
| File | Location |
|------|----------|
| Settings | `%AppData%\CloudflareTunnelMonitor\settings.json` |
| Error Log | `%AppData%\CloudflareTunnelMonitor\error.log` |
| URL Log | `<SaveDirectory>\tunnel_urls.txt` |

### Default Paths
- **Save Directory**: `Documents\TunnelLogs\`
- **URL Log File**: `Documents\TunnelLogs\tunnel_urls.txt`

### Log File Format
```
[timestamp] Generated URL: https://your-tunnel-url.trycloudflare.com
```

---

## Troubleshooting

### Common Issues

#### "cloudflared.exe not found"
**Solution**:
1. Download cloudflared.exe from [GitHub releases](https://github.com/cloudflare/cloudflared/releases)
2. Place it in the application folder OR
3. Browse to its location in Settings tab

#### Tunnel won't start
**Possible Causes & Solutions**:
- Check log output in Logs tab for errors
- Verify cloudflared.exe path in Settings
- Ensure cloudflared.exe is not already running
- Check if the local URL service is running

#### Application crashes on startup
**Solution**:
1. Check error log: `%AppData%\CloudflareTunnelMonitor\error.log`
2. Ensure .NET 8.0 Runtime is installed
3. Try running as Administrator

#### Network shows as "Offline"
**Solution**:
1. Check if internet is actually available
2. Verify Ping Test URL in Settings (try `8.8.8.8`)
3. Check firewall settings

#### URL not being detected
**Solution**:
1. Check Logs tab for "cf-trunnel" or "trycloudflare" patterns
2. Ensure cloudflared is running with proper output
3. Check if tunnel creation was successful

#### Single-file exe won't run
**Solution**:
- Some antivirus/security software may block the exe
- Try running from command line or as Administrator
- Check if Device Guard is enabled

### Getting Help

If you encounter issues not listed here:
1. Check the error log file
2. Review the Logs tab output
3. Verify your cloudflared.exe version is up to date

---

## Building from Source

### Development Setup

```powershell
# Clone the repository
git clone <repository-url>
cd CloudflareTunnelMonitor

# Install .NET 8.0 SDK
# Download from https://dotnet.microsoft.com/download/dotnet/8.0

# Restore dependencies
dotnet restore

# Build debug version
dotnet build

# Run in development
dotnet run
```

### Publishing Options

#### Framework-Dependent (Smaller size, requires .NET runtime)
```powershell
dotnet publish -c Release -o ./publish-fd
```

#### Self-Contained (Larger size, no .NET required)
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish-sc
```

#### Single File (One exe, self-contained)
```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ./publish-single
```

---

## Technology Stack

| Component | Technology |
|-----------|------------|
| Framework | .NET 8.0 |
| UI | Windows Forms (WinForms) |
| Language | C# 12 |
| Process Management | System.Diagnostics |
| Network Monitoring | System.Net.NetworkInformation |
| Settings Storage | System.Text.Json |
| Logging | System.IO.File |

---

## License

This project is provided as-is for personal and commercial use.

### Third-Party Components
- **cloudflared.exe**: Copyright Cloudflare, Inc. - [License](https://github.com/cloudflare/cloudflared/blob/master/LICENSE)

---

## Acknowledgments

- Cloudflare for providing the cloudflared tool
- .NET team for the excellent development framework
- All users and contributors

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2024 | Initial release |

---

**Note**: This application requires a valid Cloudflare account and configured tunnel to work properly. For Cloudflare Tunnel setup, please refer to [Cloudflare's documentation](https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/).
