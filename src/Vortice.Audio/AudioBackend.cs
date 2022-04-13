// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Audio;

public enum AudioBackend : byte
{
    Null = 0,
    XAudio2,
    OpenAL,

    Count
}
