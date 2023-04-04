// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Platform.SDL;

namespace Vortice.Platform;

public static class SDLPlatformExtensions
{
    public static ModuleList UseSDL(this ModuleList builder)
    {
        builder.Register<SDLPlatform>();
        return builder;
    }
}
