// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

/// <summary>
/// Structure that describes the <see cref="GraphicsDevice"/>.
/// </summary>
public record struct GraphicsDeviceDescription
{
    public GraphicsDeviceDescription()
    {
    }

    public GraphicsDeviceDescription(ValidationMode validationMode, GpuPowerPreference powerPreference, string? label = default)
    {
        ValidationMode = validationMode;
        PowerPreference = powerPreference;
        Label = label;
    }

    /// <summary>
    /// Gets the <see cref="GraphicsDevice"/> validation mode.
    /// </summary>
    public ValidationMode ValidationMode { get; init; } = ValidationMode.Disabled;

    /// <summary>
    /// Gets the GPU adapter selection power preference.
    /// </summary>
    public GpuPowerPreference PowerPreference { get; init; } = GpuPowerPreference.HighPerformance;

    // <summary>
    /// Gets or sets the label of <see cref="GraphicsDevice"/>.
    /// </summary>
    public string? Label { get; init; } = default;
}
