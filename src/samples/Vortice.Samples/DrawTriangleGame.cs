// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using Microsoft.Extensions.DependencyInjection;
using Vortice.Audio;
using Vortice.Engine;
using Vortice.Graphics;
using Vortice.Mathematics;

namespace Vortice.Samples;

public sealed class DrawTriangleGame : Game
{
    public DrawTriangleGame(GameContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    protected override void ConfigureServices(IServiceCollection services)
    {
        // Audio
        if (XAudio2Device.IsSupported())
        {
            services.AddSingleton<AudioDevice>(new XAudio2Device());
        }

        base.ConfigureServices(services);
    }

    /// <inheritdoc />
    protected override void Initialize()
    {
        base.Initialize();

        Span<VertexPositionColor> triangleVertices = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(0f, 0.5f, 0.5f), Colors.Red),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Colors.LightBlue),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Colors.Blue)
        };

        //using GraphicsBuffer vertexBuffer = GraphicsDevice.CreateBuffer(triangleVertices, BufferUsage.Vertex);
        //
        //using (Texture texture = GraphicsDevice.CreateTexture(TextureDescriptor.Texture2D(TextureFormat.RGBA8UNorm, 256, 256)))
        //{
        //}
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        //CommandBuffer commandBuffer = GraphicsDevice.GraphicsQueue.BeginCommandBuffer();
        //commandBuffer.Commit();
    }

    public readonly record struct VertexPositionColor(Vector3 Position, Color4 Color);
}

