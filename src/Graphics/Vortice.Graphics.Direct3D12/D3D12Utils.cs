// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Graphics.Utils;
using Microsoft.Toolkit.Diagnostics;
using Vortice.Direct3D12;
using System.Runtime.CompilerServices;

namespace Vortice.Graphics;

internal static class D3D12Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ResourceDimension ToD3D12(TextureDimension dimension)
    {
        return dimension switch
        {
            TextureDimension.Texture1D => ResourceDimension.Texture1D,
            TextureDimension.Texture2D => ResourceDimension.Texture2D,
            TextureDimension.Texture3D => ResourceDimension.Texture3D,
            TextureDimension.TextureCube => ResourceDimension.Texture2D,
            _ => ThrowHelper.ThrowArgumentException<ResourceDimension>("Invalid texture dimension"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CommandListType ToD3D12(CommandQueueType type)
    {
        return type switch
        {
            CommandQueueType.Compute => CommandListType.Compute,
            CommandQueueType.Copy => CommandListType.Copy,
            _ => CommandListType.Direct,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureDimension FromD3D12(ResourceDimension dimension)
    {
        return dimension switch
        {
            ResourceDimension.Texture1D => TextureDimension.Texture1D,
            ResourceDimension.Texture2D => TextureDimension.Texture2D,
            ResourceDimension.Texture3D => TextureDimension.Texture3D,
            _ => ThrowHelper.ThrowArgumentException<TextureDimension>("Invalid texture dimension"),
        };
    }

    public static TextureUsage ToTextureUsage(ResourceFlags flags)
    {
        TextureUsage usage = TextureUsage.None;

        if (!flags.HasFlag(ResourceFlags.DenyShaderResource))
            usage |= TextureUsage.ShaderRead;

        if (flags.HasFlag(ResourceFlags.AllowUnorderedAccess))
            usage |= TextureUsage.ShaderWrite;

        if (flags.HasFlag(ResourceFlags.AllowRenderTarget))
            usage |= TextureUsage.RenderTarget;

        if (flags.HasFlag(ResourceFlags.AllowDepthStencil))
            usage |= TextureUsage.RenderTarget;

        return usage;
    }


    public static TextureDescriptor FromD3D12(ResourceDescription d3dDesc)
    {
        TextureDimension dimension = FromD3D12(d3dDesc.Dimension);
        TextureUsage usage = ToTextureUsage(d3dDesc.Flags);
        TextureSampleCount sampleCount = FromD3D(d3dDesc.SampleDescription.Count);

        return new TextureDescriptor(dimension,
            FromDXGIFormat(d3dDesc.Format),
            (int)d3dDesc.Width,
            d3dDesc.Height,
            d3dDesc.DepthOrArraySize,
            d3dDesc.MipLevels,
            usage,
            sampleCount);
    }
}
