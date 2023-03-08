// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Alimer.Platform.SDL;

namespace Alimer.Platform;

public static class SDLPlatformExtensions
{
    public static ModuleList UseSDL(this ModuleList builder)
    {
        builder.Register<SDLPlatform>();
        return builder;
    }
}
