// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using Vortice.Graphics.D3DCommon;
using static TerraFX.Interop.Windows.Windows;

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

            D3D11_TEXTURE2D_DESC description;
            backbufferTexture.Get()->GetDesc(&description);
            BackbufferTexture = new D3D11Texture(Device, backbufferTexture.Get(), description);
        }
    }
}
