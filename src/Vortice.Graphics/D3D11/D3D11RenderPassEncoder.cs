// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Win32.Graphics.Direct3D11;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11RenderPassEncoder : RenderPassEncoder
{
    private readonly D3D11CommandBuffer _commandBuffer;
    private readonly ID3D11DeviceContext1* _handle;

    public D3D11RenderPassEncoder(D3D11CommandBuffer commandBuffer)
    {
        _commandBuffer = commandBuffer;
        _handle = commandBuffer.Handle;
    }

    /// <inheritdoc />
    public override CommandBuffer CommandBuffer => _commandBuffer;

    public void Begin(in RenderPassDescription renderPass)
    {

    }

    public override void End() => _commandBuffer.EndEncoder();

    private void PrepareDraw()
    {
    }
}
