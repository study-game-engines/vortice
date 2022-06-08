// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

// This file includes code based on code from https://github.com/Sergio0694/ComputeSharp
// The original code is Copyright (c) 2019 Sergio Pedri. All rights reserved. Licensed under the MIT License (MIT).

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

namespace Vortice.Graphics.D3D12;

internal readonly unsafe struct D3D12CommandAllocatorPool : IDisposable
{
    private readonly D3D12_COMMAND_LIST_TYPE _queueType;
    private readonly Queue<D3D12CommandListBundle> _bundleQueue;

    public D3D12CommandAllocatorPool(D3D12_COMMAND_LIST_TYPE queueType)
    {
        _queueType = queueType;
        _bundleQueue = new Queue<D3D12CommandListBundle>();
    }

    public void Dispose()
    {
        lock (_bundleQueue)
        {
            foreach (D3D12CommandListBundle commandListBundle in _bundleQueue)
            {
                commandListBundle.D3D12CommandList->Release();
                commandListBundle.D3D12CommandAllocator->Release();
            }

            _bundleQueue.Clear();
        }
    }

    public void Rent(ID3D12Device5* d3D12Device, ID3D12PipelineState* pipelineState, out ID3D12GraphicsCommandList4* commandList, out ID3D12CommandAllocator* commandAllocator)
    {
        lock (_bundleQueue)
        {
            if (_bundleQueue.TryDequeue(out D3D12CommandListBundle d3D12CommandListBundle))
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
            CreateCommandListAndAllocator(d3D12Device, pipelineState, out commandList, out commandAllocator);
        }
    }

    /// <summary>
    /// Returns a <see cref="ID3D12GraphicsCommandLis4t"/> and <see cref="ID3D12CommandAllocator"/> pair.
    /// </summary>
    /// <param name="d3D12CommandList">The returned <see cref="ID3D12GraphicsCommandList"/> value.</param>
    /// <param name="d3D12CommandAllocator">The returned <see cref="ID3D12CommandAllocator"/> value.</param>
    public void Return(ID3D12GraphicsCommandList4* d3D12CommandList, ID3D12CommandAllocator* d3D12CommandAllocator)
    {
        lock (_bundleQueue)
        {
            _bundleQueue.Enqueue(new D3D12CommandListBundle(d3D12CommandList, d3D12CommandAllocator));
        }
    }

    /// <summary>
    /// Creates a new <see cref="ID3D12CommandList"/> and <see cref="ID3D12CommandAllocator"/> pair.
    /// </summary>
    /// <param name="d3D12Device">The <see cref="ID3D12Device"/> renting the command list.</param>
    /// <param name="d3D12PipelineState">The <see cref="ID3D12PipelineState"/> instance to use for the new command list.</param>
    /// <param name="d3D12CommandList">The resulting <see cref="ID3D12GraphicsCommandList"/> value.</param>
    /// <param name="d3D12CommandAllocator">The resulting <see cref="ID3D12CommandAllocator"/> value.</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void CreateCommandListAndAllocator(ID3D12Device5* d3D12Device, ID3D12PipelineState* d3D12PipelineState, out ID3D12GraphicsCommandList4* d3D12CommandList, out ID3D12CommandAllocator* d3D12CommandAllocator)
    {
        using ComPtr<ID3D12CommandAllocator> d3D12CommandAllocatorComPtr = d3D12Device->CreateCommandAllocator(_queueType);
        using ComPtr<ID3D12GraphicsCommandList4> d3D12CommandListComPtr = d3D12Device->CreateCommandList(_queueType, d3D12CommandAllocatorComPtr.Get(), d3D12PipelineState);

        fixed (ID3D12GraphicsCommandList4** d3D12CommandListPtr = &d3D12CommandList)
        fixed (ID3D12CommandAllocator** d3D12CommandAllocatorPtr = &d3D12CommandAllocator)
        {
            d3D12CommandListComPtr.CopyTo(d3D12CommandListPtr);
            d3D12CommandAllocatorComPtr.CopyTo(d3D12CommandAllocatorPtr);
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
