// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Win32.Graphics.Direct3D11;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11ComputePassEncoder : ComputePassEncoder
{
    private readonly D3D11CommandBuffer _commandBuffer;
    private readonly ID3D11DeviceContext1* _handle;

    public D3D11ComputePassEncoder(D3D11CommandBuffer commandBuffer)
    {
        _commandBuffer = commandBuffer;
        _handle = commandBuffer.Handle;
    }

    /// <inheritdoc />
    public override CommandBuffer CommandBuffer => _commandBuffer;

    public void Begin()
    {

    }

    public override void End() => _commandBuffer.EndEncoder();

    public override void Dispatch(int groupCountX, int groupCountY, int groupCountZ)
    {
        Prepare();
        _handle->Dispatch((uint)groupCountX, (uint)groupCountY, (uint)groupCountZ);

        ID3D11Buffer* nullBuffer = default;
        ID3D11UnorderedAccessView* nullUAV = default;
        _handle->CSSetConstantBuffers(0, 1, &nullBuffer);
        _handle->CSSetUnorderedAccessViews(0, 1, &nullUAV, null);
    }

    private void Prepare()
    {
    }
}
