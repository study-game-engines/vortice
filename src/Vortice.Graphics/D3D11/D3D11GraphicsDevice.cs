// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using static Vortice.Graphics.D3DCommon.D3DUtils;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.DirectX.DXGI_INFO_QUEUE_MESSAGE_SEVERITY;
using static TerraFX.Interop.DirectX.DXGI_FEATURE;
using System.Diagnostics;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11GraphicsDevice : GraphicsDevice
{
    public const uint D3D11_REQ_TEXTURE1D_U_DIMENSION = 16384u;
    public const uint D3D11_REQ_TEXTURE2D_U_OR_V_DIMENSION = 16384u;
    public const uint D3D11_REQ_TEXTURE3D_U_V_OR_W_DIMENSION = 2048u;
    public const uint D3D11_REQ_TEXTURE2D_ARRAY_AXIS_DIMENSION = 2048u;
    public const uint D3D11_REQ_IMMEDIATE_CONSTANT_BUFFER_ELEMENT_COUNT = 4096u;

    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    private static readonly D3D_FEATURE_LEVEL[] s_featureLevels = new[]
    {
        D3D_FEATURE_LEVEL_11_1,
        D3D_FEATURE_LEVEL_11_0
    };

    public static bool IsSupported() => s_isSupported.Value;

    private readonly ComPtr<IDXGIFactory2> _dxgiFactory2;

    private readonly GraphicsAdapterInfo _adapterInfo;
    private readonly GraphicsDeviceLimits _limits;
    private readonly D3D_FEATURE_LEVEL _featureLevel;

    private readonly SRWLOCK* _commandBufferAcquisitionMutex;
    private readonly SRWLOCK* _contextLock;

    private readonly List<D3D11CommandBuffer> _commandBuffersPool = new();
    private readonly Queue<D3D11CommandBuffer> _availableCommandBuffers = new();

    public D3D11GraphicsDevice(ValidationMode validationMode)
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
            if (OperatingSystem.IsWindowsVersionAtLeast(8, 1))
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
        }

        if (OperatingSystem.IsWindowsVersionAtLeast(8, 1))
        {
            ThrowIfFailed(CreateDXGIFactory2(dxgiDebugFlags, __uuidof<IDXGIFactory2>(), _dxgiFactory2.GetVoidAddressOf()));
        }
        else
        {
            ThrowIfFailed(CreateDXGIFactory1(__uuidof<IDXGIFactory2>(), _dxgiFactory2.GetVoidAddressOf()));
        }

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
#if TODO

        IDXGIAdapter1? adapter = default;

        using (IDXGIFactory6? dxgiFactory6 = DXGIFactory.QueryInterfaceOrNull<IDXGIFactory6>())
        {
            if (dxgiFactory6 != null)
            {
                for (int adapterIndex = 0; dxgiFactory6.EnumAdapterByGpuPreference(adapterIndex,
                    GpuPreference.HighPerformance, out adapter).Success; adapterIndex++)
                {
                    AdapterDescription1 desc = adapter!.Description1;

                    // Don't select the Basic Render Driver adapter.
                    if ((desc.Flags & AdapterFlags.Software) != AdapterFlags.None)
                    {
                        adapter.Dispose();

                        continue;
                    }

                    break;
                }
            }
            else
            {
                for (int adapterIndex = 0; DXGIFactory.EnumAdapters1(adapterIndex, out adapter).Success; adapterIndex++)
                {
                    AdapterDescription1 desc = adapter.Description1;

                    // Don't select the Basic Render Driver adapter.
                    if ((desc.Flags & AdapterFlags.Software) != AdapterFlags.None)
                    {
                        adapter.Dispose();

                        continue;
                    }

                    break;
                }
            }
        }

        if (adapter == null)
            throw new GraphicsException("D3D11: No adapter detected");

        // Create device now
        {
            DeviceCreationFlags creationFlags = DeviceCreationFlags.BgraSupport;
            if (validationMode != ValidationMode.Disabled && SdkLayersAvailable())
            {
                creationFlags |= DeviceCreationFlags.Debug;
            }

            if (D3D11CreateDevice(
                adapter,
                DriverType.Unknown,
                creationFlags,
                s_featureLevels,
                out ID3D11Device tempDevice, out _featureLevel, out ID3D11DeviceContext tempContext).Failure)
            {
                // If the initialization fails, fall back to the WARP device.
                // For more information on WARP, see:
                // http://go.microsoft.com/fwlink/?LinkId=286690
                D3D11CreateDevice(
                    IntPtr.Zero,
                    DriverType.Warp,
                    creationFlags,
                    s_featureLevels,
                    out tempDevice, out _featureLevel, out tempContext).CheckError();
            }

            NativeDevice = tempDevice.QueryInterface<ID3D11Device1>();
            ImmediateContext = tempContext.QueryInterface<ID3D11DeviceContext1>();
            tempContext.Dispose();
            tempDevice.Dispose();
        }

        // Configure debug device (if active).
        if (validationMode != ValidationMode.Disabled)
        {
            ID3D11Debug? d3dDebug = NativeDevice.QueryInterfaceOrNull<ID3D11Debug>();
            if (d3dDebug != null)
            {
                ID3D11InfoQueue? d3dInfoQueue = d3dDebug.QueryInterfaceOrNull<ID3D11InfoQueue>();
                if (d3dInfoQueue != null)
                {
                    d3dInfoQueue!.SetBreakOnSeverity(MessageSeverity.Corruption, true);
                    d3dInfoQueue!.SetBreakOnSeverity(MessageSeverity.Error, true);

                    List<MessageSeverity> enabledSeverities = new()
                    {
                        // These severities should be seen all the time
                        MessageSeverity.Corruption,
                        MessageSeverity.Error,
                        MessageSeverity.Warning,
                        MessageSeverity.Message
                    };

                    if (validationMode == ValidationMode.Verbose)
                    {
                        // Verbose only filters
                        enabledSeverities.Add(MessageSeverity.Info);
                    }

                    List<MessageId> disabledMessages = new();
                    disabledMessages.Add(MessageId.SetPrivateDataChangingParams);

                    Direct3D11.Debug.InfoQueueFilter filter = new()
                    {
                        AllowList = new Direct3D11.Debug.InfoQueueFilterDescription()
                        {
                            Severities = enabledSeverities.ToArray()
                        },
                        DenyList = new Direct3D11.Debug.InfoQueueFilterDescription
                        {
                            Ids = disabledMessages.ToArray()
                        }
                    };

                    // Clear out the existing filters since we're taking full control of them
                    d3dInfoQueue.PushEmptyStorageFilter();

                    d3dInfoQueue.AddStorageFilterEntries(filter);
                    d3dInfoQueue.AddApplicationMessage(MessageSeverity.Message, "D3D11 Debug Filters setup");
                    d3dInfoQueue.Dispose();
                }

                d3dDebug.Dispose();
            }
        }

        // Init capabilites.
        AdapterDescription1 adapterDesc = adapter.Description1;
        FeatureDataArchitectureInfo architectureInfo = NativeDevice.CheckFeatureArchitectureInfo();
        var options = NativeDevice.CheckFeatureOptions();
        var options1 = NativeDevice.CheckFeatureOptions1();
        var options2 = NativeDevice.CheckFeatureOptions2();
        var options3 = NativeDevice.CheckFeatureOptions3();

        // Detect adapter type.
        GpuAdapterType adapterType = GpuAdapterType.Other;
        if ((adapterDesc.Flags & AdapterFlags.Software) != AdapterFlags.None)
        {
            adapterType = GpuAdapterType.Cpu;
        }
        else
        {
            adapterType = options2.UnifiedMemoryArchitecture ? GpuAdapterType.IntegratedGpu : GpuAdapterType.DiscreteGpu;
        }

        // Convert the adapter's D3D12 driver version to a readable string like "24.21.13.9793".
        string driverDescription = string.Empty;
        if (adapter.CheckInterfaceSupport<IDXGIDevice>(out long umdVersion))
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
            DeviceId = (uint)adapterDesc.DeviceId,
            AdapterName = adapterDesc.Description,
            DriverDescription = driverDescription,
            AdapterType = adapterType,
        }; 
