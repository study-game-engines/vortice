// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using Win32;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi;
using static Win32.Apis;
using static Vortice.Graphics.D3D11.D3D11Utils;
using static Vortice.Graphics.D3DCommon.D3DUtils;
using Win32.Graphics.Direct3D;
using static Win32.Graphics.Direct3D11.Apis;
using static Win32.Graphics.Dxgi.Apis;
using DxgiInfoQueueFilter = Win32.Graphics.Dxgi.InfoQueueFilter;
using D3DFeature = Win32.Graphics.Direct3D11.Feature;
using MessageId = Win32.Graphics.Direct3D11.MessageId;
using Vortice.Graphics.D3DCommon;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11GraphicsDevice : GraphicsDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    public static bool IsSupported() => s_isSupported.Value;

    private readonly ComPtr<IDXGIFactory2> _dxgiFactory;
    private readonly ComPtr<ID3D11Device1> _device = default;
    private readonly ComPtr<ID3D11DeviceContext1> _immediateContext;

    private readonly GraphicsAdapterInfo _adapterInfo;
    private readonly GraphicsDeviceLimits _limits;
    private readonly FeatureLevel _featureLevel;

    private readonly void* _commandBufferAcquisitionMutex;
    private readonly void* _contextLock;

    private readonly List<D3D11CommandBuffer> _commandBuffersPool = new();
    private readonly Queue<D3D11CommandBuffer> _availableCommandBuffers = new();

    public D3D11GraphicsDevice(in GraphicsDeviceDescription description)
        : base(GraphicsBackend.Direct3D11)
    {
        Guard.IsTrue(IsSupported(), nameof(D3D11GraphicsDevice), "Direct3D11 is not supported");

        _commandBufferAcquisitionMutex = NativeMemory.Alloc((nuint)sizeof(void*));
        Kernel32.InitializeSRWLock(_commandBufferAcquisitionMutex);

        _contextLock = NativeMemory.Alloc((nuint)sizeof(void*));
        Kernel32.InitializeSRWLock(_contextLock);

        uint dxgiDebugFlags = 0u;
        if (description.ValidationMode != ValidationMode.Disabled)
        {
#if DEBUG
            using ComPtr<IDXGIInfoQueue> dxgiInfoQueue = default;
            if (DXGIGetDebugInterface1(0, __uuidof<IDXGIInfoQueue>(), dxgiInfoQueue.GetVoidAddressOf()).Success)
            {
                dxgiDebugFlags = DXGI_CREATE_FACTORY_DEBUG;
                dxgiInfoQueue.Get()->SetBreakOnSeverity(DXGI_DEBUG_ALL, InfoQueueMessageSeverity.Error, true);
                dxgiInfoQueue.Get()->SetBreakOnSeverity(DXGI_DEBUG_ALL, InfoQueueMessageSeverity.Corruption, true);

                int* hide = stackalloc int[1]
                {
                    80 /* IDXGISwapChain::GetContainingOutput: The swapchain's adapter does not control the output on which the swapchain's window resides. */,
                };
                DxgiInfoQueueFilter filter = new()
                {
                    DenyList = new()
                    {
                        NumIDs = 1u,
                        pIDList = hide
                    }
                };
                dxgiInfoQueue.Get()->AddStorageFilterEntries(DXGI_DEBUG_DXGI, &filter);
            }
#endif
        }

        ThrowIfFailed(CreateDXGIFactory2(dxgiDebugFlags, __uuidof<IDXGIFactory2>(), _dxgiFactory.GetVoidAddressOf()));

        // Determines whether tearing support is available for fullscreen borderless windows.
        {
            using ComPtr<IDXGIFactory5> dxgiFactory5 = default;
            if (_dxgiFactory.CopyTo(dxgiFactory5.GetAddressOf()).Success)
            {
                IsTearingSupported = dxgiFactory5.Get()->IsTearingSupported();
            }
        }

        // Get adapter and create device
        {
            using ComPtr<IDXGIAdapter1> dxgiAdapter = default;

            using ComPtr<IDXGIFactory6> dxgiFactory6 = default;
            HResult hr = _dxgiFactory.CopyTo(dxgiFactory6.GetAddressOf());
            if (hr.Success)
            {
                for (uint adapterIndex = 0;
                    dxgiFactory6.Get()->EnumAdapterByGpuPreference(adapterIndex,
                        description.PowerPreference.ToDxgi(),
                        __uuidof<IDXGIAdapter1>(), dxgiAdapter.ReleaseAndGetVoidAddressOf()
                        ).Success;
                    adapterIndex++)
                {
                    AdapterDescription1 desc;
                    ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&desc));

                    if ((desc.Flags & AdapterFlags.Software) != 0)
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
                    _dxgiFactory.Get()->EnumAdapters1(
                        adapterIndex,
                        dxgiAdapter.ReleaseAndGetAddressOf()).Success;
                    ++adapterIndex)
                {
                    AdapterDescription1 desc;
                    ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&desc));

                    if ((desc.Flags & AdapterFlags.Software) != 0)
                    {
                        // Don't select the Basic Render Driver adapter.
                        continue;
                    }

                    break;
                }
            }

            if (dxgiAdapter.Get() is null)
            {
                throw new GraphicsException("Direct3D11: No adapter detected");
            }

            CreateDeviceFlags creationFlags = CreateDeviceFlags.BgraSupport;
            if (description.ValidationMode != ValidationMode.Disabled && SdkLayersAvailable())
            {
                creationFlags |= CreateDeviceFlags.Debug;
            }

            using ComPtr<ID3D11Device> tempDevice = default;
            using ComPtr<ID3D11DeviceContext> tempContext = default;

            ReadOnlySpan<FeatureLevel> featureLevels = stackalloc FeatureLevel[]
            {
                FeatureLevel.Level_11_1,
                FeatureLevel.Level_11_0,
            };

            FeatureLevel d3dFeatureLevel;
            hr = D3D11CreateDevice(
                (IDXGIAdapter*)dxgiAdapter.Get(),
                DriverType.Unknown,
                creationFlags,
                featureLevels,
                tempDevice.GetAddressOf(),
                &d3dFeatureLevel,
                tempContext.GetAddressOf()
            );

            if (hr.Failure)
            {
                // If the initialization fails, fall back to the WARP device.
                // For more information on WARP, see:
                // http://go.microsoft.com/fwlink/?LinkId=286690
                hr = D3D11CreateDevice(
                    null,
                    DriverType.Warp, // Create a WARP device instead of a hardware device.
                    creationFlags,
                    featureLevels,
                    tempDevice.GetAddressOf(),
                    &d3dFeatureLevel,
                    tempContext.GetAddressOf()
                );

                if (hr.Success)
                {
                    Debug.WriteLine("Direct3D11 Adapter - WARP");
                }
            }

            ThrowIfFailed(hr);

            // Configure debug device (if active).
            if (description.ValidationMode != ValidationMode.Disabled)
            {
                using ComPtr<ID3D11Debug> d3d11Debug = default;
                if (tempDevice.CopyTo(d3d11Debug.GetAddressOf()).Success)
                {
                    using ComPtr<ID3D11InfoQueue> d3d11InfoQueue = default;
                    if (d3d11Debug.CopyTo(d3d11InfoQueue.GetAddressOf()).Success)
                    {
                        d3d11InfoQueue.Get()->SetBreakOnSeverity(MessageSeverity.Corruption, true);
                        d3d11InfoQueue.Get()->SetBreakOnSeverity(MessageSeverity.Error, true);

                        uint NumSeverities = 4;
                        //uint NumIDs = 6;
                        MessageSeverity* enabledSeverities = stackalloc MessageSeverity[5];

                        // These severities should be seen all the time
                        enabledSeverities[0] = MessageSeverity.Corruption;
                        enabledSeverities[1] = MessageSeverity.Error;
                        enabledSeverities[2] = MessageSeverity.Warning;
                        enabledSeverities[3] = MessageSeverity.Message;

                        if (description.ValidationMode == ValidationMode.Verbose)
                        {
                            // Verbose only filters
                            enabledSeverities[4] = MessageSeverity.Info;
                            NumSeverities++;
                        }

                        uint NumIDs = 1;
                        MessageId* disabledMessages = stackalloc MessageId[10];
                        disabledMessages[0] = MessageId.SetPrivateDataChangingParams;

                        Win32.Graphics.Direct3D11.InfoQueueFilter filter = new();
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
            AdapterDescription1 adapterDesc;
            ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&adapterDesc));

            FeatureDataArchitectureInfo architectureInfo = _device.Get()->CheckFeatureSupport<FeatureDataArchitectureInfo>(D3DFeature.ArchitectureInfo);
            FeatureDataD3D11Options options = _device.Get()->CheckFeatureSupport<FeatureDataD3D11Options>(D3DFeature.Options);
            FeatureDataD3D11Options1 options1 = _device.Get()->CheckFeatureSupport<FeatureDataD3D11Options1>(D3DFeature.Options1);
            FeatureDataD3D11Options2 options2 = _device.Get()->CheckFeatureSupport<FeatureDataD3D11Options2>(D3DFeature.Options2);

            // Detect adapter type.
            GpuAdapterType adapterType = GpuAdapterType.Other;
            if ((adapterDesc.Flags & AdapterFlags.Software) != 0)
            {
                adapterType = GpuAdapterType.Cpu;
            }
            else
            {
                adapterType = options2.UnifiedMemoryArchitecture ? GpuAdapterType.IntegratedGpu : GpuAdapterType.DiscreteGpu;
            }

            // Convert the adapter's D3D12 driver version to a readable string like "24.21.13.9793".
            string driverDescription = string.Empty;
            LargeInteger umdVersion = default;
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

    public IDXGIFactory2* DXGIFactory => _dxgiFactory;

    public bool IsTearingSupported { get; }
    public ID3D11Device1* NativeDevice => _device;
    public ID3D11DeviceContext1* ImmediateContext => _immediateContext;
    public FeatureLevel FeatureLevel => _featureLevel;

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
                if (_device.CopyTo(d3d11Debug.GetAddressOf()).Success)
                {
                    d3d11Debug.Get()->ReportLiveDeviceObjects(ReportLiveDeviceObjectFlags.Detail | ReportLiveDeviceObjectFlags.IgnoreInternal);
                }
            }
