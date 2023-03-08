// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/xaudio2.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.XAUDIO2_FILTER_TYPE;

namespace TerraFX.Interop.DirectX;

internal static unsafe partial class DirectX
{
    [NativeTypeName("#define Processor1 0x00000001")]
    public const int Processor1 = 0x00000001;

    [NativeTypeName("#define Processor2 0x00000002")]
    public const int Processor2 = 0x00000002;

    [NativeTypeName("#define Processor3 0x00000004")]
    public const int Processor3 = 0x00000004;

    [NativeTypeName("#define Processor4 0x00000008")]
    public const int Processor4 = 0x00000008;

    [NativeTypeName("#define Processor5 0x00000010")]
    public const int Processor5 = 0x00000010;

    [NativeTypeName("#define Processor6 0x00000020")]
    public const int Processor6 = 0x00000020;

    [NativeTypeName("#define Processor7 0x00000040")]
    public const int Processor7 = 0x00000040;

    [NativeTypeName("#define Processor8 0x00000080")]
    public const int Processor8 = 0x00000080;

    [NativeTypeName("#define Processor9 0x00000100")]
    public const int Processor9 = 0x00000100;

    [NativeTypeName("#define Processor10 0x00000200")]
    public const int Processor10 = 0x00000200;

    [NativeTypeName("#define Processor11 0x00000400")]
    public const int Processor11 = 0x00000400;

    [NativeTypeName("#define Processor12 0x00000800")]
    public const int Processor12 = 0x00000800;

    [NativeTypeName("#define Processor13 0x00001000")]
    public const int Processor13 = 0x00001000;

    [NativeTypeName("#define Processor14 0x00002000")]
    public const int Processor14 = 0x00002000;

    [NativeTypeName("#define Processor15 0x00004000")]
    public const int Processor15 = 0x00004000;

    [NativeTypeName("#define Processor16 0x00008000")]
    public const int Processor16 = 0x00008000;

    [NativeTypeName("#define Processor17 0x00010000")]
    public const int Processor17 = 0x00010000;

    [NativeTypeName("#define Processor18 0x00020000")]
    public const int Processor18 = 0x00020000;

    [NativeTypeName("#define Processor19 0x00040000")]
    public const int Processor19 = 0x00040000;

    [NativeTypeName("#define Processor20 0x00080000")]
    public const int Processor20 = 0x00080000;

    [NativeTypeName("#define Processor21 0x00100000")]
    public const int Processor21 = 0x00100000;

    [NativeTypeName("#define Processor22 0x00200000")]
    public const int Processor22 = 0x00200000;

    [NativeTypeName("#define Processor23 0x00400000")]
    public const int Processor23 = 0x00400000;

    [NativeTypeName("#define Processor24 0x00800000")]
    public const int Processor24 = 0x00800000;

    [NativeTypeName("#define Processor25 0x01000000")]
    public const int Processor25 = 0x01000000;

    [NativeTypeName("#define Processor26 0x02000000")]
    public const int Processor26 = 0x02000000;

    [NativeTypeName("#define Processor27 0x04000000")]
    public const int Processor27 = 0x04000000;

    [NativeTypeName("#define Processor28 0x08000000")]
    public const int Processor28 = 0x08000000;

    [NativeTypeName("#define Processor29 0x10000000")]
    public const int Processor29 = 0x10000000;

    [NativeTypeName("#define Processor30 0x20000000")]
    public const int Processor30 = 0x20000000;

    [NativeTypeName("#define Processor31 0x40000000")]
    public const int Processor31 = 0x40000000;

    [NativeTypeName("#define Processor32 0x80000000")]
    public const uint Processor32 = 0x80000000;

    [NativeTypeName("#define XAUDIO2_MAX_BUFFER_BYTES 0x80000000")]
    public const uint XAUDIO2_MAX_BUFFER_BYTES = 0x80000000;

    [NativeTypeName("#define XAUDIO2_MAX_QUEUED_BUFFERS 64")]
    public const int XAUDIO2_MAX_QUEUED_BUFFERS = 64;

    [NativeTypeName("#define XAUDIO2_MAX_BUFFERS_SYSTEM 2")]
    public const int XAUDIO2_MAX_BUFFERS_SYSTEM = 2;

