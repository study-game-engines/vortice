// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Graphics.VGPU;

namespace Vortice.Graphics;

/// <summary>
/// Defines a Graphics buffer.
/// </summary>
public abstract class GraphicsBuffer : GraphicsResource
{
    protected GraphicsBuffer(GraphicsDevice device, in BufferDescription description)
        : base(device, description.Label)
    {
        BufferDesc nativeDesc = description.ToVGPU();
        //Handle = vgpuCreateBuffer(device.Handle, &nativeDesc, initialData);

        Size = description.Size;
        Usage = description.Usage;
    }

    public ulong Size { get; }
    public BufferUsage Usage { get; }
}
