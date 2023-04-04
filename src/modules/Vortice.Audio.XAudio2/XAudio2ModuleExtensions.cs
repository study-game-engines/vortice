// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Audio.XAudio2;

namespace Vortice.Audio;

public static class XAudio2ModuleExtensions
{
#if DEBUG
    public static ModuleList UseXAudio2(this ModuleList builder, bool enableValidation = true)
#else
    public static ModuleList UseXAudio2(this ModuleList builder, bool enableValidation = false)
#endif
    {
        if (XAudio2Module.IsSupported())
        {
            XAudio2Module.EnableValidation = enableValidation;

            builder.Register<XAudio2Module>();
        }

        return builder;
    }
}
