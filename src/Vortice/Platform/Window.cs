// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using Vortice.Graphics;

namespace Vortice.Platform;

/// <summary>
/// Defines an OS window.
/// </summary>
public abstract class Window
{
    public event EventHandler? SizeChanged;

    public abstract bool IsMinimized { get; }
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
