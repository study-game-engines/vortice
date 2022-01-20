// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Direct3D11;
using Vortice.Graphics.D3D;

namespace Vortice.Graphics.D3D11;

internal class D3D11SwapChain : D3DSwapChain
{
    private D3D11Texture? _backbufferTexture;

    public D3D11SwapChain(D3D11GraphicsDevice device, in SwapChainSource source, in SwapChainDescriptor descriptor)
        : base(device, source, descriptor, device.DXGIFactory, device.NativeDevice)
    {
    }

    // <inheritdoc />
    public override Texture? CurrentBackBuffer => _backbufferTexture;

    // <inheritdoc />
    public override int CurrentBackBufferIndex => 0;

    // <inheritdoc />
    public override int BackBufferCount => 1;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _backbufferTexture?.Dispose();
            base.Dispose(disposing);
        }
    }

    // <inheritdoc />
    protected override void AfterReset()
    {
        base.AfterReset();

        ID3D11Texture2D backbufferTexture = Handle.GetBuffer<ID3D11Texture2D>(0);
        _backbufferTexture = new D3D11Texture(Device, backbufferTexture);
    }
}
