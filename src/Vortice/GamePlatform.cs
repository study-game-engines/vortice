// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Vortice.Graphics;

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

    protected void Tick()
    {
        TickRequested?.Invoke(this, EventArgs.Empty);
    }

    public static GamePlatform CreateDefault()
    {
#if WINDOWS
        return new WinFormsGamePlatform();
#elif NETSTANDARD2_0 || NET6_0_OR_GREATER
        return new SDLGamePlatform();
#elif WINDOWS_UWP
        return new WinUIGamePlatform();
#endif
    }
}
