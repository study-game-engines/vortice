// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;

namespace Vortice.Graphics;

public abstract class SwapChain : GraphicsResource
{
    public SwapChain(GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device, description.Label)
    {
        Surface = surface;
        DrawableSize = new(description.Width, description.Height);
    }

    public SwapChainSurface Surface { get; }

    public TextureFormat ColorFormat { get; }
    public PresentMode PresentMode { get; }

    public bool AutoResizeDrawable { get; set; } = true;
    public Size DrawableSize { get; set; }

    protected abstract void ResizeBackBuffer(int width, int height);
}
