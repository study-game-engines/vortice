// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.DirectX.DXGI_ADAPTER_FLAG;
using static TerraFX.Interop.DirectX.D3D_FEATURE_LEVEL;
using System.Diagnostics;

namespace Vortice.Graphics;

public sealed unsafe class D3D12GraphicsDevice : GraphicsDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);
    private readonly GraphicsDeviceCaps _caps;

    public D3D12GraphicsDevice(ValidationMode validationMode = ValidationMode.Disabled, GpuPowerPreference powerPreference = GpuPowerPreference.HighPerformance)
    {
        if (!s_isSupported.Value)
        {
            throw new InvalidOperationException("Vulkan is not supported");
        }

        if (validationMode != ValidationMode.Disabled)
        {
            using ComPtr<ID3D12Debug> d3D12Debug = default;

            if (D3D12GetDebugInterface(__uuidof<ID3D12Debug>(), d3D12Debug.GetVoidAddressOf()).SUCCEEDED)
            {
                d3D12Debug.Get()->EnableDebugLayer();

                if (validationMode == ValidationMode.GPU)
                {
                    using ComPtr<ID3D12Debug1> d3D12Debug1 = default;
                    if (d3D12Debug.CopyTo(d3D12Debug1.GetAddressOf()).SUCCEEDED)
                    {
                        d3D12Debug1.Get()->SetEnableGPUBasedValidation(true);
                        d3D12Debug1.Get()->SetEnableSynchronizedCommandQueueValidation(true);
                    }

                    using ComPtr<ID3D12Debug2> d3d12Debug2 = default;

                    if (d3D12Debug.CopyTo(d3d12Debug2.GetAddressOf()).SUCCEEDED)
                    {
                        d3d12Debug2.Get()->SetGPUBasedValidationFlags(D3D12_GPU_BASED_VALIDATION_FLAGS.D3D12_GPU_BASED_VALIDATION_FLAGS_NONE);
                    }
                }
            }
            else
            {
                Debug.WriteLine("WARNING: Direct3D Debug Device is not available\n");
            }
        }
    }

    /// <inheritdoc />
    public override void WaitIdle()
    {
    }

    // <inheritdoc />
    public override GpuBackend BackendType => GpuBackend.Direct3D12;

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
    }

    private static bool CheckIsSupported()
    {
        try
        {
            using ComPtr<IDXGIFactory4> dxgiFactory4 = default;

            HRESULT hr = CreateDXGIFactory2(0, __uuidof<IDXGIFactory4>(), (void**)dxgiFactory4.GetAddressOf());

            if (hr.FAILED)
            {
                return false;
            }

            using ComPtr<IDXGIAdapter1> dxgiAdapter = default;

            bool foundCompatibleDevice = false;
            for (uint i = 0; DXGI_ERROR_NOT_FOUND != dxgiFactory4.Get()->EnumAdapters1(i, dxgiAdapter.ReleaseAndGetAddressOf()); ++i)
            {
                DXGI_ADAPTER_DESC1 adapterDesc;
                ThrowIfFailed(dxgiAdapter.Get()->GetDesc1(&adapterDesc));

                // Don't select the Basic Render Driver adapter.
                if ((adapterDesc.Flags & (uint)DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
                {
                    continue;
                }

                // Check to see if the adapter supports Direct3D 12,
                // but don't create the actual device.
                if (D3D12CreateDevice((IUnknown*)dxgiAdapter.Get(), D3D_FEATURE_LEVEL_12_0, __uuidof<ID3D12Device>(), null).SUCCEEDED)
                {
                    foundCompatibleDevice = true;
                    break;
                }
            }

            if (foundCompatibleDevice)
            {
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    protected override GraphicsBuffer CreateBufferCore(in BufferDescriptor descriptor, IntPtr initialData) => throw new NotImplementedException();
    /// <inheritdoc />
    protected override Texture CreateTextureCore(in TextureDescriptor descriptor) => throw new NotImplementedException();
    /// <inheritdoc />
    protected override SwapChain CreateSwapChainCore(in SwapChainSource source, in SwapChainDescriptor descriptor) => throw new NotImplementedException();
}
