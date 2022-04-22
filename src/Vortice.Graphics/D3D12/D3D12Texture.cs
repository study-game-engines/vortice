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

internal unsafe class D3D12Texture : Texture
{
    private readonly ComPtr<ID3D12Resource> _handle;
    private readonly D3D12_PLACED_SUBRESOURCE_FOOTPRINT _footprint;
    private readonly ulong _allocatedSize;

    public D3D12Texture(GraphicsDevice device, ID3D12Resource* handle)
        : base(device, FromD3D12(handle->GetDesc()))
    {
        _handle = handle;
    }

    public D3D12Texture(D3D12GraphicsDevice device, in TextureDescriptor descriptor)
        : base(device, descriptor)
    {
        D3D12_RESOURCE_DESC resourceDesc = new()
        {
            Dimension = ToD3D12(descriptor.Dimension),
            Alignment = 0u,
            Width = (ulong)descriptor.Width,
            Height = (uint)descriptor.Height,
            DepthOrArraySize = (ushort)descriptor.DepthOrArraySize,
            MipLevels = (ushort)descriptor.MipLevels,
            Format = ToDXGIFormat(descriptor.Format),
            SampleDesc = new(ToD3D(descriptor.SampleCount), 0),
            Layout = D3D12_TEXTURE_LAYOUT_UNKNOWN,
            Flags = D3D12_RESOURCE_FLAG_NONE
        };

        if ((descriptor.Usage & TextureUsage.ShaderWrite) != 0)
        {
            resourceDesc.Flags |= D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS;
        }

        D3D12_RESOURCE_STATES state = D3D12_RESOURCE_STATE_COMMON;
        D3D12_CLEAR_VALUE clearValue = default;
        D3D12_CLEAR_VALUE* optimizedClearValue = null;
        if ((descriptor.Usage & TextureUsage.RenderTarget) == TextureUsage.RenderTarget)
        {
            clearValue.Format = resourceDesc.Format;

            if (descriptor.Format.IsDepthStencilFormat())
            {
                state = D3D12_RESOURCE_STATE_DEPTH_WRITE;
                resourceDesc.Flags |= D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL;
                if ((descriptor.Usage & TextureUsage.ShaderRead) == TextureUsage.None)
                {
                    resourceDesc.Flags |= D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE;
                }

                clearValue.DepthStencil.Depth = 1.0f;
            }
            else
            {
                state = D3D12_RESOURCE_STATE_RENDER_TARGET;
                resourceDesc.Flags |= D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET;
            }

            optimizedClearValue = &clearValue;
        }

        // If depth and either sampled or storage, set to typeless
        if (descriptor.Format.IsDepthFormat())
        {
            if ((descriptor.Usage & TextureUsage.ShaderRead) != 0
                || (descriptor.Usage & TextureUsage.ShaderWrite) != 0)
            {
                resourceDesc.Format = GetTypelessFormatFromDepthFormat(descriptor.Format);
                optimizedClearValue = null;
            }
        }

        D3D12_HEAP_PROPERTIES heapProps = default;
        heapProps.Type = D3D12_HEAP_TYPE_DEFAULT;

        HRESULT hr = device.NativeDevice->CreateCommittedResource(
            &heapProps,
            D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE,
            &resourceDesc,
            state,
            optimizedClearValue,
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
    protected override void OnDispose()
    {
        _handle.Dispose();
    }
}
