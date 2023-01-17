// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Win32;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi.Common;
using static Vortice.Graphics.D3D11.D3D11Utils;
using static Vortice.Graphics.D3DCommon.D3DUtils;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11Texture : Texture
{
    private readonly ComPtr<ID3D11Resource> _handle;

    public ID3D11Resource* Handle => _handle;

    public D3D11Texture(D3D11GraphicsDevice device, in TextureDescription description)
        : base(device, description)
    {
        Usage usage = (description.CpuAccess == CpuAccessMode.None) ? Win32.Graphics.Direct3D11.Usage.Default : Win32.Graphics.Direct3D11.Usage.Staging;
        BindFlags bindFlags = 0;
        CpuAccessFlags cpuAccessFlags = 0u;
        Format format = description.Format.ToDxgiFormat();
        ResourceMiscFlags miscFlags = 0;

        bool isDepthStencil = description.Format.IsDepthStencilFormat();

        // Staging textures has special threatment
        if (description.CpuAccess == CpuAccessMode.None)
        {
            if ((description.Usage & TextureUsage.ShaderRead) != TextureUsage.None)
                bindFlags |= BindFlags.ShaderResource;

            if ((description.Usage & TextureUsage.ShaderWrite) != TextureUsage.None)
                bindFlags |= BindFlags.UnorderedAccess;

            if ((description.Usage & TextureUsage.RenderTarget) != TextureUsage.None)
            {
                if (isDepthStencil)
                {
                    bindFlags |= BindFlags.DepthStencil;
                }
                else
                {
                    bindFlags |= BindFlags.RenderTarget;
                }
            }

            // If ShaderRead and depth format, set to typeless.
            if (isDepthStencil && description.Usage.HasFlag(TextureUsage.ShaderRead))
            {
                format = description.Format.GetTypelessFormatFromDepthFormat();
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
                cpuAccessFlags = CpuAccessFlags.Read;
            if (description.CpuAccess == CpuAccessMode.Write)
                cpuAccessFlags = CpuAccessFlags.Write;
        }

        if (description.Dimension == TextureDimension.Texture1D)
        {
            Texture1DDescription desc = new(
                format,
                (uint)description.Width,
                (uint)description.DepthOrArrayLayers,
                (uint)description.MipLevels,
                bindFlags,
                usage,
                cpuAccessFlags,
                miscFlags);

            HResult hr = device.NativeDevice->CreateTexture1D(&desc, null, (ID3D11Texture1D**)_handle.GetAddressOf());
            if (hr.Failure)
            {
                //LOGE("D3D11: Failed to create 1D texture");
                return;
            }
        }
        else if (description.Dimension == TextureDimension.Texture3D)
        {
        }
        else
        {
            Texture2DDescription desc = new()
            {
                Width = (uint)description.Width,
                Height = (uint)description.Height,
                MipLevels = (uint)description.MipLevels,
                ArraySize = (uint)description.DepthOrArrayLayers,
                Format = format,
                SampleDesc = new(ToSampleCount(description.SampleCount), 0),
                Usage = usage,
                BindFlags = bindFlags,
                CPUAccessFlags = cpuAccessFlags,
                MiscFlags = miscFlags
            };

            if (description.Width == description.Height &&
                description.DepthOrArrayLayers >= 6)
            {
                desc.MiscFlags |= ResourceMiscFlags.TextureCube;
            }

            HResult hr = device.NativeDevice->CreateTexture2D(&desc, null, (ID3D11Texture2D**)_handle.GetAddressOf());
            if (hr.Failure)
            {
                //LOGE("D3D11: Failed to create 2D texture");
                return;
            }
        }
    }

    public D3D11Texture(GraphicsDevice device, ID3D11Texture2D* existingTexture, in Texture2DDescription desc)
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
            base.Dispose(disposing);

            _handle.Dispose();
        }
    }

    /// <inheritdoc />
    protected override void OnLabelChanged(string newLabel)
    {
        _handle.Get()->SetDebugName(newLabel);
    }

    protected override TextureView CreateView(in TextureViewDescription description) => new D3D11TextureView(this, description);
}
