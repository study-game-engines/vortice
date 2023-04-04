// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.AUDIO_STREAM_CATEGORY;
using static TerraFX.Interop.DirectX.XAUDIO2;
using static TerraFX.Interop.DirectX.X3DAUDIO;
using System.Runtime.InteropServices;

namespace Vortice.Audio.XAudio2;

internal unsafe class XAudio2Module : AudioModule
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    private readonly AUDIO_STREAM_CATEGORY _category = AudioCategory_GameEffects;

    private readonly ComPtr<IXAudio2> _xaudio2 = default;
    private XAudio2EngineCallback* _engineCallback;
    private IXAudio2MasteringVoice* _masterVoice;
    private IXAudio2SubmixVoice* _reverbVoice = default;

    private readonly uint _masterChannelMask;
    private readonly uint _masterChannels;
    private readonly uint _masterRate;
    private byte* _X3DAudio;

    public static bool EnableValidation { get; set; }

    public override string ApiName { get; }

    public override Version ApiVersion { get; }

    public static bool IsSupported() => s_isSupported.Value;

    public XAudio2Module()
    {
        ApiName = "XAudio2";
        ApiVersion = new Version(2, 9, 0);

        HRESULT hr = XAudio2Create(_xaudio2.GetAddressOf());
        ThrowIfFailed(hr);

#if DEBUG
        if (EnableValidation)
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

        XAudio2EngineCallback.Create(out _engineCallback);
        hr = _xaudio2.Get()->RegisterForCallbacks((IXAudio2EngineCallback*)_engineCallback);
        if (hr.FAILED)
        {
            _xaudio2.Dispose();
            return;
        }

        IXAudio2MasteringVoice* masterVoice;
        hr = _xaudio2.Get()->CreateMasteringVoice(
            &masterVoice,
            XAUDIO2_DEFAULT_CHANNELS,
            XAUDIO2_DEFAULT_SAMPLERATE,
            0u,
            null,
            null,
            _category
            );
        if (hr.FAILED)
        {
            _xaudio2.Dispose();
            return;
        }

        _masterVoice = masterVoice;

        uint dwChannelMask;
        hr = _masterVoice->GetChannelMask(&dwChannelMask);
        if (hr.FAILED)
        {
            _masterVoice->DestroyVoice();
            _masterVoice = default;
            _xaudio2.Dispose();
            return;
        }

        XAUDIO2_VOICE_DETAILS details;
        _masterVoice->GetVoiceDetails(&details);

        _masterChannelMask = dwChannelMask;
        _masterChannels = details.InputChannels;
        _masterRate = details.InputSampleRate;
        Debug.WriteLine($"Mastering voice has {_masterChannels} channels, {_masterRate} sample rate, {_masterChannelMask} channels");

        // Setup 3D audio
        _X3DAudio = (byte*)NativeMemory.Alloc(X3DAUDIO_HANDLE_BYTESIZE);
        hr = X3DAudioInitialize(_masterChannelMask, X3DAUDIO_SPEED_OF_SOUND, _X3DAudio);
        if (hr.FAILED)
        {
            if (_reverbVoice != null)
            {
                _reverbVoice->DestroyVoice();
                _reverbVoice = default;
            }

            _masterVoice->DestroyVoice();
            _masterVoice = default;
            //_reverbEffect.Reset();
            //_volumeLimiter.Reset();
            _xaudio2.Dispose();
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            if (_reverbVoice != null)
            {
                _reverbVoice->DestroyVoice();
                _reverbVoice = default;
            }

            _masterVoice->DestroyVoice();
            _masterVoice = default;
            _xaudio2.Dispose();
            XAudio2EngineCallback.Free(_engineCallback);
            NativeMemory.Free(_X3DAudio);
        }
    }

    /// <inheritdoc />
    protected override void OnMasterVolumeChanged(float volume)
    {
        Debug.Assert(volume >= -XAUDIO2_MAX_VOLUME_LEVEL && volume <= XAUDIO2_MAX_VOLUME_LEVEL);

        if (_masterVoice != null)
        {
            ThrowIfFailed(_masterVoice->SetVolume(volume));
        }
    }

    private static bool CheckIsSupported()
    {
        return OperatingSystem.IsWindows();
    }
}
