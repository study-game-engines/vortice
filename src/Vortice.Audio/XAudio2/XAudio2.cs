// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/xaudio2.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

//[SupportedOSPlatform("windows8.0")]
internal enum AUDIO_STREAM_CATEGORY
{
    AudioCategory_Other = 0,
    AudioCategory_ForegroundOnlyMedia = 1,
    AudioCategory_Communications = 3,
    AudioCategory_Alerts = 4,
    AudioCategory_SoundEffects = 5,
    AudioCategory_GameEffects = 6,
    AudioCategory_GameMedia = 7,
    AudioCategory_GameChat = 8,
    AudioCategory_Speech = 9,
    AudioCategory_Movie = 10,
    AudioCategory_Media = 11,
    AudioCategory_FarFieldSpeech = 12,
    AudioCategory_UniformSpeech = 13,
    AudioCategory_VoiceTyping = 14,
}

internal unsafe partial struct IXAudio2EngineCallback : IXAudio2EngineCallback.Interface
{
    public void** lpVtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnProcessingPassStart()
    {
        ((delegate* unmanaged<IXAudio2EngineCallback*, void>)(lpVtbl[0]))((IXAudio2EngineCallback*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnProcessingPassEnd()
    {
        ((delegate* unmanaged<IXAudio2EngineCallback*, void>)(lpVtbl[1]))((IXAudio2EngineCallback*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnCriticalError(HRESULT Error)
    {
        ((delegate* unmanaged<IXAudio2EngineCallback*, HRESULT, void>)(lpVtbl[2]))((IXAudio2EngineCallback*)Unsafe.AsPointer(ref this), Error);
    }

    public interface Interface
    {
        void OnProcessingPassStart();

        void OnProcessingPassEnd();

        void OnCriticalError(HRESULT Error);
    }

    public partial struct Vtbl<TSelf>
        where TSelf : unmanaged, Interface
    {
        public delegate* unmanaged<TSelf*, void> OnProcessingPassStart;

        public delegate* unmanaged<TSelf*, void> OnProcessingPassEnd;

        public delegate* unmanaged<TSelf*, HRESULT, void> OnCriticalError;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal partial struct XAUDIO2_PERFORMANCE_DATA
{
    public ulong AudioCyclesSinceLastQuery;
    public ulong TotalCyclesSinceLastQuery;
    public uint MinimumCyclesPerQuantum;
    public uint MaximumCyclesPerQuantum;
    public uint MemoryUsageInBytes;
    public uint CurrentLatencyInSamples;
    public uint GlitchesSinceEngineStarted;
    public uint ActiveSourceVoiceCount;
    public uint TotalSourceVoiceCount;
    public uint ActiveSubmixVoiceCount;
    public uint ActiveResamplerCount;
    public uint ActiveMatrixMixCount;
    public uint ActiveXmaSourceVoices;
    public uint ActiveXmaStreams;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal partial struct XAUDIO2_DEBUG_CONFIGURATION
{
    public uint TraceMask;
    public uint BreakMask;
    public BOOL LogThreadID;
    public BOOL LogFileline;
    public BOOL LogFunctionName;
    public BOOL LogTiming;
}

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

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe partial struct XAUDIO2_EFFECT_DESCRIPTOR
{
    public IUnknown* pEffect;
    public BOOL InitialState;
    [NativeTypeName("UINT32")]
    public uint OutputChannels;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe partial struct XAUDIO2_EFFECT_CHAIN
{
    /// <include file='XAUDIO2_EFFECT_CHAIN.xml' path='doc/member[@name="XAUDIO2_EFFECT_CHAIN.EffectCount"]/*' />
    [NativeTypeName("UINT32")]
    public uint EffectCount;

    /// <include file='XAUDIO2_EFFECT_CHAIN.xml' path='doc/member[@name="XAUDIO2_EFFECT_CHAIN.pEffectDescriptors"]/*' />
    public XAUDIO2_EFFECT_DESCRIPTOR* pEffectDescriptors;
}

[NativeTypeName("struct IXAudio2MasteringVoice : IXAudio2Voice")]
[NativeInheritance("IXAudio2Voice")]
internal unsafe partial struct IXAudio2MasteringVoice 
{
    public void** lpVtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public void GetVoiceDetails(XAUDIO2_VOICE_DETAILS* pVoiceDetails)
    {
        ((delegate* unmanaged<IXAudio2MasteringVoice*, XAUDIO2_VOICE_DETAILS*, void>)(lpVtbl[0]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pVoiceDetails);
    }

#if TODO
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    public HRESULT SetOutputVoices([NativeTypeName("const XAUDIO2_VOICE_SENDS *")] XAUDIO2_VOICE_SENDS* pSendList)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, XAUDIO2_VOICE_SENDS*, int>)(lpVtbl[1]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pSendList);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    public HRESULT SetEffectChain([NativeTypeName("const XAUDIO2_EFFECT_CHAIN *")] XAUDIO2_EFFECT_CHAIN* pEffectChain)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, XAUDIO2_EFFECT_CHAIN*, int>)(lpVtbl[2]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pEffectChain);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(3)]
    public HRESULT EnableEffect([NativeTypeName("UINT32")] uint EffectIndex, [NativeTypeName("UINT32")] uint OperationSet = 0)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, uint, uint, int>)(lpVtbl[3]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), EffectIndex, OperationSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(4)]
    public HRESULT DisableEffect([NativeTypeName("UINT32")] uint EffectIndex, [NativeTypeName("UINT32")] uint OperationSet = 0)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, uint, uint, int>)(lpVtbl[4]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), EffectIndex, OperationSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(5)]
    public void GetEffectState([NativeTypeName("UINT32")] uint EffectIndex, BOOL* pEnabled)
    {
        ((delegate* unmanaged<IXAudio2MasteringVoice*, uint, BOOL*, void>)(lpVtbl[5]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), EffectIndex, pEnabled);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(6)]
    public HRESULT SetEffectParameters([NativeTypeName("UINT32")] uint EffectIndex, [NativeTypeName("const void *")] void* pParameters, [NativeTypeName("UINT32")] uint ParametersByteSize, [NativeTypeName("UINT32")] uint OperationSet = 0)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, uint, void*, uint, uint, int>)(lpVtbl[6]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), EffectIndex, pParameters, ParametersByteSize, OperationSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(7)]
    public HRESULT GetEffectParameters([NativeTypeName("UINT32")] uint EffectIndex, void* pParameters, [NativeTypeName("UINT32")] uint ParametersByteSize)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, uint, void*, uint, int>)(lpVtbl[7]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), EffectIndex, pParameters, ParametersByteSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(8)]
    public HRESULT SetFilterParameters([NativeTypeName("const XAUDIO2_FILTER_PARAMETERS *")] XAUDIO2_FILTER_PARAMETERS* pParameters, [NativeTypeName("UINT32")] uint OperationSet = 0)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, XAUDIO2_FILTER_PARAMETERS*, uint, int>)(lpVtbl[8]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pParameters, OperationSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(9)]
    public void GetFilterParameters(XAUDIO2_FILTER_PARAMETERS* pParameters)
    {
        ((delegate* unmanaged<IXAudio2MasteringVoice*, XAUDIO2_FILTER_PARAMETERS*, void>)(lpVtbl[9]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pParameters);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(10)]
    public HRESULT SetOutputFilterParameters(IXAudio2Voice* pDestinationVoice, [NativeTypeName("const XAUDIO2_FILTER_PARAMETERS *")] XAUDIO2_FILTER_PARAMETERS* pParameters, [NativeTypeName("UINT32")] uint OperationSet = 0)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, IXAudio2Voice*, XAUDIO2_FILTER_PARAMETERS*, uint, int>)(lpVtbl[10]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pDestinationVoice, pParameters, OperationSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(11)]
    public void GetOutputFilterParameters(IXAudio2Voice* pDestinationVoice, XAUDIO2_FILTER_PARAMETERS* pParameters)
    {
        ((delegate* unmanaged<IXAudio2MasteringVoice*, IXAudio2Voice*, XAUDIO2_FILTER_PARAMETERS*, void>)(lpVtbl[11]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pDestinationVoice, pParameters);
    } 
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(12)]
    public HRESULT SetVolume(float Volume, [NativeTypeName("UINT32")] uint OperationSet = 0)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, float, uint, int>)(lpVtbl[12]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), Volume, OperationSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(13)]
    public void GetVolume(float* pVolume)
    {
        ((delegate* unmanaged<IXAudio2MasteringVoice*, float*, void>)(lpVtbl[13]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pVolume);
    }

    /// <inheritdoc cref="IXAudio2Voice.SetChannelVolumes" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(14)]
    public HRESULT SetChannelVolumes([NativeTypeName("UINT32")] uint Channels, [NativeTypeName("const float *")] float* pVolumes, [NativeTypeName("UINT32")] uint OperationSet = 0)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, uint, float*, uint, int>)(lpVtbl[14]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), Channels, pVolumes, OperationSet);
    }

    /// <inheritdoc cref="IXAudio2Voice.GetChannelVolumes" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(15)]
    public void GetChannelVolumes([NativeTypeName("UINT32")] uint Channels, float* pVolumes)
    {
        ((delegate* unmanaged<IXAudio2MasteringVoice*, uint, float*, void>)(lpVtbl[15]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), Channels, pVolumes);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[VtblIndex(16)]
    //public HRESULT SetOutputMatrix(IXAudio2Voice* pDestinationVoice, [NativeTypeName("UINT32")] uint SourceChannels, [NativeTypeName("UINT32")] uint DestinationChannels, [NativeTypeName("const float *")] float* pLevelMatrix, [NativeTypeName("UINT32")] uint OperationSet = 0)
    //{
    //    return ((delegate* unmanaged<IXAudio2MasteringVoice*, IXAudio2Voice*, uint, uint, float*, uint, int>)(lpVtbl[16]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pDestinationVoice, SourceChannels, DestinationChannels, pLevelMatrix, OperationSet);
    //}
    //
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[VtblIndex(17)]
    //public void GetOutputMatrix(IXAudio2Voice* pDestinationVoice, [NativeTypeName("UINT32")] uint SourceChannels, [NativeTypeName("UINT32")] uint DestinationChannels, float* pLevelMatrix)
    //{
    //    ((delegate* unmanaged<IXAudio2MasteringVoice*, IXAudio2Voice*, uint, uint, float*, void>)(lpVtbl[17]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pDestinationVoice, SourceChannels, DestinationChannels, pLevelMatrix);
    //}

    /// <inheritdoc cref="IXAudio2Voice.DestroyVoice" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(18)]
    public void DestroyVoice()
    {
        ((delegate* unmanaged<IXAudio2MasteringVoice*, void>)(lpVtbl[18]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(19)]
    public HRESULT GetChannelMask([NativeTypeName("DWORD *")] uint* pChannelmask)
    {
        return ((delegate* unmanaged<IXAudio2MasteringVoice*, uint*, int>)(lpVtbl[19]))((IXAudio2MasteringVoice*)Unsafe.AsPointer(ref this), pChannelmask);
    }
}

internal unsafe partial struct IXAudio2
{
    public void** lpVtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IXAudio2*, Guid*, void**, int>)(lpVtbl[0]))((IXAudio2*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IXAudio2*, uint>)(lpVtbl[1]))((IXAudio2*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
    {
        return ((delegate* unmanaged<IXAudio2*, uint>)(lpVtbl[2]))((IXAudio2*)Unsafe.AsPointer(ref this));
    }

    public HRESULT RegisterForCallbacks(IXAudio2EngineCallback* pCallback)
    {
        return ((delegate* unmanaged<IXAudio2*, IXAudio2EngineCallback*, int>)(lpVtbl[3]))((IXAudio2*)Unsafe.AsPointer(ref this), pCallback);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnregisterForCallbacks(IXAudio2EngineCallback* pCallback)
    {
        ((delegate* unmanaged<IXAudio2*, IXAudio2EngineCallback*, void>)(lpVtbl[4]))((IXAudio2*)Unsafe.AsPointer(ref this), pCallback);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public HRESULT CreateSourceVoice(IXAudio2SourceVoice** ppSourceVoice, [NativeTypeName("const WAVEFORMATEX *")] WAVEFORMATEX* pSourceFormat, [NativeTypeName("UINT32")] uint Flags = 0, float MaxFrequencyRatio = 2.0f, IXAudio2VoiceCallback* pCallback = null, [NativeTypeName("const XAUDIO2_VOICE_SENDS *")] XAUDIO2_VOICE_SENDS* pSendList = null, [NativeTypeName("const XAUDIO2_EFFECT_CHAIN *")] XAUDIO2_EFFECT_CHAIN* pEffectChain = null)
    //{
    //    return ((delegate* unmanaged<IXAudio2*, IXAudio2SourceVoice**, WAVEFORMATEX*, uint, float, IXAudio2VoiceCallback*, XAUDIO2_VOICE_SENDS*, XAUDIO2_EFFECT_CHAIN*, int>)(lpVtbl[5]))((IXAudio2*)Unsafe.AsPointer(ref this), ppSourceVoice, pSourceFormat, Flags, MaxFrequencyRatio, pCallback, pSendList, pEffectChain);
    //}
    //
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public HRESULT CreateSubmixVoice(IXAudio2SubmixVoice** ppSubmixVoice, [NativeTypeName("UINT32")] uint InputChannels, [NativeTypeName("UINT32")] uint InputSampleRate, [NativeTypeName("UINT32")] uint Flags = 0, [NativeTypeName("UINT32")] uint ProcessingStage = 0, [NativeTypeName("const XAUDIO2_VOICE_SENDS *")] XAUDIO2_VOICE_SENDS* pSendList = null, [NativeTypeName("const XAUDIO2_EFFECT_CHAIN *")] XAUDIO2_EFFECT_CHAIN* pEffectChain = null)
    //{
    //    return ((delegate* unmanaged<IXAudio2*, IXAudio2SubmixVoice**, uint, uint, uint, uint, XAUDIO2_VOICE_SENDS*, XAUDIO2_EFFECT_CHAIN*, int>)(lpVtbl[6]))((IXAudio2*)Unsafe.AsPointer(ref this), ppSubmixVoice, InputChannels, InputSampleRate, Flags, ProcessingStage, pSendList, pEffectChain);
    //}
    //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateMasteringVoice(IXAudio2MasteringVoice** ppMasteringVoice, [NativeTypeName("UINT32")] uint InputChannels = 0, [NativeTypeName("UINT32")] uint InputSampleRate = 0, [NativeTypeName("UINT32")] uint Flags = 0, [NativeTypeName("LPCWSTR")] ushort* szDeviceId = null, [NativeTypeName("const XAUDIO2_EFFECT_CHAIN *")] XAUDIO2_EFFECT_CHAIN* pEffectChain = null, AUDIO_STREAM_CATEGORY StreamCategory = AUDIO_STREAM_CATEGORY.AudioCategory_GameEffects)
    {
        return ((delegate* unmanaged<IXAudio2*, IXAudio2MasteringVoice**, uint, uint, uint, ushort*, XAUDIO2_EFFECT_CHAIN*, AUDIO_STREAM_CATEGORY, int>)(lpVtbl[7]))((IXAudio2*)Unsafe.AsPointer(ref this), ppMasteringVoice, InputChannels, InputSampleRate, Flags, szDeviceId, pEffectChain, StreamCategory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT StartEngine()
    {
        return ((delegate* unmanaged<IXAudio2*, int>)(lpVtbl[8]))((IXAudio2*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void StopEngine()
    {
        ((delegate* unmanaged<IXAudio2*, void>)(lpVtbl[9]))((IXAudio2*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CommitChanges(uint OperationSet)
    {
        return ((delegate* unmanaged<IXAudio2*, uint, int>)(lpVtbl[10]))((IXAudio2*)Unsafe.AsPointer(ref this), OperationSet);
    }

    /// <include file='IXAudio2.xml' path='doc/member[@name="IXAudio2.GetPerformanceData"]/*' />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetPerformanceData(XAUDIO2_PERFORMANCE_DATA* pPerfData)
    {
        ((delegate* unmanaged<IXAudio2*, XAUDIO2_PERFORMANCE_DATA*, void>)(lpVtbl[11]))((IXAudio2*)Unsafe.AsPointer(ref this), pPerfData);
    }

    /// <include file='IXAudio2.xml' path='doc/member[@name="IXAudio2.SetDebugConfiguration"]/*' />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDebugConfiguration(XAUDIO2_DEBUG_CONFIGURATION* pDebugConfiguration, void* pReserved = null)
    {
        ((delegate* unmanaged<IXAudio2*, XAUDIO2_DEBUG_CONFIGURATION*, void*, void>)(lpVtbl[12]))((IXAudio2*)Unsafe.AsPointer(ref this), pDebugConfiguration, pReserved);
    }
}

internal static unsafe partial class DirectX
{

    public const int XAUDIO2_DEFAULT_PROCESSOR = 0x00000001;

    public const int XAUDIO2_LOG_ERRORS = 0x0001;

    public const int XAUDIO2_LOG_WARNINGS = 0x0002;
    public const int XAUDIO2_LOG_INFO = 0x0004;

    [NativeTypeName("#define XAUDIO2_DEFAULT_CHANNELS 0")]
    public const int XAUDIO2_DEFAULT_CHANNELS = 0;

    [NativeTypeName("#define XAUDIO2_DEFAULT_SAMPLERATE 0")]
    public const int XAUDIO2_DEFAULT_SAMPLERATE = 0;

    [NativeTypeName("#define XAUDIO2_DEBUG_ENGINE 0x0001")]
    public const int XAUDIO2_DEBUG_ENGINE = 0x0001;


    public static float XAudio2DecibelsToAmplitudeRatio(float Decibels)
    {
        return MathF.Pow(10.0f, Decibels / 20.0f);
    }

    public static float XAudio2AmplitudeRatioToDecibels(float Volume)
    {
        if (Volume == 0)
        {
            return -3.402823466e+38f;
        }

        return 20.0f * MathF.Log10(Volume);
    }

    public static float XAudio2SemitonesToFrequencyRatio(float Semitones)
    {
        return MathF.Pow(2.0f, Semitones / 12.0f);
    }

    public static float XAudio2FrequencyRatioToSemitones(float FrequencyRatio)
    {
        return 39.86313713864835f * MathF.Log10(FrequencyRatio);
    }

    public static float XAudio2CutoffFrequencyToRadians(float CutoffFrequency, uint SampleRate)
    {
        if ((uint)(CutoffFrequency * 6.0f) >= SampleRate)
        {
            return 1.0f;
        }

        return 2.0f * MathF.Sin((float)3.14159265358979323846 * CutoffFrequency / SampleRate);
    }

    public static float XAudio2RadiansToCutoffFrequency(float Radians, float SampleRate)
    {
        return SampleRate * MathF.Asin(Radians / 2.0f) / (float)3.14159265358979323846;
    }

    public static float XAudio2CutoffFrequencyToOnePoleCoefficient(float CutoffFrequency, uint SampleRate)
    {
        if ((uint)CutoffFrequency >= SampleRate)
        {
            return 1.0f;
        }

        return (1.0f - MathF.Pow(1.0f - 2.0f * CutoffFrequency / SampleRate, 2.0f));
    }

    [DllImport("XAudio2_9", ExactSpelling = true)]
    public static extern HRESULT XAudio2Create(IXAudio2** ppXAudio2, uint Flags = 0, uint XAudio2Processor = XAUDIO2_DEFAULT_PROCESSOR);
}

internal static unsafe partial class X3DAUDIO
{
    [NativeTypeName("#define X3DAUDIO_HANDLE_BYTESIZE 20")]
    public const int X3DAUDIO_HANDLE_BYTESIZE = 20;

    [NativeTypeName("#define X3DAUDIO_PI 3.141592654f")]
    public const float X3DAUDIO_PI = 3.141592654f;

    [NativeTypeName("#define X3DAUDIO_2PI 6.283185307f")]
    public const float X3DAUDIO_2PI = 6.283185307f;

    [NativeTypeName("#define X3DAUDIO_SPEED_OF_SOUND 343.5f")]
    public const float X3DAUDIO_SPEED_OF_SOUND = 343.5f;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_MATRIX 0x00000001")]
    public const int X3DAUDIO_CALCULATE_MATRIX = 0x00000001;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_DELAY 0x00000002")]
    public const int X3DAUDIO_CALCULATE_DELAY = 0x00000002;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_LPF_DIRECT 0x00000004")]
    public const int X3DAUDIO_CALCULATE_LPF_DIRECT = 0x00000004;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_LPF_REVERB 0x00000008")]
    public const int X3DAUDIO_CALCULATE_LPF_REVERB = 0x00000008;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_REVERB 0x00000010")]
    public const int X3DAUDIO_CALCULATE_REVERB = 0x00000010;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_DOPPLER 0x00000020")]
    public const int X3DAUDIO_CALCULATE_DOPPLER = 0x00000020;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_EMITTER_ANGLE 0x00000040")]
    public const int X3DAUDIO_CALCULATE_EMITTER_ANGLE = 0x00000040;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_ZEROCENTER 0x00010000")]
    public const int X3DAUDIO_CALCULATE_ZEROCENTER = 0x00010000;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_REDIRECT_TO_LFE 0x00020000")]
    public const int X3DAUDIO_CALCULATE_REDIRECT_TO_LFE = 0x00020000;

    [DllImport("X3DAudio1_7", ExactSpelling = true)]
    public static extern HRESULT X3DAudioInitialize([NativeTypeName("UINT32")] uint SpeakerChannelMask, [NativeTypeName("FLOAT32")] float SpeedOfSound, [NativeTypeName("X3DAUDIO_HANDLE")] byte* Instance);

    //[DllImport("X3DAudio1_7", ExactSpelling = true)]
    //public static extern void X3DAudioCalculate([NativeTypeName("const X3DAUDIO_HANDLE")] byte* Instance, [NativeTypeName("const X3DAUDIO_LISTENER *")] X3DAUDIO_LISTENER* pListener, [NativeTypeName("const X3DAUDIO_EMITTER *")] X3DAUDIO_EMITTER* pEmitter, [NativeTypeName("UINT32")] uint Flags, X3DAUDIO_DSP_SETTINGS* pDSPSettings);
}
