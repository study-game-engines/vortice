// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D12_COMMAND_LIST_FLAGS;
using static TerraFX.Interop.Windows.Windows;
using static Vortice.Graphics.D3D12.D3D12Utils;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12CommandBuffer : CommandBuffer, IDisposable
{
    private ComPtr<ID3D12CommandAllocator>[] _commandAllocators = new ComPtr<ID3D12CommandAllocator>[Constants.MaxFramesInFlight];
    private ComPtr<ID3D12GraphicsCommandList4> _commandList;

    public D3D12CommandBuffer(D3D12CommandQueue queue)
    {
        Queue = queue;

        Unsafe.SkipInit(out _commandList);

        var d3d12GraphicsDevice = (D3D12GraphicsDevice)queue.Device;
        D3D12_COMMAND_LIST_TYPE listType = ToD3D12(queue.QueueType);

        for (uint i = 0; i < Constants.MaxFramesInFlight; i++)
        {
            ThrowIfFailed(d3d12GraphicsDevice.NativeDevice->CreateCommandAllocator(
                listType,
                __uuidof<ID3D12CommandAllocator>(),
                _commandAllocators[i].GetVoidAddressOf())
           );
        }

        ThrowIfFailed(d3d12GraphicsDevice.NativeDevice->CreateCommandList1(
            0,
            listType,
            D3D12_COMMAND_LIST_FLAG_NONE,
            __uuidof<ID3D12GraphicsCommandList4>(),
            _commandList.GetVoidAddressOf())
            );
    }

    public D3D12CommandQueue Queue { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        for (uint i = 0; i < Constants.MaxFramesInFlight; i++)
        {
            _commandAllocators[i].Dispose();
        }

        _commandList.Dispose();
    }

    /// <inheritdoc />
    public override void Commit()
    {
        _commandList.Get()->Close();

        Queue.ExecuteCommandList(this);
    }

    public void Reset(uint frameIndex)
    {
        // Start the command list in a default state.
        ThrowIfFailed(_commandAllocators[frameIndex].Get()->Reset());
        ThrowIfFailed(_commandList.Get()->Reset(_commandAllocators[frameIndex].Get(), null));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ID3D12CommandList** GetD3D12CommandListAddressOf()
    {
        return (ID3D12CommandList**)_commandList.GetAddressOf();
    }
}
