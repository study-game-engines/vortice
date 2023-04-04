// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace Alimer.Audio;

public abstract class AudioModule : AppModule
{
    private float _masterVolume = 1.0f;

    protected AudioModule()
        : base(400)
    {
    }

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = value;
            OnMasterVolumeChanged(value);
        }
    }

    protected abstract void OnMasterVolumeChanged(float volume);
}
