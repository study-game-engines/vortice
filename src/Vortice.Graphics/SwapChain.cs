// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Mathematics;

namespace Vortice.Graphics;

public abstract class SwapChain : GraphicsResource
{
    protected SwapChain(GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device, description.Label)
    {
        Surface = surface;
        Size = new(description.Width, description.Height);
        ColorFormat = description.ColorFormat == PixelFormat.Invalid ? PixelFormat.BGRA8UNorm : description.ColorFormat;
        PresentMode = description.PresentMode;
        IsFullscreen = description.IsFullscreen;
    }

    public SwapChainSurface Surface { get; }

    public SizeI Size { get; protected set; }

    public PixelFormat ColorFormat { get; protected set; }
    public PresentMode PresentMode { get; }
    public bool IsFullscreen { get; }

    public abstract Texture? CurrentBackBuffer { get; }

    public abstract int CurrentBackBufferIndex { get; }
    public abstract int BackBufferCount { get; }

    public abstract void Resize(int newWidth, int newHeight);

    public abstract void Present();
}
