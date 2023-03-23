// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi1_2.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

internal partial struct DXGI_SWAP_CHAIN_FULLSCREEN_DESC
{
    public DXGI_RATIONAL RefreshRate;

    public DXGI_MODE_SCANLINE_ORDER ScanlineOrdering;

    public DXGI_MODE_SCALING Scaling;

    /// <include file='DXGI_SWAP_CHAIN_FULLSCREEN_DESC.xml' path='doc/member[@name="DXGI_SWAP_CHAIN_FULLSCREEN_DESC.Windowed"]/*' />
    public BOOL Windowed;
}
