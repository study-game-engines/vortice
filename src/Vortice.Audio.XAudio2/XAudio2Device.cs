// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using TerraFX.Interop;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.XAUDIO2;
using static TerraFX.Interop.DirectX.X3DAUDIO;
using static TerraFX.Interop.Windows.AUDIO_STREAM_CATEGORY;

namespace Vortice.Audio;

public sealed unsafe class XAudio2Device : AudioDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    private readonly AUDIO_STREAM_CATEGORY _category = AudioCategory_GameEffects;

    private readonly ComPtr<IXAudio2> _xaudio2;
    private IXAudio2MasteringVoice* _masterVoice;
    private readonly uint _masterChannelMask;
    private readonly uint _masterChannels;
    private readonly uint _masterRate;
    private X3DAUDIO_HANDLE _x3DAudio;
    //private readonly X3DAudio _x3DAudio;

    public static bool IsSupported() => s_isSupported.Value;

    public XAudio2Device()
    {
        ThrowIfFailed(XAudio2Create(_xaudio2.GetAddressOf(), 0u, XAUDIO2_DEFAULT_PROCESSOR));

#if DEBUG
        //if (mEngineFlags & AudioEngine_Debug)
        {
            XAUDIO2_DEBUG_CONFIGURATION debug = new()
            {
                TraceMask = XAUDIO2_LOG_ERRORS | XAUDIO2_LOG_WARNINGS,
                BreakMask = XAUDIO2_LOG_ERRORS
            };
            _xaudio2.Get()->SetDebugConfiguration(&debug);
            Debug.WriteLine("INFO: XAudio 2.9 debugging enabled");

        }
#endif

        _masterVoice = CreateMasteringVoice();

        uint dwChannelMask;
        HRESULT hr = _masterVoice->GetChannelMask(&dwChannelMask);
        if (hr.FAILED)
        {
            _masterVoice->DestroyVoice();
            _masterVoice = null;
            _xaudio2.Dispose();
            return;
        }

        XAUDIO2_VOICE_DETAILS details;
        _masterVoice->GetVoiceDetails(&details);

        _masterChannelMask = dwChannelMask;
        _masterChannels = details.InputChannels;
        _masterRate = details.InputSampleRate;
        Debug.WriteLine($"Mastering voice has {_masterChannels} channels, {_masterRate} sample rate, {_masterChannelMask} channels");

        ThrowIfFailed(_masterVoice->SetVolume(1.0f));

        // Setup 3D audio
        float SPEEDOFSOUND = X3DAUDIO_SPEED_OF_SOUND;

        hr = X3DAudioInitialize(_masterChannelMask, SPEEDOFSOUND, out _x3DAudio);
        if (hr.FAILED)
        {
            //SAFE_DESTROY_VOICE(mReverbVoice);
            _masterVoice->DestroyVoice();
            _masterVoice = null;
            //mReverbEffect.Reset();
            //mVolumeLimiter.Reset();
            _xaudio2.Dispose();
            return;
        }

        IXAudio2MasteringVoice* CreateMasteringVoice()
        {
            IXAudio2MasteringVoice* voice = default;

            HRESULT hr = _xaudio2.Get()->CreateMasteringVoice(
                &voice,
                XAUDIO2_DEFAULT_CHANNELS,
                XAUDIO2_DEFAULT_SAMPLERATE,
                0u,
                null,
                null,
                _category);

            ThrowIfFailed(hr);

            return voice;
        }
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        _x3DAudio = default;
        _masterVoice->DestroyVoice();
        _masterVoice = default;
        _xaudio2.Dispose();
    }

    // <inheritdoc />
    public override AudioBackend BackendType => AudioBackend.XAudio2;

    private static bool CheckIsSupported()
    {
        return OperatingSystem.IsWindows();
    }
}
