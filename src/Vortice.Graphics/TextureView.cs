// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using CommunityToolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class TextureView : GraphicsResource
{
    protected TextureView(Texture texture, in TextureViewDescription description)
        : base(texture.Device, description.Label)
    {
        Guard.IsNotNull(texture, nameof(texture));

        Texture = texture;

        Dimension = description.Dimension;
        Format = description.Format;
        BaseMipLevel = description.BaseMipLevel;
        MipLevelCount = description.MipLevelCount;
        BaseArrayLayer = description.BaseArrayLayer;
        ArrayLayerCount = description.ArrayLayerCount;

        // The default value for the view dimension depends on the texture's dimension with a
        // special case for 2DArray being chosen if texture is 2D but has more than one array layer.
        if (Dimension == TextureViewDimension.Undefined)
        {
            switch (texture.Dimension)
            {
                case TextureDimension.Texture1D:
                    if (texture.DepthOrArrayLayers == 1)
                    {
                        Dimension = TextureViewDimension.View1D;
                    }
                    else
                    {
                        Dimension = TextureViewDimension.View1DArray;
                    }
                    break;
                case TextureDimension.Texture2D:
                    if (texture.DepthOrArrayLayers == 1)
                    {
                        Dimension = TextureViewDimension.View2D;
                    }
                    else
                    {
                        Dimension = TextureViewDimension.View2DArray;
                    }
                    break;
                case TextureDimension.Texture3D:
                    Dimension = TextureViewDimension.View3D;
                    break;
            }
        }

        if (Format == PixelFormat.Undefined)
        {
            Format = texture.Format;
        }

        if (ArrayLayerCount == 0)
        {
            switch (Dimension)
            {
                case TextureViewDimension.View1D:
                case TextureViewDimension.View2D:
                case TextureViewDimension.View3D:
                    ArrayLayerCount = 1;
                    break;
                case TextureViewDimension.ViewCube:
                    ArrayLayerCount = 6;
                    break;
                case TextureViewDimension.View1DArray:
                case TextureViewDimension.View2DArray:
                case TextureViewDimension.ViewCubeArray:
                    ArrayLayerCount = texture.DepthOrArrayLayers - description.BaseArrayLayer;
                    break;
                default:
                    break;
            }
        }

        if (MipLevelCount == 0)
        {
            MipLevelCount = texture.MipLevels - description.BaseMipLevel;
        }

        if (ArrayLayerCount == 0)
        {
            ArrayLayerCount = texture.DepthOrArrayLayers - description.BaseArrayLayer;
        }
    }

    /// <summary>
    /// Get the <see cref="Texture"/> object that created this view.
    /// </summary>
    public Texture Texture { get; }

    public TextureViewDimension Dimension { get; }

    public PixelFormat Format { get; }

    public int BaseMipLevel { get; }
    public int MipLevelCount { get; }
    public int BaseArrayLayer { get; }
    public int ArrayLayerCount { get; }
}
