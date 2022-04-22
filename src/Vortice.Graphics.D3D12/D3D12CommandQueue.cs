// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D12_FENCE_FLAGS;
using static TerraFX.Interop.Windows.Windows;
using static Vortice.Graphics.D3D12.D3D12Utils;

namespace Vortice.Graphics.D3D12;

internal unsafe class D3D12CommandQueue : CommandQueue
{
    private readonly ComPtr<ID3D12CommandQueue> _handle;
    private readonly ComPtr<ID3D12Fence> _fence;
    private ulong _fenceValue;
    private readonly D3D12CommandAllocatorPool _allocatorPool;

    /* Command contexts */
    private SRWLOCK _cmdBuffersLocker;
    private readonly Queue<D3D12CommandBuffer> _commandBuffers = new();
    private ulong _frameCount;
    private uint _frameIndex;

    public D3D12CommandQueue(D3D12GraphicsDevice device, CommandQueueType queueType)
        : base(device)
    {
        QueueType = queueType;

        D3D12_COMMAND_LIST_TYPE listType = ToD3D12(queueType);

        D3D12_COMMAND_QUEUE_DESC commandQueueDesc = new()
        {
            Type = listType,
            Priority = (int)D3D12_COMMAND_QUEUE_PRIORITY.D3D12_COMMAND_QUEUE_PRIORITY_NORMAL,
            Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE,
            NodeMask = 0
        };

        ThrowIfFailed(device.NativeDevice->CreateCommandQueue(
            &commandQueueDesc,
            __uuidof<ID3D12CommandQueue>(),
            _handle.GetVoidAddressOf())
            );

        ThrowIfFailed(device.NativeDevice->CreateFence(0,
            D3D12_FENCE_FLAG_NONE,
            __uuidof<ID3D12Fence>(),
            _fence.GetVoidAddressOf())
            );

        _handle.Get()->SetName($"{queueType} Command Queue");

        _allocatorPool = new D3D12CommandAllocatorPool(listType);

        fixed (SRWLOCK* pCmdBuffersLocker = &_cmdBuffersLocker)
        {
            InitializeSRWLock(pCmdBuffersLocker);
        }
    }

    // <summary>
    /// Finalizes an instance of the <see cref="D3D12CommandQueue" /> class.
    /// </summary>
    ~D3D12CommandQueue() => Dispose(isDisposing: false);

    /// <inheritdoc />
    public override CommandQueueType QueueType { get; }

    public ID3D12CommandQueue* Handle => _handle.Get();

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _allocatorPool.Dispose();
            _handle.Dispose();
            _fence.Dispose();

            fixed (SRWLOCK* pCmdBuffersLocker = &_cmdBuffersLocker)
            {
                ReleaseSRWLockExclusive(pCmdBuffersLocker);
            }
        }
    }

    /// <inheritdoc />
    public override CommandBuffer BeginCommandBuffer()
    {
        /* Make sure multiple threads can't acquire the same command buffer. */
        //AcquireSRWLockExclusive(&_cmdBuffersLocker);

        D3D12CommandBuffer commandBuffer;

        if (_commandBuffers.Count == 0)
        {
            commandBuffer = new D3D12CommandBuffer(this);
        }
        else
        {
            commandBuffer = _commandBuffers.Dequeue();
        }

        commandBuffer.Reset(_frameIndex);

        return commandBuffer;
    }

    public ulong Signal()
    {
        ulong updatedFenceValue = Interlocked.Increment(ref _fenceValue);
        ThrowIfFailed(_handle.Get()->Signal(_fence.Get(), updatedFenceValue));
        return updatedFenceValue;
    }

    public bool IsFenceComplete(ulong fenceValue)
    {
        return _fence.Get()->GetCompletedValue() >= fenceValue;
    }

    public void WaitForFenceValue(ulong fenceValue)
    {
        if (IsFenceComplete(fenceValue))
            return;

        ThrowIfFailed(_fence.Get()->SetEventOnCompletion(fenceValue, default));
    }

    public void WaitIdle()
    {
        WaitForFenceValue(Signal());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetCommandListAndAllocator(out ID3D12GraphicsCommandList4* commandList, out ID3D12CommandAllocator* commandAllocator)
    {
        var d3d12GraphicsDevice = (D3D12GraphicsDevice)Device;
        _allocatorPool.Rent(d3d12GraphicsDevice.NativeDevice, null, out commandList, out commandAllocator);
    }

    /// <summary>
    /// Executes a given command list and waits for the operation to be completed.
    /// </summary>
    /// <param name="commandBuffer">The input <see cref="D3D12CommandBuffer"/> to execute.</param>
    internal void ExecuteCommandList(D3D12CommandBuffer commandBuffer)
    {
        // Execute the command list
        _handle.Get()->ExecuteCommandLists(1, commandBuffer.GetD3D12CommandListAddressOf());

        ulong nextFenceValue = Signal();

        // If the fence value hasn't been reached, wait until the operation completes
        if (nextFenceValue > _fence.Get()->GetCompletedValue())
        {
            ThrowIfFailed(_fence.Get()->SetEventOnCompletion(nextFenceValue, default));
        }

        _commandBuffers.Enqueue(commandBuffer);

        _frameCount++;
        _frameIndex = (uint)(_frameCount % Constants.MaxFramesInFlight);
    }
}
