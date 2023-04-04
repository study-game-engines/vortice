// Copyright © Amer Koleci.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_FLAGS;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_STATES;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12GraphicsBuffer : GraphicsBuffer
{
    private readonly ComPtr<ID3D12Resource> _handle;

    public D3D12GraphicsBuffer(D3D12GraphicsDevice device, in BufferDescription description, void* initialData)
        : base(device, in description)
    {
        var d3d12ResourceDesc = D3D12_RESOURCE_DESC.Buffer(
              description.Size,
              D3D12_RESOURCE_FLAG_NONE
        );

        var d3d12Device = device.Handle;
        var d3d12ResourceAllocationInfo = d3d12Device->GetResourceAllocationInfo(visibleMask: 0, numResourceDescs: 1, &d3d12ResourceDesc);

        //d3d12Device->CreateCommittedResource()
    }

    public ID3D12Resource* Handle => _handle;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _handle.Dispose();
        }
    }

    protected override void OnLabelChanged(string newLabel)
    {
        fixed (char* pName = newLabel)
        {
            _ = Handle->SetName((ushort*)pName);
        }
    }
}
