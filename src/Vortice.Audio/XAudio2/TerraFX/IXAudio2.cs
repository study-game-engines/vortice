// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/xaudio2.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.AUDIO_STREAM_CATEGORY;

namespace TerraFX.Interop.DirectX;

[Guid("2B02E3CF-2E0B-4EC3-BE45-1B2A3FE7210D")]
[NativeTypeName("struct IXAudio2 : IUnknown")]
[NativeInheritance("IUnknown")]
internal unsafe partial struct IXAudio2
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IXAudio2*, Guid*, void**, int>)(lpVtbl[0]))((IXAudio2*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IXAudio2*, uint>)(lpVtbl[1]))((IXAudio2*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IXAudio2*, uint>)(lpVtbl[2]))((IXAudio2*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(3)]
    public HRESULT RegisterForCallbacks(IXAudio2EngineCallback* pCallback)
    {
        return ((delegate* unmanaged<IXAudio2*, IXAudio2EngineCallback*, int>)(lpVtbl[3]))((IXAudio2*)Unsafe.AsPointer(ref this), pCallback);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(4)]
    public void UnregisterForCallbacks(IXAudio2EngineCallback* pCallback)
    {
        ((delegate* unmanaged<IXAudio2*, IXAudio2EngineCallback*, void>)(lpVtbl[4]))((IXAudio2*)Unsafe.AsPointer(ref this), pCallback);
    }
    
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[VtblIndex(5)]
    //public HRESULT CreateSourceVoice(IXAudio2SourceVoice** ppSourceVoice, [NativeTypeName("const WAVEFORMATEX *")] WAVEFORMATEX* pSourceFormat, [NativeTypeName("UINT32")] uint Flags = 0, float MaxFrequencyRatio = 2.0f, IXAudio2VoiceCallback* pCallback = null, [NativeTypeName("const XAUDIO2_VOICE_SENDS *")] XAUDIO2_VOICE_SENDS* pSendList = null, [NativeTypeName("const XAUDIO2_EFFECT_CHAIN *")] XAUDIO2_EFFECT_CHAIN* pEffectChain = null)
    //{
    //    return ((delegate* unmanaged<IXAudio2*, IXAudio2SourceVoice**, WAVEFORMATEX*, uint, float, IXAudio2VoiceCallback*, XAUDIO2_VOICE_SENDS*, XAUDIO2_EFFECT_CHAIN*, int>)(lpVtbl[5]))((IXAudio2*)Unsafe.AsPointer(ref this), ppSourceVoice, pSourceFormat, Flags, MaxFrequencyRatio, pCallback, pSendList, pEffectChain);
    //}
    //
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[VtblIndex(6)]
    //public HRESULT CreateSubmixVoice(IXAudio2SubmixVoice** ppSubmixVoice, [NativeTypeName("UINT32")] uint InputChannels, [NativeTypeName("UINT32")] uint InputSampleRate, [NativeTypeName("UINT32")] uint Flags = 0, [NativeTypeName("UINT32")] uint ProcessingStage = 0, [NativeTypeName("const XAUDIO2_VOICE_SENDS *")] XAUDIO2_VOICE_SENDS* pSendList = null, [NativeTypeName("const XAUDIO2_EFFECT_CHAIN *")] XAUDIO2_EFFECT_CHAIN* pEffectChain = null)
    //{
    //    return ((delegate* unmanaged<IXAudio2*, IXAudio2SubmixVoice**, uint, uint, uint, uint, XAUDIO2_VOICE_SENDS*, XAUDIO2_EFFECT_CHAIN*, int>)(lpVtbl[6]))((IXAudio2*)Unsafe.AsPointer(ref this), ppSubmixVoice, InputChannels, InputSampleRate, Flags, ProcessingStage, pSendList, pEffectChain);
    //}
    //
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(7)]
    public HRESULT CreateMasteringVoice(IXAudio2MasteringVoice** ppMasteringVoice, [NativeTypeName("UINT32")] uint InputChannels = 0, [NativeTypeName("UINT32")] uint InputSampleRate = 0, [NativeTypeName("UINT32")] uint Flags = 0, [NativeTypeName("LPCWSTR")] ushort* szDeviceId = null, [NativeTypeName("const XAUDIO2_EFFECT_CHAIN *")] XAUDIO2_EFFECT_CHAIN* pEffectChain = null, AUDIO_STREAM_CATEGORY StreamCategory = AudioCategory_GameEffects)
    {
        return ((delegate* unmanaged<IXAudio2*, IXAudio2MasteringVoice**, uint, uint, uint, ushort*, XAUDIO2_EFFECT_CHAIN*, AUDIO_STREAM_CATEGORY, int>)(lpVtbl[7]))((IXAudio2*)Unsafe.AsPointer(ref this), ppMasteringVoice, InputChannels, InputSampleRate, Flags, szDeviceId, pEffectChain, StreamCategory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(8)]
    public HRESULT StartEngine()
    {
        return ((delegate* unmanaged<IXAudio2*, int>)(lpVtbl[8]))((IXAudio2*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(9)]
    public void StopEngine()
    {
        ((delegate* unmanaged<IXAudio2*, void>)(lpVtbl[9]))((IXAudio2*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(10)]
    public HRESULT CommitChanges([NativeTypeName("UINT32")] uint OperationSet)
    {
        return ((delegate* unmanaged<IXAudio2*, uint, int>)(lpVtbl[10]))((IXAudio2*)Unsafe.AsPointer(ref this), OperationSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(11)]
    public void GetPerformanceData(XAUDIO2_PERFORMANCE_DATA* pPerfData)
    {
        ((delegate* unmanaged<IXAudio2*, XAUDIO2_PERFORMANCE_DATA*, void>)(lpVtbl[11]))((IXAudio2*)Unsafe.AsPointer(ref this), pPerfData);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(12)]
    public void SetDebugConfiguration([NativeTypeName("const XAUDIO2_DEBUG_CONFIGURATION *")] XAUDIO2_DEBUG_CONFIGURATION* pDebugConfiguration, void* pReserved = null)
    {
        ((delegate* unmanaged<IXAudio2*, XAUDIO2_DEBUG_CONFIGURATION*, void*, void>)(lpVtbl[12]))((IXAudio2*)Unsafe.AsPointer(ref this), pDebugConfiguration, pReserved);
    }
}
