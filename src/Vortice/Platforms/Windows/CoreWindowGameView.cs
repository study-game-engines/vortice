// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using Vortice.Graphics;
using Vortice.Mathematics;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Vortice;

internal class CoreWindowGameView : GameView, IFrameworkView
{
    private readonly WindowsGamePlatform _platform;
    private bool _minimized;

    public CoreWindowGameView(WindowsGamePlatform platform)
    {
        _platform = platform;
        IsVisible = true;
    }

    /// <inheritdoc />
    public override bool IsMinimized => _minimized;
    public override Size ClientSize => throw new NotImplementedException();
    public override SwapChainSurface Surface => throw new NotImplementedException();
    public bool IsVisible { get; private set; }

    #region IFrameworkView methods
    public void Initialize(CoreApplicationView applicationView)
    {

    }
    public void Uninitialize()
    {

    }

    public void Load(string entryPoint)
    {

    }
    public void SetWindow(CoreWindow window)
    {

    }

    public void Run()
    {
        while (!_platform.ExitRequested)
        {
            if (IsVisible)
            {
                _platform.OnTick();

                CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
            }
            else
            {
                CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessOneAndAllPending);
            }
        }
    }

    #endregion  IFrameworkView methods
}
