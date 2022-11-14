// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3D11_USAGE;
using static TerraFX.Interop.DirectX.D3D11_BIND_FLAG;
using static TerraFX.Interop.DirectX.D3D11_CPU_ACCESS_FLAG;
using static TerraFX.Interop.DirectX.D3D11_RESOURCE_MISC_FLAG;
using static Vortice.Graphics.D3D11.D3D11Utils;
using static Vortice.Graphics.D3DCommon.D3DUtils;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11Texture : Texture
{
    private readonly ComPtr<ID3D11Resource> _handle;

    public D3D11Texture(D3D11GraphicsDevice device, in TextureDescription description)
        : base(device, description)
    {
        D3D11_USAGE usage = (description.CpuAccess == CpuAccessMode.None) ? D3D11_USAGE_DEFAULT : D3D11_USAGE_STAGING;
        D3D11_BIND_FLAG bindFlags = 0;
        D3D11_CPU_ACCESS_FLAG cpuAccessFlags = 0u;
        DXGI_FORMAT format = ToDXGIFormat(description.Format);
        D3D11_RESOURCE_MISC_FLAG miscFlags = 0;

        bool isDepthStencil = description.Format.IsDepthStencilFormat();

        // Staging textures has special threatment
        if (description.CpuAccess == CpuAccessMode.None)
        {
            if ((description.Usage & TextureUsage.ShaderRead) != TextureUsage.None)
                bindFlags |= D3D11_BIND_SHADER_RESOURCE;

            if ((description.Usage & TextureUsage.ShaderWrite) != TextureUsage.None)
                bindFlags |= D3D11_BIND_UNORDERED_ACCESS;

            if ((description.Usage & TextureUsage.RenderTarget) != TextureUsage.None)
            {
                if (isDepthStencil)
                {
                    bindFlags |= D3D11_BIND_DEPTH_STENCIL;
                }
                else
                {
                    bindFlags |= D3D11_BIND_RENDER_TARGET;
                }
            }

            // If ShaderRead and depth format, set to typeless.
            if (isDepthStencil && description.Usage.HasFlag(TextureUsage.ShaderRead))
            {
                format = GetTypelessFormatFromDepthFormat(description.Format);
            }

            // Mutually exclusive.
            //if (CheckBitsAny(info.sharedResourceFlags, SharedResourceFlags::Shared_NTHandle))
            //{
            //    miscFlags |= D3D11_RESOURCE_MISC_SHARED_KEYEDMUTEX | D3D11_RESOURCE_MISC_SHARED_NTHANDLE;
            //}
            //else if (CheckBitsAny(info.sharedResourceFlags, SharedResourceFlags::Shared))
            //{
            //    miscFlags |= D3D11_RESOURCE_MISC_SHARED;
            //}
        }
        else
        {
            if (description.CpuAccess == CpuAccessMode.Read)
                cpuAccessFlags = D3D11_CPU_ACCESS_READ;
            if (description.CpuAccess == CpuAccessMode.Write)
                cpuAccessFlags = D3D11_CPU_ACCESS_WRITE;
        }

        if (description.Dimension == TextureDimension.Texture1D)
        {
        }
        else if (description.Dimension == TextureDimension.Texture3D)
        {
        }
        else
        {
            D3D11_TEXTURE2D_DESC desc = new();
            desc.Width = (uint)description.Width;
            desc.Height = (uint)description.Height;
            desc.MipLevels = (uint)description.MipLevels;
            desc.ArraySize = (uint)description.DepthOrArraySize;
            desc.Format = format;
            desc.SampleDesc = new DXGI_SAMPLE_DESC(ToSampleCount(description.SampleCount), 0);
            desc.Usage = usage;
            desc.BindFlags = (uint)bindFlags;
            desc.CPUAccessFlags = (uint)cpuAccessFlags;
            desc.MiscFlags = (uint)miscFlags;

            if (description.SampleCount == TextureSampleCount.Count1 && desc.Width == desc.Height && (description.DepthOrArraySize % 6 == 0))
            {
                desc.MiscFlags |= (uint)D3D11_RESOURCE_MISC_TEXTURECUBE;
            }

            HRESULT hr = device.NativeDevice->CreateTexture2D(&desc, null, (ID3D11Texture2D**)_handle.GetAddressOf());
            if (hr.FAILED)
            {
                //LOGE("D3D11: Failed to create 2D texture");
                return;
            }
        }
    }

    public D3D11Texture(GraphicsDevice device, ID3D11Texture2D* existingTexture, in D3D11_TEXTURE2D_DESC desc)
        : base(device, FromD3D11(desc))
    {
        // Keep reference to texture.
        _handle.Attach((ID3D11Resource*)existingTexture);
        _ = existingTexture->AddRef();
    }


    // <summary>
    /// Finalizes an instance of the <see cref="D3D11Texture" /> class.
    /// </summary>
    ~D3D11Texture() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _handle.Dispose();
        }
    }

    public ID3D11Resource* Handle => _handle;

    protected override void OnLabelChanged(string newLabel)
    {
        //Handle.DebugName = newLabel;
    }
}
