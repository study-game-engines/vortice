// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using Microsoft.Toolkit.Diagnostics;
using static Vortice.Graphics.VGPU;

namespace Vortice.Graphics;

public sealed class GraphicsDevice : DisposableObject
{
    /// <summary>
    /// The configuration property name for <see cref="IsDebugOutputEnabled"/>.
    /// </summary>
    private const string EnableDebugOutput = "VORTICE_ENABLE_DEBUG_OUTPUT";

    static GraphicsDevice()
    {
        SetLogCallback(OnLogInfoCallback);
    }

    public GraphicsDevice(in GraphicsDeviceDescription? description = default)
    {
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
        }
    }

    /// <summary>
    /// Checks whether the given <see cref="GraphicsBackend"/> is supported on this system.
    /// </summary>
    /// <param name="backend">The GraphicsBackend to check.</param>
    /// <returns>True if the GraphicsBackend is supported; false otherwise.</returns>
    public static bool IsBackendSupported(GraphicsBackend backend)
    {
        return vgpuIsSupported(backend);
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

    public GpuVendorId VendorId { get; }
    public uint AdapterId { get; }
    public GpuAdapterType AdapterType { get; }
    public string AdapterName { get; }

    /// <summary>
    /// Get the device capabilities.
    /// </summary>
    public GraphicsDeviceCaps Capabilities { get; }

    /// <summary>
    /// Get the graphics <see cref="CommandQueue"/>.
    /// </summary>
    //public abstract CommandQueue GraphicsQueue { get; }

    /// <summary>
    /// Get the compute <see cref="CommandQueue"/>.
    /// </summary>
    //public abstract CommandQueue ComputeQueue { get; }

    /// <summary>
    /// Wait for device to finish pending GPU operations.
    /// </summary>
    public void WaitIdle()
    {

    }

    //public GraphicsBuffer CreateBuffer(in BufferDescription description)
    //{
    //    Guard.IsGreaterThanOrEqualTo(description.Size, 1, nameof(BufferDescription.Size));

    //    return CreateBufferCore(description, IntPtr.Zero);
    //}

    //public GraphicsBuffer CreateBuffer<T>(Span<T> data, BufferUsage usage = BufferUsage.ShaderReadWrite) where T : unmanaged
    //{
    //    unsafe
    //    {
    //        BufferDescription description = new BufferDescription(usage, (ulong)(data.Length * sizeof(T)));
    //        fixed (T* dataPtr = data)
    //        {
    //            return CreateBufferCore(description, (IntPtr)dataPtr);
    //        }
    //    }
    //}

    //public Texture CreateTexture(in TextureDescriptor descriptor)
    //{
    //    Guard.IsGreaterThanOrEqualTo(descriptor.Width, 1, nameof(TextureDescriptor.Width));
    //    Guard.IsGreaterThanOrEqualTo(descriptor.Height, 1, nameof(TextureDescriptor.Height));
    //    Guard.IsGreaterThanOrEqualTo(descriptor.DepthOrArraySize, 1, nameof(TextureDescriptor.DepthOrArraySize));

    //    return CreateTextureCore(descriptor);
    //}

    //public SwapChain CreateSwapChain(in SwapChainSurface surface, in SwapChainDescription description)
    //{
    //    Guard.IsNotNull(surface, nameof(surface));

    //    return CreateSwapChainCore(surface, description);
    //}

    [MonoPInvokeCallback(typeof(VGPULogCallback))]
    private static void OnLogInfoCallback(LogLevel level, IntPtr msgPtr)
    {
        string message = FromUTF8(msgPtr);
        Console.WriteLine($"[{level}]: {message}");
    }

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
