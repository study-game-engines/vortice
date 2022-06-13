// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.Diagnostics;
using Vortice.DXGI;

namespace Vortice.Graphics.D3DCommon;

internal static unsafe class D3DUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Format ToDXGISwapChainFormat(TextureFormat format)
    {
        // FLIP_DISCARD and FLIP_SEQEUNTIAL swapchain buffers only support these formats
        switch (format)
        {
            case TextureFormat.RGBA16Float:
                return Format.R16G16B16A16_Float;

            case TextureFormat.BGRA8UNorm:
            case TextureFormat.BGRA8UNormSrgb:
                return Format.B8G8R8A8_UNorm;

            case TextureFormat.RGBA8UNorm:
            case TextureFormat.RGBA8UNormSrgb:
                return Format.R8G8B8A8_UNorm;

            case TextureFormat.RGB10A2UNorm:
                return Format.R10G10B10A2_UNorm;

            default:
                return Format.B8G8R8A8_UNorm;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Format ToDXGIFormat(TextureFormat format)
    {
        switch (format)
        {
            // 8-bit formats
            case TextureFormat.R8UNorm: return Format.R8_UNorm;
            case TextureFormat.R8SNorm: return Format.R8_SNorm;
            case TextureFormat.R8UInt: return Format.R8_UInt;
            case TextureFormat.R8SInt: return Format.R8_SInt;
            // 16-bit formats
            case TextureFormat.R16UNorm: return Format.R16_UNorm;
            case TextureFormat.R16SNorm: return Format.R16_SNorm;
            case TextureFormat.R16UInt: return Format.R16_UInt;
            case TextureFormat.R16SInt: return Format.R16_SInt;
            case TextureFormat.R16Float: return Format.R16_Float;
            case TextureFormat.RG8UNorm: return Format.R8G8_UNorm;
            case TextureFormat.RG8SNorm: return Format.R8G8_SNorm;
            case TextureFormat.RG8UInt: return Format.R8G8_UInt;
            case TextureFormat.RG8SInt: return Format.R8G8_SInt;
            // 32-bit formats
            case TextureFormat.R32UInt: return Format.R32_UInt;
            case TextureFormat.R32SInt: return Format.R32_SInt;
            case TextureFormat.R32Float: return Format.R32_Float;
            case TextureFormat.RG16UNorm: return Format.R16G16_UNorm;
            case TextureFormat.RG16SNorm: return Format.R16G16_SNorm;
            case TextureFormat.RG16UInt: return Format.R16G16_UInt;
            case TextureFormat.RG16SInt: return Format.R16G16_SInt;
            case TextureFormat.RG16Float: return Format.R16G16_Float;
            case TextureFormat.RGBA8UNorm: return Format.R8G8B8A8_UNorm;
            case TextureFormat.RGBA8UNormSrgb: return Format.R8G8B8A8_UNorm_SRgb;
            case TextureFormat.RGBA8SNorm: return Format.R8G8B8A8_SNorm;
            case TextureFormat.RGBA8UInt: return Format.R8G8B8A8_UInt;
            case TextureFormat.RGBA8SInt: return Format.R8G8B8A8_SInt;
            case TextureFormat.BGRA8UNorm: return Format.B8G8R8A8_UNorm;
            case TextureFormat.BGRA8UNormSrgb: return Format.B8G8R8A8_UNorm_SRgb;
            // Packed 32-Bit formats
            case TextureFormat.RGB10A2UNorm: return Format.R10G10B10A2_UNorm;
            case TextureFormat.RG11B10Float: return Format.R11G11B10_Float;
            case TextureFormat.RGB9E5Float: return Format.R9G9B9E5_SharedExp;
            // 64-Bit formats
            case TextureFormat.RG32UInt: return Format.R32G32_UInt;
            case TextureFormat.RG32SInt: return Format.R32G32_SInt;
            case TextureFormat.RG32Float: return Format.R32G32_Float;
            case TextureFormat.RGBA16UNorm: return Format.R16G16B16A16_UNorm;
            case TextureFormat.RGBA16SNorm: return Format.R16G16B16A16_SNorm;
            case TextureFormat.RGBA16UInt: return Format.R16G16B16A16_UInt;
            case TextureFormat.RGBA16SInt: return Format.R16G16B16A16_SInt;
            case TextureFormat.RGBA16Float: return Format.R16G16B16A16_Float;
            // 128-Bit formats
            case TextureFormat.RGBA32UInt: return Format.R32G32B32A32_UInt;
            case TextureFormat.RGBA32SInt: return Format.R32G32B32A32_SInt;
            case TextureFormat.RGBA32Float: return Format.R32G32B32A32_Float;
            // Depth-stencil formats
            case TextureFormat.Depth16UNorm: return Format.D16_UNorm;
            case TextureFormat.Depth32Float: return Format.D32_Float;
            case TextureFormat.Depth24UNormStencil8: return Format.D24_UNorm_S8_UInt;
            case TextureFormat.Depth32FloatStencil8: return Format.D32_Float_S8X24_UInt;
            // Compressed BC formats
            case TextureFormat.BC1UNorm: return Format.BC1_UNorm;
            case TextureFormat.BC1UNormSrgb: return Format.BC1_UNorm_SRgb;
            case TextureFormat.BC2UNorm: return Format.BC2_UNorm;
            case TextureFormat.BC2UNormSrgb: return Format.BC2_UNorm_SRgb;
            case TextureFormat.BC3UNorm: return Format.BC3_UNorm;
            case TextureFormat.BC3UNormSrgb: return Format.BC3_UNorm_SRgb;
            case TextureFormat.BC4UNorm: return Format.BC4_UNorm;
            case TextureFormat.BC4SNorm: return Format.BC4_SNorm;
            case TextureFormat.BC5UNorm: return Format.BC5_UNorm;
            case TextureFormat.BC5SNorm: return Format.BC5_SNorm;
            case TextureFormat.BC6HUFloat: return Format.BC6H_Uf16;
            case TextureFormat.BC6HSFloat: return Format.BC6H_Sf16;
            case TextureFormat.BC7UNorm: return Format.BC7_UNorm;
            case TextureFormat.BC7UNormSrgb: return Format.BC7_UNorm_SRgb;

            default:
                return ThrowHelper.ThrowArgumentException<Format>("Invalid texture format");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Format GetTypelessFormatFromDepthFormat(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.Depth16UNorm:
                return Format.R16_Typeless;
            case TextureFormat.Depth32Float:
                return Format.R32_Typeless;
            case TextureFormat.Depth24UNormStencil8:
                return Format.R24G8_Typeless;
            case TextureFormat.Depth32FloatStencil8:
                return Format.R32G8X24_Typeless;

            default:
                Guard.IsFalse(format.IsDepthFormat(), nameof(format));
                return ToDXGIFormat(format);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureFormat FromDXGIFormat(Format format)
    {
        switch (format)
        {
#if TODO
            // 8-bit formats
            case TextureFormat.R8UNorm: return Format.R8_UNorm;
            case TextureFormat.R8SNorm: return Format.R8_SNorm;
            case TextureFormat.R8UInt: return Format.R8_UInt;
            case TextureFormat.R8SInt: return Format.R8_SInt;
            // 16-bit formats
            case TextureFormat.R16UNorm: return Format.R16_UNorm;
            case TextureFormat.R16SNorm: return Format.R16_SNorm;
            case TextureFormat.R16UInt: return Format.R16_UInt;
            case TextureFormat.R16SInt: return Format.R16_SInt;
            case TextureFormat.R16Float: return Format.R16_Float;
            case TextureFormat.RG8UNorm: return Format.R8G8_UNorm;
            case TextureFormat.RG8SNorm: return Format.R8G8_SNorm;
            case TextureFormat.RG8UInt: return Format.R8G8_UInt;
            case TextureFormat.RG8SInt: return Format.R8G8_SInt;
            // 32-bit formats
            case TextureFormat.R32UInt: return Format.R32_UInt;
            case TextureFormat.R32SInt: return Format.R32_SInt;
            case TextureFormat.R32Float: return Format.R32_Float;
            case TextureFormat.RG16UNorm: return Format.R16G16_UNorm;
            case TextureFormat.RG16SNorm: return Format.R16G16_SNorm;
            case TextureFormat.RG16UInt: return Format.R16G16_UInt;
            case TextureFormat.RG16SInt: return Format.R16G16_SInt;
            case TextureFormat.RG16Float: return Format.R16G16_Float;
#endif
            case Format.R8G8B8A8_UNorm:         return TextureFormat.RGBA8UNorm;
            case Format.R8G8B8A8_UNorm_SRgb:    return TextureFormat.RGBA8UNormSrgb;
            case Format.R8G8B8A8_SNorm:         return TextureFormat.RGBA8SNorm;
            case Format.R8G8B8A8_UInt:          return TextureFormat.RGBA8UInt;
            case Format.R8G8B8A8_SInt:          return TextureFormat.RGBA8SInt;
            case Format.B8G8R8A8_UNorm:         return TextureFormat.BGRA8UNorm;
            case Format.B8G8R8A8_UNorm_SRgb:    return TextureFormat.BGRA8UNormSrgb;
            // Packed 32-Bit formats
            case Format.R10G10B10A2_UNorm:      return TextureFormat.RGB10A2UNorm;
            case Format.R11G11B10_Float:        return TextureFormat.RG11B10Float;
            case Format.R9G9B9E5_SharedExp:     return TextureFormat.RGB9E5Float;
#if TODO
            // 64-Bit formats
            case TextureFormat.RG32UInt: return Format.R32G32_UInt;
            case TextureFormat.RG32SInt: return Format.R32G32_SInt;
            case TextureFormat.RG32Float: return Format.R32G32_Float;
            case TextureFormat.RGBA16UNorm: return Format.R16G16B16A16_UNorm;
            case TextureFormat.RGBA16SNorm: return Format.R16G16B16A16_SNorm;
            case TextureFormat.RGBA16UInt: return Format.R16G16B16A16_UInt;
            case TextureFormat.RGBA16SInt: return Format.R16G16B16A16_SInt;
            case TextureFormat.RGBA16Float: return Format.R16G16B16A16_Float;
            // 128-Bit formats
            case TextureFormat.RGBA32UInt: return Format.R32G32B32A32_UInt;
            case TextureFormat.RGBA32SInt: return Format.R32G32B32A32_SInt;
            case TextureFormat.RGBA32Float: return Format.R32G32B32A32_Float;
            // Depth-stencil formats
            case TextureFormat.Depth16UNorm: return Format.D16_UNorm;
            case TextureFormat.Depth32Float: return Format.D32_Float;
            case TextureFormat.Depth24UNormStencil8: return Format.D24_UNorm_S8_UInt;
            case TextureFormat.Depth32FloatStencil8: return Format.D32_Float_S8X24_UInt;
            // Compressed BC formats
            case TextureFormat.BC1UNorm: return Format.BC1_UNorm;
            case TextureFormat.BC1UNormSrgb: return Format.BC1_UNorm_SRgb;
            case TextureFormat.BC2UNorm: return Format.BC2_UNorm;
            case TextureFormat.BC2UNormSrgb: return Format.BC2_UNorm_SRgb;
            case TextureFormat.BC3UNorm: return Format.BC3_UNorm;
            case TextureFormat.BC3UNormSrgb: return Format.BC3_UNorm_SRgb;
            case TextureFormat.BC4UNorm: return Format.BC4_UNorm;
            case TextureFormat.BC4SNorm: return Format.BC4_SNorm;
            case TextureFormat.BC5UNorm: return Format.BC5_UNorm;
            case TextureFormat.BC5SNorm: return Format.BC5_SNorm;
            case TextureFormat.BC6HUFloat: return Format.BC6H_Uf16;
            case TextureFormat.BC6HSFloat: return Format.BC6H_Sf16;
            case TextureFormat.BC7UNorm: return Format.BC7_UNorm;
            case TextureFormat.BC7UNormSrgb: return Format.BC7_UNorm_SRgb; 
#endif

            default:
                return ThrowHelper.ThrowArgumentException<TextureFormat>("Invalid texture format");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GpuPreference ToDXGI(GpuPowerPreference powerPreference)
    {
        return powerPreference switch
        {
            GpuPowerPreference.HighPerformance => GpuPreference.HighPerformance,
            GpuPowerPreference.LowPower => GpuPreference.MinimumPower,
            _ => ThrowHelper.ThrowArgumentException<GpuPreference>("Invalid powerPreference"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToD3DSampleCount(TextureSampleCount sampleCount)
    {
        return sampleCount switch
        {
            TextureSampleCount.Count1 => 1,
            TextureSampleCount.Count2 => 2,
            TextureSampleCount.Count4 => 4,
            TextureSampleCount.Count8 => 8,
            TextureSampleCount.Count16 => 16,
            TextureSampleCount.Count32 => 32,
            TextureSampleCount.Count64 => 64,
            _ => 1,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureSampleCount FromD3DSampleCount(int sampleCount)
    {
        return sampleCount switch
        {
            1 => TextureSampleCount.Count1,
            2 => TextureSampleCount.Count2,
            4 => TextureSampleCount.Count4,
            8 => TextureSampleCount.Count8,
            16 => TextureSampleCount.Count16,
            32 => TextureSampleCount.Count32,
            64 => TextureSampleCount.Count64,
            _ => TextureSampleCount.Count1,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PresentModeToBufferCount(PresentMode mode)
    {
        return mode switch
        {
            PresentMode.Immediate or PresentMode.Fifo => 2,
            PresentMode.Mailbox => 3,
            _ => ThrowHelper.ThrowArgumentException<int>("Invalid present mode"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int PresentModeToSwapInterval(PresentMode mode)
    {
        switch (mode)
        {
            case PresentMode.Immediate:
            case PresentMode.Mailbox:
                return 0;
            case PresentMode.Fifo:
                return 1;
            default:
                return ThrowHelper.ThrowArgumentException<int>("Invalid present mode");
        }
    }

    public unsafe readonly struct SRWLOCK
    {
        public readonly void* Ptr;
    }

    [DllImport("kernel32", ExactSpelling = true)]
    public static extern void InitializeSRWLock(SRWLOCK* SRWLock);

    [DllImport("kernel32", ExactSpelling = true)]
    public static extern void ReleaseSRWLockExclusive(SRWLOCK* SRWLock);

    [DllImport("kernel32", ExactSpelling = true)]
    public static extern void AcquireSRWLockExclusive(SRWLOCK* SRWLock);

    [DllImport("kernel32", ExactSpelling = true)]
    public static extern byte TryAcquireSRWLockExclusive(SRWLOCK* SRWLock);
}
