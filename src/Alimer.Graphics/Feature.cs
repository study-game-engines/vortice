// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Graphics;

public enum Feature
{
    TextureCompressionBC,
    TextureCompressionETC2,
    TextureCompressionASTC,
    ShaderFloat16,
    PipelineStatisticsQuery,
    TimestampQuery,
    DepthClamping,
    Depth24UNormStencil8,
    Depth32FloatStencil8,

    TextureCubeArray,
    IndependentBlend,
    Tessellation,
    DescriptorIndexing,
    ConditionalRendering,
    DrawIndirectFirstInstance,
    ShaderOutputViewportIndex,
    SamplerMinMax,
    RayTracing,
}
