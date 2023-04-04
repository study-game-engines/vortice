// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.Versioning;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Vortice;

internal class WinFormsGamePlatform : GamePlatform
{
    private readonly WinFormsGameView _view;
    private bool _exitRequested;

    public WinFormsGamePlatform(Control? control = default)
    {
        View = (_view = new WinFormsGameView(this, control));
    }

    // <inheritdoc />
    public override GameView View { get; }

    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        //services.AddSingleton<GraphicsPresenter>(new HwndSwapChainGraphicsPresenter(GraphicsDevice, PresentationParameters, Control.Handle));
        //services.AddSingleton<IInputSourceConfiguration>(new WinFormsInputSourceConfiguration(Control));
    }

    public override void RunMainLoop(Action init)
    {
        init();

        try
        {
            Application.Idle += ApplicationIdle;
            Application.Run((Form)_view.Control);
        }
        finally
        {
            Application.Idle -= ApplicationIdle;
            //doneRun = true;
            //OnExiting();
        }
    }

    private void ApplicationIdle(object? sender, EventArgs e)
    {
        while (PeekMessage(out Message msg, IntPtr.Zero, 0u, 0u, 0u) == 0)
        {
            if (_exitRequested)
            {
                _view.Close();
            }
            else
            {
                RunOneFrame();
            }
        }
    }

    private void RunOneFrame()
    {
        Tick();
    }

    [DllImport("User32", ExactSpelling = true, EntryPoint = "PeekMessageW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [SupportedOSPlatform("windows5.0")]
    private static extern int PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);
}
