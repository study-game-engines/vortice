// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Toolkit.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_DIMENSION;
using static TerraFX.Interop.DirectX.D3D12_COMMAND_LIST_TYPE;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_FLAGS;
using static Vortice.Graphics.D3DUtilities;

namespace Vortice.Graphics.D3D12;

internal static class D3D12Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static D3D12_RESOURCE_DIMENSION ToD3D12(TextureDimension dimension)
    {
        return dimension switch
        {
            TextureDimension.Texture1D => D3D12_RESOURCE_DIMENSION_TEXTURE1D,
            TextureDimension.Texture2D => D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            TextureDimension.Texture3D => D3D12_RESOURCE_DIMENSION_TEXTURE3D,
            TextureDimension.TextureCube => D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            _ => ThrowHelper.ThrowArgumentException<D3D12_RESOURCE_DIMENSION>("Invalid texture dimension"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static D3D12_COMMAND_LIST_TYPE ToD3D12(CommandQueueType type)
    {
        return type switch
        {
            CommandQueueType.Compute => D3D12_COMMAND_LIST_TYPE_COMPUTE,
            CommandQueueType.Copy => D3D12_COMMAND_LIST_TYPE_COPY,
            _ => D3D12_COMMAND_LIST_TYPE_DIRECT,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureDimension FromD3D12(D3D12_RESOURCE_DIMENSION dimension)
    {
        return dimension switch
        {
            D3D12_RESOURCE_DIMENSION_TEXTURE1D => TextureDimension.Texture1D,
            D3D12_RESOURCE_DIMENSION_TEXTURE2D => TextureDimension.Texture2D,
            D3D12_RESOURCE_DIMENSION_TEXTURE3D => TextureDimension.Texture3D,
            _ => ThrowHelper.ThrowArgumentException<TextureDimension>("Invalid texture dimension"),
        };
    }

    public static TextureUsage ToTextureUsage(D3D12_RESOURCE_FLAGS flags)
    {
        TextureUsage usage = TextureUsage.None;

        if (!flags.HasFlag(D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE))
            usage |= TextureUsage.ShaderRead;

        if (flags.HasFlag(D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS))
            usage |= TextureUsage.ShaderWrite;

        if (flags.HasFlag(D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET))
            usage |= TextureUsage.RenderTarget;

        if (flags.HasFlag(D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL))
            usage |= TextureUsage.RenderTarget;

        return usage;
    }


    public static TextureDescriptor FromD3D12(D3D12_RESOURCE_DESC d3dDesc)
    {
        TextureDimension dimension = FromD3D12(d3dDesc.Dimension);
        TextureUsage usage = ToTextureUsage(d3dDesc.Flags);
        TextureSampleCount sampleCount = FromD3D(d3dDesc.SampleDesc.Count);

        return new TextureDescriptor(dimension,
            FromDXGIFormat(d3dDesc.Format),
            (int)d3dDesc.Width,
            (int)d3dDesc.Height,
            (int)d3dDesc.DepthOrArraySize,
            d3dDesc.MipLevels,
            usage,
            sampleCount);
    }
}
