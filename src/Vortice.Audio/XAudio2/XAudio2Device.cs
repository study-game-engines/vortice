// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Vortice.Multimedia;
using Vortice.XAudio2;
using static Vortice.XAudio2.XAudio2;

namespace Vortice.Audio.XAudio2;

internal class XAudio2Device : AudioDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    private readonly AudioStreamCategory _category = AudioStreamCategory.GameEffects;

    private readonly IXAudio2 _xaudio2;
    private IXAudio2MasteringVoice _masterVoice;
    private readonly int _masterChannelMask;
    private readonly int _masterChannels;
    private readonly int _masterRate;
    private readonly X3DAudio _x3DAudio;

    public static bool IsSupported() => s_isSupported.Value;

    public XAudio2Device()
        : base(AudioBackend.XAudio2)
    {
        _xaudio2 = XAudio2Create(ProcessorSpecifier.DefaultProcessor);

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
            _category,
            string.Empty
            );

        _masterChannelMask = _masterVoice.ChannelMask;
        VoiceDetails details = _masterVoice.VoiceDetails;

        _masterChannels = details.InputChannels;
        _masterRate = details.InputSampleRate;
        Debug.WriteLine($"Mastering voice has {_masterChannels} channels, {_masterRate} sample rate, {_masterChannelMask} channels");

        _masterVoice.SetVolume(1.0f);

        // Setup 3D audio
        _x3DAudio = new(_masterChannelMask);
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        _masterVoice.DestroyVoice();
        _xaudio2.Dispose();
    }

    private static bool CheckIsSupported()
    {
#if WINDOWS_UWP
        return true;
#elif NETSTANDARD2_0
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
        return OperatingSystem.IsWindows();
#endif
    }
}
