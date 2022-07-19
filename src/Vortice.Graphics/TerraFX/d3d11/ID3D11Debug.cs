// Copyright � Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11sdklayers.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright � Microsoft. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[Guid("79CF2233-7536-4948-9D36-1E4692DC5760")]
[NativeTypeName("struct ID3D11Debug : IUnknown")]
[NativeInheritance("IUnknown")]
internal unsafe partial struct ID3D11Debug
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<ID3D11Device*, Guid*, void**, int>)(lpVtbl[0]))((ID3D11Device*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<ID3D11Device*, uint>)(lpVtbl[1]))((ID3D11Device*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<ID3D11Device*, uint>)(lpVtbl[2]))((ID3D11Device*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(10)]
    public HRESULT ReportLiveDeviceObjects(D3D11_RLDO_FLAGS Flags)
    {
        return ((delegate* unmanaged<ID3D11Debug*, D3D11_RLDO_FLAGS, int>)(lpVtbl[10]))((ID3D11Debug*)Unsafe.AsPointer(ref this), Flags);
    }
}
