// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi1_2.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Interop.Windows;
using Vortice.Mathematics;

namespace TerraFX.Interop.DirectX;

internal unsafe partial struct DXGI_PRESENT_PARAMETERS
{
    public uint DirtyRectsCount;

    public RECT* pDirtyRects;

    public RECT* pScrollRect;

    public Int2* pScrollOffset;
}
