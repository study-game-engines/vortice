// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

/// <summary>
/// Defines a Graphics buffer.
/// </summary>
public abstract class GraphicsBuffer : GraphicsResource
{
    protected GraphicsBuffer(GraphicsDevice device, in BufferDescriptor descriptor)
        : base(device)
    {
        Usage = descriptor.Usage;
        Size = descriptor.Size;
        Label = descriptor.Label;
    }

    public BufferUsage Usage { get; }

    public ulong Size { get; }

    public virtual string? Label { get; set; }
}
