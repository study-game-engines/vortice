// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Mathematics;
using Vortice.Graphics;

namespace Vortice;

public abstract class GameView
{
    public event EventHandler? SizeChanged;

    public abstract SizeI ClientSize { get; }

    public abstract SwapChainSurface Surface { get; }

    public SwapChain? SwapChain { get; private set; }

    public void CreateSwapChain(GraphicsDevice device)
    {
        SwapChainDescription descriptor = new(ClientSize.Width, ClientSize.Height);
        SwapChain = device.CreateSwapChain(Surface, descriptor);
    }

    protected virtual void OnSizeChanged()
    {
        SizeChanged?.Invoke(this, EventArgs.Empty);
    }
}
