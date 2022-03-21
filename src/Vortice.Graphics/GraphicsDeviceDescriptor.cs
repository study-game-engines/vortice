// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

/// <summary>
/// Structure that describes the <see cref="GraphicsDevice"/>.
/// </summary>
public record struct GraphicsDeviceDescriptor(
    string? Name,
    ValidationMode ValidationMode = ValidationMode.Disabled,
    GpuPowerPreference PowerPreference = GpuPowerPreference.HighPerformance
    );
