// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public readonly struct ScopedDebugGroup : IDisposable
{
    private readonly CommandBuffer _commandBuffer;

    public ScopedDebugGroup(CommandBuffer commandBuffer)
    {
        _commandBuffer = commandBuffer;
    }

    public void Dispose()
    {
        _commandBuffer.PopDebugGroup();
    }
}
