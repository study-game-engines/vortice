// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public enum GpuBackend : byte
{
    Default = 0,
    Vulkan,
    Direct3D12,
    Direct3D11,
}
