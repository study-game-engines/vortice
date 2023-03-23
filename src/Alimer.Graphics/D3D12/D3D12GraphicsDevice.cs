// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CommunityToolkit.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.DXGI_INFO_QUEUE_MESSAGE_SEVERITY;
using TerraFX.Interop.Windows;

namespace Alimer.Graphics.D3D12;

internal unsafe class D3D12GraphicsDevice : GraphicsDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    public static bool IsSupported() => s_isSupported.Value;

    //private readonly ComPtr<IDXGIFactory4> _dxgiFactory;

    private readonly GraphicsAdapterInfo _adapterInfo;
    //private readonly GraphicsDeviceFeatures _features;
    private readonly GraphicsDeviceLimits _limits;
    private readonly D3D_FEATURE_LEVEL _featureLevel;

    public D3D12GraphicsDevice(in GraphicsDeviceDescription description)
        : base(GraphicsBackend.Direct3D12, description.Label)
    {
        Guard.IsTrue(IsSupported(), nameof(D3D12GraphicsDevice), "Direct3D12 is not supported");

        uint dxgiFactoryFlags = 0u;

        if (description.ValidationMode != ValidationMode.Disabled)
        {
            dxgiFactoryFlags = DXGI_CREATE_FACTORY_DEBUG;
#if DEBUG
            using ComPtr<IDXGIInfoQueue> dxgiInfoQueue = default;

            if (DXGIGetDebugInterface1(0u, __uuidof<IDXGIInfoQueue>(), dxgiInfoQueue.GetVoidAddressOf()).SUCCEEDED)
            {
                dxgiInfoQueue.Get()->SetBreakOnSeverity(DXGI_DEBUG_ALL, DXGI_INFO_QUEUE_MESSAGE_SEVERITY_ERROR, true);
                dxgiInfoQueue.Get()->SetBreakOnSeverity(DXGI_DEBUG_ALL, DXGI_INFO_QUEUE_MESSAGE_SEVERITY_CORRUPTION, true);

                int* hide = stackalloc int[1]
                {
                    80 /* IDXGISwapChain::GetContainingOutput: The swapchain's adapter does not control the output on which the swapchain's window resides. */,
                };

                DXGI_INFO_QUEUE_FILTER filter = new();
                filter.DenyList = new DXGI_INFO_QUEUE_FILTER_DESC()
                {
                    NumIDs = 1,
                    pIDList = hide
                };

                dxgiInfoQueue.Get()->AddStorageFilterEntries(DXGI_DEBUG_DXGI, &filter);
            }
#endif
        }

        //ThrowIfFailed(CreateDXGIFactory2(dxgiFactoryFlags, IID_PPV_ARGS(&dxgiFactory)));
    }

    /// <inheritdoc />
    public override GraphicsAdapterInfo AdapterInfo => _adapterInfo;

    /// <inheritdoc />
    //public override GraphicsDeviceFeatures Features => _features;

    /// <inheritdoc />
    public override GraphicsDeviceLimits Limits => _limits;


    /// <summary>
    /// Finalizes an instance of the <see cref="D3D12GraphicsDevice" /> class.
    /// </summary>
    ~D3D12GraphicsDevice() => Dispose(disposing: false);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            //ImmediateContext.Flush();
            //ImmediateContext.Dispose();

#if DEBUG
            //uint refCount = NativeDevice.Release();
            //if (refCount > 0)
            //{
            //    System.Diagnostics.Debug.WriteLine($"Direct3D11: There are {refCount} unreleased references left on the device");
            //
            //    ID3D11Debug? d3d11Debug = NativeDevice.QueryInterfaceOrNull<ID3D11Debug>();
            //    if (d3d11Debug != null)
            //    {
            //        d3d11Debug.ReportLiveDeviceObjects(ReportLiveDeviceObjectFlags.Detail | ReportLiveDeviceObjectFlags.IgnoreInternal);
            //        d3d11Debug.Dispose();
            //    }
            //}
#else
            //NativeDevice.Dispose();
#endif

            //DXGIFactory.Dispose();

#if DEBUG
            //if (DXGIGetDebugInterface1(out IDXGIDebug1? dxgiDebug).Success)
            //{
            //    dxgiDebug!.ReportLiveObjects(DebugAll, ReportLiveObjectFlags.Summary | ReportLiveObjectFlags.IgnoreInternal);
            //    dxgiDebug!.Dispose();
            //}
#endif
        }
    }

    /// <inheritdoc />
    public override void WaitIdle()
    {
        //ImmediateContext.Flush();
    }

    public override bool QueryFeature(Feature feature) => throw new NotImplementedException();
    public override CommandBuffer BeginCommandBuffer(string? label = null) => throw new NotImplementedException();
    protected override unsafe GraphicsBuffer CreateBufferCore(in BufferDescription description, void* initialData) => throw new NotImplementedException();
    protected override unsafe Texture CreateTextureCore(in TextureDescription description, void* initialData) => throw new NotImplementedException();
    protected override void SubmitCommandBuffers(CommandBuffer[] commandBuffers, int count) => throw new NotImplementedException();

    /// <inheritdoc />
    protected override SwapChain CreateSwapChainCore(SwapChainSurface surface, in SwapChainDescription description)
    {
        throw new NotImplementedException();
    }

    private static bool CheckIsSupported()
    {
        try
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 19041))
            {
                return false;
            }

            return true;
#if TODO
            using IDXGIFactory2 dxgiFactory = CreateDXGIFactory1<IDXGIFactory2>();

            bool foundCompatibleDevice = false;
            for (int adapterIndex = 0;
                dxgiFactory.EnumAdapters1(adapterIndex, out IDXGIAdapter1? dxgiAdapter).Success;
                adapterIndex++)
            {
                AdapterDescription1 desc = dxgiAdapter.Description1;

                dxgiAdapter.Dispose();

                if ((desc.Flags & AdapterFlags.Software) != AdapterFlags.None)
                {
                    // Don't select the Basic Render Driver adapter.
                    continue;
                }

                foundCompatibleDevice = true;
                break;
            }

            if (!foundCompatibleDevice)
            {
                return false;
            }

            return IsSupportedFeatureLevel(FeatureLevel.Level_11_0, DeviceCreationFlags.BgraSupport); 
#endif
        }
        catch
        {
            return false;
        }
    }

}
