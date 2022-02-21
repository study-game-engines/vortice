// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
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

    protected override void Initialize()
    {
        base.Initialize();

        Span<VertexPositionColor> triangleVertices = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(0f, 0.5f, 0.5f), Colors.Red),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Colors.LightBlue),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Colors.Blue)
        };

        using GraphicsBuffer vertexBuffer = GraphicsDevice.CreateBuffer(triangleVertices, BufferUsage.Vertex);
        
        using (var texture = GraphicsDevice.CreateTexture(TextureDescriptor.Texture2D(TextureFormat.RGBA8UNorm, 256, 256)))
        {
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        using CommandBuffer commandBuffer = GraphicsDevice.BeginCommandBuffer();
        commandBuffer.ExecuteAndWaitForCompletion();
    }

    public readonly struct VertexPositionColor
    {
        public readonly Vector3 Position;
        public readonly Color4 Color;

        public VertexPositionColor(in Vector3 position, in Color4 color)
        {
            Position = position;
            Color = color;
        }
    }
}