#else
            _device.Dispose();
#endif

            _dxgiFactory.Dispose();
#if DEBUG
            using ComPtr<IDXGIDebug1> dxgiDebug = default;
            if (DXGIGetDebugInterface1(0, __uuidof<IDXGIDebug1>(), dxgiDebug.GetVoidAddressOf()).Success)
            {
                dxgiDebug.Get()->ReportLiveObjects(DXGI_DEBUG_ALL, ReportLiveObjectFlags.Summary | ReportLiveObjectFlags.IgnoreInternal);
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
        Kernel32.AcquireSRWLockExclusive(_commandBufferAcquisitionMutex);

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

        Kernel32.ReleaseSRWLockExclusive(_commandBufferAcquisitionMutex);
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
                Kernel32.AcquireSRWLockExclusive(_contextLock);
                ImmediateContext->ExecuteCommandList(commandBuffer.CommandList, false);
                Kernel32.ReleaseSRWLockExclusive(_contextLock);
            }

            // Mark the command buffer as not-recording so that it can be used to record again. 
            {
                Kernel32.AcquireSRWLockExclusive(_commandBufferAcquisitionMutex);
                commandBuffer.IsRecording = false;
                _availableCommandBuffers.Enqueue(commandBuffer);
                Kernel32.ReleaseSRWLockExclusive(_commandBufferAcquisitionMutex);
            }

            // Present acquired SwapChains 
            {
                Kernel32.AcquireSRWLockExclusive(_contextLock);
                commandBuffer.PresentSwapChains();
                Kernel32.ReleaseSRWLockExclusive(_contextLock);
            }
        }
    }

    /// <inheritdoc />
    protected override GraphicsBuffer CreateBufferCore(in BufferDescription description, void* initialData)
    {
        return new D3D11Buffer(this, description, initialData);
    }

    /// <inheritdoc />
    protected override Texture CreateTextureCore(in TextureDescription description, void* initialData)
    {
        return new D3D11Texture(this, description, initialData);
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
            HResult hr = CreateDXGIFactory2(0u, __uuidof<IDXGIFactory2>(), dxgiFactory.GetVoidAddressOf());
            if (hr.Failure)
            {
                return false;
            }

            using ComPtr<IDXGIAdapter1> dxgiAdapter = default;

            bool foundCompatibleDevice = false;
            for (uint adapterIndex = 0;
                dxgiFactory.Get()->EnumAdapters1(adapterIndex, dxgiAdapter.ReleaseAndGetAddressOf()).Success;
                adapterIndex++)
            {
                AdapterDescription1 desc;
                ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&desc));

                if ((desc.Flags & AdapterFlags.Software) != 0)
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
