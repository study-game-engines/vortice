// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using TerraFX.Interop;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.XAUDIO2;

namespace Vortice.Audio;

public sealed unsafe class XAudio2Device : AudioDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    //private readonly AudioStreamCategory _category = AudioStreamCategory.GameEffects;

    private readonly ComPtr<IXAudio2> _xaudio2;
    //private IXAudio2MasteringVoice _masterVoice;
    //private readonly Speakers _masterChannelMask;
    //private readonly int _masterChannels;
    //private readonly int _masterRate;
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

        //_masterVoice = _xaudio2.CreateMasteringVoice(
        //    DefaultChannels,
        //    DefaultSampleRate,
        //    _category);
        //
        //_masterChannelMask = (Speakers)_masterVoice.ChannelMask;
        //VoiceDetails details = _masterVoice.VoiceDetails;
        //
        //_masterChannels = details.InputChannels;
        //_masterRate = details.InputSampleRate;
        //
        //Debug.WriteLine($"Mastering voice has {_masterChannels} channels, {_masterRate} sample rate, {_masterChannelMask} channels");

        //_masterVoice.Get()->SetVolume(1.0f);

        //_x3DAudio = new X3DAudio(_masterChannelMask);
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        //_masterVoice.DestroyVoice();
        _xaudio2.Dispose();
    }

    // <inheritdoc />
    public override AudioBackend BackendType => AudioBackend.XAudio2;

    private static bool CheckIsSupported()
    {
        return OperatingSystem.IsWindows();
    }
}
