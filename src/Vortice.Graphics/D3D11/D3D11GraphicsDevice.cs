// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Direct3D;
using Vortice.Direct3D11;
using static Vortice.Direct3D11.D3D11;

namespace Vortice.Graphics.D3D11;

internal sealed unsafe class D3D11GraphicsDevice : GraphicsDevice
{
    public static bool IsSupported() => s_isSupported.Value;

    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    private static bool CheckIsSupported()
    {
        return IsSupportedFeatureLevel(FeatureLevel.Level_11_0, DeviceCreationFlags.BgraSupport);
    }

    private readonly ID3D11Device1 _handle;
    private readonly GraphicsDeviceCaps _caps;

    public D3D11GraphicsDevice(in GraphicsDeviceDescriptor descriptor)
    {
        
    }

    public ID3D11Device1 NativeDevice => _handle;

    // <inheritdoc />
    public override GpuBackend BackendType => GpuBackend.Direct3D11;

    // <inheritdoc />
    public override GpuVendorId VendorId { get; }

    /// <inheritdoc />
    public override uint AdapterId { get; }

    /// <inheritdoc />
    public override GpuAdapterType AdapterType { get; }

    /// <inheritdoc />
    public override string AdapterName { get; }

    /// <inheritdoc />
    public override GraphicsDeviceCaps Capabilities => _caps;


    /// <inheritdoc />
    protected override void OnDispose()
    {
        WaitIdle();
    }

    /// <inheritdoc />
    public override void WaitIdle()
    {
    }

    /// <inheritdoc />
    public override CommandBuffer BeginCommandBuffer(CommandQueueType queueType = CommandQueueType.Graphics) => default;

    /// <inheritdoc />
    protected override GraphicsBuffer CreateBufferCore(in BufferDescriptor descriptor, IntPtr initialData) => throw new NotImplementedException();
    /// <inheritdoc />
    protected override Texture CreateTextureCore(in TextureDescriptor descriptor) => new D3D11Texture(this, descriptor);
    /// <inheritdoc />
    protected override SwapChain CreateSwapChainCore(in GraphicsSurface surface, in SwapChainDescriptor descriptor) => new D3D11SwapChain(this, surface, descriptor);
}
