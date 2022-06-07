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
    public static bool IsDepthFormat(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.Depth16UNorm:
            case PixelFormat.Depth32Float:
            case PixelFormat.Depth24UNormStencil8:
            case PixelFormat.Depth32FloatStencil8:
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
    public static bool IsStencilFormat(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.Depth24UNormStencil8:
            case PixelFormat.Depth32FloatStencil8:
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
    public static bool IsDepthStencilFormat(this PixelFormat format)
    {
        return IsDepthFormat(format) || IsStencilFormat(format);
    }

    /// <summary>
    /// Check if the format is a compressed format.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public static bool IsBlockCompressedFormat(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.BC1RGBAUNorm:
            case PixelFormat.BC1RGBAUNormSrgb:
            case PixelFormat.BC2RGBAUNorm:
            case PixelFormat.BC2RGBAUNormSrgb:
            case PixelFormat.BC3RGBAUNorm:
            case PixelFormat.BC3RGBAUNormSrgb:
            case PixelFormat.BC4RUNorm:
            case PixelFormat.BC4RSNorm:
            case PixelFormat.BC5RGUNorm:
            case PixelFormat.BC5RGSNorm:
            case PixelFormat.BC6HRGBUFloat:
            case PixelFormat.BC6HRGBFloat:
            case PixelFormat.BC7RGBAUNorm:
            case PixelFormat.BC7RGBAUNormSrgb:
                return true;

            default:
                return false;
        }
    }
}