    [NativeTypeName("#define XAUDIO2_MAX_AUDIO_CHANNELS 64")]
    public const int XAUDIO2_MAX_AUDIO_CHANNELS = 64;

    [NativeTypeName("#define XAUDIO2_MIN_SAMPLE_RATE 1000")]
    public const int XAUDIO2_MIN_SAMPLE_RATE = 1000;

    [NativeTypeName("#define XAUDIO2_MAX_SAMPLE_RATE 200000")]
    public const int XAUDIO2_MAX_SAMPLE_RATE = 200000;

    [NativeTypeName("#define XAUDIO2_MAX_VOLUME_LEVEL 16777216.0f")]
    public const float XAUDIO2_MAX_VOLUME_LEVEL = 16777216.0f;

    [NativeTypeName("#define XAUDIO2_MIN_FREQ_RATIO (1/1024.0f)")]
    public const float XAUDIO2_MIN_FREQ_RATIO = (1 / 1024.0f);

    [NativeTypeName("#define XAUDIO2_MAX_FREQ_RATIO 1024.0f")]
    public const float XAUDIO2_MAX_FREQ_RATIO = 1024.0f;

    [NativeTypeName("#define XAUDIO2_DEFAULT_FREQ_RATIO 2.0f")]
    public const float XAUDIO2_DEFAULT_FREQ_RATIO = 2.0f;

    [NativeTypeName("#define XAUDIO2_MAX_FILTER_ONEOVERQ 1.5f")]
    public const float XAUDIO2_MAX_FILTER_ONEOVERQ = 1.5f;

    [NativeTypeName("#define XAUDIO2_MAX_FILTER_FREQUENCY 1.0f")]
    public const float XAUDIO2_MAX_FILTER_FREQUENCY = 1.0f;

    [NativeTypeName("#define XAUDIO2_MAX_LOOP_COUNT 254")]
    public const int XAUDIO2_MAX_LOOP_COUNT = 254;

    [NativeTypeName("#define XAUDIO2_MAX_INSTANCES 8")]
    public const int XAUDIO2_MAX_INSTANCES = 8;

    [NativeTypeName("#define XAUDIO2_MAX_RATIO_TIMES_RATE_XMA_MONO 600000")]
    public const int XAUDIO2_MAX_RATIO_TIMES_RATE_XMA_MONO = 600000;

    [NativeTypeName("#define XAUDIO2_MAX_RATIO_TIMES_RATE_XMA_MULTICHANNEL 300000")]
    public const int XAUDIO2_MAX_RATIO_TIMES_RATE_XMA_MULTICHANNEL = 300000;

    [NativeTypeName("#define XAUDIO2_COMMIT_NOW 0")]
    public const int XAUDIO2_COMMIT_NOW = 0;

    [NativeTypeName("#define XAUDIO2_COMMIT_ALL 0")]
    public const int XAUDIO2_COMMIT_ALL = 0;

    [NativeTypeName("#define XAUDIO2_INVALID_OPSET (UINT32)(-1)")]
    public const uint XAUDIO2_INVALID_OPSET = unchecked((uint)(-1));

    [NativeTypeName("#define XAUDIO2_NO_LOOP_REGION 0")]
    public const int XAUDIO2_NO_LOOP_REGION = 0;

    [NativeTypeName("#define XAUDIO2_LOOP_INFINITE 255")]
    public const int XAUDIO2_LOOP_INFINITE = 255;

    [NativeTypeName("#define XAUDIO2_DEFAULT_CHANNELS 0")]
    public const int XAUDIO2_DEFAULT_CHANNELS = 0;

    [NativeTypeName("#define XAUDIO2_DEFAULT_SAMPLERATE 0")]
    public const int XAUDIO2_DEFAULT_SAMPLERATE = 0;

    [NativeTypeName("#define XAUDIO2_DEBUG_ENGINE 0x0001")]
    public const int XAUDIO2_DEBUG_ENGINE = 0x0001;

    [NativeTypeName("#define XAUDIO2_VOICE_NOPITCH 0x0002")]
    public const int XAUDIO2_VOICE_NOPITCH = 0x0002;

    [NativeTypeName("#define XAUDIO2_VOICE_NOSRC 0x0004")]
    public const int XAUDIO2_VOICE_NOSRC = 0x0004;

