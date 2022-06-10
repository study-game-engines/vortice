// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Graphics;
using Vortice.Mathematics;
using Windows.UI.Core;

namespace Vortice;

internal class CoreWindowGameWindow : GameView
{
    private readonly CoreWindow _coreWindow;

    public CoreWindowGameWindow(CoreWindow coreWindow)
    {
        _coreWindow = coreWindow;
        //coreWindow.SizeChanged += OnCoreWindowSizeChanged;
    }

    public override SizeI ClientSize => throw new NotImplementedException();

    public override SwapChainSurface Surface => throw new NotImplementedException();
}
