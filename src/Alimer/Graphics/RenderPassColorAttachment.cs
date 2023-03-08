// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CommunityToolkit.Diagnostics;
using Vortice.Mathematics;

namespace Alimer.Graphics;

public readonly record struct RenderPassColorAttachment
{
    public RenderPassColorAttachment(TextureView view)
    {
        Guard.IsNotNull(view, nameof(view));

        View = view;
    }

    /// <summary>
    /// The <see cref="TextureView"/> associated with this attachment.
    /// </summary>
    public TextureView View { get; init; }

    /// <summary>
    /// The action performed by this attachment at the start of a rendering pass.
    /// </summary>
    public LoadAction LoadAction { get; init; } = LoadAction.Load;

    /// <summary>
    /// The action performed by this attachment at the end of a rendering pass
    /// </summary>
    public StoreAction StoreAction { get; init; } = StoreAction.Store;

    /// <summary>
    /// The color to use when clearing the color attachment.
    /// </summary>
    public Color4 ClearColor { get; init; } = new(0.0f, 0.0f, 0.0f, 1.0f);

    /// <summary>
    /// The destination <see cref="TextureView"/> used when resolving multisampled texture data into single sample values.
    /// </summary>
    public TextureView? ResolveView { get; init; }
}
