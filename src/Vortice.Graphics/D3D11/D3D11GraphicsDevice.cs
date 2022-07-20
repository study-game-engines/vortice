// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.D3D11;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static TerraFX.Interop.DirectX.D3D_DRIVER_TYPE;
using static Vortice.Graphics.D3DCommon.D3DUtils;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.DirectX.DXGI_INFO_QUEUE_MESSAGE_SEVERITY;
using static TerraFX.Interop.DirectX.DXGI_FEATURE;
using static TerraFX.Interop.DirectX.DXGI_ADAPTER_FLAG;
using static TerraFX.Interop.DirectX.D3D11_CREATE_DEVICE_FLAG;
using static TerraFX.Interop.DirectX.D3D11_RLDO_FLAGS;
using static TerraFX.Interop.DirectX.D3D11_MESSAGE_SEVERITY;
using static TerraFX.Interop.DirectX.D3D11_MESSAGE_ID;
using static TerraFX.Interop.DirectX.D3D11_FEATURE;
using static Vortice.Graphics.D3D11.D3D11Utils;
using System.Diagnostics;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11GraphicsDevice : GraphicsDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    public static bool IsSupported() => s_isSupported.Value;

    private readonly ComPtr<IDXGIFactory2> _dxgiFactory2;
    private readonly ComPtr<ID3D11Device1> _device;
    private readonly ComPtr<ID3D11DeviceContext1> _immediateContext;

    private readonly GraphicsAdapterInfo _adapterInfo;
    private readonly GraphicsDeviceLimits _limits;
    private readonly D3D_FEATURE_LEVEL _featureLevel;

    private readonly SRWLOCK* _commandBufferAcquisitionMutex;
    private readonly SRWLOCK* _contextLock;

    private readonly List<D3D11CommandBuffer> _commandBuffersPool = new();
    private readonly Queue<D3D11CommandBuffer> _availableCommandBuffers = new();

    public D3D11GraphicsDevice(ValidationMode validationMode, GpuPowerPreference powerPreference)
        : base(GraphicsBackend.Direct3D11)
    {
        Guard.IsTrue(IsSupported(), nameof(D3D11GraphicsDevice), "Direct3D11 is not supported");

        _commandBufferAcquisitionMutex = (SRWLOCK*)NativeMemory.Alloc((nuint)sizeof(SRWLOCK));
        InitializeSRWLock(_commandBufferAcquisitionMutex);

        _contextLock = (SRWLOCK*)NativeMemory.Alloc((nuint)sizeof(SRWLOCK));
        InitializeSRWLock(_contextLock);

        uint dxgiDebugFlags = 0u;
        if (validationMode != ValidationMode.Disabled)
        {
            using ComPtr<IDXGIInfoQueue> dxgiInfoQueue = default;
            if (DXGIGetDebugInterface1(0, __uuidof<IDXGIInfoQueue>(), dxgiInfoQueue.GetVoidAddressOf()).SUCCEEDED)
            {
                dxgiDebugFlags = DXGI_CREATE_FACTORY_DEBUG;
                dxgiInfoQueue.Get()->SetBreakOnSeverity(DXGI_DEBUG_ALL, DXGI_INFO_QUEUE_MESSAGE_SEVERITY_ERROR, true);
                dxgiInfoQueue.Get()->SetBreakOnSeverity(DXGI_DEBUG_ALL, DXGI_INFO_QUEUE_MESSAGE_SEVERITY_CORRUPTION, true);

                int* hide = stackalloc int[1]
                {
                    80 /* IDXGISwapChain::GetContainingOutput: The swapchain's adapter does not control the output on which the swapchain's window resides. */,
                };
                DXGI_INFO_QUEUE_FILTER filter = new();
                filter.DenyList = new DXGI_INFO_QUEUE_FILTER_DESC()
                {
                    NumIDs = 1u,
                    pIDList = hide
                };
                dxgiInfoQueue.Get()->AddStorageFilterEntries(DXGI_DEBUG_DXGI, &filter);
            }
        }

        ThrowIfFailed(CreateDXGIFactory2(dxgiDebugFlags, __uuidof<IDXGIFactory2>(), _dxgiFactory2.GetVoidAddressOf()));

        // Determines whether tearing support is available for fullscreen borderless windows.
        {
            BOOL allowTearing = false;

            using ComPtr<IDXGIFactory5> dxgiFactory5 = default;
            HRESULT hr = _dxgiFactory2.CopyTo(dxgiFactory5.GetAddressOf());
            if (hr.SUCCEEDED)
            {
                hr = dxgiFactory5.Get()->CheckFeatureSupport(DXGI_FEATURE_PRESENT_ALLOW_TEARING, &allowTearing, sizeof(BOOL));
            }

            if (hr.FAILED || !allowTearing)
            {
                IsTearingSupported = false;
                Debug.WriteLine("Direct3D11: Variable refresh rate displays not supported");
            }
            else
            {
                IsTearingSupported = true;
            }
        }

        // Get adapter and create device
        {
            using ComPtr<IDXGIAdapter1> dxgiAdapter = default;

            using ComPtr<IDXGIFactory6> dxgiFactory6 = default;
            HRESULT hr = _dxgiFactory2.CopyTo(dxgiFactory6.GetAddressOf());
            if (hr.SUCCEEDED)
            {
                for (uint adapterIndex = 0;
                    dxgiFactory6.Get()->EnumAdapterByGpuPreference(adapterIndex,
                        ToDXGI(powerPreference),
                        __uuidof<IDXGIAdapter1>(), (void**)dxgiAdapter.ReleaseAndGetAddressOf()
                        ).SUCCEEDED;
                    adapterIndex++)
                {
                    DXGI_ADAPTER_DESC1 desc;
                    ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&desc));

                    if ((desc.Flags & (uint)DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
                    {
                        // Don't select the Basic Render Driver adapter.
                        continue;
                    }

                    break;
                }
            }

            if (dxgiAdapter.Get() == null)
            {
                for (uint adapterIndex = 0;
                    _dxgiFactory2.Get()->EnumAdapters1(
                        adapterIndex,
                        dxgiAdapter.ReleaseAndGetAddressOf()).SUCCEEDED;
                    ++adapterIndex)
                {
                    DXGI_ADAPTER_DESC1 desc;
                    ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&desc));

                    if ((desc.Flags & (uint)DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
                    {
                        // Don't select the Basic Render Driver adapter.
                        continue;
                    }

                    break;
                }
            }

            if (dxgiAdapter.Get() is null)
                throw new GraphicsException("Direct3D11: No adapter detected");

            D3D11_CREATE_DEVICE_FLAG creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;
            if (validationMode != ValidationMode.Disabled && SdkLayersAvailable())
            {
                creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
            }

            using ComPtr<ID3D11Device> tempDevice = default;
            using ComPtr<ID3D11DeviceContext> tempContext = default;

            D3D_FEATURE_LEVEL* featureLevels = stackalloc[]
            {
                D3D_FEATURE_LEVEL_11_1,
                D3D_FEATURE_LEVEL_11_0,
            };

            D3D_FEATURE_LEVEL d3dFeatureLevel;
            hr = D3D11CreateDevice(
                dxgiAdapter.Get(),
               D3D_DRIVER_TYPE_UNKNOWN,
               null,
               (uint)creationFlags,
               featureLevels,
               2,
               D3D11_SDK_VERSION,
               tempDevice.GetAddressOf(),
               &d3dFeatureLevel,
               tempContext.GetAddressOf()
            );

            if (hr.FAILED)
            {
                // If the initialization fails, fall back to the WARP device.
                // For more information on WARP, see:
                // http://go.microsoft.com/fwlink/?LinkId=286690
                hr = D3D11CreateDevice(
                    null,
                    D3D_DRIVER_TYPE_WARP, // Create a WARP device instead of a hardware device.
                    null,
                    (uint)creationFlags,
                    featureLevels,
                    2,
                    D3D11_SDK_VERSION,
                    tempDevice.GetAddressOf(),
                    &d3dFeatureLevel,
                    tempContext.GetAddressOf()
                );

                if (hr.SUCCEEDED)
                {
                    Debug.WriteLine("Direct3D11 Adapter - WARP");
                }
            }

            ThrowIfFailed(hr);

            // Configure debug device (if active).
            if (validationMode != ValidationMode.Disabled)
            {
                using ComPtr<ID3D11Debug> d3d11Debug = default;
                if (tempDevice.CopyTo(d3d11Debug.GetAddressOf()).SUCCEEDED)
                {
                    using ComPtr<ID3D11InfoQueue> d3d11InfoQueue = default;
                    if (d3d11Debug.CopyTo(d3d11InfoQueue.GetAddressOf()).SUCCEEDED)
                    {
                        d3d11InfoQueue.Get()->SetBreakOnSeverity(D3D11_MESSAGE_SEVERITY_CORRUPTION, true);
                        d3d11InfoQueue.Get()->SetBreakOnSeverity(D3D11_MESSAGE_SEVERITY_ERROR, true);

                        uint NumSeverities = 4;
                        //uint NumIDs = 6;
                        D3D11_MESSAGE_SEVERITY* enabledSeverities = stackalloc D3D11_MESSAGE_SEVERITY[5];

                        // These severities should be seen all the time
                        enabledSeverities[0] = D3D11_MESSAGE_SEVERITY_CORRUPTION;
                        enabledSeverities[1] = D3D11_MESSAGE_SEVERITY_ERROR;
                        enabledSeverities[2] = D3D11_MESSAGE_SEVERITY_WARNING;
                        enabledSeverities[3] = D3D11_MESSAGE_SEVERITY_MESSAGE;

                        if (validationMode == ValidationMode.Verbose)
                        {
                            // Verbose only filters
                            enabledSeverities[4] = D3D11_MESSAGE_SEVERITY_INFO;
                            NumSeverities++;
                        }

                        uint NumIDs = 1;
                        D3D11_MESSAGE_ID* disabledMessages = stackalloc D3D11_MESSAGE_ID[10];
                        disabledMessages[0] = D3D11_MESSAGE_ID_SETPRIVATEDATA_CHANGINGPARAMS;

                        D3D11_INFO_QUEUE_FILTER filter = new();
                        filter.AllowList.NumSeverities = NumSeverities;
                        filter.AllowList.pSeverityList = enabledSeverities;
                        filter.DenyList.NumIDs = NumIDs;
                        filter.DenyList.pIDList = disabledMessages;

                        // Clear out the existing filters since we're taking full control of them
                        d3d11InfoQueue.Get()->PushEmptyStorageFilter();

                        d3d11InfoQueue.Get()->AddStorageFilterEntries(&filter);
                    }
                }
            }

            ThrowIfFailed(tempDevice.CopyTo(_device.GetAddressOf()));
            ThrowIfFailed(tempContext.CopyTo(_immediateContext.GetAddressOf()));
            _featureLevel = d3dFeatureLevel;

            // Init capabilites.
            DXGI_ADAPTER_DESC1 adapterDesc;
            ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&adapterDesc));

            D3D11_FEATURE_DATA_ARCHITECTURE_INFO architectureInfo = NativeDevice->CheckFeatureSupport<D3D11_FEATURE_DATA_ARCHITECTURE_INFO>(D3D11_FEATURE_ARCHITECTURE_INFO);
            D3D11_FEATURE_DATA_D3D11_OPTIONS options = NativeDevice->CheckFeatureSupport<D3D11_FEATURE_DATA_D3D11_OPTIONS>(D3D11_FEATURE_D3D11_OPTIONS);
            D3D11_FEATURE_DATA_D3D11_OPTIONS1 options1 = NativeDevice->CheckFeatureSupport<D3D11_FEATURE_DATA_D3D11_OPTIONS1>(D3D11_FEATURE_D3D11_OPTIONS1);
            D3D11_FEATURE_DATA_D3D11_OPTIONS2 options2 = NativeDevice->CheckFeatureSupport<D3D11_FEATURE_DATA_D3D11_OPTIONS2>(D3D11_FEATURE_D3D11_OPTIONS2);

            // Detect adapter type.
            GpuAdapterType adapterType = GpuAdapterType.Other;
            if ((adapterDesc.Flags & (uint)DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
            {
                adapterType = GpuAdapterType.Cpu;
            }
            else
            {
                adapterType = options2.UnifiedMemoryArchitecture ? GpuAdapterType.IntegratedGpu : GpuAdapterType.DiscreteGpu;
            }

            // Convert the adapter's D3D12 driver version to a readable string like "24.21.13.9793".
            string driverDescription = string.Empty;
            LARGE_INTEGER umdVersion = default;
            if (dxgiAdapter.Get()->CheckInterfaceSupport(__uuidof<IDXGIDevice>(), &umdVersion) != DXGI_ERROR_UNSUPPORTED)
            {
                driverDescription = "D3D11 driver version ";

                for (int i = 0; i < 4; ++i)
                {
                    ushort driverVersion = (ushort)((umdVersion >> (48 - 16 * i)) & 0xFFFF);
                    driverDescription += driverVersion + ".";
                }
            }

            _adapterInfo = new()
            {
                VendorId = (GpuVendorId)adapterDesc.VendorId,
                DeviceId = adapterDesc.DeviceId,
                //AdapterName = adapterDesc.Description,
                DriverDescription = driverDescription,
                AdapterType = adapterType,
            };
        }

        //_features = new()
        //{
        //    IndependentBlend = true,
        //    ComputeShader = true,
        //    TessellationShader = true,
        //    MultiViewport = true,
        //    IndexUInt32 = true,
        //    MultiDrawIndirect = true,
        //    FillModeNonSolid = true,
        //    SamplerAnisotropy = true,
        //    TextureCompressionETC2 = false,
        //    TextureCompressionASTC_LDR = false,
        //    TextureCompressionBC = true,
        //    TextureCubeArray = true,
        //    Raytracing = false
        //};

        _limits = new()
        {
            MaxVertexAttributes = 16,
            MaxVertexBuffers = 16,
            //MaxVertexAttributeOffset = 2047,
            MaxVertexBufferArrayStride = 2048,
            MaxTextureDimension1D = D3D11_REQ_TEXTURE1D_U_DIMENSION,
            MaxTextureDimension2D = D3D11_REQ_TEXTURE2D_U_OR_V_DIMENSION,
            MaxTextureDimension3D = D3D11_REQ_TEXTURE3D_U_V_OR_W_DIMENSION,
            MaxTextureDimensionCube = D3D11_REQ_TEXTURE2D_ARRAY_AXIS_DIMENSION,
            MaxTextureArrayLayers = D3D11_REQ_TEXTURE2D_ARRAY_AXIS_DIMENSION,
            MaxColorAttachments = D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT,
            //MaxUniformBufferRange = D3D11_REQ_IMMEDIATE_CONSTANT_BUFFER_ELEMENT_COUNT * 16,
            //MaxStorageBufferRange = uint.MaxValue,
            //MinUniformBufferOffsetAlignment = 256,
            //MinStorageBufferOffsetAlignment = 16,
            //MaxComputeWorkGroupStorageSize = 16,
            //MaxViewports = 16,
            //MaxViewportWidth = 32767,
            //MaxViewportHeight = 32767,
            //MaxTessellationPatchSize = 32,
            //MaxComputeSharedMemorySize = ComputeShaderThreadLocalTempRegisterPool,
            //MaxComputeWorkGroupCountX = ComputeShaderDispatchMaxThreadGroupsPerDimension,
            //MaxComputeWorkGroupCountY = ComputeShaderDispatchMaxThreadGroupsPerDimension,
            //MaxComputeWorkGroupCountZ = ComputeShaderDispatchMaxThreadGroupsPerDimension,
            //MaxComputeWorkGroupInvocations = ComputeShaderThreadGroupMaxThreadsPerGroup,
            //MaxComputeWorkGroupSizeX = ComputeShaderThreadGroupMaxX,
            //MaxComputeWorkGroupSizeY = ComputeShaderThreadGroupMaxY,
            //MaxComputeWorkGroupSizeZ = ComputeShaderThreadGroupMaxZ,
        };
    }

    public IDXGIFactory2* DXGIFactory => _dxgiFactory2;

    public bool IsTearingSupported { get; }
    public ID3D11Device1* NativeDevice => _device;
    public ID3D11DeviceContext1* ImmediateContext => _immediateContext;
    public D3D_FEATURE_LEVEL FeatureLevel => _featureLevel;

    /// <inheritdoc />
    public override GraphicsAdapterInfo AdapterInfo => _adapterInfo;

    /// <inheritdoc />
    public override GraphicsDeviceLimits Limits => _limits;

    /// <summary>
    /// Finalizes an instance of the <see cref="D3D11GraphicsDevice" /> class.
    /// </summary>
    ~D3D11GraphicsDevice() => Dispose(isDisposing: false);

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _immediateContext.Get()->Flush();
            _immediateContext.Dispose();

            for (int i = 0; i < _commandBuffersPool.Count; ++i)
            {
                _commandBuffersPool[i].Dispose();
            }
            _commandBuffersPool.Clear();