#endif

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
            //MaxVertexAttributes = 16,
            //MaxVertexBindings = 16,
            //MaxVertexAttributeOffset = 2047,
            //MaxVertexBindingStride = 2048,
            //MaxTextureDimension1D = D3D11_REQ_TEXTURE1D_U_DIMENSION,
            //MaxTextureDimension2D = D3D11_REQ_TEXTURE2D_U_OR_V_DIMENSION,
            //MaxTextureDimension3D = D3D11_REQ_TEXTURE3D_U_V_OR_W_DIMENSION,
            //MaxTextureDimensionCube = D3D11_REQ_TEXTURE2D_ARRAY_AXIS_DIMENSION,
            //MaxTextureArrayLayers = 2048,
            //MaxColorAttachments = 8,
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

        //adapter.Dispose();
    }

    public IDXGIFactory2* DXGIFactory => _dxgiFactory2;

    public bool IsTearingSupported { get; }
    //public ID3D11Device1 NativeDevice { get; }
    //public ID3D11DeviceContext1 ImmediateContext { get; }
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
            //ImmediateContext.Flush();
            //ImmediateContext.Dispose();

            for (int i = 0; i < _commandBuffersPool.Count; ++i)
            {
                _commandBuffersPool[i].Dispose();
            }
            _commandBuffersPool.Clear();

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

            _dxgiFactory2.Dispose();

#if DEBUG
            //using ComPtr<IDXGIDebug1> dxgiDebug = default;
            //if (DXGIGetDebugInterface1(0, __uuidof<IDXGIDebug1>(), dxgiDebug.GetVoidAddressOf()).SUCCEEDED)
            //{
            //    dxgiDebug!.ReportLiveObjects(DebugAll, ReportLiveObjectFlags.Summary | ReportLiveObjectFlags.IgnoreInternal);
            //    dxgiDebug!.Dispose();
            //}
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
        //ImmediateContext.Flush();
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
                //ImmediateContext.ExecuteCommandList(commandBuffer.CommandList!, false);
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

    private static bool CheckIsSupported()
    {
        try
        {
            if (!OperatingSystem.IsWindows())
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
