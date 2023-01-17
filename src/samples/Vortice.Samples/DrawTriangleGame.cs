// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using Vortice.Engine;
using Vortice.Graphics;
using Vortice.Mathematics;

namespace Vortice.Samples;

public sealed class DrawTriangleGame : Game
{
    public DrawTriangleGame(GamePlatform? platform = null)
        : base(platform)
    {
    }

    /// <inheritdoc />
    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
    }

    /// <inheritdoc />
    protected override void Initialize()
    {
        base.Initialize();

        ReadOnlySpan<VertexPositionColor> triangleVertices = stackalloc VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(0f, 0.5f, 0.5f), Colors.Red),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Colors.LightBlue),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Colors.Blue)
        };

        using (Texture texture = GraphicsDevice.CreateTexture(TextureDescription.Texture2D(PixelFormat.Rgba8Unorm, 256, 256, 1, 6)))
        {
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        CommandBuffer commandBuffer = GraphicsDevice.BeginCommandBuffer("Frame");

        {
            ComputePassEncoder computePass = commandBuffer.BeginComputePass();
            computePass.End();
        }

        Texture? swapChainTexture = commandBuffer.AcquireSwapchainTexture(View.SwapChain!);
        if (swapChainTexture != null)
        {
            RenderPassColorAttachment colorAttachment = new(swapChainTexture.GetView())
            {
                LoadAction = LoadAction.Clear,
                ClearColor = Colors.CornflowerBlue,
            };

            RenderPassDescription renderPassDesc = new(colorAttachment)
            {
                Label = "Frame"
            };

            RenderPassEncoder renderPass = commandBuffer.BeginRenderPass(renderPassDesc);
            renderPass.End();
        }

        GraphicsDevice.Submit(commandBuffer);
    }

    public readonly record struct VertexPositionColor(Vector3 Position, Color4 Color);
}

