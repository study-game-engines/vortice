// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using Alimer.Audio.XAudio2;
using Alimer.Engine;
using Alimer.Graphics;
using Vortice.Mathematics;
using Alimer.Platform;

namespace Alimer.Samples;

public sealed class DrawTriangleGame : Game
{
    public DrawTriangleGame()
        : base("Draw Triangle")
    {
    }

    /// <inheritdoc />
    protected override void ConfigureModules()
    {
        base.ConfigureModules();

        Modules
            .UseSDL()
            .UseXAudio2();
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

        //using (Texture texture = GraphicsDevice.CreateTexture(TextureDescription.Texture2D(PixelFormat.Rgba8Unorm, 256, 256, 1, 6)))
        //{
        //}
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        // TODO:
        return;

        CommandBuffer commandBuffer = GraphicsDevice.BeginCommandBuffer("Frame");

        {
            ComputePassEncoder computePass = commandBuffer.BeginComputePass();
            computePass.End();
        }

        Texture? swapChainTexture = commandBuffer.AcquireSwapchainTexture(MainWindow.SwapChain!);
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