#if DEBUG
            uint refCount = NativeDevice->Release();
            if (refCount > 0)
            {
                Debug.WriteLine($"Direct3D11: There are {refCount} unreleased references left on the device");

                using ComPtr<ID3D11Debug> d3d11Debug = default;
                if (_device.CopyTo(d3d11Debug.GetAddressOf()).SUCCEEDED)
                {
                    d3d11Debug.Get()->ReportLiveDeviceObjects(D3D11_RLDO_DETAIL | D3D11_RLDO_IGNORE_INTERNAL);
                }
            }
#else
            _device.Dispose();
#endif

            _dxgiFactory2.Dispose();
#if DEBUG
            using ComPtr<IDXGIDebug1> dxgiDebug = default;
            if (DXGIGetDebugInterface1(0, __uuidof<IDXGIDebug1>(), dxgiDebug.GetVoidAddressOf()).SUCCEEDED)
            {
                dxgiDebug.Get()->ReportLiveObjects(DXGI_DEBUG_ALL, DXGI_DEBUG_RLO_FLAGS.DXGI_DEBUG_RLO_SUMMARY | DXGI_DEBUG_RLO_FLAGS.DXGI_DEBUG_RLO_IGNORE_INTERNAL);
            }
#endif

            NativeMemory.Free(_contextLock);
            NativeMemory.Free(_commandBufferAcquisitionMutex);
        }
    }

    /// <inheritdoc />
    public override bool QueryFeature(Feature feature)
    {
        return false;
    }

    /// <inheritdoc />
    public override void WaitIdle()
    {
        _immediateContext.Get()->Flush();
    }

    /// <inheritdoc />
    public override CommandBuffer BeginCommandBuffer(string? label = null)
    {
        AcquireSRWLockExclusive(_commandBufferAcquisitionMutex);

        /* Try to use an existing command buffer, if one is available. */
        D3D11CommandBuffer commandBuffer;

        if (_availableCommandBuffers.Count == 0)
        {
            commandBuffer = new D3D11CommandBuffer(this);
            _commandBuffersPool.Add(commandBuffer);
        }
        else
        {
            commandBuffer = _availableCommandBuffers.Dequeue();
            commandBuffer.Reset();
        }

        commandBuffer.IsRecording = true;
        commandBuffer.HasLabel = false;
        if (string.IsNullOrEmpty(label) == false)
        {
            commandBuffer.PushDebugGroup(label!);
            commandBuffer.HasLabel = true;
        }

        ReleaseSRWLockExclusive(_commandBufferAcquisitionMutex);
        return commandBuffer;
    }

    /// <inheritdoc />
    protected override void SubmitCommandBuffers(CommandBuffer[] commandBuffers, int count)
    {
        for (int i = 0; i < count; i += 1)
        {
            D3D11CommandBuffer commandBuffer = (D3D11CommandBuffer)commandBuffers[i];

            if (commandBuffer.HasLabel)
            {
                commandBuffer.PopDebugGroup();
            }

            commandBuffer.End();

            // Submit the command list to the immediate context *
            {
                AcquireSRWLockExclusive(_contextLock);
                ImmediateContext->ExecuteCommandList(commandBuffer.CommandList, false);
                ReleaseSRWLockExclusive(_contextLock);
            }

            // Mark the command buffer as not-recording so that it can be used to record again. 
            {
                AcquireSRWLockExclusive(_commandBufferAcquisitionMutex);
                commandBuffer.IsRecording = false;
                _availableCommandBuffers.Enqueue(commandBuffer);
                ReleaseSRWLockExclusive(_commandBufferAcquisitionMutex);
            }

            // Present acquired SwapChains 
            {
                AcquireSRWLockExclusive(_contextLock);
                commandBuffer.PresentSwapChains();
                ReleaseSRWLockExclusive(_contextLock);
            }
        }
    }

    /// <inheritdoc />
    protected override Texture CreateTextureCore(in TextureDescription description)
    {
        return new D3D11Texture(this, description);
    }

    /// <inheritdoc />
    protected override SwapChain CreateSwapChainCore(SwapChainSurface surface, in SwapChainDescription description)
    {
        return new D3D11SwapChain(this, surface, description);
    }

    private static bool CheckIsSupported()
    {
        try
        {
            if (!OperatingSystem.IsWindowsVersionAtLeast(10))
            {
                return false;
            }

            using ComPtr<IDXGIFactory2> dxgiFactory = default;
            HRESULT hr = CreateDXGIFactory2(0u, __uuidof<IDXGIFactory2>(), dxgiFactory.GetVoidAddressOf());
            if (hr.FAILED)
            {
                return false;
            }

            using ComPtr<IDXGIAdapter1> dxgiAdapter = default;

            bool foundCompatibleDevice = false;
            for (uint adapterIndex = 0;
                dxgiFactory.Get()->EnumAdapters1(adapterIndex, dxgiAdapter.ReleaseAndGetAddressOf()).SUCCEEDED;
                adapterIndex++)
            {
                DXGI_ADAPTER_DESC1 desc;
                ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&desc));

                if ((desc.Flags & (uint)DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
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

            return true;
        }
        catch
        {
            return false;
        }
    }
}
