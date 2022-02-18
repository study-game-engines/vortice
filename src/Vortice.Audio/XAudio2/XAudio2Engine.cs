// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.XAudio2;
using static Vortice.XAudio2.XAudio2;

namespace Vortice.Audio.XAudio2;

internal unsafe class XAudio2Engine : AudioDevice
{
    private readonly IXAudio2 _xaudio2;

    public XAudio2Engine()
    {
        _xaudio2 = XAudio2Create(ProcessorSpecifier.DefaultProcessor, registerCallback: true);
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        _xaudio2.Dispose();
    }

    // <inheritdoc />
    public override AudioBackend BackendType => AudioBackend.XAudio2;
}
