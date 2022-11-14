// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Windows.ApplicationModel.Core;

namespace Vortice;

internal sealed class WindowsGamePlatform : GamePlatform, IFrameworkViewSource
{
    private readonly CoreWindowGameView _view;

    public WindowsGamePlatform()
    {
        _view = new CoreWindowGameView(this);
    }

    public override GameView View => _view;
    public bool ExitRequested { get; }

    public override void RunMainLoop(Action init)
    {
        CoreApplication.Run(this);
    }

    IFrameworkView IFrameworkViewSource.CreateView()
    {
        return _view;
    }
}
