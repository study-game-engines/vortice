// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[Guid("6F15AAF2-D208-4E89-9AB4-489535D34F9C")]
[NativeTypeName("struct ID3D11Texture2D : ID3D11Resource")]
[NativeInheritance("ID3D11Resource")]
internal unsafe partial struct ID3D11Texture2D 
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<ID3D11Texture2D*, Guid*, void**, int>)(lpVtbl[0]))((ID3D11Texture2D*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<ID3D11Texture2D*, uint>)(lpVtbl[1]))((ID3D11Texture2D*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<ID3D11Texture2D*, uint>)(lpVtbl[2]))((ID3D11Texture2D*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(7)]
    public void GetType(D3D11_RESOURCE_DIMENSION* pResourceDimension)
    {
        ((delegate* unmanaged<ID3D11Texture2D*, D3D11_RESOURCE_DIMENSION*, void>)(lpVtbl[7]))((ID3D11Texture2D*)Unsafe.AsPointer(ref this), pResourceDimension);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(10)]
    public void GetDesc(D3D11_TEXTURE2D_DESC* pDesc)
    {
        ((delegate* unmanaged<ID3D11Texture2D*, D3D11_TEXTURE2D_DESC*, void>)(lpVtbl[10]))((ID3D11Texture2D*)Unsafe.AsPointer(ref this), pDesc);
    }

}
