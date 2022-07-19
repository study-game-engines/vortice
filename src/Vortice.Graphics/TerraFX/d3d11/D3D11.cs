// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

internal static unsafe partial class D3D11
{
    [NativeTypeName("#define D3D11_SDK_VERSION ( 7 )")]
    public const int D3D11_SDK_VERSION = 7;

    [NativeTypeName("#define D3D11_REQ_TEXTURE1D_U_DIMENSION ( 16384 )")]
    public const int D3D11_REQ_TEXTURE1D_U_DIMENSION = (16384);

    [NativeTypeName("#define D3D11_REQ_TEXTURE2D_ARRAY_AXIS_DIMENSION ( 2048 )")]
    public const int D3D11_REQ_TEXTURE2D_ARRAY_AXIS_DIMENSION = (2048);

    [NativeTypeName("#define D3D11_REQ_TEXTURE2D_U_OR_V_DIMENSION ( 16384 )")]
    public const int D3D11_REQ_TEXTURE2D_U_OR_V_DIMENSION = (16384);

    [NativeTypeName("#define D3D11_REQ_TEXTURE3D_U_V_OR_W_DIMENSION ( 2048 )")]
    public const int D3D11_REQ_TEXTURE3D_U_V_OR_W_DIMENSION = (2048);

    [NativeTypeName("#define D3D11_REQ_TEXTURECUBE_DIMENSION ( 16384 )")]
    public const int D3D11_REQ_TEXTURECUBE_DIMENSION = (16384);

    [NativeTypeName("#define D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT ( 8 )")]
    public const int D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT = (8);

    [NativeTypeName("#define D3D11_REQ_IMMEDIATE_CONSTANT_BUFFER_ELEMENT_COUNT ( 4096 )")]
    public const int D3D11_REQ_IMMEDIATE_CONSTANT_BUFFER_ELEMENT_COUNT = (4096);

    [NativeTypeName("#define D3D11_REQ_MAXANISOTROPY ( 16 )")]
    public const int D3D11_REQ_MAXANISOTROPY = (16);

    [NativeTypeName("#define D3D11_REQ_MIP_LEVELS ( 15 )")]
    public const int D3D11_REQ_MIP_LEVELS = (15);

    public static uint D3D11CalcSubresource(uint MipSlice, uint ArraySlice, uint MipLevels)
    {
        return MipSlice + ArraySlice * MipLevels;
    }

    [DllImport("d3d11", ExactSpelling = true)]
    public static extern HRESULT D3D11CreateDevice(IDXGIAdapter1* pAdapter, D3D_DRIVER_TYPE DriverType, /*HMODULE*/void* Software, uint Flags, [NativeTypeName("const D3D_FEATURE_LEVEL *")] D3D_FEATURE_LEVEL* pFeatureLevels, uint FeatureLevels, uint SDKVersion, ID3D11Device** ppDevice, D3D_FEATURE_LEVEL* pFeatureLevel, ID3D11DeviceContext** ppImmediateContext);
}
