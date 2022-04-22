// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using Microsoft.Toolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class GraphicsDevice : IDisposable
{
    /// <summary>
    /// The configuration property name for <see cref="IsDebugOutputEnabled"/>.
    /// </summary>
    private const string EnableDebugOutput = "VORTICE_ENABLE_DEBUG_OUTPUT";

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
    /// Indicates whether or not the debug output is enabled.
    /// </summary>
    public static bool IsDebugOutputEnabled
    {
        get => GetConfigurationValue(EnableDebugOutput);
        set => AppContext.SetSwitch(EnableDebugOutput, value);
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

    public abstract CommandBuffer BeginCommandBuffer(CommandQueueType queueType = CommandQueueType.Graphics);

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

    public SwapChain CreateSwapChain(in GraphicsSurface surface, in SwapChainDescriptor descriptor)
    {
        Guard.IsNotNull(surface, nameof(surface));

        return CreateSwapChainCore(surface, descriptor);
    }

    protected abstract SwapChain CreateSwapChainCore(in GraphicsSurface surface, in SwapChainDescriptor descriptor);

    protected abstract Texture CreateTextureCore(in TextureDescriptor descriptor);
    protected abstract GraphicsBuffer CreateBufferCore(in BufferDescriptor descriptor, IntPtr initialData);

    /// <summary>
    /// Gets a configuration value for a specified property.
    /// </summary>
    /// <param name="propertyName">The property name to retrieve the value for.</param>
    /// <returns>The value of the specified configuration setting.</returns>
    private static bool GetConfigurationValue(string propertyName)
    {
#if DEBUG
        if (Debugger.IsAttached)
        {
            return true;
        }
#endif

        if (AppContext.TryGetSwitch(propertyName, out bool isEnabled))
        {
            return isEnabled;
        }

        return false;
    }
}
