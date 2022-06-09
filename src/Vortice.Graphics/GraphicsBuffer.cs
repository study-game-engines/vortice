// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Graphics.VGPU;

namespace Vortice.Graphics;

/// <summary>
/// Defines a Graphics buffer.
/// </summary>
public unsafe sealed class GraphicsBuffer : GraphicsResource
{
    internal GraphicsBuffer(GraphicsDevice device, in BufferDescription description, void* initialData)
        : base(device, IntPtr.Zero, description.Label)
    {
        BufferDesc nativeDesc = description.ToVGPU();
        Handle = vgpuCreateBuffer(device.Handle, &nativeDesc, initialData);

        Size = description.Size;
        Usage = description.Usage;
    }

    // <summary>
    /// Finalizes an instance of the <see cref="GraphicsBuffer" /> class.
    /// </summary>
    ~GraphicsBuffer() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Handle != IntPtr.Zero)
            {
                vgpuDestroyBuffer(Device.Handle, Handle);
            }
        }
    }

    public ulong Size { get; }
    public BufferUsage Usage { get; }

    public static GraphicsBuffer Create<T>(GraphicsDevice device, ReadOnlySpan<T> data, BufferUsage usage = BufferUsage.ShaderReadWrite) where T : unmanaged
    {
        BufferDescription description = new((ulong)(data.Length * sizeof(T)), usage);
        fixed (T* dataPtr = data)
        {
            return new GraphicsBuffer(device, description, dataPtr);
        }
    }
}
