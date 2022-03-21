// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Toolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class GraphicsResource : IDisposable
{
    private volatile int _isDisposed;

    protected GraphicsResource(GraphicsDevice device)
    {
        Guard.IsNotNull(device, nameof(device));

        Device = device;
    }

    /// <summary>
    /// Get the <see cref="GraphicsDevice"/> object that created the resource.
    /// </summary>
    public GraphicsDevice Device { get; }

    /// <summary>
    /// Releases unmanaged resources and performs other cleanup operations.
    /// </summary>
    ~GraphicsResource()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            OnDispose();
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
}
