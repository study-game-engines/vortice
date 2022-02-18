// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_DIMENSION;
using static TerraFX.Interop.DirectX.D3D12_HEAP_TYPE;
using static TerraFX.Interop.DirectX.D3D12_TEXTURE_LAYOUT;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_FLAGS;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_STATES;
using static Vortice.Graphics.D3DUtilities;
using static TerraFX.Interop.Windows.Windows;
using static Vortice.Graphics.D3D12.D3D12Utils;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12Buffer : GraphicsBuffer
{
    private readonly ComPtr<ID3D12Resource> _handle;
    private readonly D3D12_PLACED_SUBRESOURCE_FOOTPRINT _footprint;
    private readonly ulong _allocatedSize;

    public D3D12Buffer(D3D12GraphicsDevice device, in BufferDescriptor descriptor, IntPtr initialData)
        : base(device, descriptor)
    {
        D3D12_RESOURCE_DESC resourceDesc = new()
        {
            Dimension = D3D12_RESOURCE_DIMENSION_BUFFER,
            Width = descriptor.Size,
            Height = 1,
            DepthOrArraySize = 1,
            MipLevels = 1,
            SampleDesc = new(1, 0),
            Layout = D3D12_TEXTURE_LAYOUT_ROW_MAJOR,
            Flags = D3D12_RESOURCE_FLAG_NONE
        };

        if ((descriptor.Usage & BufferUsage.ShaderWrite) != 0)
        {
            resourceDesc.Flags |= D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS;
        }

        if ((descriptor.Usage & BufferUsage.ShaderRead) == 0
            || (descriptor.Usage & BufferUsage.RayTracing) == 0)
        {
            resourceDesc.Flags |= D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE;
        }

        D3D12_RESOURCE_STATES state = D3D12_RESOURCE_STATE_COMMON;

        D3D12_HEAP_PROPERTIES heapProps = default;
        heapProps.Type = D3D12_HEAP_TYPE_DEFAULT;

        HRESULT hr = device.NativeDevice->CreateCommittedResource(
            &heapProps,
            D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE,
            &resourceDesc,
            state,
            null,
            __uuidof<ID3D12Resource>(),
            _handle.GetVoidAddressOf()
            );

        if (hr.FAILED)
        {
            ThrowIfFailed(hr);
        }

        device.NativeDevice->GetCopyableFootprint(&resourceDesc, out _footprint, out _, out _, out _allocatedSize);
    }

    public ID3D12Resource* Handle => _handle;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _handle.Dispose();
        }
    }
}
