// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using SharpGen.Runtime;
using Vortice.DXGI;
using static Vortice.Graphics.D3DCommon.D3DUtils;

namespace Vortice.Graphics.D3DCommon;

internal abstract class D3DSwapChainBase : SwapChain
{
    protected D3DSwapChainBase(IDXGIFactory2 factory, bool tearingSupported, IUnknown deviceOrCommandQueue,
        GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device, surface, description)
    {
        Vortice.DXGI.SwapChainDescription1 swapChainDesc = new()
        {
            Width = description.Width,
            Height = description.Height,
            Format = ToDXGISwapChainFormat(description.Format),
            Stereo = false,
            SampleDescription = SampleDescription.Default,
            BufferUsage = Usage.RenderTargetOutput,
            BufferCount = PresentModeToBufferCount(description.PresentMode),
            Scaling = Scaling.Stretch,
            SwapEffect = SwapEffect.FlipDiscard,
            AlphaMode = AlphaMode.Ignore,
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

                Handle = factory.CreateSwapChainForHwnd(
                    deviceOrCommandQueue,
                    win32Source.Hwnd,
                    swapChainDesc,
                    fsSwapChainDesc);

                // This class does not support exclusive full-screen mode and prevents DXGI from responding to the ALT+ENTER shortcut
                factory.MakeWindowAssociation(win32Source.Hwnd, WindowAssociationFlags.IgnoreAltEnter).CheckError();
                break;
#endif

            default:
                throw new GraphicsException("Surface not supported");
        }
    }

#if WINDOWS_UWP
    public IDXGISwapChain3 Handle { get; }
#else
    public IDXGISwapChain1 Handle { get; }
#endif
}
