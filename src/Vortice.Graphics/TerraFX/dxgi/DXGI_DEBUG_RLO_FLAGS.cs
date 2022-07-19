// Copyright � Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/dxgidebug.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright � Microsoft. All rights reserved.

using System.Runtime.Versioning;

namespace TerraFX.Interop.DirectX;

[Flags]
[SupportedOSPlatform("windows8.0")]
internal enum DXGI_DEBUG_RLO_FLAGS
{
    DXGI_DEBUG_RLO_SUMMARY = 0x1,
    DXGI_DEBUG_RLO_DETAIL = 0x2,
    DXGI_DEBUG_RLO_IGNORE_INTERNAL = 0x4,
    DXGI_DEBUG_RLO_ALL = 0x7,
}
