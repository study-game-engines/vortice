// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Direct3D11;
using Vortice.Graphics.D3DCommon;

namespace Vortice.Graphics.D3D11;

internal class D3D11Texture : Texture
{
    public D3D11Texture(D3D11GraphicsDevice device, in TextureDescription description)
        : base(device, description)
    {
        switch (description.Dimension)
        {
        }
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
            Handle.Dispose();
        }
    }

    public ID3D11Resource Handle { get; }

    protected override void OnLabelChanged(string newLabel)
    {
        Handle.DebugName = newLabel;
    }
}
