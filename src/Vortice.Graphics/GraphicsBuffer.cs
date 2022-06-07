// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

/// <summary>
/// Defines a Graphics buffer.
/// </summary>
public abstract class GraphicsBuffer : GraphicsResource
{
    protected GraphicsBuffer(GraphicsDevice device, in BufferDescription description)
        : base(device, description.Label)
    {
        Usage = description.Usage;
        Size = description.Size;
    }

    public BufferUsage Usage { get; }

    public ulong Size { get; }
}
