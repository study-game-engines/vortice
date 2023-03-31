// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using CommunityToolkit.Diagnostics;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.D3D12_DRED_ENABLEMENT;
using static TerraFX.Interop.DirectX.D3D12_GPU_BASED_VALIDATION_FLAGS;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.DirectX.DXGI_ADAPTER_FLAG;
using static TerraFX.Interop.DirectX.DXGI_FEATURE;
using static TerraFX.Interop.DirectX.DXGI_INFO_QUEUE_MESSAGE_SEVERITY;
using static TerraFX.Interop.DirectX.D3D12_MESSAGE_SEVERITY;
using static TerraFX.Interop.DirectX.D3D12_MESSAGE_ID;
using static TerraFX.Interop.DirectX.DXGI_DEBUG_RLO_FLAGS;
using static TerraFX.Interop.Windows.Windows;

namespace Alimer.Graphics.D3D12;

internal unsafe class D3D12GraphicsDevice : GraphicsDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    public static bool IsSupported() => s_isSupported.Value;

    private readonly ComPtr<IDXGIFactory4> _dxgiFactory;
    private readonly ComPtr<ID3D12Device5> _handle;
    private readonly ComPtr<D3D12MA_Allocator> _allocator;

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

            using ComPtr<ID3D12Debug> d3d12Debug = default;
            if (D3D12GetDebugInterface(__uuidof<ID3D12Debug>(), d3d12Debug.GetVoidAddressOf()).SUCCEEDED)
            {
                d3d12Debug.Get()->EnableDebugLayer();

                if (description.ValidationMode == ValidationMode.GPU)
                {
                    using ComPtr<ID3D12Debug1> d3d12Debug1 = default;
                    using ComPtr<ID3D12Debug2> d3d12Debug2 = default;

                    if (d3d12Debug.CopyTo(d3d12Debug1.GetAddressOf()).SUCCEEDED)
                    {
                        d3d12Debug1.Get()->SetEnableGPUBasedValidation(true);
                        d3d12Debug1.Get()->SetEnableSynchronizedCommandQueueValidation(true);
                    }

                    if (d3d12Debug.CopyTo(d3d12Debug2.GetAddressOf()).SUCCEEDED)
                    {
                        const bool g_D3D12DebugLayer_GPUBasedValidation_StateTracking_Enabled = true;

                        if (g_D3D12DebugLayer_GPUBasedValidation_StateTracking_Enabled)
                            d3d12Debug2.Get()->SetGPUBasedValidationFlags(D3D12_GPU_BASED_VALIDATION_FLAGS_DISABLE_STATE_TRACKING);
                        else
                            d3d12Debug2.Get()->SetGPUBasedValidationFlags(D3D12_GPU_BASED_VALIDATION_FLAGS_NONE);
                    }
                }
            }
            else
            {
                Debug.WriteLine("WARNING: Direct3D Debug Device is not available");
            }

            // DRED
            using ComPtr<ID3D12DeviceRemovedExtendedDataSettings1> pDredSettings = default;
            if (D3D12GetDebugInterface(__uuidof<ID3D12DeviceRemovedExtendedDataSettings1>(), pDredSettings.GetVoidAddressOf()).SUCCEEDED)
            {
                // Turn on auto-breadcrumbs and page fault reporting.
                pDredSettings.Get()->SetAutoBreadcrumbsEnablement(D3D12_DRED_ENABLEMENT_FORCED_ON);
                pDredSettings.Get()->SetPageFaultEnablement(D3D12_DRED_ENABLEMENT_FORCED_ON);
                pDredSettings.Get()->SetBreadcrumbContextEnablement(D3D12_DRED_ENABLEMENT_FORCED_ON);
            }

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

        ThrowIfFailed(CreateDXGIFactory2(dxgiFactoryFlags, __uuidof<IDXGIFactory4>(), _dxgiFactory.GetVoidAddressOf()));

        // Determines whether tearing support is available for fullscreen borderless windows.
        {
            BOOL allowTearing = false;

            using ComPtr<IDXGIFactory5> dxgiFactory5 = default;
            HRESULT hr = _dxgiFactory.CopyTo(dxgiFactory5.GetAddressOf());

            if (hr.SUCCEEDED)
            {
                hr = dxgiFactory5.Get()->CheckFeatureSupport(DXGI_FEATURE_PRESENT_ALLOW_TEARING, &allowTearing, (uint)sizeof(BOOL));
            }

            if (hr.FAILED || !allowTearing)
            {
                TearingSupported = false;
#if DEBUG
                Debug.WriteLine("WARNING: Variable refresh rate displays not supported");
#endif
            }
            else
            {
                TearingSupported = true;
            }
        }

        {
            DXGI_GPU_PREFERENCE gpuPreference = description.PowerPreference.ToDxgi();

            using ComPtr<IDXGIFactory6> dxgiFactory6 = default;
            bool queryByPreference = _dxgiFactory.CopyTo(dxgiFactory6.GetAddressOf()).SUCCEEDED;

            using ComPtr<IDXGIAdapter1> dxgiAdapter = default;
            for (uint i = 0; NextAdapter(i, dxgiAdapter.ReleaseAndGetAddressOf()); ++i)
            {
                DXGI_ADAPTER_DESC1 adapterDesc;
                ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&adapterDesc));

                // Don't select the Basic Render Driver adapter.
                if ((adapterDesc.Flags & (uint)DXGI_ADAPTER_FLAG_SOFTWARE) != 0u)
                {
                    continue;
                }

                if (D3D12CreateDevice(dxgiAdapter.AsIUnknown().Get(), D3D_FEATURE_LEVEL_12_0,
                    __uuidof<ID3D12Device5>(), _handle.GetVoidAddressOf()).SUCCEEDED)
                {
                    break;
                }
            }

            // Create the DX12 API device object.
            Handle->SetName("AlimerDevice");

            if (description.ValidationMode != ValidationMode.Disabled)
            {
                //ID3D12DebugDevice1* debugDevice;
                //if (SUCCEEDED(d3dDevice->QueryInterface(&debugDevice)))
                //{
                //    const bool g_D3D12DebugLayer_AllowBehaviorChangingDebugAids = true;
                //    const bool g_D3D12DebugLayer_ConservativeResourceStateTracking = true;
                //    const bool g_D3D12DebugLayer_DisableVirtualizedBundlesValidation = false;
                //
                //    uint32_t featureFlags = 0;
                //    if (g_D3D12DebugLayer_AllowBehaviorChangingDebugAids)
                //        featureFlags |= D3D12_DEBUG_FEATURE_ALLOW_BEHAVIOR_CHANGING_DEBUG_AIDS;
                //    if (g_D3D12DebugLayer_ConservativeResourceStateTracking)
                //        featureFlags |= D3D12_DEBUG_FEATURE_CONSERVATIVE_RESOURCE_STATE_TRACKING;
                //    if (g_D3D12DebugLayer_DisableVirtualizedBundlesValidation)
                //        featureFlags |= D3D12_DEBUG_FEATURE_DISABLE_VIRTUALIZED_BUNDLES_VALIDATION;
                //
                //    ThrowIfFailed(debugDevice->SetDebugParameter(D3D12_DEBUG_DEVICE_PARAMETER_FEATURE_FLAGS, &featureFlags, sizeof featureFlags));
                //    debugDevice->Release();
                //}

                // Configure debug device (if active).
                using ComPtr<ID3D12InfoQueue> infoQueue = default;
                if (_handle.CopyTo(infoQueue.GetAddressOf()).SUCCEEDED)
                {
                    infoQueue.Get()->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_CORRUPTION, TRUE);
                    infoQueue.Get()->SetBreakOnSeverity(D3D12_MESSAGE_SEVERITY_ERROR, TRUE);

                    // These severities should be seen all the time
                    uint enabledSeveritiesCount = (description.ValidationMode == ValidationMode.Verbose) ? 5u : 4u;
                    D3D12_MESSAGE_SEVERITY* enabledSeverities = stackalloc D3D12_MESSAGE_SEVERITY[5]
                    {
                        D3D12_MESSAGE_SEVERITY_CORRUPTION,
                        D3D12_MESSAGE_SEVERITY_ERROR,
                        D3D12_MESSAGE_SEVERITY_WARNING,
                        D3D12_MESSAGE_SEVERITY_MESSAGE,
                        D3D12_MESSAGE_SEVERITY_INFO
                    };

                    const int disabledMessagesCount = 9;
                    D3D12_MESSAGE_ID* disabledMessages = stackalloc D3D12_MESSAGE_ID[disabledMessagesCount]
                    {
                        D3D12_MESSAGE_ID_CLEARRENDERTARGETVIEW_MISMATCHINGCLEARVALUE,
                        D3D12_MESSAGE_ID_CLEARDEPTHSTENCILVIEW_MISMATCHINGCLEARVALUE,
                        D3D12_MESSAGE_ID_MAP_INVALID_NULLRANGE,
                        D3D12_MESSAGE_ID_UNMAP_INVALID_NULLRANGE,
                        D3D12_MESSAGE_ID_EXECUTECOMMANDLISTS_WRONGSWAPCHAINBUFFERREFERENCE,
                        D3D12_MESSAGE_ID_RESOURCE_BARRIER_MISMATCHING_COMMAND_LIST_TYPE,
                        D3D12_MESSAGE_ID_EXECUTECOMMANDLISTS_GPU_WRITTEN_READBACK_RESOURCE_MAPPED,
                        D3D12_MESSAGE_ID_LOADPIPELINE_NAMENOTFOUND,
                        D3D12_MESSAGE_ID_STOREPIPELINE_DUPLICATENAME
                    };

                    D3D12_INFO_QUEUE_FILTER filter = new();
                    filter.AllowList.NumSeverities = enabledSeveritiesCount;
                    filter.AllowList.pSeverityList = enabledSeverities;
                    filter.DenyList.NumIDs = disabledMessagesCount;
                    filter.DenyList.pIDList = disabledMessages;

                    // Clear out the existing filters since we're taking full control of them
                    _ = infoQueue.Get()->PushEmptyStorageFilter();

                    ThrowIfFailed(infoQueue.Get()->AddStorageFilterEntries(&filter));
                }
            }

            // Create allocator
            D3D12MA_ALLOCATOR_DESC allocatorDesc = default;
            allocatorDesc.pDevice = (ID3D12Device*)Handle;
            allocatorDesc.pAdapter = (IDXGIAdapter*)dxgiAdapter.Get();
            ThrowIfFailed(D3D12MemAlloc.D3D12MA_CreateAllocator(&allocatorDesc, _allocator.GetAddressOf()));

            bool NextAdapter(uint index, IDXGIAdapter1** ppAdapter)
            {
                if (queryByPreference)
                    return dxgiFactory6.Get()->EnumAdapterByGpuPreference(index, gpuPreference, __uuidof<IDXGIAdapter1>(), (void**)ppAdapter) != DXGI_ERROR_NOT_FOUND;
                else
                    return _dxgiFactory.Get()->EnumAdapters1(index, ppAdapter) != DXGI_ERROR_NOT_FOUND;
            }
        }
    }

    /// <inheritdoc />
    public override GraphicsAdapterInfo AdapterInfo => _adapterInfo;

    /// <inheritdoc />
    //public override GraphicsDeviceFeatures Features => _features;

    /// <inheritdoc />
    public override GraphicsDeviceLimits Limits => _limits;

    public IDXGIFactory4* DXGIFactory => _dxgiFactory;
    public ID3D12Device5* Handle => _handle;

    public bool TearingSupported { get; }

    /// <summary>
    /// Finalizes an instance of the <see cref="D3D12GraphicsDevice" /> class.
    /// </summary>
    ~D3D12GraphicsDevice() => Dispose(disposing: false);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _allocator.Dispose();

