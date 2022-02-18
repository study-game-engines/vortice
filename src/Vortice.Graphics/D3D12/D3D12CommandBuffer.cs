// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D12_COMMAND_LIST_TYPE;
using static TerraFX.Interop.DirectX.D3D12_COMMAND_LIST_FLAGS;
using static Vortice.Graphics.D3DUtilities;
using static TerraFX.Interop.Windows.Windows;
using static Vortice.Graphics.D3D12.D3D12Utils;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12CommandBuffer : CommandBuffer
{
    private readonly ComPtr<ID3D12CommandAllocator>[] _commandAllocators = new ComPtr<ID3D12CommandAllocator>[2];
    private readonly ComPtr<ID3D12GraphicsCommandList4> _commandList;

    public D3D12CommandBuffer(D3D12GraphicsDevice device, CommandQueueType queueType)
        : base(device)
    {
        D3D12_COMMAND_LIST_TYPE listType = ToD3D12(queueType);
        for (int i = 0; i < _commandAllocators.Length; i++)
        {
            ThrowIfFailed(
                device.NativeDevice->CreateCommandAllocator(listType, __uuidof<ID3D12CommandAllocator>(), _commandAllocators[i].GetVoidAddressOf())
                );
        }

        ThrowIfFailed(device.NativeDevice->CreateCommandList1(0,
            listType,
            D3D12_COMMAND_LIST_FLAG_NONE,
            __uuidof<ID3D12GraphicsCommandList4>(),
            _commandList.GetVoidAddressOf()
            ));
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            for (int i = 0; i < _commandAllocators.Length; i++)
            {
                _commandAllocators[i].Dispose();
            }

            _commandList.Dispose();
        }
    }
}
