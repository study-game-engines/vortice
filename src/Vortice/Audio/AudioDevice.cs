// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Toolkit.Diagnostics;

namespace Vortice.Audio;

public abstract class AudioDevice : IDisposable
{
    private volatile int _isDisposed;
    private static readonly List<AudioDeviceFactory> _factories = new();

    public static void RegisterFactory(AudioDeviceFactory factory)
    {
        Guard.IsNotNull(factory, nameof(factory));

        _factories.Add(factory);
        _factories.Sort((x, y) => x.Priority.CompareTo(y.Priority));
    }

    protected AudioDevice()
    {
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
    public abstract AudioBackend BackendType { get; }

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

    public static AudioDevice CreateDefault(AudioBackend preferredBackend = AudioBackend.Count)
    {
        foreach (AudioDeviceFactory factory in _factories)
        {
            if (preferredBackend == AudioBackend.Count)
            {
                return factory.CreateDevice();
            }
            else if (factory.BackendType == preferredBackend)
            {
                return factory.CreateDevice();
            }
        }

        throw new PlatformNotSupportedException("Cannot find capable audio device");
    }
}
