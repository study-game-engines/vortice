// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Toolkit.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_DIMENSION;
using static TerraFX.Interop.DirectX.D3D12_COMMAND_LIST_TYPE;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_FLAGS;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DXGI_FORMAT;
using static TerraFX.Interop.DirectX.DXGI_GPU_PREFERENCE;
using TerraFX.Interop;

namespace Vortice.Graphics.D3D12;

internal static unsafe class D3D12Utils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DXGI_FORMAT ToDXGISwapChainFormat(TextureFormat format)
    {
        // FLIP_DISCARD and FLIP_SEQEUNTIAL swapchain buffers only support these formats
        return format switch
        {
            TextureFormat.RGBA16Float => DXGI_FORMAT_R16G16B16A16_FLOAT,
            TextureFormat.BGRA8UNorm or TextureFormat.BGRA8UNormSrgb => DXGI_FORMAT_B8G8R8A8_UNORM,
            TextureFormat.RGBA8UNorm or TextureFormat.RGBA8UNormSrgb => DXGI_FORMAT_R8G8B8A8_UNORM,
            TextureFormat.RGB10A2UNorm => DXGI_FORMAT_R10G10B10A2_UNORM,
            _ => DXGI_FORMAT_B8G8R8A8_UNORM,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DXGI_FORMAT ToDXGIFormat(TextureFormat format)
    {
        return format switch
        {
            // 8-bit formats
            TextureFormat.R8UNorm => DXGI_FORMAT_R8_UNORM,
            TextureFormat.R8SNorm => DXGI_FORMAT_R8_SNORM,
            TextureFormat.R8UInt => DXGI_FORMAT_R8_UINT,
            TextureFormat.R8SInt => DXGI_FORMAT_R8_SINT,
            // 16-bit formats
            TextureFormat.R16UNorm => DXGI_FORMAT_R16_UNORM,
            TextureFormat.R16SNorm => DXGI_FORMAT_R16_SNORM,
            TextureFormat.R16UInt => DXGI_FORMAT_R16_UINT,
            TextureFormat.R16SInt => DXGI_FORMAT_R16_SINT,
            TextureFormat.R16Float => DXGI_FORMAT_R16_FLOAT,
            TextureFormat.RG8UNorm => DXGI_FORMAT_R8G8_UNORM,
            TextureFormat.RG8SNorm => DXGI_FORMAT_R8G8_SNORM,
            TextureFormat.RG8UInt => DXGI_FORMAT_R8G8_UINT,
            TextureFormat.RG8SInt => DXGI_FORMAT_R8G8_SINT,
            // 32-bit formats
            TextureFormat.R32UInt => DXGI_FORMAT_R32_UINT,
            TextureFormat.R32SInt => DXGI_FORMAT_R32_SINT,
            TextureFormat.R32Float => DXGI_FORMAT_R32_FLOAT,
            TextureFormat.RG16UNorm => DXGI_FORMAT_R16G16_UNORM,
            TextureFormat.RG16SNorm => DXGI_FORMAT_R16G16_SNORM,
            TextureFormat.RG16UInt => DXGI_FORMAT_R16G16_UINT,
            TextureFormat.RG16SInt => DXGI_FORMAT_R16G16_SINT,
            TextureFormat.RG16Float => DXGI_FORMAT_R16G16_FLOAT,
            TextureFormat.RGBA8UNorm => DXGI_FORMAT_R8G8B8A8_UNORM,
            TextureFormat.RGBA8UNormSrgb => DXGI_FORMAT_R8G8B8A8_UNORM_SRGB,
            TextureFormat.RGBA8SNorm => DXGI_FORMAT_R8G8B8A8_SNORM,
            TextureFormat.RGBA8UInt => DXGI_FORMAT_R8G8B8A8_UINT,
            TextureFormat.RGBA8SInt => DXGI_FORMAT_R8G8B8A8_SINT,
            TextureFormat.BGRA8UNorm => DXGI_FORMAT_B8G8R8A8_UNORM,
            TextureFormat.BGRA8UNormSrgb => DXGI_FORMAT_B8G8R8A8_UNORM_SRGB,
            // Packed 32-Bit formats
            TextureFormat.RGB10A2UNorm => DXGI_FORMAT_R10G10B10A2_UNORM,
            TextureFormat.RG11B10Float => DXGI_FORMAT_R11G11B10_FLOAT,
            TextureFormat.RGB9E5Float => DXGI_FORMAT_R9G9B9E5_SHAREDEXP,
            // 64-Bit formats
            TextureFormat.RG32UInt => DXGI_FORMAT_R32G32_UINT,
            TextureFormat.RG32SInt => DXGI_FORMAT_R32G32_SINT,
            TextureFormat.RG32Float => DXGI_FORMAT_R32G32_FLOAT,
            TextureFormat.RGBA16UNorm => DXGI_FORMAT_R16G16B16A16_UNORM,
            TextureFormat.RGBA16SNorm => DXGI_FORMAT_R16G16B16A16_SNORM,
            TextureFormat.RGBA16UInt => DXGI_FORMAT_R16G16B16A16_UINT,
            TextureFormat.RGBA16SInt => DXGI_FORMAT_R16G16B16A16_SINT,
            TextureFormat.RGBA16Float => DXGI_FORMAT_R16G16B16A16_FLOAT,
            // 128-Bit formats
            TextureFormat.RGBA32UInt => DXGI_FORMAT_R32G32B32A32_UINT,
            TextureFormat.RGBA32SInt => DXGI_FORMAT_R32G32B32A32_SINT,
            TextureFormat.RGBA32Float => DXGI_FORMAT_R32G32B32A32_FLOAT,
            // Depth-stencil formats
            TextureFormat.Depth16UNorm => DXGI_FORMAT_D16_UNORM,
            TextureFormat.Depth32Float => DXGI_FORMAT_D32_FLOAT,
            TextureFormat.Depth24UNormStencil8 => DXGI_FORMAT_D24_UNORM_S8_UINT,
            TextureFormat.Depth32FloatStencil8 => DXGI_FORMAT_D32_FLOAT_S8X24_UINT,
            // Compressed BC formats
            TextureFormat.BC1RGBAUNorm => DXGI_FORMAT_BC1_UNORM,
            TextureFormat.BC1RGBAUNormSrgb => DXGI_FORMAT_BC1_UNORM_SRGB,
            TextureFormat.BC2RGBAUNorm => DXGI_FORMAT_BC2_UNORM,
            TextureFormat.BC2RGBAUNormSrgb => DXGI_FORMAT_BC2_UNORM_SRGB,
            TextureFormat.BC3RGBAUNorm => DXGI_FORMAT_BC3_UNORM,
            TextureFormat.BC3RGBAUNormSrgb => DXGI_FORMAT_BC3_UNORM_SRGB,
            TextureFormat.BC4RSNorm => DXGI_FORMAT_BC4_SNORM,
            TextureFormat.BC4RUNorm => DXGI_FORMAT_BC4_UNORM,
            TextureFormat.BC5RGSNorm => DXGI_FORMAT_BC5_SNORM,
            TextureFormat.BC5RGUNorm => DXGI_FORMAT_BC5_UNORM,
            TextureFormat.BC6HRGBUFloat => DXGI_FORMAT_BC6H_UF16,
            TextureFormat.BC6HRGBFloat => DXGI_FORMAT_BC6H_SF16,
            TextureFormat.BC7RGBAUNorm => DXGI_FORMAT_BC7_UNORM,
            TextureFormat.BC7RGBAUNormSrgb => DXGI_FORMAT_BC7_UNORM_SRGB,
            _ => ThrowHelper.ThrowArgumentException<DXGI_FORMAT>("Invalid texture format"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureFormat FromDXGIFormat(DXGI_FORMAT format)
    {
        return format switch
        {
            // 8-bit formats
            DXGI_FORMAT_R8_UNORM => TextureFormat.R8UNorm,
            DXGI_FORMAT_R8_SNORM => TextureFormat.R8SNorm,
            DXGI_FORMAT_R8_UINT => TextureFormat.R8UInt,
            DXGI_FORMAT_R8_SINT => TextureFormat.R8SInt,
            // 16-bit formats
            DXGI_FORMAT_R16_UNORM => TextureFormat.R16UNorm,
            DXGI_FORMAT_R16_SNORM => TextureFormat.R16SNorm,
            DXGI_FORMAT_R16_UINT => TextureFormat.R16UInt,
            DXGI_FORMAT_R16_SINT => TextureFormat.R16SInt,
            DXGI_FORMAT_R16_FLOAT => TextureFormat.R16Float,
            DXGI_FORMAT_R8G8_UNORM => TextureFormat.RG8UNorm,
            DXGI_FORMAT_R8G8_SNORM => TextureFormat.RG8SNorm,
            DXGI_FORMAT_R8G8_UINT => TextureFormat.RG8UInt,
            DXGI_FORMAT_R8G8_SINT => TextureFormat.RG8SInt,
            // 32-bit formats
            DXGI_FORMAT_R32_UINT => TextureFormat.R32UInt,
            DXGI_FORMAT_R32_SINT => TextureFormat.R32SInt,
            DXGI_FORMAT_R32_FLOAT => TextureFormat.R32Float,
            DXGI_FORMAT_R16G16_UNORM => TextureFormat.RG16UNorm,
            DXGI_FORMAT_R16G16_SNORM => TextureFormat.RG16SNorm,
            DXGI_FORMAT_R16G16_UINT => TextureFormat.RG16UInt,
            DXGI_FORMAT_R16G16_SINT => TextureFormat.RG16SInt,
            DXGI_FORMAT_R16G16_FLOAT => TextureFormat.RG16Float,
            DXGI_FORMAT_R8G8B8A8_UNORM => TextureFormat.RGBA8UNorm,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB => TextureFormat.RGBA8UNormSrgb,
            DXGI_FORMAT_R8G8B8A8_SNORM => TextureFormat.RGBA8SNorm,
            DXGI_FORMAT_R8G8B8A8_UINT => TextureFormat.RGBA8UInt,
            DXGI_FORMAT_R8G8B8A8_SINT => TextureFormat.RGBA8SInt,
            DXGI_FORMAT_B8G8R8A8_UNORM => TextureFormat.BGRA8UNorm,
            DXGI_FORMAT_B8G8R8A8_UNORM_SRGB => TextureFormat.BGRA8UNormSrgb,
            // Packed 32-Bit formats
            DXGI_FORMAT_R10G10B10A2_UNORM => TextureFormat.RGB10A2UNorm,
            DXGI_FORMAT_R11G11B10_FLOAT => TextureFormat.RG11B10Float,
            DXGI_FORMAT_R9G9B9E5_SHAREDEXP => TextureFormat.RGB9E5Float,
            // 64-Bit formats
            DXGI_FORMAT_R32G32_UINT => TextureFormat.RG32UInt,
            DXGI_FORMAT_R32G32_SINT => TextureFormat.RG32SInt,
            DXGI_FORMAT_R32G32_FLOAT => TextureFormat.RG32Float,
            DXGI_FORMAT_R16G16B16A16_UNORM => TextureFormat.RGBA16UNorm,
            DXGI_FORMAT_R16G16B16A16_SNORM => TextureFormat.RGBA16SNorm,
            DXGI_FORMAT_R16G16B16A16_UINT => TextureFormat.RGBA16UInt,
            DXGI_FORMAT_R16G16B16A16_SINT => TextureFormat.RGBA16SInt,
            DXGI_FORMAT_R16G16B16A16_FLOAT => TextureFormat.RGBA16Float,
            // 128-Bit formats
            DXGI_FORMAT_R32G32B32A32_UINT => TextureFormat.RGBA32UInt,
            DXGI_FORMAT_R32G32B32A32_SINT => TextureFormat.RGBA32SInt,
            DXGI_FORMAT_R32G32B32A32_FLOAT => TextureFormat.RGBA32Float,
            // Depth-stencil formats
            DXGI_FORMAT_D16_UNORM => TextureFormat.Depth16UNorm,
            DXGI_FORMAT_D32_FLOAT => TextureFormat.Depth32Float,
            DXGI_FORMAT_D24_UNORM_S8_UINT => TextureFormat.Depth24UNormStencil8,
            DXGI_FORMAT_D32_FLOAT_S8X24_UINT => TextureFormat.Depth32FloatStencil8,
            // Compressed BC formats
            DXGI_FORMAT_BC1_UNORM => TextureFormat.BC1RGBAUNorm,
            DXGI_FORMAT_BC1_UNORM_SRGB => TextureFormat.BC1RGBAUNormSrgb,
            DXGI_FORMAT_BC2_UNORM => TextureFormat.BC2RGBAUNorm,
            DXGI_FORMAT_BC2_UNORM_SRGB => TextureFormat.BC2RGBAUNormSrgb,
            DXGI_FORMAT_BC3_UNORM => TextureFormat.BC3RGBAUNorm,
            DXGI_FORMAT_BC3_UNORM_SRGB => TextureFormat.BC3RGBAUNormSrgb,
            DXGI_FORMAT_BC4_SNORM => TextureFormat.BC4RSNorm,
            DXGI_FORMAT_BC4_UNORM => TextureFormat.BC4RUNorm,
            DXGI_FORMAT_BC5_SNORM => TextureFormat.BC5RGSNorm,
            DXGI_FORMAT_BC5_UNORM => TextureFormat.BC5RGUNorm,
            DXGI_FORMAT_BC6H_UF16 => TextureFormat.BC6HRGBUFloat,
            DXGI_FORMAT_BC6H_SF16 => TextureFormat.BC6HRGBFloat,
            DXGI_FORMAT_BC7_UNORM => TextureFormat.BC7RGBAUNorm,
            DXGI_FORMAT_BC7_UNORM_SRGB => TextureFormat.BC7RGBAUNormSrgb,
            _ => ThrowHelper.ThrowArgumentException<TextureFormat>("Invalid texture format"),
        };
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
    public static uint ToD3D(TextureSampleCount sampleCount)
    {
        if (sampleCount == TextureSampleCount.None)
            return 0;

        return (uint)sampleCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureSampleCount FromD3D(uint count)
    {
        return (TextureSampleCount)count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DXGI_GPU_PREFERENCE ToDXGI(GpuPowerPreference powerPreference)
    {
        return powerPreference switch
        {
            GpuPowerPreference.HighPerformance => DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE,
            GpuPowerPreference.LowPower => DXGI_GPU_PREFERENCE_MINIMUM_POWER,
            _ => DXGI_GPU_PREFERENCE_UNSPECIFIED,
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
        return mode switch
        {
            PresentMode.Immediate or PresentMode.Mailbox => 0,
            PresentMode.Fifo => 1,
            _ => ThrowHelper.ThrowArgumentException<uint>("Invalid present mode"),
        };
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static D3D12_RESOURCE_DIMENSION ToD3D12(TextureDimension dimension)
    {
        return dimension switch
        {
            TextureDimension.Texture1D => D3D12_RESOURCE_DIMENSION_TEXTURE1D,
            TextureDimension.Texture2D => D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            TextureDimension.Texture3D => D3D12_RESOURCE_DIMENSION_TEXTURE3D,
            TextureDimension.TextureCube => D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            _ => ThrowHelper.ThrowArgumentException<D3D12_RESOURCE_DIMENSION>("Invalid texture dimension"),
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static D3D12_COMMAND_LIST_TYPE ToD3D12(CommandQueueType type)
    {
        return type switch
        {
            CommandQueueType.Compute => D3D12_COMMAND_LIST_TYPE_COMPUTE,
            CommandQueueType.Copy => D3D12_COMMAND_LIST_TYPE_COPY,
            _ => D3D12_COMMAND_LIST_TYPE_DIRECT,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureDimension FromD3D12(D3D12_RESOURCE_DIMENSION dimension)
    {
        return dimension switch
        {
            D3D12_RESOURCE_DIMENSION_TEXTURE1D => TextureDimension.Texture1D,
            D3D12_RESOURCE_DIMENSION_TEXTURE2D => TextureDimension.Texture2D,
            D3D12_RESOURCE_DIMENSION_TEXTURE3D => TextureDimension.Texture3D,
            _ => ThrowHelper.ThrowArgumentException<TextureDimension>("Invalid texture dimension"),
        };
    }

    public static TextureUsage ToTextureUsage(D3D12_RESOURCE_FLAGS flags)
    {
        TextureUsage usage = TextureUsage.None;

        if (!flags.HasFlag(D3D12_RESOURCE_FLAG_DENY_SHADER_RESOURCE))
            usage |= TextureUsage.ShaderRead;

        if (flags.HasFlag(D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS))
            usage |= TextureUsage.ShaderWrite;

        if (flags.HasFlag(D3D12_RESOURCE_FLAG_ALLOW_RENDER_TARGET))
            usage |= TextureUsage.RenderTarget;

        if (flags.HasFlag(D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL))
            usage |= TextureUsage.RenderTarget;

        return usage;
    }

    public static TextureDescriptor FromD3D12(D3D12_RESOURCE_DESC d3dDesc)
    {
        TextureDimension dimension = FromD3D12(d3dDesc.Dimension);
        TextureUsage usage = ToTextureUsage(d3dDesc.Flags);
        TextureSampleCount sampleCount = FromD3D(d3dDesc.SampleDesc.Count);

        return new TextureDescriptor(dimension,
            FromDXGIFormat(d3dDesc.Format),
            (int)d3dDesc.Width,
            (int)d3dDesc.Height,
            (int)d3dDesc.DepthOrArraySize,
            d3dDesc.MipLevels,
            usage,
            sampleCount);
    }

    public static void GetCopyableFootprint(this ref ID3D12Device5 device,
        D3D12_RESOURCE_DESC* pResourceDesc,
        out D3D12_PLACED_SUBRESOURCE_FOOTPRINT footprint,
        out uint numRows,
        out ulong rowSizeInBytes,
        out ulong totalSizeInBytes)
    {
        uint pNumRows;
        ulong pRowSizeInBytes;
        ulong pTotalBytes;

        fixed (D3D12_PLACED_SUBRESOURCE_FOOTPRINT* pFootprint = &footprint)
        {
            device.GetCopyableFootprints(pResourceDesc, 0, 1, 0, pFootprint, &pNumRows, &pRowSizeInBytes, &pTotalBytes);
        }

        numRows = pNumRows;
        rowSizeInBytes = pRowSizeInBytes;
        totalSizeInBytes = pTotalBytes;
    }

    /// <summary>
    /// Creates a new <see cref="ID3D12CommandAllocator"/> for a given device.
    /// </summary>
    /// <param name="device">The target <see cref="ID3D12Device"/> to use to create the command allocator.</param>
    /// <param name="commandListType">The type of command list allocator to create.</param>
    /// <returns>A pointer to the newly allocated <see cref="ID3D12CommandAllocator"/> instance.</returns>
    /// <exception cref="Exception">Thrown when the creation of the command allocator fails.</exception>
    public static ComPtr<ID3D12CommandAllocator> CreateCommandAllocator(this ref ID3D12Device5 device, D3D12_COMMAND_LIST_TYPE commandListType)
    {
        using ComPtr<ID3D12CommandAllocator> commandAllocator = default;

        ThrowIfFailed(
            device.CreateCommandAllocator(
            commandListType,
            Windows.__uuidof<ID3D12CommandAllocator>(),
            commandAllocator.GetVoidAddressOf())
            );

        return commandAllocator.Move();
    }

    /// <summary>
    /// Creates a new <see cref="ID3D12GraphicsCommandList4"/> for a given device.
    /// </summary>
    /// <param name="d3D12Device">The target <see cref="ID3D12Device"/> to use to create the command list.</param>
    /// <param name="d3D12CommandListType">The type of command list to create.</param>
    /// <param name="d3D12CommandAllocator">The command allocator to use to create the command list.</param>
    /// <param name="d3D12PipelineState">The initial <see cref="ID3D12PipelineState"/> object, if present.</param>
    /// <returns>A pointer to the newly allocated <see cref="ID3D12GraphicsCommandList4"/> instance.</returns>
    /// <exception cref="Exception">Thrown when the creation of the command list fails.</exception>
    public static ComPtr<ID3D12GraphicsCommandList4> CreateCommandList(
        this ref ID3D12Device5 d3D12Device,
        D3D12_COMMAND_LIST_TYPE d3D12CommandListType,
        ID3D12CommandAllocator* d3D12CommandAllocator,
        ID3D12PipelineState* d3D12PipelineState)
    {
        using ComPtr<ID3D12GraphicsCommandList4> d3D12GraphicsCommandList = default;

        ThrowIfFailed(d3D12Device.CreateCommandList(
            0,
            d3D12CommandListType,
            d3D12CommandAllocator,
            d3D12PipelineState,
            __uuidof<ID3D12GraphicsCommandList4>(),
            d3D12GraphicsCommandList.GetVoidAddressOf())
            );

        return d3D12GraphicsCommandList.Move();
    }


    [DllImport("kernel32", ExactSpelling = true)]
    public static extern void InitializeSRWLock([NativeTypeName("PSRWLOCK")] SRWLOCK* SRWLock);

    [DllImport("kernel32", ExactSpelling = true)]
    public static extern void ReleaseSRWLockExclusive([NativeTypeName("PSRWLOCK")] SRWLOCK* SRWLock);

    [DllImport("kernel32", ExactSpelling = true)]
    public static extern void AcquireSRWLockExclusive([NativeTypeName("PSRWLOCK")] SRWLOCK* SRWLock);
}


public unsafe partial struct SRWLOCK
{
    [NativeTypeName("PVOID")]
    public void* Ptr;
}
