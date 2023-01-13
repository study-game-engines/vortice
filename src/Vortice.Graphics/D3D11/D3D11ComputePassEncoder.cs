// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Win32.Graphics.Direct3D11;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11ComputePassEncoder : ComputePassEncoder
{
    private readonly D3D11GraphicsDevice _device;
    private readonly ID3D11DeviceContext1* _handle;

    public D3D11ComputePassEncoder(D3D11GraphicsDevice device, D3D11CommandBuffer commandBuffer)
        : base(commandBuffer)
    {
        _device = device;
        _handle = commandBuffer.Handle;
    }

    public override void End() => ((D3D11CommandBuffer)CommandBuffer).EndComputePass();

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
