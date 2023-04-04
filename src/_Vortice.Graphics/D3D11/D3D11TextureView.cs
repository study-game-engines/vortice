// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Win32;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi.Common;
using static Vortice.Graphics.D3D11.D3D11Utils;
using static Vortice.Graphics.D3DCommon.D3DUtils;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11TextureView : TextureView
{
    private readonly ComPtr<ID3D11RenderTargetView> _rtv;
    private readonly ComPtr<ID3D11DepthStencilView> _dsv;

    public Format DxgiFormat { get; }
    public ID3D11RenderTargetView* RTV => _rtv;
    public ID3D11DepthStencilView* DSV => _dsv;

    public D3D11TextureView(D3D11Texture texture, in TextureViewDescription description)
        : base(texture, description)
    {
        DxgiFormat = Format.ToDxgiFormat();
        D3D11GraphicsDevice device = (D3D11GraphicsDevice)texture.Device;
        TextureUsage usage = texture.Usage;

        if ((usage & TextureUsage.RenderTarget) != 0)
        {
            if (!Format.IsDepthStencilFormat())
            {
                RenderTargetViewDescription rtvDesc = new()
                {
                    Format = DxgiFormat
                };

                switch (Dimension)
                {
                    case TextureViewDimension.View1D:
                        rtvDesc.ViewDimension = RtvDimension.Texture1D;
                        rtvDesc.Texture1D.MipSlice = (uint)BaseMipLevel;
                        break;

                    case TextureViewDimension.View1DArray:
                        rtvDesc.ViewDimension = RtvDimension.Texture1DArray;
                        rtvDesc.Texture1DArray.MipSlice = (uint)BaseMipLevel;
                        rtvDesc.Texture1DArray.FirstArraySlice = (uint)BaseArrayLayer;
                        rtvDesc.Texture1DArray.ArraySize = (uint)ArrayLayerCount;
                        break;

                    case TextureViewDimension.View2D:
                        if (Texture.SampleCount > TextureSampleCount.Count1)
                        {
                            rtvDesc.ViewDimension = RtvDimension.Texture2DMs;
                        }
                        else
                        {
                            rtvDesc.ViewDimension = RtvDimension.Texture2D;
                            rtvDesc.Texture2D.MipSlice = (uint)BaseMipLevel;
                        }
                        break;

                    case TextureViewDimension.View2DArray:
                        if (Texture.SampleCount > TextureSampleCount.Count1)
                        {
                            rtvDesc.ViewDimension = RtvDimension.Texture2DMsArray;
                            rtvDesc.Texture2DMSArray.FirstArraySlice = (uint)BaseArrayLayer;
                            rtvDesc.Texture2DArray.ArraySize = (uint)ArrayLayerCount;
                        }
                        else
                        {
                            rtvDesc.ViewDimension = RtvDimension.Texture2DArray;
                            rtvDesc.Texture2DArray.MipSlice = (uint)BaseMipLevel;
                            rtvDesc.Texture2DArray.FirstArraySlice = (uint)BaseArrayLayer;
                            rtvDesc.Texture2DArray.ArraySize = (uint)ArrayLayerCount;
                        }
                        break;

                    case TextureViewDimension.View3D:
                        rtvDesc.ViewDimension = RtvDimension.Texture3D;
                        rtvDesc.Texture3D.MipSlice = (uint)BaseMipLevel;
                        rtvDesc.Texture3D.FirstWSlice = (uint)BaseArrayLayer;
                        rtvDesc.Texture3D.WSize = (uint)ArrayLayerCount;
                        break;
                }

                HResult hr = device.NativeDevice->CreateRenderTargetView(texture.Handle, &rtvDesc, _rtv.GetAddressOf());
                if (hr.Failure)
                {
                    return;
                }
            }
            else
            {
                HResult hr = device.NativeDevice->CreateDepthStencilView(texture.Handle, null, _dsv.GetAddressOf());
                if (hr.Failure)
                {
                    return;
                }
            }
        }
    }

    // <summary>
    /// Finalizes an instance of the <see cref="D3D11TextureView" /> class.
    /// </summary>
    ~D3D11TextureView() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _rtv.Dispose();
        }
    }

    /// <inheritdoc />
    protected override void OnLabelChanged(string newLabel)
    {
        if (_rtv.Get() is not null)
        {
            _rtv.Get()->SetDebugName(newLabel + "_RTV");
        }
    }
}
