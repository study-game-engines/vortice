// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.AUDIO_STREAM_CATEGORY;

namespace Vortice.Audio.XAudio2;

internal unsafe class XAudio2Device : AudioDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    private readonly AUDIO_STREAM_CATEGORY _category = AudioCategory_GameEffects;

    private readonly ComPtr<IXAudio2> _xaudio2 = default;
    private XAudio2EngineCallback* _engineCallback;
    //private IXAudio2MasteringVoice _masterVoice;
    private readonly int _masterChannelMask;
    private readonly int _masterChannels;
    private readonly int _masterRate;
    //private readonly X3DAudio _x3DAudio;

    public static bool IsSupported() => s_isSupported.Value;

    public XAudio2Device()
        : base(AudioBackend.XAudio2)
    {
        ThrowIfFailed(XAudio2Create(_xaudio2.GetAddressOf()));

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

        XAudio2EngineCallback.Create(out _engineCallback);
        HRESULT hr = _xaudio2.Get()->RegisterForCallbacks((IXAudio2EngineCallback*)_engineCallback);
        if (hr.FAILED)
        {
            _xaudio2.Dispose();
            return;
        }

        //_masterVoice = _xaudio2.CreateMasteringVoice(
        //    DefaultChannels,
        //    DefaultSampleRate,
        //    _category,
        //    string.Empty
        //    );
        //
        //_masterChannelMask = _masterVoice.ChannelMask;
        //VoiceDetails details = _masterVoice.VoiceDetails;
        //
        //_masterChannels = details.InputChannels;
        //_masterRate = details.InputSampleRate;
        //Debug.WriteLine($"Mastering voice has {_masterChannels} channels, {_masterRate} sample rate, {_masterChannelMask} channels");
        //
        //_masterVoice.SetVolume(1.0f);
        //
        //// Setup 3D audio
        //_x3DAudio = new(_masterChannelMask);
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        //_masterVoice.DestroyVoice();
        _xaudio2.Dispose();
    }

    private static bool CheckIsSupported()
    {
        return OperatingSystem.IsWindows();
    }
}
