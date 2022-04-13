// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using Vortice.Multimedia;
using Vortice.XAudio2;
using static Vortice.XAudio2.XAudio2;

namespace Vortice.Audio.XAudio2;

internal sealed class XAudio2Device : AudioDevice
{
    private readonly AudioStreamCategory _category = AudioStreamCategory.GameEffects;

    private readonly IXAudio2 _xaudio2;
    private IXAudio2MasteringVoice _masterVoice;
    private readonly Speakers _masterChannelMask;
    private readonly int _masterChannels;
    private readonly int _masterRate;
    private readonly X3DAudio _x3DAudio;

    public XAudio2Device()
    {
        _xaudio2 = XAudio2Create(ProcessorSpecifier.DefaultProcessor, true);

#if DEBUG
        //if (mEngineFlags & AudioEngine_Debug)
        {
            DebugConfiguration debug = new()
            {
                TraceMask = LogType.Errors | LogType.Warnings,
                BreakMask = LogType.Errors
            };
            _xaudio2.SetDebugConfiguration(debug);
            Debug.WriteLine("INFO: XAudio 2.9 debugging enabled");

        }
#endif

        _masterVoice = _xaudio2.CreateMasteringVoice(
            DefaultChannels,
            DefaultSampleRate,
            _category);

        _masterChannelMask = (Speakers)_masterVoice.ChannelMask;
        VoiceDetails details = _masterVoice.VoiceDetails;

        _masterChannels = details.InputChannels;
        _masterRate = details.InputSampleRate;

        Debug.WriteLine($"Mastering voice has {_masterChannels} channels, {_masterRate} sample rate, {_masterChannelMask} channels");

        //_masterVoice.Get()->SetVolume(1.0f);

        _x3DAudio = new X3DAudio(_masterChannelMask);
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        _masterVoice.DestroyVoice();
        _xaudio2.Dispose();
    }

    // <inheritdoc />
    public override AudioBackend BackendType => AudioBackend.XAudio2;
}
