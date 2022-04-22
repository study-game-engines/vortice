// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DXGI;
using static TerraFX.Interop.DirectX.DXGI_SCALING;
using static TerraFX.Interop.DirectX.DXGI_SWAP_EFFECT;
using static TerraFX.Interop.DirectX.DXGI_ALPHA_MODE;
using static TerraFX.Interop.DirectX.DXGI_SWAP_CHAIN_FLAG;
using static Vortice.Graphics.D3DUtilities;
using TerraFX.Interop.Windows;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12SwapChain : SwapChain
{
    private readonly ComPtr<IDXGISwapChain3> _handle;
    private readonly uint _syncInterval = 1;
    private readonly uint _presentFlags = 0; // PresentFlags.None;

    private readonly D3D12Texture[] _backbufferTextures = new D3D12Texture[3];

    public D3D12SwapChain(D3D12GraphicsDevice device, in GraphicsSurface surface, in SwapChainDescriptor descriptor)
       : base(device, surface, descriptor)
    {
        BackBufferCount = (int)PresentModeToBufferCount(descriptor.PresentMode);

        DXGI_SWAP_CHAIN_DESC1 swapChainDesc = new()
        {
            Width = (uint)descriptor.Size.Width,
            Height = (uint)descriptor.Size.Height,
            Format = ToDXGISwapChainFormat(descriptor.ColorFormat),
            Stereo = false,
            SampleDesc = new(1, 0),
            BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT,
            BufferCount = (uint)BackBufferCount,
            Scaling = DXGI_SCALING_STRETCH,
            SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD,
            AlphaMode = DXGI_ALPHA_MODE_IGNORE,
            Flags = device.TearingSupported ? DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING : 0u
        };

        using ComPtr<IDXGISwapChain1> dxgiSwapChain1 = default;

        switch (surface.Type)
        {
            case SwapChainSourceType.Win32:
                Win32SwapChainSource win32Source = (Win32SwapChainSource)surface;
                DXGI_SWAP_CHAIN_FULLSCREEN_DESC fsSwapChainDesc = new()
                {
                    Windowed = descriptor.IsFullscreen ? false : true
                };

                HRESULT hr = device.Factory->CreateSwapChainForHwnd(
                    (IUnknown*)device.GraphicsQueue.Handle,
                    win32Source.Hwnd,
                    &swapChainDesc,
                    &fsSwapChainDesc,
                    null,
                    dxgiSwapChain1.GetAddressOf());
                ThrowIfFailed(hr);

                // This class does not support exclusive full-screen mode and prevents DXGI from responding to the ALT+ENTER shortcut
                ThrowIfFailed(device.Factory->MakeWindowAssociation(win32Source.Hwnd, DXGI_MWA_NO_ALT_ENTER));
                break;

            case SwapChainSourceType.CoreWindow:
                swapChainDesc.Scaling = DXGI_SCALING_ASPECT_RATIO_STRETCH;
                CoreWindowChainSource coreSource = (CoreWindowChainSource)surface;

                ThrowIfFailed(device.Factory->CreateSwapChainForCoreWindow(
                    (IUnknown*)device.GraphicsQueue.Handle,
                    (IUnknown*)coreSource.CoreWindow,
                    &swapChainDesc,
                    null,
                    dxgiSwapChain1.GetAddressOf()
                ));

                break;

            default:
                throw new GraphicsException("Surface not supported");
        }

        ThrowIfFailed(dxgiSwapChain1.CopyTo(_handle.GetAddressOf()));

        _syncInterval = PresentModeToSwapInterval(descriptor.PresentMode);
        if (!descriptor.IsFullscreen
            && _syncInterval == 0
            && device.TearingSupported)
        {
            _presentFlags = DXGI_PRESENT_ALLOW_TEARING;
        }

        AfterReset();
    }

    // <inheritdoc />
    public override Texture? CurrentBackBuffer => null;

    // <inheritdoc />
    public override int CurrentBackBufferIndex => 0; // Handle3!.CurrentBackBufferIndex;

    // <inheritdoc />
    public override int BackBufferCount { get; }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        for (uint i = 0; i < BackBufferCount; i++)
        {
            _backbufferTextures[i].Dispose();
        }

        _handle.Dispose();
    }

    // <inheritdoc />
    public override void Resize(int newWidth, int newHeight)
    {
    }

    /// <inheritdoc />
    public override void Present()
    {
        HRESULT hr = _handle.Get()->Present(_syncInterval, _presentFlags);
        if (hr.FAILED)
        {
        }
    }


    // <inheritdoc />
    private void AfterReset()
    {
        // Obtain the back buffers for this window which will be the final render targets
        // and create render target views for each of them.
        for (uint i = 0; i < BackBufferCount; i++)
        {
            using ComPtr<ID3D12Resource> backbufferTexture = default;
            ThrowIfFailed(
                _handle.Get()->GetBuffer(i, __uuidof<ID3D12Resource>(), backbufferTexture.GetVoidAddressOf())
                );

            _backbufferTextures[i] = new D3D12Texture(Device, backbufferTexture.Get());
        }
    }
}
