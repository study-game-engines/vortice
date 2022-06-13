// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Direct3D11;
using static Vortice.Graphics.D3DCommon.D3DUtils;

namespace Vortice.Graphics.D3D11;

internal static class D3D11Utils
{
    public static TextureUsage FromD3D11(in BindFlags flags)
    {
        TextureUsage usage = TextureUsage.None;
        if ((flags & BindFlags.ShaderResource) != BindFlags.None)
        {
            usage |= TextureUsage.ShaderRead;
        }

        if ((flags & BindFlags.UnorderedAccess) != BindFlags.None)
        {
            usage |= TextureUsage.ShaderWrite;
        }

        if ((flags & BindFlags.RenderTarget) != BindFlags.None)
        {
            usage |= TextureUsage.RenderTarget;
        }

        if ((flags & BindFlags.DepthStencil) != BindFlags.None)
        {
            usage |= TextureUsage.RenderTarget;
        }

        return usage;
    }

    public static TextureDescription FromD3D11(in Texture2DDescription description)
    {
        return new TextureDescription()
        {
            Dimension = TextureDimension.Texture2D,
            Format = FromDXGIFormat(description.Format),
            Width = description.Width,
            Height = description.Height,
            DepthOrArraySize = description.ArraySize,
            MipLevels = description.MipLevels,
            Usage = FromD3D11(description.BindFlags),
            SampleCount = FromD3DSampleCount(description.SampleDescription.Count)
        };
    }
}
