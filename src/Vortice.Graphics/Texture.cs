// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static Vortice.Graphics.VGPU;

namespace Vortice.Graphics;

public sealed class Texture : GraphicsResource
{
    protected Texture(GraphicsDevice device, in TextureDescription descriptor)
        : base(device, IntPtr.Zero, descriptor.Label)
    {
        TextureType = descriptor.TextureType;
        Format = descriptor.Format;
        Width = descriptor.Width;
        Height = descriptor.Height;
        Depth = descriptor.TextureType == TextureType.Type3D ? descriptor.DepthOrArraySize : 1;
        ArraySize = descriptor.TextureType != TextureType.Type3D ? descriptor.DepthOrArraySize : 1;
        MipLevels = descriptor.MipLevels;
        SampleCount = descriptor.SampleCount;
        Usage = descriptor.Usage;
    }

    internal Texture(GraphicsDevice device, IntPtr handle)
        : base(device, handle)
    {
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Texture" /> class.
    /// </summary>
    ~Texture() => Dispose(isDisposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            if (Handle != IntPtr.Zero)
            {
                vgpuDestroyTexture(Device.Handle, Handle);
            }
        }
    }

    public int CalculateSubresource(int mipSlice, int arraySlice, int planeSlice = 0)
    {
        return mipSlice + arraySlice * MipLevels + planeSlice * MipLevels * ArraySize;
    }

    public TextureType TextureType { get; }

    public TextureFormat Format { get; }

    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }
    public int ArraySize { get; }
    public int MipLevels { get; }
    public TextureSampleCount SampleCount { get; }
    public TextureUsage Usage { get; }
}
