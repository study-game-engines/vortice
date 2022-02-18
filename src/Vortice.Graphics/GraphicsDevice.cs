// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Toolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class GraphicsDevice : IDisposable
{
    private volatile int _isDisposed;

    protected GraphicsDevice()
    {
    }

    /// <summary>
    /// Releases unmanaged resources and performs other cleanup operations.
    /// </summary>
    ~GraphicsDevice()
    {
        if (Interlocked.CompareExchange(ref _isDisposed, 1, 0) == 0)
        {
            OnDispose();
        }
    }

    /// <summary>
    /// Get the device backend type.
    /// </summary>
    public abstract GpuBackend BackendType { get; }

    public abstract GpuVendorId VendorId { get; }
    public abstract uint AdapterId { get; }
    public abstract GpuAdapterType AdapterType { get; }
    public abstract string AdapterName { get; }

    /// <summary>
    /// Get the device capabilities.
    /// </summary>
    public abstract GraphicsDeviceCaps Capabilities { get; }

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
    /// Wait for device to finish pending GPU operations.
    /// </summary>
    public abstract void WaitIdle();

    public GraphicsBuffer CreateBuffer(in BufferDescriptor descriptor)
    {
        Guard.IsGreaterThanOrEqualTo(descriptor.Size, 1, nameof(BufferDescriptor.Size));

        return CreateBufferCore(descriptor, IntPtr.Zero);
    }

    public GraphicsBuffer CreateBuffer<T>(Span<T> data, BufferUsage usage = BufferUsage.ShaderReadWrite) where T : unmanaged
    {
        unsafe
        {
            BufferDescriptor descriptor = new BufferDescriptor(usage, (ulong)(data.Length * sizeof(T)));
            fixed (T* dataPtr = data)
            {
                return CreateBufferCore(descriptor, (IntPtr)dataPtr);
            }
        }
    }

    public Texture CreateTexture(in TextureDescriptor descriptor)
    {
        Guard.IsGreaterThanOrEqualTo(descriptor.Width, 1, nameof(TextureDescriptor.Width));
        Guard.IsGreaterThanOrEqualTo(descriptor.Height, 1, nameof(TextureDescriptor.Height));
        Guard.IsGreaterThanOrEqualTo(descriptor.DepthOrArraySize, 1, nameof(TextureDescriptor.DepthOrArraySize));

        return CreateTextureCore(descriptor);
    }

    public SwapChain CreateSwapChain(in SwapChainSource source, in SwapChainDescriptor descriptor)
    {
        Guard.IsNotNull(source, nameof(source));

        return CreateSwapChainCore(source, descriptor);
    }

    protected abstract SwapChain CreateSwapChainCore(in SwapChainSource source, in SwapChainDescriptor descriptor);

    protected abstract Texture CreateTextureCore(in TextureDescriptor descriptor);
    protected abstract GraphicsBuffer CreateBufferCore(in BufferDescriptor descriptor, IntPtr initialData);
}
