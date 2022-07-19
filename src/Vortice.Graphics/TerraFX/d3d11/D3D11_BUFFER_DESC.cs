// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

namespace TerraFX.Interop.DirectX;

internal partial struct D3D11_BUFFER_DESC
{
    public uint ByteWidth;
    public D3D11_USAGE Usage;
    public D3D11_BIND_FLAG BindFlags;
    public D3D11_CPU_ACCESS_FLAG CPUAccessFlags;
    public D3D11_RESOURCE_MISC_FLAG MiscFlags;
    public uint StructureByteStride;
}