#if DEBUG
            uint refCount = _handle.Get()->Release();
            if (refCount > 0)
            {
                Debug.WriteLine($"Direct3D11: There are {refCount} unreleased references left on the device");
            
                //ID3D11Debug? d3d11Debug = NativeDevice.QueryInterfaceOrNull<ID3D11Debug>();
                //if (d3d11Debug != null)
                //{
                //    d3d11Debug.ReportLiveDeviceObjects(ReportLiveDeviceObjectFlags.Detail | ReportLiveDeviceObjectFlags.IgnoreInternal);
                //    d3d11Debug.Dispose();
                //}
            }
#else
            _handle.Dispose();
#endif

            _dxgiFactory.Dispose();

#if DEBUG
            using ComPtr<IDXGIDebug1> dxgiDebug = default;
            if (DXGIGetDebugInterface1(0u, __uuidof<IDXGIDebug1>(), dxgiDebug.GetVoidAddressOf()).SUCCEEDED)
            {
                dxgiDebug.Get()->ReportLiveObjects(DXGI_DEBUG_ALL, DXGI_DEBUG_RLO_SUMMARY | DXGI_DEBUG_RLO_IGNORE_INTERNAL);
            }
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
    protected override GraphicsBuffer CreateBufferCore(in BufferDescription description, void* initialData) => new D3D12GraphicsBuffer(this, in description, initialData);
    protected override Texture CreateTextureCore(in TextureDescription description, void* initialData) => new D3D12Texture(this, in description, initialData);
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

            using ComPtr<IDXGIFactory4> dxgiFactory = default;

            if (CreateDXGIFactory1(__uuidof<IDXGIFactory2>(), dxgiFactory.GetVoidAddressOf()).FAILED)
            {
                return false;
            }

            using ComPtr<IDXGIAdapter1> dxgiAdapter = default;
            bool foundCompatibleDevice = false;
            for (uint adapterIndex = 0;
                dxgiFactory.Get()->EnumAdapters1(adapterIndex, dxgiAdapter.ReleaseAndGetAddressOf()).SUCCEEDED;
                adapterIndex++)
            {
                DXGI_ADAPTER_DESC1 adapterDesc;
                ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&adapterDesc));

                if ((adapterDesc.Flags & (uint)DXGI_ADAPTER_FLAG_SOFTWARE) != 0u)
                {
                    // Don't select the Basic Render Driver adapter.
                    continue;
                }

                // Check to see if the adapter supports Direct3D 12, but don't create the actual device.
                if (D3D12CreateDevice(dxgiAdapter.AsIUnknown().Get(), D3D_FEATURE_LEVEL_12_0,
                    __uuidof<ID3D12Device>(), null).SUCCEEDED)
                {
                    foundCompatibleDevice = true;
                    break;
                }
            }

            if (!foundCompatibleDevice)
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

}
