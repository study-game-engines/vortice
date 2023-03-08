// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Alimer.Audio.OpenAL.OpenALNative;

namespace Alimer.Audio.OpenAL;

public static class OpenALModuleExtensions
{
    public static ModuleList UseOpenAL(this ModuleList builder)
    {
        builder.Register<OpenALModule>();
        return builder;
    }
}

internal class OpenALModule : AudioModule
{
    private readonly nint _device;

    public override string ApiName { get; }

    public override Version ApiVersion { get; }

    public unsafe OpenALModule()
    {
        ApiName = "OpenAL";
        ApiVersion = new Version(2, 9, 0);

        _device = alcOpenDevice(null);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="OpenALModule" /> class.
    /// </summary>
    ~OpenALModule() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            alcCloseDevice(_device);
        }
    }

    /// <inheritdoc />
    protected override void OnMasterVolumeChanged(float volume)
    {
        //Debug.Assert(volume >= -XAUDIO2_MAX_VOLUME_LEVEL && volume <= XAUDIO2_MAX_VOLUME_LEVEL);
    }
}
