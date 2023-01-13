// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using Vortice.Graphics;

namespace Vortice;

public abstract class GameView
{
    public event EventHandler? SizeChanged;

    public abstract Size ClientSize { get; }

    public abstract SwapChainSurface Surface { get; }

    public SwapChain? SwapChain { get; private set; }

    public void CreateSwapChain(GraphicsDevice device)
    {
        SwapChainDescription description = new(ClientSize.Width, ClientSize.Height);
        SwapChain = device.CreateSwapChain(Surface, description);
    }

    protected virtual void OnSizeChanged()
    {
        SizeChanged?.Invoke(this, EventArgs.Empty);
    }
}
