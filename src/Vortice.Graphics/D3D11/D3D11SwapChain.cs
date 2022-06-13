// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Direct3D11;
using Vortice.Graphics.D3DCommon;

namespace Vortice.Graphics.D3D11;

internal class D3D11SwapChain : D3DSwapChainBase
{
    public D3D11SwapChain(D3D11GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device.DXGIFactory, device.IsTearingSupported, device.NativeDevice, device, surface, description)
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
            Handle.Dispose();
        }
    }

    public int SyncInterval { get; }

    public D3D11Texture? BackbufferTexture { get; private set; }

    /// <inheritdoc />
    protected override void ResizeBackBuffer(int width, int height)
    {
        if (BackbufferTexture != null)
        {

        }
        else
        {
            using ID3D11Texture2D backbufferTexture = Handle.GetBuffer<ID3D11Texture2D>(0);
            BackbufferTexture = new D3D11Texture(Device, backbufferTexture);
        }
    }
}
