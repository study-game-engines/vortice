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

    public ID3D11RenderTargetView* RTV => _rtv;

    public D3D11TextureView(D3D11Texture texture, in TextureViewDescription description)
        : base(texture, description)
    {
        D3D11GraphicsDevice device = (D3D11GraphicsDevice)texture.Device;
        TextureUsage usage = texture.Usage;

        if ((usage & TextureUsage.RenderTarget) != 0)
        {
            HResult hr = device.NativeDevice->CreateRenderTargetView(texture.Handle, null, _rtv.GetAddressOf());
            if (hr.Failure)
            {
                //LOGE("D3D11: Failed to create 2D texture");
                return;
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
