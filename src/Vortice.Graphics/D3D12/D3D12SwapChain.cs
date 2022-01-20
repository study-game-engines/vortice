// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Direct3D12;
using Vortice.DXGI;
using Vortice.Graphics.D3D;
using static Vortice.Graphics.D3D.Utils;

namespace Vortice.Graphics.D3D12;

internal sealed class D3D12SwapChain : D3DSwapChain
{
    private readonly D3D12Texture[] _backbufferTextures = new D3D12Texture[3];

    public D3D12SwapChain(D3D12GraphicsDevice device, in SwapChainSource source, in SwapChainDescriptor descriptor)
        : base(device, source, descriptor, device.DXGIFactory, device.GetDirectQueue().Handle)
    {
        
    }

    // <inheritdoc />
    public override Texture? CurrentBackBuffer => null;

    // <inheritdoc />
    public override int CurrentBackBufferIndex => Handle3!.CurrentBackBufferIndex;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            for (uint i = 0; i < 3; i++)
            {
                _backbufferTextures[i].Dispose();
            }

            base.Dispose(disposing);
        }
    }


    // <inheritdoc />
    protected override void AfterReset()
    {
        base.AfterReset();

        // Obtain the back buffers for this window which will be the final render targets
        // and create render target views for each of them.
        for (int i = 0; i < BackBufferCount; i++)
        {
            _backbufferTextures[i] = new D3D12Texture(Device, Handle.GetBuffer<ID3D12Resource>(i));
        }
    }
}
