// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public abstract class SwapChain : GraphicsResource
{
    public SwapChain(GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device, description.Label)
    {
        Surface = surface;
    }

    public SwapChainSurface Surface { get; }

    public TextureFormat ColorFormat { get; }
    public PresentMode PresentMode { get; }

    public bool AutoResizeDrawable { get; set; } = true;

    //public void Resize(int width, int height)
    //{
    //    PresentationParameters.BackBufferWidth = width;
    //    PresentationParameters.BackBufferHeight = height;

    //    ResizeBackBuffer(width, height);
    //    ResizeDepthStencilBuffer(width, height);
    //}

    protected abstract void ResizeBackBuffer(int width, int height);
}
