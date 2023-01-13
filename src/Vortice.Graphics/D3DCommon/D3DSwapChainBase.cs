// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Win32;
using Win32.Graphics.Dxgi;
using static Win32.Apis;
using static Vortice.Graphics.D3DCommon.D3DUtils;

namespace Vortice.Graphics.D3DCommon;

internal abstract unsafe class D3DSwapChainBase : SwapChain
{
    private readonly ComPtr<IDXGISwapChain1> _handle;

    protected D3DSwapChainBase(IDXGIFactory2* factory, bool tearingSupported, IUnknown* deviceOrCommandQueue,
        GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device, surface, description)
    {
        SwapChainDescription1 swapChainDesc = new()
        {
            Width = (uint)description.Width,
            Height = (uint)description.Height,
            Format = ToDXGISwapChainFormat(description.Format),
            Stereo = false,
            SampleDesc = new(1, 0),
            BufferUsage = Usage.RenderTargetOutput,
            BufferCount = PresentModeToBufferCount(description.PresentMode),
            Scaling = Scaling.Stretch,
            SwapEffect = SwapEffect.FlipDiscard,
            AlphaMode = Win32.Graphics.Dxgi.Common.AlphaMode.Ignore,
            Flags = tearingSupported ? SwapChainFlags.AllowTearing : SwapChainFlags.None
        };

        switch (surface)
        {
#if WINDOWS_UWP
            case CoreWindowSwapChainSurface coreWindowSurface:
                swapChainDesc.Scaling = Scaling.Stretch;

                using (ComObject comObject = new(coreWindowSurface.CoreWindow))
                {
                    using IDXGISwapChain1 tempSwapChain = factory.CreateSwapChainForCoreWindow(
                        deviceOrCommandQueue,
                        comObject,
                        swapChainDesc);

                    Handle = tempSwapChain.QueryInterface<IDXGISwapChain3>();
                }

                break;

            case SwapChainPanelSwapChainSurface swapChainPanelSurface:
                using (ComObject comObject = new ComObject(swapChainPanelSurface.Panel))
                {
                    using ISwapChainPanelNative nativePanel = comObject.QueryInterface<ISwapChainPanelNative>();
                    using IDXGISwapChain1 tempSwapChain = factory.CreateSwapChainForComposition(deviceOrCommandQueue, swapChainDesc);

                    Handle = tempSwapChain.QueryInterface<IDXGISwapChain3>();
                    nativePanel.SetSwapChain(Handle);
                    Handle.MatrixTransform = new System.Numerics.Matrix3x2
                    {
                        M11 = 1.0f / swapChainPanelSurface.Panel.CompositionScaleX,
                        M22 = 1.0f / swapChainPanelSurface.Panel.CompositionScaleY
                    };
                }

                break;
#else
            case Win32SwapChainSurface win32Source:
                SwapChainFullscreenDescription fsSwapChainDesc = new()
                {
                    Windowed = !description.IsFullscreen
                };

                ThrowIfFailed(factory->CreateSwapChainForHwnd(
                    deviceOrCommandQueue,
                    win32Source.Hwnd,
                    &swapChainDesc,
                    &fsSwapChainDesc,
                    null,
                    _handle.GetAddressOf())
                    );

                // This class does not support exclusive full-screen mode and prevents DXGI from responding to the ALT+ENTER shortcut
                ThrowIfFailed(factory->MakeWindowAssociation(win32Source.Hwnd, WindowAssociationFlags.NoAltEnter));
                break;
#endif

            default:
                throw new GraphicsException("Surface not supported");
        }
    }

    public IDXGISwapChain1* Handle => _handle;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            _handle.Dispose();
        }
    }

    public bool NeedResize()
    {
        // Check for window size changes and resize the swapchain if needed.
        SwapChainDescription1 swapChainDesc;
        ThrowIfFailed(_handle.Get()->GetDesc1(&swapChainDesc));
        if (swapChainDesc.Width != DrawableSize.Width ||
            swapChainDesc.Height != DrawableSize.Height)
        {
            return true;
        }

        return false;
    }
}
