// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

/// <summary>
/// Structure that describes the <see cref="GraphicsBuffer"/>.
/// </summary>
public record struct BufferDescription 
{
    public BufferDescription(BufferUsage usage, ulong size, CpuAccess access = CpuAccess.None, string? label = default)
    {
        Usage = usage;
        Size = size;
        Access = access;
        Label = label;
    }

    /// <summary>
    /// Gets or Sets the <see cref="BufferUsage"/> of <see cref="Buffer"/>.
    /// </summary>
    public BufferUsage Usage { get; init; }

    /// <summary>
    /// Size in bytes of <see cref="Buffer"/>
    /// </summary>
    public ulong Size { get; init; }

    /// <summary>
    /// CPU access of <see cref="Buffer"/>
    /// </summary>
    public CpuAccess Access { get; init; }

    // <summary>
    /// Gets or sets the label of <see cref="Buffer"/>.
    /// </summary>
    public string? Label { get; init; }
}
