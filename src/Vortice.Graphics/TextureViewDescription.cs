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

    public TextureFormat Format { get; init; } = TextureFormat.Undefined;

    public uint BaseMipLevel { get; init; }
    public uint MipLevelCount { get; init; }
    public uint BaseArrayLayer { get; init; }
    public uint ArrayLayerCount { get; init; }

    // <summary>
    /// Gets or sets the label of <see cref="Texture"/>.
    /// </summary>
    public string? Label { get; init; }
}
