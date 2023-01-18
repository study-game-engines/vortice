// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using CommunityToolkit.Diagnostics;
using Vortice.Mathematics;
using Win32.Graphics.Direct3D11;
using static Win32.Graphics.Direct3D11.Apis;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11RenderPassEncoder : RenderPassEncoder
{
    private readonly D3D11CommandBuffer _commandBuffer;
    private readonly ID3D11DeviceContext1* _handle;
    private readonly ID3D11RenderTargetView*[] _rtvs = new ID3D11RenderTargetView*[D3D11_SIMULTANEOUS_RENDER_TARGET_COUNT];
    private ID3D11DepthStencilView* DSV = null;
    private RenderPassDescription _currentRenderPass;

    public D3D11RenderPassEncoder(D3D11CommandBuffer commandBuffer)
    {
        _commandBuffer = commandBuffer;
        _handle = commandBuffer.Handle;
    }

    /// <inheritdoc />
    public override CommandBuffer CommandBuffer => _commandBuffer;

    public void Begin(in RenderPassDescription renderPass)
    {
        uint numRTVs = 0;
        DSV = default;
        Size renderArea = new(int.MaxValue, int.MaxValue);

        if (!string.IsNullOrEmpty(renderPass.Label))
        {
            PushDebugGroup(renderPass.Label);
        }

        if (renderPass.ColorAttachments.Length > 0)
        {
            for (uint slot = 0; slot < renderPass.ColorAttachments.Length; slot++)
            {
                ref RenderPassColorAttachment attachment = ref renderPass.ColorAttachments[slot];
                Guard.IsTrue(attachment.View is not null);

                D3D11TextureView view = (D3D11TextureView)attachment.View;
                D3D11Texture texture = (D3D11Texture)view.Texture;

                int mipLevel = view.BaseMipLevel;
                int slice = view.BaseArrayLayer;

                renderArea.Width = Math.Min(renderArea.Width, texture.GetWidth(mipLevel));
                renderArea.Height = Math.Min(renderArea.Height, texture.GetHeight(mipLevel));

                _rtvs[numRTVs] = view.RTV;

                switch (attachment.LoadAction)
                {
                    case LoadAction.Load:
                        break;

                    case LoadAction.Clear:
                        Color4 clearColorValue = attachment.ClearColor;
                        _handle->ClearRenderTargetView(_rtvs[numRTVs], (float*)&clearColorValue);
                        break;
                    case LoadAction.Discard:
                        _handle->DiscardView((ID3D11View*)view.RTV);
                        break;
                }

                numRTVs++;
            }
        }

        if (renderPass.DepthStencilAttachment.HasValue)
        {
            RenderPassDepthStencilAttachment attachment = renderPass.DepthStencilAttachment.Value;
            Guard.IsTrue(attachment.View is not null);

            D3D11TextureView view = (D3D11TextureView)attachment.View;
            D3D11Texture texture = (D3D11Texture)view.Texture;
            int mipLevel = view.BaseMipLevel;
            int slice = view.BaseArrayLayer;

            renderArea.Width = Math.Min(renderArea.Width, texture.GetWidth(mipLevel));
            renderArea.Height = Math.Min(renderArea.Height, texture.GetHeight(mipLevel));

            DSV = view.DSV;

            ClearFlags clearFlags = ClearFlags.None;
            switch (attachment.DepthLoadAction)
            {
                case LoadAction.Load:
                    break;

                case LoadAction.Clear:
                    clearFlags |= ClearFlags.Depth;
                    break;
                case LoadAction.Discard:
                    _handle->DiscardView((ID3D11View*)DSV);
                    break;
            }

            if (texture.Format.IsDepthStencilFormat())
            {
                switch (attachment.StencilLoadAction)
                {
                    case LoadAction.Load:
                        break;

                    case LoadAction.Clear:
                        clearFlags |= ClearFlags.Stencil;
                        break;
                    case LoadAction.Discard:
                        _handle->DiscardView((ID3D11View*)DSV);
                        break;
                }
            }

            if (clearFlags != ClearFlags.None)
            {
                _handle->ClearDepthStencilView(DSV, clearFlags, attachment.ClearDepth, (byte)attachment.ClearStencil);
            }
        }

        fixed (ID3D11RenderTargetView** RTVS = _rtvs)
        {
            _handle->OMSetRenderTargets(numRTVs, RTVS, DSV);
        }

        _handle->RSSetViewport(0.0f, 0.0f, (float)renderArea.Width, (float)renderArea.Height, 0.0f, 1.0f);
        _handle->RSSetScissorRect(renderArea.Width, renderArea.Height);

        _currentRenderPass = renderPass;
    }

    public override void End() => _commandBuffer.EndEncoder();

    private void PrepareDraw()
    {
    }
}
