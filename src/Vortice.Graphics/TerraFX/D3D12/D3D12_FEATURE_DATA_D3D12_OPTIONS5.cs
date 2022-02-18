// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d12.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

internal enum D3D12_RENDER_PASS_TIER
{
    D3D12_RENDER_PASS_TIER_0 = 0,
    D3D12_RENDER_PASS_TIER_1 = 1,
    D3D12_RENDER_PASS_TIER_2 = 2,
}

internal enum D3D12_RAYTRACING_TIER
{
    D3D12_RAYTRACING_TIER_NOT_SUPPORTED = 0,
    D3D12_RAYTRACING_TIER_1_0 = 10,
    D3D12_RAYTRACING_TIER_1_1 = 11,
}

internal readonly struct D3D12_FEATURE_DATA_D3D12_OPTIONS5
{
    public readonly BOOL SRVOnlyTiledResourceTier3;
    public readonly D3D12_RENDER_PASS_TIER RenderPassesTier;
    public readonly D3D12_RAYTRACING_TIER RaytracingTier;
}
