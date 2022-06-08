// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Toolkit.Diagnostics;

namespace Vortice.Graphics;

public readonly record struct SwapChainDescription
{
    public SwapChainDescription(
        int width,
        int height,
        PixelFormat colorFormat = PixelFormat.BGRA8UNormSrgb,
        PresentMode presentMode = PresentMode.Fifo)
    {
        Guard.IsTrue(width >= 0, nameof(width));
        Guard.IsTrue(height >= 0, nameof(height));

        Width = width;
        Height = height;
        ColorFormat = colorFormat;
        PresentMode = presentMode;
    }

    public int Width { get; init; }
    public int Height { get; init; }
    public PixelFormat ColorFormat { get; init; } = PixelFormat.BGRA8UNormSrgb;
    public PresentMode PresentMode { get; init; } = PresentMode.Fifo;
    public bool IsFullscreen { get; init; } = false;

    public string? Label { get; init; } = default;
}
