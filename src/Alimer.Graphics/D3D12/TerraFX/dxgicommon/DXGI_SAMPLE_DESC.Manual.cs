// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgicommon.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

namespace TerraFX.Interop.DirectX;

internal partial struct DXGI_SAMPLE_DESC
{
    public DXGI_SAMPLE_DESC(uint count, uint quality)
    {
        Count = count;
        Quality = quality;
    }
}
