// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12CommandBuffer : CommandBuffer
{
    private readonly ComPtr<ID3D12GraphicsCommandList4> _commandList;
    private readonly ComPtr<ID3D12CommandAllocator> _commandAllocator;

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
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _commandAllocator.Dispose();
            _commandList.Dispose();
        }
    }
}
