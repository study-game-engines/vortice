// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Platform;

namespace Vortice;

public abstract class GamePlatform
{
    public event EventHandler? TickRequested;

    /// <summary>
    /// Gets the main window.
    /// </summary>
    public abstract Window MainWindow { get; }

    public virtual void ConfigureModules(ModuleList modules)
    {
    }

    public abstract void RunMainLoop(Action init);

    protected internal void OnTick()
    {
        TickRequested?.Invoke(this, EventArgs.Empty);
    }

    public static GamePlatform CreateDefault()
    {
#if WINDOWS_CLASSIC
        return new WinFormsGamePlatform();
#elif WINDOWS
        return new WindowsGamePlatform();
#else
        return new SDLGamePlatform();
#endif
    }
}
