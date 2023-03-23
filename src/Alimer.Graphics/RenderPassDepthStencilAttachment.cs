// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CommunityToolkit.Diagnostics;
using Vortice.Mathematics;

namespace Alimer.Graphics;

public readonly record struct RenderPassDepthStencilAttachment
{
    public RenderPassDepthStencilAttachment(TextureView view)
    {
        Guard.IsNotNull(view, nameof(view));

        View = view;
    }

    /// <summary>
    /// The <see cref="TextureView"/> associated with this attachment.
    /// </summary>
    public TextureView View { get; init; }

    // <summary>
    /// The action performed by this attachment at the start of a rendering pass.
    /// </summary>
    public LoadAction DepthLoadAction { get; init; } = LoadAction.Clear;

    /// <summary>
    /// The action performed by this attachment at the end of a rendering pass.
    /// </summary>
    public StoreAction DepthStoreAction { get; init; } = StoreAction.Discard;

    /// <summary>
    /// The depth to use when clearing the depth attachment.
    /// </summary>
    public float ClearDepth { get; init; } = 1.0f;

    public bool DepthReadOnly { get; init; } = false;

    /// <summary>
    /// The action performed by this attachment at the start of a rendering pass.
    /// </summary>
    public LoadAction StencilLoadAction { get; init; } = LoadAction.Clear;

    /// <summary>
    /// The action performed by this attachment at the end of a rendering pass.
    /// </summary>
    public StoreAction StencilStoreAction { get; init; } = StoreAction.Discard;

    /// <summary>
    /// The value to use when clearing the stencil attachment.
    /// </summary>
    public uint ClearStencil { get; init; } = 0;
}
