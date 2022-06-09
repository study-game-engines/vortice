// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using static Vortice.Graphics.VGPU;

namespace Vortice.Graphics;

public unsafe sealed class GraphicsDevice : DisposableObject
{
    private readonly GraphicsDeviceLimits _limits;
    private readonly Dictionary<IntPtr, CommandBuffer> _commandBuffers = new();
    private readonly Dictionary<IntPtr, Texture> _textures = new();
    private ulong _frameCount;

    static GraphicsDevice()
    {
        SetLogCallback(OnLogInfoCallback);
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

    public GraphicsDevice(in GraphicsDeviceDescription? description = default)
    {
        unsafe
        {
            GraphicsDeviceDescription usageDesc = description.HasValue ? description.Value : new GraphicsDeviceDescription();
            DeviceDesc deviceDesc = new();
            deviceDesc.validationMode = usageDesc.ValidationMode;
            if (vgpuIsSupported(GraphicsBackend.Vulkan))
            {
                deviceDesc.preferredBackend = GraphicsBackend.Vulkan;
            }

            Handle = vgpuCreateDevice(&deviceDesc);
            Backend = vgpuGetBackendType(Handle);
            vgpuGetAdapterProperties(Handle, out AdapterProperties adapterProperties);
            VendorId = (GpuVendorId)adapterProperties.vendorID;
            DeviceId = adapterProperties.deviceID;
            AdapterName = adapterProperties.name;
            DriverDescription = adapterProperties.driverDescription;
            AdapterType = adapterProperties.adapterType;
            vgpuGetLimits(Handle, out _limits);
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            if (Handle != IntPtr.Zero)
            {
                vgpuWaitIdle(Handle);
                _textures.Clear();
                _commandBuffers.Clear();
                vgpuDestroyDevice(Handle);
            }
        }
    }

    public IntPtr Handle { get; }

    /// <summary>
    /// Get the device backend type.
    /// </summary>
    public GraphicsBackend Backend { get; }

    public GpuVendorId VendorId { get; }
    public uint DeviceId { get; }
    public string AdapterName { get; }
    public string DriverDescription { get; }
    public GpuAdapterType AdapterType { get; }

    /// <summary>
    /// Get the device limits.
    /// </summary>
    public GraphicsDeviceLimits Limits { get; }

    /// <summary>
    /// Gets the number of frame being executed.
    /// </summary>
    public ulong FrameCount => _frameCount;

    /// <summary>
    /// Get the graphics <see cref="CommandQueue"/>.
    /// </summary>
    //public abstract CommandQueue GraphicsQueue { get; }

    /// <summary>
    /// Get the compute <see cref="CommandQueue"/>.
    /// </summary>
    //public abstract CommandQueue ComputeQueue { get; }

    public bool QueryFeature(Feature feature)
    {
        return vgpuQueryFeature(Handle, feature, default, 0);
    }

    /// <summary>
    /// Wait for device to finish pending GPU operations.
    /// </summary>
    public void WaitIdle()
    {
        vgpuWaitIdle(Handle);
    }

    public ulong CommitFrame()
    {
        _frameCount = vgpuFrame(Handle);
        return _frameCount;
    }

    public CommandBuffer BeginCommandBuffer(string? label = default)
    {
        IntPtr cmdBufferHandle = vgpuBeginCommandBuffer(Handle, label);

        if (_commandBuffers.TryGetValue(cmdBufferHandle, out CommandBuffer? commandBuffer))
        {
            return commandBuffer!;
        }

        commandBuffer = new CommandBuffer(this, cmdBufferHandle);
        _commandBuffers.Add(cmdBufferHandle, commandBuffer);
        return commandBuffer;
    }

    public unsafe void Submit(CommandBuffer commandBuffer)
    {
        IntPtr cmdBufferHandle = commandBuffer.Handle;
        vgpuSubmit(Handle, &cmdBufferHandle, 1u);
    }

    public unsafe void Submit(CommandBuffer[] commandBuffers)
    {
        IntPtr* commandBufferPtrs = stackalloc IntPtr[commandBuffers.Length];

        for (int i = 0; i < commandBuffers.Length; i += 1)
        {
            commandBufferPtrs[i] = commandBuffers[i].Handle;
        }

        vgpuSubmit(Handle, commandBufferPtrs, (uint)commandBuffers.Length);
    }

    //public Texture CreateTexture(in TextureDescriptor descriptor)
    //{
    //    Guard.IsGreaterThanOrEqualTo(descriptor.Width, 1, nameof(TextureDescriptor.Width));
    //    Guard.IsGreaterThanOrEqualTo(descriptor.Height, 1, nameof(TextureDescriptor.Height));
    //    Guard.IsGreaterThanOrEqualTo(descriptor.DepthOrArraySize, 1, nameof(TextureDescriptor.DepthOrArraySize));

    //    return CreateTextureCore(descriptor);
    //}

    [MonoPInvokeCallback(typeof(VGPULogCallback))]
    private static void OnLogInfoCallback(LogLevel level, sbyte* msgPtr)
    {
        string message = new string(msgPtr);
        Console.WriteLine($"[{level}]: {message}");
    }

    internal Texture GetTexture(IntPtr handle)
    {
        if (_textures.TryGetValue(handle, out Texture? texture))
        {
            return texture!;
        }

        texture = new Texture(this, handle);
        _textures.Add(handle, texture);
        return texture;
    }
}
