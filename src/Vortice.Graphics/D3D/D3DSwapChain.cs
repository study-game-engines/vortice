// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using SharpGen.Runtime;
using Vortice.Direct3D12;
using Vortice.DXGI;
using static Vortice.Graphics.D3D.Utils;

namespace Vortice.Graphics.D3D;

internal abstract class D3DSwapChain : SwapChain
{
    private readonly bool _tearingSupported;
    private readonly int _syncInterval = 1;
    private readonly PresentFlags _presentFlags = PresentFlags.None;

    public D3DSwapChain(GraphicsDevice device, in SwapChainSource source, in SwapChainDescriptor descriptor, IDXGIFactory2 dxgiFactory, IUnknown deviceOrCommandQueue)
        : base(device, descriptor)
    {
        // Check tearing support
        {
            IDXGIFactory5? dxgiFactory5 = dxgiFactory.QueryInterfaceOrNull<IDXGIFactory5>();
            if (dxgiFactory5 != null)
            {
                _tearingSupported = dxgiFactory5.PresentAllowTearing;
                dxgiFactory5.Dispose();
            }
        }

        BackBufferCount = PresentModeToBufferCount(descriptor.PresentMode);

        SwapChainDescription1 swapChainDesc = new()
        {
            Width = descriptor.Size.Width,
            Height = descriptor.Size.Height,
            Format = ToDXGISwapChainFormat(descriptor.ColorFormat),
            Stereo = false,
            SampleDescription = new(1, 0),
            BufferUsage = Usage.RenderTargetOutput,
            BufferCount = BackBufferCount,
            Scaling = Scaling.Stretch,
            SwapEffect = SwapEffect.FlipDiscard,
            AlphaMode = AlphaMode.Ignore,
            Flags = _tearingSupported ? SwapChainFlags.AllowTearing : SwapChainFlags.None
        };

        switch (source.Type)
        {
            case SwapChainSourceType.Win32:
                Win32SwapChainSource win32Source = (Win32SwapChainSource)source;
                SwapChainFullscreenDescription fsSwapChainDesc = new()
                {
                    Windowed = descriptor.IsFullscreen ? false : true
                };

                Handle = dxgiFactory.CreateSwapChainForHwnd(
                    deviceOrCommandQueue,
                    win32Source.Hwnd,
                    swapChainDesc,
                    fsSwapChainDesc,
                    null);

                // This class does not support exclusive full-screen mode and prevents DXGI from responding to the ALT+ENTER shortcut
                dxgiFactory.MakeWindowAssociation(win32Source.Hwnd, WindowAssociationFlags.IgnoreAltEnter).CheckError();
                break;

            case SwapChainSourceType.CoreWindow:
                CoreWindowChainSource coreSource = (CoreWindowChainSource)source;
                using (ComObject coreWindowIUnknown = new(coreSource))
                {
                    Handle = dxgiFactory.CreateSwapChainForCoreWindow(
                        deviceOrCommandQueue,
                        coreWindowIUnknown,
                        swapChainDesc,
                        null);
                }

                break;

            default:
                throw new GraphicsException("Surface not supported");
        }

        Handle3 = Handle.QueryInterfaceOrNull<IDXGISwapChain3>();

        _syncInterval = PresentModeToSwapInterval(descriptor.PresentMode);
        if (!descriptor.IsFullscreen
            && _syncInterval == 0
            && _tearingSupported)
        {
            _presentFlags |= PresentFlags.AllowTearing;
        }

        AfterReset();
    }

    // <inheritdoc />
    public override int BackBufferCount { get; }

    public IDXGISwapChain1 Handle { get; }
    public IDXGISwapChain3? Handle3 { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Handle3?.Dispose();
            Handle.Dispose();
        }
    }

    /// <inheritdoc />
    public override void Present()
    {
        Handle.Present(_syncInterval, _presentFlags).CheckError();
    }

    protected virtual void AfterReset()
    {
        SwapChainDescription1 swapChainDesc = Handle.Description1;

        Size = new(swapChainDesc.Width, swapChainDesc.Height);

        // Handle color space settings for HDR
        UpdateColorSpace();
    }

    private void UpdateColorSpace()
    {
        //DXGI_COLOR_SPACE_TYPE colorSpace = DXGI_COLOR_SPACE_RGB_FULL_G22_NONE_P709;

        bool isDisplayHDR10 = false;

        if (Handle != null)
        {
            // Until - we fix this
            //IDXGIOutput output = Handle.GetContainingOutput();
            //if (SUCCEEDED(Handle.GetContainingOutput(output.GetAddressOf())))
            //{
            //    ComPtr<IDXGIOutput6> output6;
            //    if (SUCCEEDED(output.As(&output6)))
            //    {
            //        DXGI_OUTPUT_DESC1 desc;
            //        ThrowIfFailed(output6->GetDesc1(&desc));
            //
            //        if (desc.ColorSpace == DXGI_COLOR_SPACE_RGB_FULL_G2084_NONE_P2020)
            //        {
            //            // Display output is HDR10.
            //            isDisplayHDR10 = true;
            //        }
            //    }
            //}
        }
    }
}
