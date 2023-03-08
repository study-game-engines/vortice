// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/xaudio2.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.InteropServices;

namespace TerraFX.Interop.DirectX;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal partial struct XAUDIO2_VOICE_DETAILS
{
    [NativeTypeName("UINT32")]
    public uint CreationFlags;
    [NativeTypeName("UINT32")]
    public uint ActiveFlags;
    [NativeTypeName("UINT32")]
    public uint InputChannels;
    [NativeTypeName("UINT32")]
    public uint InputSampleRate;
}
