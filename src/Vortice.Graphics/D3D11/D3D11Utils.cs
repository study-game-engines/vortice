// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Graphics.D3D.Utils;
using Vortice.Direct3D11;

namespace Vortice.Graphics.D3D11;

internal static partial class D3D11Utils
{
    
    public static TextureUsage ToTextureUsage(BindFlags flags)
    {
        TextureUsage usage = TextureUsage.None;

        if (flags.HasFlag(BindFlags.ShaderResource))
            usage |= TextureUsage.ShaderRead;

        if (flags.HasFlag(BindFlags.UnorderedAccess))
            usage |= TextureUsage.ShaderWrite;

        if (flags.HasFlag(BindFlags.RenderTarget))
            usage |= TextureUsage.RenderTarget;

        if (flags.HasFlag(BindFlags.DepthStencil))
            usage |= TextureUsage.RenderTarget;

        return usage;
    }

    public static TextureDescriptor FromD3D11(Texture2DDescription d3d11Desc)
    {
        TextureUsage usage = ToTextureUsage(d3d11Desc.BindFlags);
        TextureSampleCount sampleCount = FromD3D(d3d11Desc.SampleDescription.Count);

        return TextureDescriptor.Texture2D(FromDXGIFormat(d3d11Desc.Format),
            d3d11Desc.Width,
            d3d11Desc.Height,
            d3d11Desc.MipLevels,
            d3d11Desc.ArraySize,
            usage,
            sampleCount);
    }
}
