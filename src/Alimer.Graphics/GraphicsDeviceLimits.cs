// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Graphics;

public readonly struct GraphicsDeviceLimits
{
    public readonly uint MaxTextureDimension1D { get; init; }
    public readonly uint MaxTextureDimension2D { get; init; }
    public readonly uint MaxTextureDimension3D { get; init; }
    public readonly uint MaxTextureDimensionCube { get; init; }
    public readonly uint MaxTextureArrayLayers { get; init; }
    public readonly uint MaxVertexBuffers { get; init; }
    public readonly uint MaxVertexAttributes { get; init; }
    public readonly uint MaxVertexBufferArrayStride { get; init; }
    public readonly uint MaxColorAttachments { get; init; }
    public readonly uint maxComputeWorkgroupStorageSize;
    public readonly uint maxComputeInvocationsPerWorkGroup;
    public readonly uint maxComputeWorkGroupSizeX;
    public readonly uint maxComputeWorkGroupSizeY;
    public readonly uint maxComputeWorkGroupSizeZ;
    public readonly uint maxComputeWorkGroupsPerDimension;
}
