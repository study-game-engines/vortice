// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d12.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.Versioning;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

internal static unsafe partial class DirectX
{
    [DllImport("dxgi", ExactSpelling = true)]
    //[SupportedOSPlatform("windows8.1")]
    public static extern HRESULT CreateDXGIFactory2(uint Flags, [NativeTypeName("const IID &")] Guid* riid, void** ppFactory);

    [DllImport("dxgi", ExactSpelling = true)]
    //[SupportedOSPlatform("windows8.1")]
    public static extern HRESULT DXGIGetDebugInterface1(uint Flags, [NativeTypeName("const IID &")] Guid* riid, void** pDebug);

    [DllImport("d3d12", ExactSpelling = true)]
    public static extern HRESULT D3D12GetDebugInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvDebug);

    [DllImport("d3d12", ExactSpelling = true)]
    public static extern HRESULT D3D12CreateDevice(IUnknown* pAdapter, D3D_FEATURE_LEVEL MinimumFeatureLevel, [NativeTypeName("const IID &")] Guid* riid, void** ppDevice);
}
