﻿// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace Vortice;

/// <summary>
/// Base class for a <see cref="IDisposable"/> interface.
/// </summary>
public abstract class DisposableObject : IDisposable
{
    private volatile uint _isDisposed = 0;
    private readonly List<IDisposable> _disposables = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DisposableObject" /> class.
    /// </summary>
    protected DisposableObject()
    {
        _isDisposed = 0;
    }

    /// <summary>
    /// Gets <c>true</c> if the object has been disposed; otherwise, <c>false</c>.
    /// </summary>
    public bool IsDisposed => _isDisposed != 0;

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /// <inheritdoc cref="Dispose()" />
    /// <param name="disposing"><c>true</c> if the method was called from <see cref="Dispose()" />; otherwise, <c>false</c>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            for (int i = _disposables.Count - 1; i >= 0; i--)
            {
                _disposables[i].Dispose();
            }
        }
    }

    /// <summary>Throws an exception if the object has been disposed.</summary>
    /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
        {
            throw new ObjectDisposedException(ToString());
        }
    }

    /// <summary>
    /// Adds a disposable object to the list of the objects to dispose.
    /// </summary>
    /// <param name="item">To dispose.</param>
    protected internal T AddDisposable<T>(T objectToDispose)
        where T : IDisposable
    {
        Guard.IsNotNull(objectToDispose);

        _disposables.Add(objectToDispose);
        return objectToDispose;
    }

    /// <summary>
    /// Removes a disposable object to the list of the objects to dispose.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objectToDispose">The previously added object to dispose.</param>
    protected internal void RemoveDisposable<T>(T objectToDispose)
        where T : IDisposable
    {
        Guard.IsNotNull(objectToDispose);

        _disposables.Remove(objectToDispose);
    }

    /// <summary>
    /// Dispose a disposable object and set the reference to null. 
    /// Removes this object from the ToDispose list.
    /// </summary>
    /// <param name="objectToDispose">Object to dispose.</param>
    protected internal void RemoveAndDispose<T>(T objectToDispose)
        where T : IDisposable
    {
        Guard.IsNotNull(objectToDispose);

        _disposables.Remove(objectToDispose);
        objectToDispose.Dispose();
    }
}
