// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

//using static Vortice.Audio.OpenAL.OpenALNative;

namespace Vortice.Audio.OpenAL;

internal sealed class OpenALDeviceFactory : AudioDeviceFactory
{
    public static readonly OpenALDeviceFactory Instance = new OpenALDeviceFactory();

    // <inheritdoc />
    public override AudioBackend BackendType => AudioBackend.OpenAL;

    // <inheritdoc />
    public override int Priority => 10;

    public override AudioDevice CreateDevice() => new OpenALEngine();

    [ModuleInitializer]
    public static void Register()
    {
        AudioDevice.RegisterFactory(Instance);
    }
}
