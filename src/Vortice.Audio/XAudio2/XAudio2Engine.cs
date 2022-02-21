// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.X3DAUDIO;
using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace Vortice.Audio.XAudio2;

internal unsafe class XAudio2Engine : AudioDevice
{
    private readonly AUDIO_STREAM_CATEGORY _category = AUDIO_STREAM_CATEGORY.AudioCategory_GameEffects;

    private readonly ComPtr<IXAudio2> _xaudio2;
    private IXAudio2MasteringVoice* _masterVoice;
    private readonly uint _masterChannelMask;
    private readonly byte[] _x3DAudio = new byte[X3DAUDIO_HANDLE_BYTESIZE];

    public XAudio2Engine()
    {
        ThrowIfFailed(XAudio2Create(_xaudio2.GetAddressOf()));

#if DEBUG
        //if (mEngineFlags & AudioEngine_Debug)
        {
            XAUDIO2_DEBUG_CONFIGURATION debug = new XAUDIO2_DEBUG_CONFIGURATION();
            debug.TraceMask = XAUDIO2_LOG_ERRORS | XAUDIO2_LOG_WARNINGS;
            debug.BreakMask = XAUDIO2_LOG_ERRORS;
            _xaudio2.Get()->SetDebugConfiguration(&debug, null);
            Debug.WriteLine("INFO: XAudio 2.9 debugging enabled");

        }
#endif

        //ThrowIfFailed(_xaudio2->RegisterForCallbacks(&mEngineCallback));

        IXAudio2MasteringVoice* newMasterVoice;
        HRESULT hr = _xaudio2.Get()->CreateMasteringVoice(&newMasterVoice,
            /*(wfx) ? wfx->nChannels : */XAUDIO2_DEFAULT_CHANNELS,
            /*(wfx) ? wfx->nSamplesPerSec :*/ XAUDIO2_DEFAULT_SAMPLERATE,
            0u, null, null, _category);
        ThrowIfFailed(hr);
        _masterVoice = newMasterVoice;

        uint channelMask;
        ThrowIfFailed(_masterVoice->GetChannelMask(&channelMask));

        XAUDIO2_VOICE_DETAILS details;
        _masterVoice->GetVoiceDetails(&details);

        _masterChannelMask = channelMask;

        //_masterVoice.Get()->SetVolume(1.0f);

        float speedOfSound = X3DAUDIO_SPEED_OF_SOUND;
        fixed (byte* _x3DAudioPtr = _x3DAudio)
        {
            X3DAudioInitialize(_masterChannelMask, speedOfSound, _x3DAudioPtr);
        }
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        _masterVoice->DestroyVoice();
        _masterVoice = default;
        _xaudio2.Dispose();
    }

    // <inheritdoc />
    public override AudioBackend BackendType => AudioBackend.XAudio2;
}
