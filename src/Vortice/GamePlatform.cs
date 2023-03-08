// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Extensions.DependencyInjection;

namespace Vortice;

public abstract class GamePlatform
{
    public event EventHandler? TickRequested;

    /// <summary>
    /// Gets the main view.
    /// </summary>
    public abstract GameView View { get; }

    public virtual void ConfigureServices(IServiceCollection services)
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
