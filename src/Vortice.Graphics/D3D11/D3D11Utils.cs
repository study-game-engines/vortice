// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.D3D11;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.D3D_DRIVER_TYPE;
using static TerraFX.Interop.DirectX.D3D11_CREATE_DEVICE_FLAG;
using static TerraFX.Interop.DirectX.D3D11_BIND_FLAG;
using TerraFX.Interop.DirectX;
using static Vortice.Graphics.D3DCommon.D3DUtils;

namespace Vortice.Graphics.D3D11;

internal static unsafe class D3D11Utils
{
    // Check for SDK Layer support.
    public static bool SdkLayersAvailable()
    {
        HRESULT hr = D3D11CreateDevice(
            null,
            D3D_DRIVER_TYPE_NULL,       // There is no need to create a real hardware device.
            HMODULE.NULL,
            (uint)D3D11_CREATE_DEVICE_DEBUG,  // Check for the SDK layers.
            null,                    // Any feature level will do.
            0,
            D3D11_SDK_VERSION,
            null,                    // No need to keep the D3D device reference.
            null,                    // No need to know the feature level.
            null                     // No need to keep the D3D device context reference.
            );

        return hr.SUCCEEDED;
    }

    /// <summary>
    /// Checks the feature support of a given type for a given device.
    /// </summary>
    /// <typeparam name="TFeature">The type of feature support data to retrieve.</typeparam>
    /// <param name="device">The target <see cref="ID3D11Device1"/> to use to check features for.</param>
    /// <param name="feature">The type of features to check.</param>
    /// <returns>A <see typeparamref="TFeature"/> value with the features data.</returns>
    public static unsafe TFeature CheckFeatureSupport<TFeature>(this ref ID3D11Device1 device, D3D11_FEATURE feature)
        where TFeature : unmanaged
    {
        TFeature featureData = default;

        ThrowIfFailed(device.CheckFeatureSupport(feature, &featureData, (uint)sizeof(TFeature)));

        return featureData;
    }

    public static TextureUsage FromD3D11(in D3D11_BIND_FLAG flags)
    {
        TextureUsage usage = TextureUsage.None;
        if ((flags & D3D11_BIND_SHADER_RESOURCE) != 0u)
        {
            usage |= TextureUsage.ShaderRead;
        }

        if ((flags & D3D11_BIND_UNORDERED_ACCESS) != 0u)
        {
            usage |= TextureUsage.ShaderWrite;
        }

        if ((flags & D3D11_BIND_RENDER_TARGET) != 0u)
        {
            usage |= TextureUsage.RenderTarget;
        }

        if ((flags & D3D11_BIND_DEPTH_STENCIL) != 0u)
        {
            usage |= TextureUsage.RenderTarget;
        }

        return usage;
    }

    public static TextureDescription FromD3D11(in D3D11_TEXTURE2D_DESC description)
    {
        return new TextureDescription()
        {
            Dimension = TextureDimension.Texture2D,
            //Format = FromDXGIFormat(description.Format),
            Width = (int)description.Width,
            Height = (int)description.Height,
            DepthOrArraySize = (int)description.ArraySize,
            MipLevels = (int)description.MipLevels,
            Usage = FromD3D11((D3D11_BIND_FLAG)description.BindFlags),
            SampleCount = FromSampleCount(description.SampleDesc.Count)
        };
    }
}
