// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Graphics.D3DCommon;

namespace Vortice.Graphics.D3D11;

internal class D3D11SwapChain : D3DSwapChainBase
{
    public D3D11SwapChain(D3D11GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device.DXGIFactory, device.IsTearingSupported, device.NativeDevice, device, surface, description)
    {
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
            Handle.Dispose();
        }
    }

    /// <inheritdoc />
    protected override void ResizeBackBuffer(int width, int height) 
    {

    }
}
