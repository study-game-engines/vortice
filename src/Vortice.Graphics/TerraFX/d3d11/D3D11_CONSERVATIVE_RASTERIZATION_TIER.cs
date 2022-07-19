// Copyright � Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright � Microsoft. All rights reserved.

using System.Runtime.Versioning;

namespace TerraFX.Interop.DirectX;

[SupportedOSPlatform("windows10.0")]
internal enum D3D11_CONSERVATIVE_RASTERIZATION_TIER
{
    D3D11_CONSERVATIVE_RASTERIZATION_NOT_SUPPORTED = 0,
    D3D11_CONSERVATIVE_RASTERIZATION_TIER_1 = 1,
    D3D11_CONSERVATIVE_RASTERIZATION_TIER_2 = 2,
    D3D11_CONSERVATIVE_RASTERIZATION_TIER_3 = 3,
}
