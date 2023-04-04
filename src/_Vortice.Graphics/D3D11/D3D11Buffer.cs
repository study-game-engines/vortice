// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Mathematics;
using Win32;
using Win32.Graphics.Direct3D11;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11Buffer : GraphicsBuffer
{
    private readonly ComPtr<ID3D11Buffer> _handle;

    public ID3D11Buffer* Handle => _handle;
    public bool IsDynamic { get; }

    public D3D11Buffer(D3D11GraphicsDevice device, in BufferDescription description, void* initialData)
        : base(device, description)
    {
        uint size = (uint)description.Size;
        BindFlags bindFlags = BindFlags.None;
        Usage usage = Win32.Graphics.Direct3D11.Usage.Default;
        CpuAccessFlags cpuAccessFlags = CpuAccessFlags.None;

        if ((description.Usage & BufferUsage.Constant) != BufferUsage.None)
        {
            size = MathHelper.AlignUp(size, 64u);
            bindFlags = BindFlags.ConstantBuffer;
            usage = Win32.Graphics.Direct3D11.Usage.Dynamic;
            cpuAccessFlags = CpuAccessFlags.Write;
            IsDynamic = true;
        }
        else
        {
            if ((description.Usage & BufferUsage.Vertex) != BufferUsage.None)
            {
                bindFlags |= BindFlags.VertexBuffer;
            }

            if ((description.Usage & BufferUsage.Index) != BufferUsage.None)
            {
                bindFlags |= BindFlags.IndexBuffer;
            }
        }

        Win32.Graphics.Direct3D11.BufferDescription d3dDesc = new(size, bindFlags, usage, cpuAccessFlags);

        SubresourceData* pInitialData = default;
        SubresourceData subresourceData = default;
        if (initialData != null)
        {
            subresourceData.pSysMem = initialData;
            pInitialData = &subresourceData;
        }

        HResult hr = device.NativeDevice->CreateBuffer(&d3dDesc, pInitialData, _handle.GetAddressOf());
        if (hr.Failure)
        {
            throw new InvalidOperationException("D3D11: Failed to create buffer");
        }

        if (!string.IsNullOrEmpty(description.Label))
        {
            _handle.Get()->SetDebugName(description.Label);
        }
    }

    public D3D11Buffer(GraphicsDevice device, ID3D11Buffer* existingTexture, in BufferDescription description)
        : base(device, description)
    {
        // Keep reference to texture.
        _handle.Attach(existingTexture);
        _ = existingTexture->AddRef();
    }


    // <summary>
    /// Finalizes an instance of the <see cref="D3D11Buffer" /> class.
    /// </summary>
    ~D3D11Buffer() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _handle.Dispose();
        }
    }

    /// <inheritdoc />
    protected override void OnLabelChanged(string newLabel)
    {
        _handle.Get()->SetDebugName(newLabel);
    }
}
