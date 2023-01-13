// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public abstract class Texture : GraphicsResource
{
    protected Texture(GraphicsDevice device, in TextureDescription descriptor)
        : base(device, descriptor.Label)
    {
        Dimension = descriptor.Dimension;
        Format = descriptor.Format;
        Width = descriptor.Width;
        Height = descriptor.Height;
        DepthOrArraySize = descriptor.DepthOrArraySize;
        MipLevels = descriptor.MipLevels;
        SampleCount = descriptor.SampleCount;
        Usage = descriptor.Usage;
    }

    public TextureDimension Dimension { get; }

    public TextureFormat Format { get; }

    public int Width { get; }
    public int Height { get; }
    public int DepthOrArraySize { get; }

    /// <summary>
    /// Get the number of mipmap levels for this texture.
    /// </summary>
    public int MipLevels { get; }

    /// <summary>
    /// Get the number of samples in each fragment.
    /// </summary>
    public TextureSampleCount SampleCount { get; }

    /// <summary>
    /// Gets the <see cref="TextureUsage"/> usage.
    /// </summary>
    public TextureUsage Usage { get; }

    /// <summary>
    /// Get a mip-level width.
    /// </summary>
    /// <param name="mipLevel"></param>
    /// <returns></returns>
    public int GetWidth(int mipLevel = 0)
    {
        return (mipLevel == 0) || (mipLevel < MipLevels) ? Math.Max(1, Width >> mipLevel) : 0;
    }

    // <summary>
    /// Get a mip-level height.
    /// </summary>
    /// <param name="mipLevel"></param>
    /// <returns></returns>
    public int GetHeight(int mipLevel = 0)
    {
        return (mipLevel == 0) || (mipLevel < MipLevels) ? Math.Max(1, Height >> mipLevel) : 0;
    }

    // <summary>
    /// Get a mip-level depth.
    /// </summary>
    /// <param name="mipLevel"></param>
    /// <returns></returns>
    public int GetDepth(int mipLevel = 0)
    {
        if (Dimension != TextureDimension.Texture3D)
        {
            return 1;
        }

        return (mipLevel == 0) || (mipLevel < MipLevels) ? Math.Max(1, DepthOrArraySize >> mipLevel) : 0;
    }

    public int CalculateSubresource(int mipSlice, int arraySlice, int planeSlice = 0)
    {
        return mipSlice + arraySlice * MipLevels + planeSlice * MipLevels * DepthOrArraySize;
    }
}
