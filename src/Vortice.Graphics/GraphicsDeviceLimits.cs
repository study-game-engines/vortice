// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public readonly struct GraphicsDeviceLimits
{
    public readonly uint maxTextureDimension1D;
    public readonly uint maxTextureDimension2D;
    public readonly uint maxTextureDimension3D;
    public readonly uint maxTextureArrayLayers;
    public readonly uint maxBindGroups;
    public readonly uint maxDynamicUniformBuffersPerPipelineLayout;
    public readonly uint maxDynamicStorageBuffersPerPipelineLayout;
    public readonly uint maxSampledTexturesPerShaderStage;
    public readonly uint maxSamplersPerShaderStage;
    public readonly uint maxStorageBuffersPerShaderStage;
    public readonly uint maxStorageTexturesPerShaderStage;
    public readonly uint maxUniformBuffersPerShaderStage;
    public readonly ulong maxUniformBufferBindingSize;
    public readonly ulong maxStorageBufferBindingSize;
    public readonly uint minUniformBufferOffsetAlignment;
    public readonly uint minStorageBufferOffsetAlignment;
    public readonly uint maxVertexBuffers;
    public readonly uint maxVertexAttributes;
    public readonly uint maxVertexBufferArrayStride;
    public readonly uint maxInterStageShaderComponents;
    public readonly uint maxComputeWorkgroupStorageSize;
    public readonly uint maxComputeInvocationsPerWorkGroup;
    public readonly uint maxComputeWorkGroupSizeX;
    public readonly uint maxComputeWorkGroupSizeY;
    public readonly uint maxComputeWorkGroupSizeZ;
    public readonly uint maxComputeWorkGroupsPerDimension;
}
