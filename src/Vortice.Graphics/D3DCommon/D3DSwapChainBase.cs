// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static Vortice.Graphics.D3DCommon.D3DUtils;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.DirectX.DXGI_SCALING;
using static TerraFX.Interop.DirectX.DXGI_SWAP_EFFECT;
using static TerraFX.Interop.DirectX.DXGI_ALPHA_MODE;
using static TerraFX.Interop.DirectX.DXGI_SWAP_CHAIN_FLAG;

namespace Vortice.Graphics.D3DCommon;

internal abstract unsafe class D3DSwapChainBase : SwapChain
{
    private readonly ComPtr<IDXGISwapChain1> _handle;

    protected D3DSwapChainBase(IDXGIFactory2* factory, bool tearingSupported, IUnknown* deviceOrCommandQueue,
        GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device, surface, description)
    {
        DXGI_SWAP_CHAIN_DESC1 swapChainDesc = new()
        {
            Width = (uint)description.Width,
            Height = (uint)description.Height,
            Format = ToDXGISwapChainFormat(description.Format),
            Stereo = false,
            SampleDesc = new(1, 0),
            BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
            BufferCount = PresentModeToBufferCount(description.PresentMode),
            Scaling = DXGI_SCALING_STRETCH,
            SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
            AlphaMode = DXGI_ALPHA_MODE_IGNORE,
            Flags = tearingSupported ? (uint)DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING : 0u
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
                DXGI_SWAP_CHAIN_FULLSCREEN_DESC fsSwapChainDesc = new()
                {
                    Windowed = !description.IsFullscreen
                };

                factory->CreateSwapChainForHwnd(
                    deviceOrCommandQueue,
                    win32Source.Hwnd.ToPointer(),
                    &swapChainDesc,
                    &fsSwapChainDesc,
                    null,
                    _handle.GetAddressOf());

                // This class does not support exclusive full-screen mode and prevents DXGI from responding to the ALT+ENTER shortcut
                ThrowIfFailed(factory->MakeWindowAssociation(win32Source.Hwnd.ToPointer(), DXGI_MWA_NO_ALT_ENTER));
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
        DXGI_SWAP_CHAIN_DESC1 swapChainDesc;
        ThrowIfFailed(_handle.Get()->GetDesc1(&swapChainDesc));
        if (swapChainDesc.Width != DrawableSize.Width ||
            swapChainDesc.Height != DrawableSize.Height)
        {
            return true;
        }

        return false;
    }
}
