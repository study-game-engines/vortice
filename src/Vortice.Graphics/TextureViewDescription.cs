// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

/// <summary>
/// Structure that describes the <see cref="TextureView"/>.
/// </summary>
public record struct TextureViewDescription
{
    public TextureViewDescription()
    {
    }

    /// <summary>
    /// Dimension of the <see cref="TextureView"/>.
    /// </summary>
    public TextureViewDimension Dimension { get; init; } = TextureViewDimension.Undefined;

    public PixelFormat Format { get; init; } = PixelFormat.Undefined;

    public int BaseMipLevel { get; init; }
    public int MipLevelCount { get; init; }
    public int BaseArrayLayer { get; init; }
    public int ArrayLayerCount { get; init; }

    // <summary>
    /// Gets or sets the label of <see cref="Texture"/>.
    /// </summary>
    public string? Label { get; init; }
}
