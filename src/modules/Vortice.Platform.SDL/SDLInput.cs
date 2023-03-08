// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Input;
using static SDL2.SDL;

namespace Vortice.Platform.SDL;

internal unsafe class SDLInput : InputManager
{
    private readonly SDLPlatform _platform;

    public SDLInput(SDLPlatform platform)
    {
        _platform = platform;
    }
}
