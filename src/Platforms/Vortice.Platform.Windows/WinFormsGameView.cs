// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.InteropServices;
using Vortice.Mathematics;
using Vortice.Graphics;

namespace Vortice;

internal class WinFormsGameView : GameView
{
    private readonly Control _control;

    public WinFormsGameView(Control control)
    {
        _control = control;

        Source = SwapChainSource.CreateWin32(
            Marshal.GetHINSTANCE(Assembly.GetEntryAssembly()!.Modules.First()),
            _control.Handle
            );

        _control.ClientSizeChanged += OnControlClientSizeChanged;
    }

    /// <inheritdoc />
    public override SizeI ClientSize => new SizeI(_control.ClientSize.Width, _control.ClientSize.Height);

    /// <inheritdoc />
    public override SwapChainSource Source { get; }

    private void OnControlClientSizeChanged(object? sender, EventArgs e)
    {
        OnSizeChanged();
    }
}
