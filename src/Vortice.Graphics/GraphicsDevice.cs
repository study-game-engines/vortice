// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using Microsoft.Toolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class GraphicsDevice : DisposableObject
{
    /// <summary>
    /// The configuration property name for <see cref="IsDebugOutputEnabled"/>.
    /// </summary>
    private const string EnableDebugOutput = "VORTICE_ENABLE_DEBUG_OUTPUT";

    protected GraphicsDevice(GraphicsBackend backend)
    {
        Backend = backend;
    }

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
                return D3D12.D3D12GraphicsDevice.IsSupported();
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

    public static GraphicsDevice CreateDefault(in GraphicsDeviceDescription description)
    {
        if (PlatformInfo.IsWindows)
        {
#if !EXCLUDE_D3D11_BACKEND
        return new D3D11.D3D11GraphicsDevice(validationMode);
#endif

#if !EXCLUDE_D3D12_BACKEND
        return new D3D12.D3D12GraphicsDevice(validationMode);
#endif
        }

#if !EXCLUDE_VULKAN_BACKEND
        return new Vulkan.VulkanGraphicsDevice(description);
#endif

        throw new GraphicsException("No backend is supported");
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
    public GraphicsBackend Backend { get; }

    public abstract GpuVendorId VendorId { get; }
    public abstract uint AdapterId { get; }
    public abstract GpuAdapterType AdapterType { get; }
    public abstract string AdapterName { get; }

    /// <summary>
    /// Get the device capabilities.
    /// </summary>
    public abstract GraphicsDeviceCaps Capabilities { get; }

    /// <summary>
    /// Get the graphics <see cref="CommandQueue"/>.
    /// </summary>
    public abstract CommandQueue GraphicsQueue { get; }

    /// <summary>
    /// Get the compute <see cref="CommandQueue"/>.
    /// </summary>
    public abstract CommandQueue ComputeQueue { get; }

    /// <summary>
    /// Wait for device to finish pending GPU operations.
    /// </summary>
    public abstract void WaitIdle();

    public GraphicsBuffer CreateBuffer(in BufferDescription description)
    {
        Guard.IsGreaterThanOrEqualTo(description.Size, 1, nameof(BufferDescription.Size));

        return CreateBufferCore(description, IntPtr.Zero);
    }

    public GraphicsBuffer CreateBuffer<T>(Span<T> data, BufferUsage usage = BufferUsage.ShaderReadWrite) where T : unmanaged
    {
        unsafe
        {
            BufferDescription description = new BufferDescription(usage, (ulong)(data.Length * sizeof(T)));
            fixed (T* dataPtr = data)
            {
                return CreateBufferCore(description, (IntPtr)dataPtr);
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

    public SwapChain CreateSwapChain(in SwapChainSurface surface, in SwapChainDescription description)
    {
        Guard.IsNotNull(surface, nameof(surface));

        return CreateSwapChainCore(surface, description);
    }


    protected abstract GraphicsBuffer CreateBufferCore(in BufferDescription description, IntPtr initialData);
    protected abstract Texture CreateTextureCore(in TextureDescriptor descriptor);
    protected abstract SwapChain CreateSwapChainCore(in SwapChainSurface surface, in SwapChainDescription description);

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
