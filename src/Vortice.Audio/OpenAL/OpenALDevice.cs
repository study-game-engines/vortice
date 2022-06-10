// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

//using static Vortice.Audio.OpenAL.OpenALNative;

namespace Vortice.Audio.OpenAL;

internal unsafe class OpenALDevice : AudioDevice
{
    //private readonly nint _device;

    public OpenALDevice()
        : base(AudioBackend.OpenAL)
    {
        //_device = alcOpenDevice(null);
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
    }
}
