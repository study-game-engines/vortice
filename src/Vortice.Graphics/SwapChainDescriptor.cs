// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Mathematics;

namespace Vortice.Graphics;

public readonly record struct SwapChainDescriptor : IEquatable<SwapChainDescriptor>
{
    public SizeI Size { get; init; } = default;
    public TextureFormat ColorFormat { get; init; } = TextureFormat.BGRA8UNormSrgb;
    public PresentMode PresentMode { get; init; } = PresentMode.Fifo;
    public bool IsFullscreen { get; init; } = false;

    public string? Label { get; init; } = default;

    public SwapChainDescriptor()
    {
    }
}
