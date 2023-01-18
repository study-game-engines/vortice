// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class GraphicsDevice : GraphicsObject
{
    private readonly Dictionary<IntPtr, CommandBuffer> _commandBuffers = new();
    private ulong _frameCount;

    public GraphicsDevice(GraphicsBackend backend)
    {
        Backend = backend;
    }

    /// <summary>
    /// Get the device backend type.
    /// </summary>
    public GraphicsBackend Backend { get; }

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

    /// <summary>
    /// Checks whether the given <see cref="GraphicsBackend"/> is supported on this system.
    /// </summary>
    /// <param name="backend">The GraphicsBackend to check.</param>
    /// <returns>True if the GraphicsBackend is supported; false otherwise.</returns>
    public static bool IsBackendSupported(GraphicsBackend backend)
    {
        switch (backend)
        {
            case GraphicsBackend.Vulkan:
#if !EXCLUDE_VULKAN_BACKEND
                return Vulkan.VulkanGraphicsDevice.IsSupported();
#else
                return false;
#endif

            case GraphicsBackend.Direct3D12:
#if !EXCLUDE_D3D12_BACKEND
                //return D3D12.D3D12GraphicsDevice.IsSupported();
                return false;
#else
                return false;
#endif

            case GraphicsBackend.Direct3D11:
#if !EXCLUDE_D3D11_BACKEND
                return D3D11.D3D11GraphicsDevice.IsSupported();
#else
                return false;
#endif

            default:
                return ThrowHelper.ThrowArgumentException<bool>("Invalid GraphicsBackend value");
        }
    }

    public static GraphicsDevice CreateDefault(GraphicsDeviceDescription? description = default)
    {
        description ??= new GraphicsDeviceDescription();

        GraphicsBackend backend = description.Value.PreferredBackend;

        if (backend == GraphicsBackend.Count)
        {
#if !EXCLUDE_D3D11_BACKEND
            if (D3D11.D3D11GraphicsDevice.IsSupported())
            {
                return new D3D11.D3D11GraphicsDevice(description.Value);
            }
#endif

#if !EXCLUDE_VULKAN_BACKEND
            if (Vulkan.VulkanGraphicsDevice.IsSupported())
            {
                return new Vulkan.VulkanGraphicsDevice(description.Value);
            }
#else
            //return new Vulkan.VulkanGraphicsDevice(validationMode);
#endif
        }

        throw new PlatformNotSupportedException("No graphics backend is supported on current OS");
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
