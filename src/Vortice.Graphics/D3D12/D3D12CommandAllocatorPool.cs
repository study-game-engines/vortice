// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D12_COMMAND_LIST_FLAGS;
using static TerraFX.Interop.Windows.Windows;

namespace Vortice.Graphics.D3D12;

internal readonly unsafe struct D3D12CommandAllocatorPool : IDisposable
{
    private readonly D3D12_COMMAND_LIST_TYPE _queueType;
    private readonly Queue<D3D12CommandListBundle> _bundles;

    public D3D12CommandAllocatorPool(D3D12_COMMAND_LIST_TYPE queueType)
    {
        _queueType = queueType;
        _bundles = new Queue<D3D12CommandListBundle>();
    }

    public void Dispose()
    {
        lock (_bundles)
        {
            foreach (D3D12CommandListBundle d3D12CommandListBundle in _bundles)
            {
                d3D12CommandListBundle.D3D12CommandList->Release();
                d3D12CommandListBundle.D3D12CommandAllocator->Release();
            }

            _bundles.Clear();
        }
    }

    /// <summary>
    /// Rents a <see cref="ID3D12GraphicsCommandList4"/> and <see cref="ID3D12CommandAllocator"/> pair.
    /// </summary>
    /// <param name="device">The <see cref="ID3D12Device5"/> renting the command list.</param>
    /// <param name="pipelineState">The <see cref="ID3D12PipelineState"/> instance to use for the new command list.</param>
    /// <param name="commandList">The resulting <see cref="ID3D12GraphicsCommandList"/> value.</param>
    /// <param name="commandAllocator">The resulting <see cref="ID3D12CommandAllocator"/> value.</param>
    public void Rent(ID3D12Device5* device, ID3D12PipelineState* pipelineState, out ID3D12GraphicsCommandList4* commandList, out ID3D12CommandAllocator* commandAllocator)
    {
        lock (_bundles)
        {
            if (_bundles.TryDequeue(out D3D12CommandListBundle d3D12CommandListBundle))
            {
                commandList = d3D12CommandListBundle.D3D12CommandList;
                commandAllocator = d3D12CommandListBundle.D3D12CommandAllocator;
            }
            else
            {
                commandAllocator = null;
                commandList = null;
            }
        }

        // Reset the command allocator and command list outside of the lock, or create a new pair if one to be reused
        // wasn't available. These operations are relatively expensive, so doing so here reduces thread contention
        // when multiple shader executions are being dispatched in parallel on the same device.
        if (commandAllocator is not null)
        {
            ThrowIfFailed(commandAllocator->Reset());
            ThrowIfFailed(commandList->Reset(commandAllocator, pipelineState));
        }
        else
        {
            CreateCommandListAndAllocator(device, pipelineState, out commandList, out commandAllocator);
        }
    }

    /// <summary>
    /// Returns a <see cref="ID3D12GraphicsCommandList4"/> and <see cref="ID3D12CommandAllocator"/> pair.
    /// </summary>
    /// <param name="commandList">The returned <see cref="ID3D12GraphicsCommandList4"/> value.</param>
    /// <param name="commandAllocator">The returned <see cref="ID3D12CommandAllocator"/> value.</param>
    public void Return(ID3D12GraphicsCommandList4* commandList, ID3D12CommandAllocator* commandAllocator)
    {
        lock (_bundles)
        {
            _bundles.Enqueue(new D3D12CommandListBundle(commandList, commandAllocator));
        }
    }

    /// <summary>
    /// Creates a new <see cref="ID3D12CommandList"/> and <see cref="ID3D12CommandAllocator"/> pair.
    /// </summary>
    /// <param name="device">The <see cref="ID3D12Device5"/> renting the command list.</param>
    /// <param name="pipelineState">The <see cref="ID3D12PipelineState"/> instance to use for the new command list.</param>
    /// <param name="commandList">The resulting <see cref="ID3D12GraphicsCommandList"/> value.</param>
    /// <param name="commandAllocator">The resulting <see cref="ID3D12CommandAllocator"/> value.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void CreateCommandListAndAllocator(ID3D12Device5* device, ID3D12PipelineState* pipelineState, out ID3D12GraphicsCommandList4* commandList, out ID3D12CommandAllocator* commandAllocator)
    {
        using ComPtr<ID3D12CommandAllocator> commandAllocatorComPtr = default;
        using ComPtr<ID3D12GraphicsCommandList4> commandListComPtr = default;

        ThrowIfFailed(
                device->CreateCommandAllocator(_queueType, __uuidof<ID3D12CommandAllocator>(), commandAllocatorComPtr.GetVoidAddressOf())
                );

        ThrowIfFailed(device->CreateCommandList1(0,
            _queueType,
            D3D12_COMMAND_LIST_FLAG_NONE,
            __uuidof<ID3D12GraphicsCommandList4>(),
            commandListComPtr.GetVoidAddressOf()
            ));

        fixed (ID3D12GraphicsCommandList4** returnCommandList = &commandList)
        fixed (ID3D12CommandAllocator** returnCommandAllocator = &commandAllocator)
        {
            commandListComPtr.CopyTo(returnCommandList);
            commandAllocatorComPtr.CopyTo(returnCommandAllocator);
        }
    }

    /// <summary>
    /// A type representing a bundle of a cached command list and related allocator.
    /// </summary>
    private readonly struct D3D12CommandListBundle
    {
        /// <summary>
        /// The <see cref="ID3D12GraphicsCommandList4"/> value for the current entry.
        /// </summary>
        public readonly ID3D12GraphicsCommandList4* D3D12CommandList;

        /// <summary>
        /// The <see cref="ID3D12CommandAllocator"/> value for the current entry.
        /// </summary>
        public readonly ID3D12CommandAllocator* D3D12CommandAllocator;

        /// <summary>
        /// Creates a new <see cref="D3D12CommandListBundle"/> instance with the given parameters.
        /// </summary>
        /// <param name="d3D12CommandList">The <see cref="ID3D12GraphicsCommandList4"/> value to wrap.</param>
        /// <param name="d3D12CommandAllocator">The <see cref="ID3D12CommandAllocator"/> value to wrap.</param>
        public D3D12CommandListBundle(ID3D12GraphicsCommandList4* d3D12CommandList, ID3D12CommandAllocator* d3D12CommandAllocator)
        {
            D3D12CommandList = d3D12CommandList;
            D3D12CommandAllocator = d3D12CommandAllocator;
        }
    }
}
