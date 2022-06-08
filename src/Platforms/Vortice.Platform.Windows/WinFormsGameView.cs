// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Mathematics;
using Vortice.Graphics;

namespace Vortice;

internal unsafe class WinFormsGameView : GameView
{
    private readonly Control _control;

    public WinFormsGameView(Control control)
    {
        _control = control;

        Surface = SwapChainSurface.CreateWin32(_control.Handle);

        _control.ClientSizeChanged += OnControlClientSizeChanged;
    }

    /// <inheritdoc />
    public override SizeI ClientSize => new SizeI(_control.ClientSize.Width, _control.ClientSize.Height);

    /// <inheritdoc />
    public override SwapChainSurface Surface { get; }

    private void OnControlClientSizeChanged(object? sender, EventArgs e)
    {
        OnSizeChanged();
    }
}
