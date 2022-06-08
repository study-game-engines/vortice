// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_DIMENSION;
using static TerraFX.Interop.DirectX.D3D12_HEAP_TYPE;
using static TerraFX.Interop.DirectX.D3D12_TEXTURE_LAYOUT;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_FLAGS;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_STATES;
using static TerraFX.Interop.DirectX.D3D12_HEAP_FLAGS;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.D3D12;
using Vortice.Mathematics;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12Buffer : GraphicsBuffer
{
    private readonly ComPtr<ID3D12Resource> _handle;
    private readonly D3D12_PLACED_SUBRESOURCE_FOOTPRINT _footprint;
    private readonly ulong _allocatedSize;
    private readonly ulong _gpuVirtualAddress;
    private byte* pMappedData;

    public D3D12Buffer(D3D12GraphicsDevice device, in BufferDescription description, IntPtr initialData)
        : base(device, description)
    {
        ulong alignedSize = description.Size;
        if (description.Usage.HasFlag(BufferUsage.Constant))
        {
            alignedSize = MathHelper.AlignUp(alignedSize, D3D12_CONSTANT_BUFFER_DATA_PLACEMENT_ALIGNMENT);
        }

        D3D12_RESOURCE_DESC resourceDesc = new()
        {
            Dimension = D3D12_RESOURCE_DIMENSION_BUFFER,
            Width = alignedSize,
            Height = 1,
            DepthOrArraySize = 1,
            MipLevels = 1,
            SampleDesc = new(1, 0),
            Layout = D3D12_TEXTURE_LAYOUT_ROW_MAJOR,
            Flags = D3D12_RESOURCE_FLAG_NONE
        };

        if (description.Usage.HasFlag(BufferUsage.ShaderWrite))
        {
            resourceDesc.Flags |= D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS;
        }

        if (!description.Usage.HasFlag(BufferUsage.ShaderRead) &&
            !description.Usage.HasFlag(BufferUsage.RayTracing))
        {
            resourceDesc.Flags |= D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE;
        }

        D3D12_RESOURCE_STATES resourceState = D3D12_RESOURCE_STATE_COMMON;

        D3D12_HEAP_PROPERTIES heapProps = default;
        heapProps.Type = D3D12_HEAP_TYPE_DEFAULT;

        if (description.Access == CpuAccess.Read)
        {
            heapProps.Type = D3D12_HEAP_TYPE_READBACK;
            resourceState = D3D12_RESOURCE_STATE_COPY_DEST;
            resourceDesc.Flags |= D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE;
        }
        else if (description.Access == CpuAccess.Write)
        {
            heapProps.Type = D3D12_HEAP_TYPE_UPLOAD;
            resourceState = D3D12_RESOURCE_STATE_GENERIC_READ;
        }

        HRESULT hr = device.NativeDevice->CreateCommittedResource(
            &heapProps,
            D3D12_HEAP_FLAG_NONE,
            &resourceDesc,
            resourceState,
            null,
            __uuidof<ID3D12Resource>(),
            _handle.GetVoidAddressOf()
            );

        if (hr.FAILED)
        {
            ThrowIfFailed(hr);
        }

        device.NativeDevice->GetCopyableFootprint(&resourceDesc, out _footprint, out _, out _, out _allocatedSize);

        _gpuVirtualAddress = _handle.Get()->GetGPUVirtualAddress();

        if (description.Access == CpuAccess.Read)
        {
            fixed (byte** pMappedDataPtr = &pMappedData)
            {
                ThrowIfFailed(_handle.Get()->Map(0, null, (void**)pMappedDataPtr));
            }
        }
        else if (description.Access == CpuAccess.Write)
        {
            D3D12_RANGE readRange = default;
            fixed (byte** pMappedDataPtr = &pMappedData)
            {
                ThrowIfFailed(_handle.Get()->Map(0, &readRange, (void**)pMappedDataPtr));
            }
        }
    }

    // <summary>
    /// Finalizes an instance of the <see cref="D3D12Buffer" /> class.
    /// </summary>
    ~D3D12Buffer() => Dispose(isDisposing: false);

    public ID3D12Resource* Handle => _handle;

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        _handle.Dispose();
    }
}
