// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CommunityToolkit.Diagnostics;

namespace Alimer.Graphics;

public abstract class CommandEncoder
{
    /// <summary>
    /// Get the <see cref="Graphics.CommandBuffer"/> object that created the command encoder.
    /// </summary>
    public abstract CommandBuffer CommandBuffer { get; }

    public abstract void End();

    public void PushDebugGroup(string groupLabel) => CommandBuffer.PushDebugGroup(groupLabel);
    public void PopDebugGroup() => CommandBuffer.PopDebugGroup();
    public void InsertDebugMarker(string debugLabel) => CommandBuffer.InsertDebugMarker(debugLabel);

    public ScopedDebugGroup PushScopedDebugGroup(string groupLabel) => CommandBuffer.PushScopedDebugGroup(groupLabel);

}
