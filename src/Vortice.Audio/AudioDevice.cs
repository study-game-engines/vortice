// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace Vortice.Audio;

public abstract class AudioDevice : IDisposable
{
    private volatile int _isDisposed;
    private float _masterVolume = 1.0f;

    protected AudioDevice(AudioBackend backend)
    {
        Backend = backend;
    }

    /// <summary>
    /// Releases unmanaged resources and performs other cleanup operations.
    /// </summary>
    ~AudioDevice()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            OnDispose();
        }
    }

    /// <summary>
    /// Get the device backend type.
    /// </summary>
    public AudioBackend Backend { get; }

    public float MasterVolume
    {
        get => _masterVolume;
        set
        {
            _masterVolume = value;
            OnMasterVolumeChanged(value);
        }
    }

    /// <summary>
    /// Gets whether or not the current instance has already been disposed.
    /// </summary>
    public bool IsDisposed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _isDisposed != 0;
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        OnDispose();
        GC.SuppressFinalize(this);
    }

    protected abstract void OnDispose();

    /// <summary>
    /// Throws an <see cref="ObjectDisposedException" /> if the current instance has been disposed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfDisposed()
    {
        if (IsDisposed)
        {
            Throw();
        }
        void Throw()
        {
            throw new ObjectDisposedException(ToString());
        }
    }

    /// <summary>
    /// Checks whether the given <see cref="AudioBackend"/> is supported on this system.
    /// </summary>
    /// <param name="backend">The AudioBackend to check.</param>
    /// <returns>True if the AudioBackend is supported; false otherwise.</returns>
    public static bool IsBackendSupported(AudioBackend backend)
    {
        switch (backend)
        {
            case AudioBackend.XAudio2:
#if !EXCLUDE_XAUDIO2_BACKEND
                return false;
                //return XAudio2.XAudio2Device.IsSupported();
#else
                return false;
#endif

            case AudioBackend.OpenAL:
#if !EXCLUDE_OPENAL_BACKEND
                //return D3D12.D3D12GraphicsDevice.IsSupported();
                return false;
#else
                return false;
#endif

            default:
                return ThrowHelper.ThrowArgumentException<bool>("Invalid AudioBackend value");
        }
    }

    public static AudioDevice CreateDefault(AudioBackend preferredBackend = AudioBackend.Count)
    {
        if (preferredBackend == AudioBackend.Count)
        {
#if !EXCLUDE_XAUDIO2_BACKEND
            //if(IsBackendSupported(AudioBackend.XAudio2))
            //{
            //    return new XAudio2.XAudio2Device();
            //}
#endif

#if !EXCLUDE_OPENAL_BACKEND
            if (IsBackendSupported(AudioBackend.OpenAL))
            {
                return new OpenAL.OpenALDevice();
            }
#endif
        }

        throw new PlatformNotSupportedException("Cannot find capable audio device");
    }

    protected abstract void OnMasterVolumeChanged(float volume);
}
