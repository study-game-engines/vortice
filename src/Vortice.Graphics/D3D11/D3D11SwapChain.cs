// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.DXGI;

namespace Vortice.Graphics.D3D11;

internal class D3D11SwapChain : SwapChain
{
    public D3D11SwapChain(D3D11GraphicsDevice device, in GraphicsSurface surface, in SwapChainDescriptor descriptor)
        : base(device, surface, descriptor)
    {
        
    }

    public IDXGISwapChain1 Handle { get; }

    // <inheritdoc />
    public override Texture? CurrentBackBuffer => null;

    // <inheritdoc />
    public override int CurrentBackBufferIndex => 0;

    // <inheritdoc />
    public override int BackBufferCount => 1;

    // <inheritdoc />
    public override void Resize(int newWidth, int newHeight)
    {
        
    }

    /// <inheritdoc />
    public override void Present()
    {

    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
       
    }
}
