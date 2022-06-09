// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;


/// <summary>
/// A queue that organizes command buffers for the GPU to execute.
/// </summary>
public abstract class CommandQueue : GraphicsResource
{
    protected CommandQueue(GraphicsDevice device)
        : base(device, IntPtr.Zero, default)
    {
    }

    public abstract CommandBuffer BeginCommandBuffer();

    /// <summary>
    /// Get the queue type.
    /// </summary>
    public abstract CommandQueueType QueueType { get; }
}
