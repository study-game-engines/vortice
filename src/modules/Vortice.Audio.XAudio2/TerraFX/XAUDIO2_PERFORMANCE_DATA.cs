// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/xaudio2.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.InteropServices;

namespace TerraFX.Interop.DirectX;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal partial struct XAUDIO2_PERFORMANCE_DATA
{
    [NativeTypeName("UINT64")]
    public ulong AudioCyclesSinceLastQuery;
    [NativeTypeName("UINT64")]
    public ulong TotalCyclesSinceLastQuery;
    [NativeTypeName("UINT32")]
    public uint MinimumCyclesPerQuantum;
    [NativeTypeName("UINT32")]
    public uint MaximumCyclesPerQuantum;
    [NativeTypeName("UINT32")]
    public uint MemoryUsageInBytes;
    [NativeTypeName("UINT32")]
    public uint CurrentLatencyInSamples;
    [NativeTypeName("UINT32")]
    public uint GlitchesSinceEngineStarted;
    [NativeTypeName("UINT32")]
    public uint ActiveSourceVoiceCount;
    [NativeTypeName("UINT32")]
    public uint TotalSourceVoiceCount;
    [NativeTypeName("UINT32")]
    public uint ActiveSubmixVoiceCount;
    [NativeTypeName("UINT32")]
    public uint ActiveResamplerCount;
    [NativeTypeName("UINT32")]
    public uint ActiveMatrixMixCount;
    [NativeTypeName("UINT32")]
    public uint ActiveXmaSourceVoices;
    [NativeTypeName("UINT32")]
    public uint ActiveXmaStreams;
}
