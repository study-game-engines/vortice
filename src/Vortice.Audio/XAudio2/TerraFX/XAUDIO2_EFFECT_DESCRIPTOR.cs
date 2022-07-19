// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/xaudio2.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe partial struct XAUDIO2_EFFECT_DESCRIPTOR
{
    public IUnknown* pEffect;
    public BOOL InitialState;
    [NativeTypeName("UINT32")]
    public uint OutputChannels;
}
