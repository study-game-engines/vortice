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
            case TextureFormat.Depth16UNorm:
            case TextureFormat.Depth32Float:
            case TextureFormat.Depth24UNormStencil8:
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
            case TextureFormat.Depth24UNormStencil8:
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
            case TextureFormat.BC1UNorm:
            case TextureFormat.BC1UNormSrgb:
            case TextureFormat.BC2UNorm:
            case TextureFormat.BC2UNormSrgb:
            case TextureFormat.BC3UNorm:
            case TextureFormat.BC3UNormSrgb:
            case TextureFormat.BC4UNorm:
            case TextureFormat.BC4SNorm:
            case TextureFormat.BC5UNorm:
            case TextureFormat.BC5SNorm:
            case TextureFormat.BC6HUFloat:
            case TextureFormat.BC6HSFloat:
            case TextureFormat.BC7UNorm:
            case TextureFormat.BC7UNormSrgb:
                return true;

            default:
                return false;
        }
    }
}
