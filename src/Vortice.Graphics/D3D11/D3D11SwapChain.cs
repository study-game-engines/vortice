// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Graphics.D3DCommon;
using Win32;
using Win32.Graphics.Direct3D11;
using static Win32.Apis;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11SwapChain : D3DSwapChainBase
{
    public D3D11SwapChain(D3D11GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device.DXGIFactory, device.IsTearingSupported, (IUnknown*)device.NativeDevice, device, surface, description)
    {
        SyncInterval = 1;
        ResizeBackBuffer(description.Width, description.Height);
    }

    // <summary>
    /// Finalizes an instance of the <see cref="D3D11SwapChain" /> class.
    /// </summary>
    ~D3D11SwapChain() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            BackbufferTexture?.Dispose();
            BackbufferTexture = default;
            base.Dispose(disposing);
        }
    }

    public uint SyncInterval { get; }

    public D3D11Texture? BackbufferTexture { get; private set; }

    /// <inheritdoc />
    protected override void ResizeBackBuffer(int width, int height)
    {
        if (BackbufferTexture != null)
        {

        }
        else
        {
            using ComPtr<ID3D11Texture2D> backbufferTexture = default;
            ThrowIfFailed(Handle->GetBuffer(0, __uuidof<ID3D11Texture2D>(), backbufferTexture.GetVoidAddressOf()));

            Texture2DDescription description;
            backbufferTexture.Get()->GetDesc(&description);

            TextureDescription textureDesc = TextureDescription.Texture2D(
                ColorFormat,
                (int)description.Width,
                (int)description.Height,
                (int)description.MipLevels,
                (int)description.ArraySize,
                TextureUsage.RenderTarget);

            BackbufferTexture = new D3D11Texture(Device, backbufferTexture.Get(), textureDesc);
        }
    }
}