    [NativeTypeName("#define XAUDIO2_VOICE_USEFILTER 0x0008")]
    public const int XAUDIO2_VOICE_USEFILTER = 0x0008;

    [NativeTypeName("#define XAUDIO2_PLAY_TAILS 0x0020")]
    public const int XAUDIO2_PLAY_TAILS = 0x0020;

    [NativeTypeName("#define XAUDIO2_END_OF_STREAM 0x0040")]
    public const int XAUDIO2_END_OF_STREAM = 0x0040;

    [NativeTypeName("#define XAUDIO2_SEND_USEFILTER 0x0080")]
    public const int XAUDIO2_SEND_USEFILTER = 0x0080;

    [NativeTypeName("#define XAUDIO2_VOICE_NOSAMPLESPLAYED 0x0100")]
    public const int XAUDIO2_VOICE_NOSAMPLESPLAYED = 0x0100;

    [NativeTypeName("#define XAUDIO2_STOP_ENGINE_WHEN_IDLE 0x2000")]
    public const int XAUDIO2_STOP_ENGINE_WHEN_IDLE = 0x2000;

    [NativeTypeName("#define XAUDIO2_1024_QUANTUM 0x8000")]
    public const int XAUDIO2_1024_QUANTUM = 0x8000;

    [NativeTypeName("#define XAUDIO2_NO_VIRTUAL_AUDIO_CLIENT 0x10000")]
    public const int XAUDIO2_NO_VIRTUAL_AUDIO_CLIENT = 0x10000;

    [NativeTypeName("#define XAUDIO2_DEFAULT_FILTER_TYPE LowPassFilter")]
    public const XAUDIO2_FILTER_TYPE XAUDIO2_DEFAULT_FILTER_TYPE = LowPassFilter;

    [NativeTypeName("#define XAUDIO2_DEFAULT_FILTER_FREQUENCY XAUDIO2_MAX_FILTER_FREQUENCY")]
    public const float XAUDIO2_DEFAULT_FILTER_FREQUENCY = 1.0f;

    [NativeTypeName("#define XAUDIO2_DEFAULT_FILTER_ONEOVERQ 1.0f")]
    public const float XAUDIO2_DEFAULT_FILTER_ONEOVERQ = 1.0f;

    [NativeTypeName("#define XAUDIO2_QUANTUM_NUMERATOR 1")]
    public const int XAUDIO2_QUANTUM_NUMERATOR = 1;

    [NativeTypeName("#define XAUDIO2_QUANTUM_DENOMINATOR 100")]
    public const int XAUDIO2_QUANTUM_DENOMINATOR = 100;

    [NativeTypeName("#define XAUDIO2_QUANTUM_MS (1000.0f * XAUDIO2_QUANTUM_NUMERATOR / XAUDIO2_QUANTUM_DENOMINATOR)")]
    public const float XAUDIO2_QUANTUM_MS = (1000.0f * 1 / 100);

    [NativeTypeName("#define XAUDIO2_E_INVALID_CALL ((HRESULT)0x88960001)")]
    public const int XAUDIO2_E_INVALID_CALL = unchecked((int)(0x88960001));

    [NativeTypeName("#define XAUDIO2_E_XMA_DECODER_ERROR ((HRESULT)0x88960002)")]
    public const int XAUDIO2_E_XMA_DECODER_ERROR = unchecked((int)(0x88960002));

    [NativeTypeName("#define XAUDIO2_E_XAPO_CREATION_FAILED ((HRESULT)0x88960003)")]
    public const int XAUDIO2_E_XAPO_CREATION_FAILED = unchecked((int)(0x88960003));

    [NativeTypeName("#define XAUDIO2_E_DEVICE_INVALIDATED ((HRESULT)0x88960004)")]
    public const int XAUDIO2_E_DEVICE_INVALIDATED = unchecked((int)(0x88960004));

    [NativeTypeName("#define XAUDIO2_ANY_PROCESSOR 0xffffffff")]
    public const uint XAUDIO2_ANY_PROCESSOR = 0xffffffff;

    [NativeTypeName("#define XAUDIO2_USE_DEFAULT_PROCESSOR 0x00000000")]
    public const int XAUDIO2_USE_DEFAULT_PROCESSOR = 0x00000000;

