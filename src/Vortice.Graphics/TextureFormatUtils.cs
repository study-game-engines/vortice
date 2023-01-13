// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public static class TextureFormatUtils
{
    /// <summary>
    /// Check if the format has a depth component.
    /// </summary>
    /// <param name="format">The <see cref="PixelFormat"/> to check.</param>
    /// <returns>True if format has depth component, false otherwise.</returns>
    public static bool IsDepthFormat(this TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.Depth16Unorm:
            case TextureFormat.Depth32Float:
            case TextureFormat.Depth24UnormStencil8:
            case TextureFormat.Depth32FloatStencil8:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the format has a stencil component.
    /// </summary>
    /// <param name="format">The <see cref="TextureFormat"/> to check.</param>
    /// <returns>True if format has stencil component, false otherwise.</returns>
    public static bool IsStencilFormat(this TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.Depth24UnormStencil8:
            case TextureFormat.Depth32FloatStencil8:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Check if the format has depth or stencil components.
    /// </summary>
    /// <param name="format">The <see cref="TextureFormat"/> to check.</param>
    /// <returns>True if format has depth or stencil component, false otherwise.</returns>
    public static bool IsDepthStencilFormat(this TextureFormat format)
    {
        return IsDepthFormat(format) || IsStencilFormat(format);
    }

    /// <summary>
    /// Check if the format is a compressed format.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public static bool IsBlockCompressedFormat(this TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.Bc1RgbaUnorm:
            case TextureFormat.Bc1RgbaUnormSrgb:
            case TextureFormat.Bc2RgbaUnorm:
            case TextureFormat.Bc2RgbaUnormSrgb:
            case TextureFormat.Bc3RgbaUnorm:
            case TextureFormat.Bc3RgbaUnormSrgb:
            case TextureFormat.Bc4RUnorm:
            case TextureFormat.Bc4RSnorm:
            case TextureFormat.Bc5RgUnorm:
            case TextureFormat.Bc5RgSnorm:
            case TextureFormat.Bc6hRgbUfloat:
            case TextureFormat.Bc6hRgbSfloat:
            case TextureFormat.Bc7RgbaUnorm:
            case TextureFormat.Bc7RgbaUnormSrgb:
                return true;

            default:
                return false;
        }
    }
}
