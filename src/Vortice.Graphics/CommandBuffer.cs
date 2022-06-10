// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Mathematics;
using static Vortice.Graphics.VGPU;

namespace Vortice.Graphics;

public unsafe sealed class CommandBuffer
{
    internal CommandBuffer(GraphicsDevice device, IntPtr handle)
    {
        Device = device;
        Handle = handle;
    }

    public GraphicsDevice Device { get; }
    public IntPtr Handle { get; }

    public void PushDebugGroup(string groupLabel) => vgpuPushDebugGroup(Handle, groupLabel);
    public void PopDebugGroup() => vgpuPopDebugGroup(Handle);
    public void InsertDebugMarker(string debugLabel) => vgpuInsertDebugMarker(Handle, debugLabel);

    public ScopedDebugGroup PushScopedDebugGroup(string groupLabel)
    {
        PushDebugGroup(groupLabel);
        return new(this);
    }

    public Texture? AcquireSwapchainTexture(SwapChain swapChain, out SizeI size)
    {
        //IntPtr texturePtr = vgpuAcquireSwapchainTexture(Handle, swapChain.Handle, out int width, out int height);
        //if (texturePtr != IntPtr.Zero)
        //{
        //    size = new(width, height);
        //    return Device.GetTexture(texturePtr);
        //}

        size = SizeI.Empty;
        return default;
    }

    public void BeginRenderPass(in Texture texture, in Color4 clearColor)
    {
        //RenderPassColorAttachment colorAttachment = new()
        //{
        //    texture = texture.Handle,
        //    loadOp = LoadAction.Clear,
        //    storeOp = StoreAction.Store,
        //    clearColor = clearColor
        //};

        //RenderPassDesc renderPass = default;
        //renderPass.colorAttachmentCount = 1;
        //renderPass.colorAttachments = &colorAttachment;
        //vgpuBeginRenderPass(Handle, &renderPass);
    }

    public void EndRenderPass()
    {
        //vgpuEndRenderPass(Handle);
    }

    public void SetViewport(float x, float y, float width, float height, float minDepth = 0.0f, float maxDepth = 1.0f)
    {
        var viewport = new Viewport(x, y, width, height, minDepth, maxDepth);
        vgpuSetViewports(Handle, 1, &viewport);
    }

    public void SetViewport(Viewport viewport)
    {
        vgpuSetViewports(Handle, 1, &viewport);
    }

    public void SetViewports(Viewport[] viewports)
    {
        fixed (Viewport* pViewports = viewports)
        {
            vgpuSetViewports(Handle, viewports.Length, pViewports);
        }
    }

    public void SetViewports(int count, Viewport[] viewports)
    {
        fixed (Viewport* pViewports = viewports)
        {
            vgpuSetViewports(Handle, count, pViewports);
        }
    }

    public void SetViewport(Span<Viewport> viewports)
    {
        fixed (Viewport* pViewports = viewports)
        {
            vgpuSetViewports(Handle, viewports.Length, pViewports);
        }
    }

    public void SetScissorRect(int x, int y, int width, int height)
    {
        RectI rect = new(x, y, width, height);
        vgpuSetScissorRects(Handle, 1, &rect);
    }

    public void SetScissorRect(RectI scissorRect)
    {
        vgpuSetScissorRects(Handle, 1, &scissorRect);
    }

    public void SetScissorRects(RectI[] scissorRects)
    {
        fixed (RectI* pRects = scissorRects)
        {
            vgpuSetScissorRects(Handle, scissorRects.Length, pRects);
        }
    }

    public void SetScissorRects(int count, RectI[] scissorRects)
    {
        fixed (RectI* pRects = scissorRects)
        {
            vgpuSetScissorRects(Handle, count, pRects);
        }
    }

    public void SetScissorRects(Span<RectI> scissorRects)
    {
        fixed (RectI* pRects = scissorRects)
        {
            vgpuSetScissorRects(Handle, scissorRects.Length, pRects);
        }
    }

    public void SetScissorRects(int count, Span<RectI> scissorRects)
    {
        fixed (RectI* pRects = scissorRects)
        {
            vgpuSetScissorRects(Handle, count, pRects);
        }
    }
}
