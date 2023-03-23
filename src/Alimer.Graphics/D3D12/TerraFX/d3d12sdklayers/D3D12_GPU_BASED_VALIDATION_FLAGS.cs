// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from d3d12sdklayers.h in microsoft/DirectX-Headers tag v1.606.4
// Original source is Copyright © Microsoft. Licensed under the MIT license

namespace TerraFX.Interop.DirectX;

[Flags]
internal enum D3D12_GPU_BASED_VALIDATION_FLAGS
{
    D3D12_GPU_BASED_VALIDATION_FLAGS_NONE = 0,
    D3D12_GPU_BASED_VALIDATION_FLAGS_DISABLE_STATE_TRACKING = 0x1,
}
