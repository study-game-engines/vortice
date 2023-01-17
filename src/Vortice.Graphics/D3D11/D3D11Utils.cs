// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Win32.Apis;
using static Vortice.Graphics.D3DCommon.D3DUtils;
using Win32.Graphics.Direct3D11;
using static Win32.Graphics.Direct3D11.Apis;
using Win32;
using Win32.Graphics.Direct3D;

namespace Vortice.Graphics.D3D11;

internal static unsafe class D3D11Utils
{
    // Check for SDK Layer support.
    public static bool SdkLayersAvailable()
    {
        HResult hr = D3D11CreateDevice(
            null,
            DriverType.Null,       // There is no need to create a real hardware device.
            IntPtr.Zero,
            CreateDeviceFlags.Debug,  // Check for the SDK layers.
            null,                    // Any feature level will do.
            0,
            D3D11_SDK_VERSION,
            null,                    // No need to keep the D3D device reference.
            null,                    // No need to know the feature level.
            null                     // No need to keep the D3D device context reference.
            );

        return hr.Success;
    }

    //public static TextureUsage FromD3D11(in BindFlags flags)
    //{
    //    TextureUsage usage = TextureUsage.None;
    //    if ((flags & BindFlags.ShaderResource) != 0u)
    //    {
    //        usage |= TextureUsage.ShaderRead;
    //    }

    //    if ((flags & BindFlags.UnorderedAccess) != 0u)
    //    {
    //        usage |= TextureUsage.ShaderWrite;
    //    }

    //    if ((flags & BindFlags.RenderTarget) != 0u)
    //    {
    //        usage |= TextureUsage.RenderTarget;
    //    }

    //    if ((flags & BindFlags.DepthStencil) != 0u)
    //    {
    //        usage |= TextureUsage.RenderTarget;
    //    }

    //    return usage;
    //}

    //public static TextureDescription FromD3D11(in Texture2DDescription description)
    //{
    //    return TextureDescription.Texture2D(
    //        description.Format.FromDxgiFormat(),
    //        (int)description.Width,
    //        (int)description.Height,
    //        (int)description.MipLevels,
    //        (int)description.ArraySize,
    //        FromD3D11(description.BindFlags),
    //        FromSampleCount(description.SampleDesc.Count),
    //        CpuAccessMode.None
    //    );
    //}

    public static unsafe TFeature CheckFeatureSupport<TFeature>(this ref ID3D11Device1 self, Win32.Graphics.Direct3D11.Feature feature)
        where TFeature : unmanaged
    {
        TFeature featureData = default;
        ThrowIfFailed(self.CheckFeatureSupport(feature, &featureData, sizeof(TFeature)));
        return featureData;
    }
}
