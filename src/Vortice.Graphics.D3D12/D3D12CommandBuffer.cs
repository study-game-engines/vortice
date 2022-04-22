// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12CommandBuffer : CommandBuffer
{
    private ComPtr<ID3D12GraphicsCommandList4> _commandList;
    private ComPtr<ID3D12CommandAllocator> _commandAllocator;

    public D3D12CommandBuffer(D3D12GraphicsDevice device, D3D12CommandQueue queue)
        : base(device)
    {
        Queue = queue;

        Unsafe.SkipInit(out _commandList);
        Unsafe.SkipInit(out _commandAllocator);

        queue.GetCommandListAndAllocator(
           out *(ID3D12GraphicsCommandList4**)Unsafe.AsPointer(ref _commandList),
            out *(ID3D12CommandAllocator**)Unsafe.AsPointer(ref _commandAllocator)
           );
    }

    public D3D12CommandQueue Queue { get; }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        _commandAllocator.Dispose();
        _commandList.Dispose();
    }

    /// <inheritdoc />
    public override void ExecuteAndWaitForCompletion()
    {
        _commandList.Get()->Close();

        Queue.ExecuteCommandList(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ID3D12CommandList** GetD3D12CommandListAddressOf()
    {
        return (ID3D12CommandList**)_commandList.GetAddressOf();
    }

    /// <summary>
    /// Detaches the <see cref="ID3D12GraphicsCommandList4"/> object in use by the current instance.
    /// </summary>
    /// <returns>The <see cref="ID3D12GraphicsCommandList4"/> object in use, with ownership.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ID3D12GraphicsCommandList4* DetachD3D12CommandList()
    {
        return _commandList.Detach();
    }

    /// <summary>
    /// Detaches the <see cref="ID3D12CommandAllocator"/> object in use by the current instance.
    /// </summary>
    /// <returns>The <see cref="ID3D12CommandAllocator"/> object in use, with ownership.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ID3D12CommandAllocator* DetachD3D12CommandAllocator()
    {
        return _commandAllocator.Detach();
    }
}