    [NativeTypeName("#define XAUDIO2_DEFAULT_PROCESSOR Processor1")]
    public const int XAUDIO2_DEFAULT_PROCESSOR = 0x00000001;

    [NativeTypeName("#define XAUDIO2_LOG_ERRORS 0x0001")]
    public const int XAUDIO2_LOG_ERRORS = 0x0001;

    [NativeTypeName("#define XAUDIO2_LOG_WARNINGS 0x0002")]
    public const int XAUDIO2_LOG_WARNINGS = 0x0002;

    [NativeTypeName("#define XAUDIO2_LOG_INFO 0x0004")]
    public const int XAUDIO2_LOG_INFO = 0x0004;

    [NativeTypeName("#define XAUDIO2_LOG_DETAIL 0x0008")]
    public const int XAUDIO2_LOG_DETAIL = 0x0008;

    [NativeTypeName("#define XAUDIO2_LOG_API_CALLS 0x0010")]
    public const int XAUDIO2_LOG_API_CALLS = 0x0010;

    [NativeTypeName("#define XAUDIO2_LOG_FUNC_CALLS 0x0020")]
    public const int XAUDIO2_LOG_FUNC_CALLS = 0x0020;

    [NativeTypeName("#define XAUDIO2_LOG_TIMING 0x0040")]
    public const int XAUDIO2_LOG_TIMING = 0x0040;

    [NativeTypeName("#define XAUDIO2_LOG_LOCKS 0x0080")]
    public const int XAUDIO2_LOG_LOCKS = 0x0080;

    [NativeTypeName("#define XAUDIO2_LOG_MEMORY 0x0100")]
    public const int XAUDIO2_LOG_MEMORY = 0x0100;

    [NativeTypeName("#define XAUDIO2_LOG_STREAMING 0x1000")]
    public const int XAUDIO2_LOG_STREAMING = 0x1000;

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

    public static float XAudio2CutoffFrequencyToRadians(float CutoffFrequency, [NativeTypeName("UINT32")] uint SampleRate)
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

    public static float XAudio2CutoffFrequencyToOnePoleCoefficient(float CutoffFrequency, [NativeTypeName("UINT32")] uint SampleRate)
    {
        if ((uint)CutoffFrequency >= SampleRate)
        {
            return 1.0f;
        }

        return (1.0f - MathF.Pow(1.0f - 2.0f * CutoffFrequency / SampleRate, 2.0f));
    }

    [DllImport("xaudio2_9", ExactSpelling = true)]
    public static extern HRESULT XAudio2CreateWithVersionInfo(IXAudio2** ppXAudio2, [NativeTypeName("UINT32")] uint Flags = 0, [NativeTypeName("XAUDIO2_PROCESSOR")] uint XAudio2Processor = 0x00000001, [NativeTypeName("DWORD")] uint ntddiVersion = 0x0A00000B);

    [DllImport("XAudio2_9", ExactSpelling = true)]
    public static extern HRESULT XAudio2Create([NativeTypeName("IXAudio2 **")] IXAudio2** ppXAudio2, [NativeTypeName("UINT32")] uint Flags = 0, [NativeTypeName("XAUDIO2_PROCESSOR")] uint XAudio2Processor = XAUDIO2_DEFAULT_PROCESSOR);

    [DllImport("XAudio2_9", ExactSpelling = true)]
    public static extern HRESULT X3DAudioInitialize([NativeTypeName("UINT32")] uint SpeakerChannelMask, [NativeTypeName("FLOAT32")] float SpeedOfSound, [NativeTypeName("X3DAUDIO_HANDLE")] byte* Instance);

    /// <include file='DirectXxml' path='doc/member[@name="DirectX.X3DAudioCalculate"]/*' />
    [DllImport("XAudio2_9", ExactSpelling = true)]
    public static extern void X3DAudioCalculate([NativeTypeName("const X3DAUDIO_HANDLE")] byte* Instance, [NativeTypeName("const X3DAUDIO_LISTENER *")] X3DAUDIO_LISTENER* pListener, [NativeTypeName("const X3DAUDIO_EMITTER *")] X3DAUDIO_EMITTER* pEmitter, [NativeTypeName("UINT32")] uint Flags, X3DAUDIO_DSP_SETTINGS* pDSPSettings);
}
