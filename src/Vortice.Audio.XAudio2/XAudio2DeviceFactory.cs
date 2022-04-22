// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Audio.XAudio2;

internal sealed class XAudio2DeviceFactory : AudioDeviceFactory
{
    public static readonly XAudio2DeviceFactory Instance = new XAudio2DeviceFactory();  

    public XAudio2DeviceFactory()
    {
    }

    // <inheritdoc />
    public override AudioBackend BackendType => AudioBackend.XAudio2;

    // <inheritdoc />
    public override int Priority => 0;

    public override AudioDevice CreateDevice() => new XAudio2Device();

    [ModuleInitializer]
    public static void Register()
    {
        AudioDevice.RegisterFactory(Instance);
    }
}
