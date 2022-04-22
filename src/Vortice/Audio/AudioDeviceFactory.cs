// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Audio;

public abstract class AudioDeviceFactory
{
    /// <summary>
    /// Get the device backend type.
    /// </summary>
    public abstract AudioBackend BackendType { get; }

    /// <summary>
    /// Get the backend priority.
    /// </summary>
    public abstract int Priority { get; }

    public abstract AudioDevice CreateDevice();
}
