// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Direct3D11;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11Buffer : Buffer
{
    public D3D11Buffer(D3D11GraphicsDevice device, in BufferDescriptor descriptor, IntPtr initialData)
    : base(device, descriptor)
    {
        BufferDescription d3dDesc = new()
        {
            SizeInBytes = (int)descriptor.Size,
            Usage = ResourceUsage.Default,
            CpuAccessFlags = CpuAccessFlags.None,
            BindFlags = BindFlags.None,
            OptionFlags = ResourceOptionFlags.None,
            StructureByteStride = 0,
        };

        if ((descriptor.Usage & BufferUsage.Vertex) != 0)
        {
            d3dDesc.BindFlags |= BindFlags.VertexBuffer;
        }

        if ((descriptor.Usage & BufferUsage.Index) != 0)
        {
            d3dDesc.BindFlags |= BindFlags.IndexBuffer;
        }

        if ((descriptor.Usage & BufferUsage.Uniform) != 0)
        {
            d3dDesc.BindFlags |= BindFlags.ConstantBuffer;
        }

        if ((descriptor.Usage & BufferUsage.ShaderRead) != 0)
        {
            d3dDesc.BindFlags |= BindFlags.ShaderResource;
            d3dDesc.OptionFlags |= ResourceOptionFlags.BufferAllowRawViews;
        }

        if ((descriptor.Usage & BufferUsage.ShaderReadWrite) != 0)
        {
            d3dDesc.BindFlags |= BindFlags.UnorderedAccess;
            d3dDesc.OptionFlags |= ResourceOptionFlags.BufferAllowRawViews;
        }

        if ((descriptor.Usage & BufferUsage.Indirect) != 0)
        {
            d3dDesc.OptionFlags |= ResourceOptionFlags.DrawIndirectArguments;
        }

        Handle = device.NativeDevice.CreateBuffer(d3dDesc, initialData);
    }

    public ID3D11Buffer Handle { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Handle.Dispose();
        }
    }
}
