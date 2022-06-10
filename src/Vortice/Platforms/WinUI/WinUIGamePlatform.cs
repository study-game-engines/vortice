// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Core;

namespace Vortice;

internal class WinUIGamePlatform : GamePlatform
{
    private readonly CoreWindowGameWindow _window;
    private bool _exitRequested;

    public WinUIGamePlatform(CoreWindow? window = null)
    {
        
    }

    public override GameView View => throw new NotImplementedException();

    public override void RunMainLoop(Action init) => throw new NotImplementedException();
}
