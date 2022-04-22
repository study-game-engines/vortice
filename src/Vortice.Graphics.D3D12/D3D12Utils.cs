// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Toolkit.Diagnostics;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_DIMENSION;
using static TerraFX.Interop.DirectX.D3D12_COMMAND_LIST_TYPE;
using static TerraFX.Interop.DirectX.D3D12_RESOURCE_FLAGS;
using static Vortice.Graphics.D3DUtilities;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

namespace Vortice.Graphics.D3D12;

internal static unsafe class D3D12Utils
{
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

}
