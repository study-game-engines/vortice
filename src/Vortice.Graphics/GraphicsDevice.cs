// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CommunityToolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class GraphicsDevice : DisposableObject
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

    public static GraphicsDevice CreateDefault(ValidationMode validationMode = ValidationMode.Disabled)
    {
#if !EXCLUDE_D3D11_BACKEND
        return new D3D11.D3D11GraphicsDevice(validationMode, GpuPowerPreference.HighPerformance);
#endif

#if WINDOWS || WINDOWS_UWP
        //return new D3D12.D3D12GraphicsDevice(validationMode);
#else
        //return new Vulkan.VulkanGraphicsDevice(validationMode);
#endif

        throw new PlatformNotSupportedException("No graphics backend is supported on current OS");
    }

    public abstract bool QueryFeature(Feature feature);

    /// <summary>
    /// Wait for device to finish pending GPU operations.
    /// </summary>
    public abstract void WaitIdle();

    public Texture CreateTexture(in TextureDescription description)
    {
        Guard.IsGreaterThanOrEqualTo(description.Width, 1, nameof(TextureDescription.Width));
        Guard.IsGreaterThanOrEqualTo(description.Height, 1, nameof(TextureDescription.Height));
        Guard.IsGreaterThanOrEqualTo(description.DepthOrArraySize, 1, nameof(TextureDescription.DepthOrArraySize));

        return CreateTextureCore(description);
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

    protected abstract Texture CreateTextureCore(in TextureDescription description);

    protected abstract SwapChain CreateSwapChainCore(SwapChainSurface surface, in SwapChainDescription description);
    protected abstract void SubmitCommandBuffers(CommandBuffer[] commandBuffers, int count);
}
