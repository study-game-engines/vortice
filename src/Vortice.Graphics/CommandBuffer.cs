// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CommunityToolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class CommandBuffer
{
    protected CommandBuffer(GraphicsDevice device)
    {
        Guard.IsNotNull(device, nameof(device));

        Device = device;
    }

    /// <summary>
    /// Get the <see cref="GraphicsDevice"/> object that created the command buffer.
    /// </summary>
    public GraphicsDevice Device { get; }

    public abstract void PushDebugGroup(string groupLabel);
    public abstract void PopDebugGroup();
    public abstract void InsertDebugMarker(string debugLabel);

    public ScopedDebugGroup PushScopedDebugGroup(string groupLabel)
    {
        PushDebugGroup(groupLabel);
        return new ScopedDebugGroup(this);
    }

    public abstract Texture? AcquireSwapchainTexture(SwapChain swapChain);

    //public void BeginRenderPass(in Texture texture, in Color4 clearColor)
    //{
    //    //RenderPassColorAttachment colorAttachment = new()
    //    //{
    //    //    texture = texture.Handle,
    //    //    loadOp = LoadAction.Clear,
    //    //    storeOp = StoreAction.Store,
    //    //    clearColor = clearColor
    //    //};

    //    //RenderPassDesc renderPass = default;
    //    //renderPass.colorAttachmentCount = 1;
    //    //renderPass.colorAttachments = &colorAttachment;
    //    //vgpuBeginRenderPass(Handle, &renderPass);
    //}

    //public void EndRenderPass()
    //{
    //    //vgpuEndRenderPass(Handle);
    //}

    //public void SetViewport(float x, float y, float width, float height, float minDepth = 0.0f, float maxDepth = 1.0f)
    //{
    //    var viewport = new Viewport(x, y, width, height, minDepth, maxDepth);
    //    vgpuSetViewports(Handle, 1, &viewport);
    //}

    //public void SetViewport(Viewport viewport)
    //{
    //    vgpuSetViewports(Handle, 1, &viewport);
    //}

    //public void SetViewports(Viewport[] viewports)
    //{
    //    fixed (Viewport* pViewports = viewports)
    //    {
    //        vgpuSetViewports(Handle, viewports.Length, pViewports);
    //    }
    //}

    //public void SetViewports(int count, Viewport[] viewports)
    //{
    //    fixed (Viewport* pViewports = viewports)
    //    {
    //        vgpuSetViewports(Handle, count, pViewports);
    //    }
    //}

    //public void SetViewport(Span<Viewport> viewports)
    //{
    //    fixed (Viewport* pViewports = viewports)
    //    {
    //        vgpuSetViewports(Handle, viewports.Length, pViewports);
    //    }
    //}

    //public void SetScissorRect(int x, int y, int width, int height)
    //{
    //    RectI rect = new(x, y, width, height);
    //    vgpuSetScissorRects(Handle, 1, &rect);
    //}

    //public void SetScissorRect(RectI scissorRect)
    //{
    //    vgpuSetScissorRects(Handle, 1, &scissorRect);
    //}

    //public void SetScissorRects(RectI[] scissorRects)
    //{
    //    fixed (RectI* pRects = scissorRects)
    //    {
    //        vgpuSetScissorRects(Handle, scissorRects.Length, pRects);
    //    }
    //}

    //public void SetScissorRects(int count, RectI[] scissorRects)
    //{
    //    fixed (RectI* pRects = scissorRects)
    //    {
    //        vgpuSetScissorRects(Handle, count, pRects);
    //    }
    //}

    //public void SetScissorRects(Span<RectI> scissorRects)
    //{
    //    fixed (RectI* pRects = scissorRects)
    //    {
    //        vgpuSetScissorRects(Handle, scissorRects.Length, pRects);
    //    }
    //}

    //public void SetScissorRects(int count, Span<RectI> scissorRects)
    //{
    //    fixed (RectI* pRects = scissorRects)
    //    {
    //        vgpuSetScissorRects(Handle, count, pRects);
    //    }
    //}
}
