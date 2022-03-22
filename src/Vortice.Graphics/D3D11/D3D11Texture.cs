// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Direct3D11;
using static Vortice.Direct3D11.D3D11;

namespace Vortice.Graphics.D3D11;

internal class D3D11Texture : Texture
{
    public D3D11Texture(D3D11GraphicsDevice device, in TextureDescriptor descriptor)
        : base(device, descriptor)
    {
        
    }

    public ID3D11Resource Handle { get; }

    /// <inheritdoc />
    protected override void OnDispose()
    {
    }
}
