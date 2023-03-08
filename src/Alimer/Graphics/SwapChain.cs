// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;

namespace Alimer.Graphics;

public abstract class SwapChain : GraphicsResource
{
    public SwapChain(GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device, description.Label)
    {
        Surface = surface;
        ColorFormat = description.Format;
        PresentMode = description.PresentMode;
        DrawableSize = new(description.Width, description.Height);
    }

    public SwapChainSurface Surface { get; }

    public PixelFormat ColorFormat { get; protected set; }
    public PresentMode PresentMode { get; }

    public bool AutoResizeDrawable { get; set; } = true;
    public Size DrawableSize { get; protected set; }

    protected abstract void ResizeBackBuffer(int width, int height);
}
