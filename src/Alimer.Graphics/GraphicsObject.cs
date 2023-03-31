// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Alimer.Graphics;

/// <summary>
/// An base graphics object that implements <see cref="IDisposable"/> pattern.
/// </summary>
public abstract class GraphicsObject : IDisposable
{
    private volatile uint _isDisposed;
    protected string _label;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsObject" /> class.
    /// </summary>
    /// <param name="label">The label of the object or <c>null</c> to use <see cref="MemberInfo.Name" />.</param>
    protected GraphicsObject(string? label = default)
    {
        _isDisposed = 0;
        _label = label ?? GetType().Name;
    }

    /// <summary>Gets <c>true</c> if the object has been disposed; otherwise, <c>false</c>.</summary>
    public bool IsDisposed => _isDisposed != 0;

    /// <summary>
    /// Gets or set the label that identifies the resource.
    /// </summary>
    public string Label
    {
        get => _label;
        set
        {
            _label = value ?? GetType().Name;
            OnLabelChanged(_label);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>Asserts that the object has not been disposed.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void AssertNotDisposed() => Debug.Assert(_isDisposed == 0);

    /// <inheritdoc cref="Dispose()" />
    /// <param name="disposing"><c>true</c> if the method was called from <see cref="Dispose()" />; otherwise, <c>false</c>.</param>
    protected abstract void Dispose(bool disposing);

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

    /// <summary>Marks the object as being disposed.</summary>
    protected void MarkDisposed() => Interlocked.Exchange(ref _isDisposed, 1);

    /// <inheritdoc />
    public override string ToString() => _label;

    protected abstract void OnLabelChanged(string newLabel);
}
