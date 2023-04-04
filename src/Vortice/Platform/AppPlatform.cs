// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using Vortice.Input;

namespace Vortice.Platform;

/// <summary>
/// Core application platform Module, used for managing Windows, Monitors, Input and Settings.
/// </summary>
public abstract class AppPlatform : AppModule
{
    public event EventHandler? TickRequested;

    /// <summary>
    /// Gets whether the multiple windows are supported.
    /// </summary>
    public abstract bool SupportsMultipleWindows { get; }

    /// <summary>
    /// Gets the main window.
    /// </summary>
    public abstract Window MainWindow { get; }

    /// <summary>
    /// Gets the input manager.
    /// </summary>
    public abstract InputManager Input { get; }

    protected AppPlatform()
        : base(100)
    {
        //Windows = new ReadOnlyCollection<Window>(windows);
       // Monitors = new ReadOnlyCollection<Monitor>(monitors);
    }

    public abstract void RunMainLoop(Action init);

    protected internal void OnTick()
    {
        TickRequested?.Invoke(this, EventArgs.Empty);
    }

    protected internal override void Startup()
    {
        Log.Info($"{ApiName} v{ApiVersion}");
    }

    /// <summary>
    /// The User Directory & a safe location to store save data or preferences
    /// </summary>
    public virtual string UserDirectory(string applicationName) => DefaultUserDirectory(applicationName);

    /// <summary>
    /// Gets the Default UserDirectory
    /// </summary>
    internal static string DefaultUserDirectory(string applicationName)
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            string? result = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(result))
                return Path.Combine(result, "Library", "Application Support", applicationName);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                 RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            string? result = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
            if (!string.IsNullOrEmpty(result))
            {
                return Path.Combine(result, applicationName);
            }
            else
            {
                result = Environment.GetEnvironmentVariable("HOME");
                if (!string.IsNullOrEmpty(result))
                    return Path.Combine(result, ".local", "share", applicationName);
            }
        }

        return AppContext.BaseDirectory;
    }
}
