// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public enum GpuBackend : byte
{
    Null = 0,
    Vulkan = 1,
    Metal = 2,
    Direct3D12 = 3,
    Direct3D11 = 4,
    OpenGL = 5,
    WebGpu = 6,
    Count,
}
