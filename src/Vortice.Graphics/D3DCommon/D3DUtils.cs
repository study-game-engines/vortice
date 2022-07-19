// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DXGI_FORMAT;
using static TerraFX.Interop.DirectX.DXGI_GPU_PREFERENCE;

namespace Vortice.Graphics.D3DCommon;

internal static unsafe class D3DUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DXGI_FORMAT ToDXGISwapChainFormat(TextureFormat format)
    {
        // FLIP_DISCARD and FLIP_SEQEUNTIAL swapchain buffers only support these formats
        switch (format)
        {
            case TextureFormat.RGBA16Float:
                return DXGI_FORMAT_R16G16B16A16_FLOAT;

            case TextureFormat.BGRA8UNorm:
            case TextureFormat.BGRA8UNormSrgb:
                return DXGI_FORMAT_B8G8R8A8_UNORM;

            case TextureFormat.RGBA8UNorm:
            case TextureFormat.RGBA8UNormSrgb:
                return DXGI_FORMAT_R8G8B8A8_UNORM;

            case TextureFormat.RGB10A2UNorm:
                return DXGI_FORMAT_R10G10B10A2_UNORM;

            default:
                return DXGI_FORMAT_B8G8R8A8_UNORM;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DXGI_FORMAT ToDXGIFormat(TextureFormat format)
    {
        switch (format)
        {
            // 8-bit formats
            case TextureFormat.R8UNorm: return DXGI_FORMAT_R8_UNORM;
            case TextureFormat.R8SNorm: return DXGI_FORMAT_R8_SNORM;
            case TextureFormat.R8UInt: return DXGI_FORMAT_R8_UINT;
            case TextureFormat.R8SInt: return DXGI_FORMAT_R8_SINT;
            // 16-bit formats
            case TextureFormat.R16UNorm: return DXGI_FORMAT_R16_UNORM;
            case TextureFormat.R16SNorm: return DXGI_FORMAT_R16_SNORM;
            case TextureFormat.R16UInt: return DXGI_FORMAT_R16_UINT;
            case TextureFormat.R16SInt: return DXGI_FORMAT_R16_SINT;
            case TextureFormat.R16Float: return DXGI_FORMAT_R16_FLOAT;
            case TextureFormat.RG8UNorm: return DXGI_FORMAT_R8G8_UNORM;
            case TextureFormat.RG8SNorm: return DXGI_FORMAT_R8G8_SNORM;
            case TextureFormat.RG8UInt: return DXGI_FORMAT_R8G8_UINT;
            case TextureFormat.RG8SInt: return DXGI_FORMAT_R8G8_SINT;
            // 32-bit formats
            case TextureFormat.R32UInt: return DXGI_FORMAT_R32_UINT;
            case TextureFormat.R32SInt: return DXGI_FORMAT_R32_SINT;
            case TextureFormat.R32Float: return DXGI_FORMAT_R32_FLOAT;
            case TextureFormat.RG16UNorm: return DXGI_FORMAT_R16G16_UNORM;
            case TextureFormat.RG16SNorm: return DXGI_FORMAT_R16G16_SNORM;
            case TextureFormat.RG16UInt: return DXGI_FORMAT_R16G16_UINT;
            case TextureFormat.RG16SInt: return DXGI_FORMAT_R16G16_SINT;
            case TextureFormat.RG16Float: return DXGI_FORMAT_R16G16_FLOAT;
            case TextureFormat.RGBA8UNorm: return DXGI_FORMAT_R8G8B8A8_UNORM;
            case TextureFormat.RGBA8UNormSrgb: return DXGI_FORMAT_R8G8B8A8_UNORM_SRGB;
            case TextureFormat.RGBA8SNorm: return DXGI_FORMAT_R8G8B8A8_SNORM;
            case TextureFormat.RGBA8UInt: return DXGI_FORMAT_R8G8B8A8_UINT;
            case TextureFormat.RGBA8SInt: return DXGI_FORMAT_R8G8B8A8_SINT;
            case TextureFormat.BGRA8UNorm: return DXGI_FORMAT_B8G8R8A8_UNORM;
            case TextureFormat.BGRA8UNormSrgb: return DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
            // Packed 32-Bit formats
            case TextureFormat.RGB10A2UNorm: return DXGI_FORMAT_R10G10B10A2_UNORM;
            case TextureFormat.RG11B10Float: return DXGI_FORMAT_R11G11B10_FLOAT;
            case TextureFormat.RGB9E5Float: return DXGI_FORMAT_R9G9B9E5_SHAREDEXP;
            // 64-Bit formats
            case TextureFormat.RG32UInt: return DXGI_FORMAT_R32G32_UINT;
            case TextureFormat.RG32SInt: return DXGI_FORMAT_R32G32_SINT;
            case TextureFormat.RG32Float: return DXGI_FORMAT_R32G32_FLOAT;
            case TextureFormat.RGBA16UNorm: return DXGI_FORMAT_R16G16B16A16_UNORM;
            case TextureFormat.RGBA16SNorm: return DXGI_FORMAT_R16G16B16A16_SNORM;
            case TextureFormat.RGBA16UInt: return DXGI_FORMAT_R16G16B16A16_UINT;
            case TextureFormat.RGBA16SInt: return DXGI_FORMAT_R16G16B16A16_SINT;
            case TextureFormat.RGBA16Float: return DXGI_FORMAT_R16G16B16A16_FLOAT;
            // 128-Bit formats
            case TextureFormat.RGBA32UInt: return DXGI_FORMAT_R32G32B32A32_UINT;
            case TextureFormat.RGBA32SInt: return DXGI_FORMAT_R32G32B32A32_SINT;
            case TextureFormat.RGBA32Float: return DXGI_FORMAT_R32G32B32A32_FLOAT;
            // Depth-stencil formats
            case TextureFormat.Depth16UNorm: return DXGI_FORMAT_D16_UNORM;
            case TextureFormat.Depth32Float: return DXGI_FORMAT_D32_FLOAT;
            case TextureFormat.Depth24UNormStencil8: return DXGI_FORMAT_D24_UNORM_S8_UINT;
            case TextureFormat.Depth32FloatStencil8: return DXGI_FORMAT_D32_FLOAT_S8X24_UINT;
            // Compressed BC formats
            case TextureFormat.BC1UNorm: return DXGI_FORMAT_BC1_UNORM;
            case TextureFormat.BC1UNormSrgb: return DXGI_FORMAT_BC1_UNORM_SRGB;
            case TextureFormat.BC2UNorm: return DXGI_FORMAT_BC2_UNORM;
            case TextureFormat.BC2UNormSrgb: return DXGI_FORMAT_BC2_UNORM_SRGB;
            case TextureFormat.BC3UNorm: return DXGI_FORMAT_BC3_UNORM;
            case TextureFormat.BC3UNormSrgb: return DXGI_FORMAT_BC3_UNORM_SRGB;
            case TextureFormat.BC4UNorm: return DXGI_FORMAT_BC4_UNORM;
            case TextureFormat.BC4SNorm: return DXGI_FORMAT_BC4_SNORM;
            case TextureFormat.BC5UNorm: return DXGI_FORMAT_BC5_UNORM;
            case TextureFormat.BC5SNorm: return DXGI_FORMAT_BC5_SNORM;
            case TextureFormat.BC6HUFloat: return DXGI_FORMAT_BC6H_UF16;
            case TextureFormat.BC6HSFloat: return DXGI_FORMAT_BC6H_SF16;
            case TextureFormat.BC7UNorm: return DXGI_FORMAT_BC7_UNORM;
            case TextureFormat.BC7UNormSrgb: return DXGI_FORMAT_BC7_UNORM_SRGB;

            default:
                return ThrowHelper.ThrowArgumentException<DXGI_FORMAT>("Invalid texture format");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DXGI_FORMAT GetTypelessFormatFromDepthFormat(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.Depth16UNorm:
                return DXGI_FORMAT_R16_TYPELESS;
            case TextureFormat.Depth32Float:
                return DXGI_FORMAT_R32_TYPELESS;
            case TextureFormat.Depth24UNormStencil8:
                return DXGI_FORMAT_R24G8_TYPELESS;
            case TextureFormat.Depth32FloatStencil8:
                return DXGI_FORMAT_R32G8X24_TYPELESS;

            default:
                Guard.IsFalse(format.IsDepthFormat(), nameof(format));
                return ToDXGIFormat(format);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToSampleCount(TextureSampleCount sampleCount)
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
    public static TextureSampleCount FromSampleCount(uint sampleCount)
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
    public static DXGI_GPU_PREFERENCE ToDXGI(GpuPowerPreference powerPreference)
    {
        return powerPreference switch
        {
            GpuPowerPreference.HighPerformance => DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE,
            GpuPowerPreference.LowPower => DXGI_GPU_PREFERENCE_MINIMUM_POWER,
            _ => ThrowHelper.ThrowArgumentException<DXGI_GPU_PREFERENCE>("Invalid power preference"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint PresentModeToBufferCount(PresentMode mode)
    {
        return mode switch
        {
            PresentMode.Immediate or PresentMode.Fifo => 2,
            PresentMode.Mailbox => 3,
            _ => ThrowHelper.ThrowArgumentException<uint>("Invalid present mode"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint PresentModeToSwapInterval(PresentMode mode)
    {
        switch (mode)
        {
            case PresentMode.Immediate:
            case PresentMode.Mailbox:
                return 0;
            case PresentMode.Fifo:
                return 1;
            default:
                return ThrowHelper.ThrowArgumentException<uint>("Invalid present mode");
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
