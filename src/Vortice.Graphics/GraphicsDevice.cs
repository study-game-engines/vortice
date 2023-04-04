// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class GraphicsDevice : IDisposable
{
    private volatile uint _isDisposed;
    private string _label;

    private readonly Dictionary<IntPtr, CommandBuffer> _commandBuffers = new();
    private ulong _frameCount;
    private uint _frameIndex;

    public GraphicsDevice(GraphicsBackend backend, string? label)
    {
        _label = label ?? GetType().Name;
        _isDisposed = 0;

        Backend = backend;
    }

    /// <summary>Gets <c>true</c> if the object has been disposed; otherwise, <c>false</c>.</summary>
    public bool IsDisposed => _isDisposed != 0;

    /// <summary>Get the device backend type.</summary>
    public GraphicsBackend Backend { get; }

    /// <summary>Gets the label of the object.</summary>
    public string Label => _label;

    /// <inheritdoc />
    public abstract GraphicsAdapterInfo AdapterInfo { get; }

    /// <summary>
    /// Get the device limits.
    /// </summary>
    public abstract GraphicsDeviceLimits Limits { get; }

    /// <summary>
    /// Gets the number of frame being executed.
    /// </summary>
    public ulong FrameCount => _frameCount;


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

    public static GraphicsDevice CreateDefault(in GraphicsDeviceDescription description)
    {
#if !EXCLUDE_D3D12_BACKEND
        return new D3D12.D3D12GraphicsDevice(in description);
#endif

#if !EXCLUDE_VULKAN_BACKEND
        return new Vulkan.VulkanGraphicsDevice(in description);
#endif

        throw new GraphicsException("No backend supported");
    }

    public abstract bool QueryFeature(Feature feature);

    /// <summary>
    /// Wait for device to finish pending GPU operations.
    /// </summary>
    public abstract void WaitIdle();

    public unsafe GraphicsBuffer CreateBuffer(in BufferDescription description)
    {
        return CreateBuffer(description, null);
    }

    public unsafe GraphicsBuffer CreateBuffer(in BufferDescription description, IntPtr initialData)
    {
        return CreateBuffer(description, initialData.ToPointer());
    }

    public unsafe GraphicsBuffer CreateBuffer(in BufferDescription description, void* initialData)
    {
        Guard.IsGreaterThanOrEqualTo(description.Size, 4, nameof(BufferDescription.Size));

        return CreateBufferCore(description, initialData);
    }

    public unsafe GraphicsBuffer CreateBuffer<T>(in BufferDescription description, ref T initialData) where T : unmanaged
    {
        Guard.IsGreaterThanOrEqualTo(description.Size, 4, nameof(BufferDescription.Size));

        fixed (void* initialDataPtr = &initialData)
        {
            return CreateBuffer(description, initialDataPtr);
        }
    }

    public GraphicsBuffer CreateBuffer<T>(T[] initialData,
        BufferUsage usage = BufferUsage.ShaderReadWrite,
        CpuAccessMode cpuAccess = CpuAccessMode.None)
        where T : unmanaged
    {
        ReadOnlySpan<T> dataSpan = initialData.AsSpan();

        return CreateBuffer(dataSpan, usage, cpuAccess);
    }

    public unsafe GraphicsBuffer CreateBuffer<T>(ReadOnlySpan<T> initialData,
        BufferUsage usage = BufferUsage.ShaderReadWrite,
        CpuAccessMode cpuAccess = CpuAccessMode.None,
        string? label = default)
        where T : unmanaged
    {
        int typeSize = sizeof(T);
        Guard.IsTrue(initialData.Length > 0, nameof(initialData));

        BufferDescription description = new((uint)(initialData.Length * typeSize), usage, cpuAccess, label);
        return CreateBuffer(description, ref MemoryMarshal.GetReference(initialData));
    }

    public unsafe Texture CreateTexture(in TextureDescription description)
    {
        Guard.IsGreaterThanOrEqualTo(description.Width, 1, nameof(TextureDescription.Width));
        Guard.IsGreaterThanOrEqualTo(description.Height, 1, nameof(TextureDescription.Height));
        Guard.IsGreaterThanOrEqualTo(description.DepthOrArrayLayers, 1, nameof(TextureDescription.DepthOrArrayLayers));

        return CreateTextureCore(description, default);
    }

    public SwapChain CreateSwapChain(SwapChainSurface surface, in SwapChainDescription description)
    {
        Guard.IsNotNull(surface, nameof(surface));

        return CreateSwapChainCore(surface, description);
    }

    public ulong CommitFrame()
    {
        //_frameCount = vgpuFrame(Handle);
        return _frameCount;
    }

    /// <summary>
    /// Begin new <see cref="CommandBuffer"/> in recording state.
    /// </summary>
    /// <param name="label">Optional label.</param>
    /// <returns></returns>
    public abstract CommandBuffer BeginCommandBuffer(string? label = default);

    public void Submit(CommandBuffer commandBuffer)
    {
        SubmitCommandBuffers(new[] { commandBuffer }, 1);
    }

    public void Submit(CommandBuffer[] commandBuffers)
    {
        SubmitCommandBuffers(commandBuffers, commandBuffers.Length);
    }

    protected abstract unsafe GraphicsBuffer CreateBufferCore(in BufferDescription description, void* initialData);
    protected abstract unsafe Texture CreateTextureCore(in TextureDescription description, void* initialData);

    protected abstract SwapChain CreateSwapChainCore(SwapChainSurface surface, in SwapChainDescription description);
    protected abstract void SubmitCommandBuffers(CommandBuffer[] commandBuffers, int count);
}
