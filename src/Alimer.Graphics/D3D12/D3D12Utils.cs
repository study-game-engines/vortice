// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D_PRIMITIVE_TOPOLOGY;
using static TerraFX.Interop.DirectX.DXGI_FORMAT;
using static TerraFX.Interop.DirectX.DXGI_GPU_PREFERENCE;

namespace Alimer.Graphics.D3D12;

internal static unsafe class D3DUtils
{
    private static readonly D3D_PRIMITIVE_TOPOLOGY[] s_d3dPrimitiveTopologyMap = new D3D_PRIMITIVE_TOPOLOGY[(int)PrimitiveTopology.Count] {
        D3D_PRIMITIVE_TOPOLOGY_POINTLIST,
        D3D_PRIMITIVE_TOPOLOGY_LINELIST,
        D3D_PRIMITIVE_TOPOLOGY_LINESTRIP,
        D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST,
        D3D_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DXGI_FORMAT ToDxgiSwapChainFormat(this PixelFormat format)
    {
        // FLIP_DISCARD and FLIP_SEQEUNTIAL swapchain buffers only support these formats
        switch (format)
        {
            case PixelFormat.Rgba16Float:
                return DXGI_FORMAT_R16G16B16A16_FLOAT;

            case PixelFormat.Bgra8Unorm:
            case PixelFormat.Bgra8UnormSrgb:
                return DXGI_FORMAT_B8G8R8A8_UNORM;

            case PixelFormat.Rgba8Unorm:
            case PixelFormat.Rgba8UnormSrgb:
                return DXGI_FORMAT_R8G8B8A8_UNORM;

            case PixelFormat.Rgb10a2Unorm:
                return DXGI_FORMAT_R10G10B10A2_UNORM;

            default:
                return DXGI_FORMAT_B8G8R8A8_UNORM;
        }
    }

    public static DXGI_FORMAT ToDxgiFormat(this PixelFormat format)
    {
        switch (format)
        {
            // 8-bit formats
            case PixelFormat.R8Unorm: return DXGI_FORMAT_R8_UNORM;
            case PixelFormat.R8Snorm: return DXGI_FORMAT_R8_SNORM;
            case PixelFormat.R8Uint: return DXGI_FORMAT_R8_UINT;
            case PixelFormat.R8Sint: return DXGI_FORMAT_R8_SINT;
            // 16-bit formats
            case PixelFormat.R16Unorm: return DXGI_FORMAT_R16_UNORM;
            case PixelFormat.R16Snorm: return DXGI_FORMAT_R16_SNORM;
            case PixelFormat.R16Uint: return DXGI_FORMAT_R16_UINT;
            case PixelFormat.R16Sint: return DXGI_FORMAT_R16_SINT;
            case PixelFormat.R16Float: return DXGI_FORMAT_R16_FLOAT;
            case PixelFormat.Rg8Unorm: return DXGI_FORMAT_R8G8_UNORM;
            case PixelFormat.Rg8Snorm: return DXGI_FORMAT_R8G8_SNORM;
            case PixelFormat.Rg8Uint: return DXGI_FORMAT_R8G8_UINT;
            case PixelFormat.Rg8Sint: return DXGI_FORMAT_R8G8_SINT;
            // Packed 16-Bit Pixel Formats
            case PixelFormat.Bgra4Unorm: return DXGI_FORMAT_B4G4R4A4_UNORM;
            case PixelFormat.B5G6R5Unorm: return DXGI_FORMAT_B5G6R5_UNORM;
            case PixelFormat.Bgr5A1Unorm: return DXGI_FORMAT_B5G5R5A1_UNORM;
            // 32-bit formats
            case PixelFormat.R32Uint: return DXGI_FORMAT_R32_UINT;
            case PixelFormat.R32Sint: return DXGI_FORMAT_R32_SINT;
            case PixelFormat.R32Float: return DXGI_FORMAT_R32_FLOAT;
            case PixelFormat.Rg16Unorm: return DXGI_FORMAT_R16G16_UNORM;
            case PixelFormat.Rg16Snorm: return DXGI_FORMAT_R16G16_SNORM;
            case PixelFormat.Rg16Uint: return DXGI_FORMAT_R16G16_UINT;
            case PixelFormat.Rg16Sint: return DXGI_FORMAT_R16G16_SINT;
            case PixelFormat.Rg16Float: return DXGI_FORMAT_R16G16_FLOAT;
            case PixelFormat.Rgba8Unorm: return DXGI_FORMAT_R8G8B8A8_UNORM;
            case PixelFormat.Rgba8UnormSrgb: return DXGI_FORMAT_R8G8B8A8_UNORM_SRGB;
            case PixelFormat.Rgba8Snorm: return DXGI_FORMAT_R8G8B8A8_SNORM;
            case PixelFormat.Rgba8Uint: return DXGI_FORMAT_R8G8B8A8_UINT;
            case PixelFormat.Rgba8Sint: return DXGI_FORMAT_R8G8B8A8_SINT;
            case PixelFormat.Bgra8Unorm: return DXGI_FORMAT_B8G8R8A8_UNORM;
            case PixelFormat.Bgra8UnormSrgb: return DXGI_FORMAT_B8G8R8A8_UNORM_SRGB;
            // Packed 32-Bit formats
            case PixelFormat.Rgb9e5Ufloat: return DXGI_FORMAT_R9G9B9E5_SHAREDEXP;
            case PixelFormat.Rgb10a2Unorm: return DXGI_FORMAT_R10G10B10A2_UNORM;
            case PixelFormat.Rgb10a2Uint: return DXGI_FORMAT_R10G10B10A2_UINT;
            case PixelFormat.Rg11b10Float: return DXGI_FORMAT_R11G11B10_FLOAT;
            // 64-Bit formats
            case PixelFormat.Rg32Uint: return DXGI_FORMAT_R32G32_UINT;
            case PixelFormat.Rg32Sint: return DXGI_FORMAT_R32G32_SINT;
            case PixelFormat.Rg32Float: return DXGI_FORMAT_R32G32_FLOAT;
            case PixelFormat.Rgba16Unorm: return DXGI_FORMAT_R16G16B16A16_UNORM;
            case PixelFormat.Rgba16Snorm: return DXGI_FORMAT_R16G16B16A16_SNORM;
            case PixelFormat.Rgba16Uint: return DXGI_FORMAT_R16G16B16A16_UINT;
            case PixelFormat.Rgba16Sint: return DXGI_FORMAT_R16G16B16A16_SINT;
            case PixelFormat.Rgba16Float: return DXGI_FORMAT_R16G16B16A16_FLOAT;
            // 128-Bit formats
            case PixelFormat.Rgba32Uint: return DXGI_FORMAT_R32G32B32A32_UINT;
            case PixelFormat.Rgba32Sint: return DXGI_FORMAT_R32G32B32A32_SINT;
            case PixelFormat.Rgba32Float: return DXGI_FORMAT_R32G32B32A32_FLOAT;
            // Depth-stencil formats
            case PixelFormat.Depth16Unorm: return DXGI_FORMAT_D16_UNORM;
            case PixelFormat.Depth32Float: return DXGI_FORMAT_D32_FLOAT;
            case PixelFormat.Stencil8: return DXGI_FORMAT_D24_UNORM_S8_UINT;
            case PixelFormat.Depth24UnormStencil8: return DXGI_FORMAT_D24_UNORM_S8_UINT;
            case PixelFormat.Depth32FloatStencil8: return DXGI_FORMAT_D32_FLOAT_S8X24_UINT;
            // Compressed BC formats
            case PixelFormat.Bc1RgbaUnorm: return DXGI_FORMAT_BC1_UNORM;
            case PixelFormat.Bc1RgbaUnormSrgb: return DXGI_FORMAT_BC1_UNORM_SRGB;
            case PixelFormat.Bc2RgbaUnorm: return DXGI_FORMAT_BC2_UNORM;
            case PixelFormat.Bc2RgbaUnormSrgb: return DXGI_FORMAT_BC2_UNORM_SRGB;
            case PixelFormat.Bc3RgbaUnorm: return DXGI_FORMAT_BC3_UNORM;
            case PixelFormat.Bc3RgbaUnormSrgb: return DXGI_FORMAT_BC3_UNORM_SRGB;
            case PixelFormat.Bc4RSnorm: return DXGI_FORMAT_BC4_UNORM;
            case PixelFormat.Bc4RUnorm: return DXGI_FORMAT_BC4_SNORM;
            case PixelFormat.Bc5RgUnorm: return DXGI_FORMAT_BC5_UNORM;
            case PixelFormat.Bc5RgSnorm: return DXGI_FORMAT_BC5_SNORM;
            case PixelFormat.Bc6hRgbSfloat: return DXGI_FORMAT_BC6H_SF16;
            case PixelFormat.Bc6hRgbUfloat: return DXGI_FORMAT_BC6H_UF16;
            case PixelFormat.Bc7RgbaUnorm: return DXGI_FORMAT_BC7_UNORM;
            case PixelFormat.Bc7RgbaUnormSrgb: return DXGI_FORMAT_BC7_UNORM_SRGB;

            default:
                return DXGI_FORMAT_UNKNOWN;
        }
    }

    public static PixelFormat FromDxgiFormat(this DXGI_FORMAT format)
    {
        switch (format)
        {
            // 8-bit formats
            case DXGI_FORMAT_R8_UNORM: return PixelFormat.R8Unorm;
            case DXGI_FORMAT_R8_SNORM: return PixelFormat.R8Snorm;
            case DXGI_FORMAT_R8_UINT: return PixelFormat.R8Uint;
            case DXGI_FORMAT_R8_SINT: return PixelFormat.R8Sint;
            // 16-bit formats
            case DXGI_FORMAT_R16_UNORM: return PixelFormat.R16Unorm;
            case DXGI_FORMAT_R16_SNORM: return PixelFormat.R16Snorm;
            case DXGI_FORMAT_R16_UINT: return PixelFormat.R16Uint;
            case DXGI_FORMAT_R16_SINT: return PixelFormat.R16Sint;
            case DXGI_FORMAT_R16_FLOAT: return PixelFormat.R16Float;
            case DXGI_FORMAT_R8G8_UNORM: return PixelFormat.Rg8Unorm;
            case DXGI_FORMAT_R8G8_SNORM: return PixelFormat.Rg8Snorm;
            case DXGI_FORMAT_R8G8_UINT: return PixelFormat.Rg8Uint;
            case DXGI_FORMAT_R8G8_SINT: return PixelFormat.Rg8Sint;
            // Packed 16-Bit Pixel Formats
            case DXGI_FORMAT_B4G4R4A4_UNORM: return PixelFormat.Bgra4Unorm;
            case DXGI_FORMAT_B5G6R5_UNORM: return PixelFormat.B5G6R5Unorm;
            case DXGI_FORMAT_B5G5R5A1_UNORM: return PixelFormat.Bgr5A1Unorm;
            // 32-bit formats
            case DXGI_FORMAT_R32_UINT: return PixelFormat.R32Uint;
            case DXGI_FORMAT_R32_SINT: return PixelFormat.R32Sint;
            case DXGI_FORMAT_R32_FLOAT: return PixelFormat.R32Float;
            case DXGI_FORMAT_R16G16_UNORM: return PixelFormat.Rg16Unorm;
            case DXGI_FORMAT_R16G16_SNORM: return PixelFormat.Rg16Snorm;
            case DXGI_FORMAT_R16G16_UINT: return PixelFormat.Rg16Uint;
            case DXGI_FORMAT_R16G16_SINT: return PixelFormat.Rg16Sint;
            case DXGI_FORMAT_R16G16_FLOAT: return PixelFormat.Rg16Float;
            case DXGI_FORMAT_R8G8B8A8_UNORM: return PixelFormat.Rgba8Unorm;
            case DXGI_FORMAT_R8G8B8A8_UNORM_SRGB: return PixelFormat.Rgba8UnormSrgb;
            case DXGI_FORMAT_R8G8B8A8_SNORM: return PixelFormat.Rgba8Snorm;
            case DXGI_FORMAT_R8G8B8A8_UINT: return PixelFormat.Rgba8Uint;
            case DXGI_FORMAT_R8G8B8A8_SINT: return PixelFormat.Rgba8Sint;
            case DXGI_FORMAT_B8G8R8A8_UNORM: return PixelFormat.Bgra8Unorm;
            case DXGI_FORMAT_B8G8R8A8_UNORM_SRGB: return PixelFormat.Bgra8UnormSrgb;
            // Packed 32-Bit formats
            case DXGI_FORMAT_R9G9B9E5_SHAREDEXP: return PixelFormat.Rgb9e5Ufloat;
            case DXGI_FORMAT_R10G10B10A2_UNORM: return PixelFormat.Rgb10a2Unorm;
            case DXGI_FORMAT_R10G10B10A2_UINT: return PixelFormat.Rgb10a2Uint;
            case DXGI_FORMAT_R11G11B10_FLOAT: return PixelFormat.Rg11b10Float;
            // 64-Bit formats
            case DXGI_FORMAT_R32G32_UINT: return PixelFormat.Rg32Uint;
            case DXGI_FORMAT_R32G32_SINT: return PixelFormat.Rg32Sint;
            case DXGI_FORMAT_R32G32_FLOAT: return PixelFormat.Rg32Float;
            case DXGI_FORMAT_R16G16B16A16_UNORM: return PixelFormat.Rgba16Unorm;
            case DXGI_FORMAT_R16G16B16A16_SNORM: return PixelFormat.Rgba16Snorm;
            case DXGI_FORMAT_R16G16B16A16_UINT: return PixelFormat.Rgba16Uint;
            case DXGI_FORMAT_R16G16B16A16_SINT: return PixelFormat.Rgba16Sint;
            case DXGI_FORMAT_R16G16B16A16_FLOAT: return PixelFormat.Rgba16Float;
            // 128-Bit formats
            case DXGI_FORMAT_R32G32B32A32_UINT: return PixelFormat.Rgba32Uint;
            case DXGI_FORMAT_R32G32B32A32_SINT: return PixelFormat.Rgba32Sint;
            case DXGI_FORMAT_R32G32B32A32_FLOAT: return PixelFormat.Rgba32Float;
            // Depth-stencil formats
            case DXGI_FORMAT_D16_UNORM: return PixelFormat.Depth16Unorm;
            case DXGI_FORMAT_D32_FLOAT: return PixelFormat.Depth32Float;
            //case DXGI_FORMAT_D24UnormS8Uint: return PixelFormat.Stencil8;
            case DXGI_FORMAT_D24_UNORM_S8_UINT: return PixelFormat.Depth24UnormStencil8;
            case DXGI_FORMAT_D32_FLOAT_S8X24_UINT: return PixelFormat.Depth32FloatStencil8;
            // Compressed BC formats
            case DXGI_FORMAT_BC1_UNORM: return PixelFormat.Bc1RgbaUnorm;
            case DXGI_FORMAT_BC1_UNORM_SRGB: return PixelFormat.Bc1RgbaUnormSrgb;
            case DXGI_FORMAT_BC2_UNORM: return PixelFormat.Bc2RgbaUnorm;
            case DXGI_FORMAT_BC2_UNORM_SRGB: return PixelFormat.Bc2RgbaUnormSrgb;
            case DXGI_FORMAT_BC3_UNORM: return PixelFormat.Bc3RgbaUnorm;
            case DXGI_FORMAT_BC3_UNORM_SRGB: return PixelFormat.Bc3RgbaUnormSrgb;
            case DXGI_FORMAT_BC4_UNORM: return PixelFormat.Bc4RSnorm;
            case DXGI_FORMAT_BC4_SNORM: return PixelFormat.Bc4RUnorm;
            case DXGI_FORMAT_BC5_UNORM: return PixelFormat.Bc5RgUnorm;
            case DXGI_FORMAT_BC5_SNORM: return PixelFormat.Bc5RgSnorm;
            case DXGI_FORMAT_BC6H_SF16: return PixelFormat.Bc6hRgbSfloat;
            case DXGI_FORMAT_BC6H_UF16: return PixelFormat.Bc6hRgbUfloat;
            case DXGI_FORMAT_BC7_UNORM: return PixelFormat.Bc7RgbaUnorm;
            case DXGI_FORMAT_BC7_UNORM_SRGB: return PixelFormat.Bc7RgbaUnormSrgb;

            default:
                return PixelFormat.Undefined;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DXGI_FORMAT GetTypelessFormatFromDepthFormat(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.Depth16Unorm:
                return DXGI_FORMAT_R16_TYPELESS;
            case PixelFormat.Depth32Float:
                return DXGI_FORMAT_R32_TYPELESS;
            case PixelFormat.Depth24UnormStencil8:
                return DXGI_FORMAT_R24G8_TYPELESS;
            case PixelFormat.Depth32FloatStencil8:
                return DXGI_FORMAT_R32G8X24_TYPELESS;

            default:
                Guard.IsFalse(format.IsDepthFormat(), nameof(format));
                return ToDxgiFormat(format);
        }
    }

    public static DXGI_FORMAT ToDxgiFormat(this VertexFormat format)
    {
        switch (format)
        {
            case VertexFormat.Uint8x2: return DXGI_FORMAT_R8G8_UINT;
            case VertexFormat.Uint8x4: return DXGI_FORMAT_R8G8B8A8_UINT;
            case VertexFormat.Sint8x2: return DXGI_FORMAT_R8G8_SINT;
            case VertexFormat.Sint8x4: return DXGI_FORMAT_R8G8B8A8_SINT;
            case VertexFormat.Unorm8x2: return DXGI_FORMAT_R8G8_UNORM;
            case VertexFormat.Unorm8x4: return DXGI_FORMAT_R8G8B8A8_UNORM;
            case VertexFormat.Snorm8x2: return DXGI_FORMAT_R8G8_SNORM;
            case VertexFormat.Snorm8x4: return DXGI_FORMAT_R8G8B8A8_SNORM;

            case VertexFormat.Uint16x2: return DXGI_FORMAT_R16G16_UINT;
            case VertexFormat.Uint16x4: return DXGI_FORMAT_R16G16B16A16_UINT;
            case VertexFormat.Sint16x2: return DXGI_FORMAT_R16G16_SINT;
            case VertexFormat.Sint16x4: return DXGI_FORMAT_R16G16B16A16_SINT;
            case VertexFormat.Unorm16x2: return DXGI_FORMAT_R16G16_UNORM;
            case VertexFormat.Unorm16x4: return DXGI_FORMAT_R16G16B16A16_UNORM;
            case VertexFormat.Snorm16x2: return DXGI_FORMAT_R16G16_SNORM;
            case VertexFormat.Snorm16x4: return DXGI_FORMAT_R16G16B16A16_SNORM;
            case VertexFormat.Float16x2: return DXGI_FORMAT_R16G16_FLOAT;
            case VertexFormat.Float16x4: return DXGI_FORMAT_R16G16B16A16_FLOAT;

            case VertexFormat.Float32: return DXGI_FORMAT_R32_FLOAT;
            case VertexFormat.Float32x2: return DXGI_FORMAT_R32G32_FLOAT;
            case VertexFormat.Float32x3: return DXGI_FORMAT_R32G32B32_FLOAT;
            case VertexFormat.Float32x4: return DXGI_FORMAT_R32G32B32A32_FLOAT;

            case VertexFormat.UInt: return DXGI_FORMAT_R32_UINT;
            case VertexFormat.UInt2: return DXGI_FORMAT_R32G32_UINT;
            case VertexFormat.UInt3: return DXGI_FORMAT_R32G32B32_UINT;
            case VertexFormat.UInt4: return DXGI_FORMAT_R32G32B32A32_UINT;

            case VertexFormat.Sint32: return DXGI_FORMAT_R32_SINT;
            case VertexFormat.Sint32x2: return DXGI_FORMAT_R32G32_SINT;
            case VertexFormat.Sint32x3: return DXGI_FORMAT_R32G32B32_SINT;
            case VertexFormat.Sint32x4: return DXGI_FORMAT_R32G32B32A32_SINT;

            case VertexFormat.RGB10A2Unorm: return DXGI_FORMAT_R10G10B10A2_UNORM;

            default:
                return DXGI_FORMAT_UNKNOWN;
        }
    }

    public static DXGI_FORMAT ToDxgiFormat(this IndexType indexType)
    {
        switch (indexType)
        {
            case IndexType.Uint16: return DXGI_FORMAT_R16_UINT;
            case IndexType.Uint32: return DXGI_FORMAT_R32_UINT;

            default:
                return DXGI_FORMAT_R16_UINT;
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

    public static DXGI_GPU_PREFERENCE ToDxgi(this GpuPowerPreference preference)
    {
        switch (preference)
        {
            case GpuPowerPreference.LowPower:
                return DXGI_GPU_PREFERENCE_MINIMUM_POWER;

            default:
            case GpuPowerPreference.HighPerformance:
                return DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static D3D_PRIMITIVE_TOPOLOGY ToD3DPrimitiveTopology(this PrimitiveTopology value) => s_d3dPrimitiveTopologyMap[(uint)value];

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
}

unsafe partial class Kernel32
{
    [LibraryImport("kernel32")]
    public static partial void InitializeSRWLock(void* SRWLock);

    [LibraryImport("kernel32")]
    public static partial void AcquireSRWLockExclusive(void* SRWLock);

    [LibraryImport("kernel32")]
    public static partial void ReleaseSRWLockExclusive(void* SRWLock);
}
