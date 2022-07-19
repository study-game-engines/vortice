// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11_1.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

/// <include file='ID3D11DeviceContext1.xml' path='doc/member[@name="ID3D11DeviceContext1"]/*' />
[Guid("BB2C6FAA-B5FB-4082-8E6B-388B8CFA90E1")]
[NativeTypeName("struct ID3D11DeviceContext1 : ID3D11DeviceContext")]
[NativeInheritance("ID3D11DeviceContext")]
internal unsafe partial struct ID3D11DeviceContext1
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<ID3D11DeviceContext1*, Guid*, void**, int>)(lpVtbl[0]))((ID3D11DeviceContext1*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<ID3D11DeviceContext1*, uint>)(lpVtbl[1]))((ID3D11DeviceContext1*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<ID3D11DeviceContext1*, uint>)(lpVtbl[2]))((ID3D11DeviceContext1*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(58)]
    public void ExecuteCommandList(ID3D11CommandList* pCommandList, BOOL RestoreContextState)
    {
        ((delegate* unmanaged<ID3D11DeviceContext1*, ID3D11CommandList*, BOOL, void>)(lpVtbl[58]))((ID3D11DeviceContext1*)Unsafe.AsPointer(ref this), pCommandList, RestoreContextState);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(110)]
    public void ClearState()
    {
        ((delegate* unmanaged<ID3D11DeviceContext1*, void>)(lpVtbl[110]))((ID3D11DeviceContext1*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(111)]
    public void Flush()
    {
        ((delegate* unmanaged<ID3D11DeviceContext1*, void>)(lpVtbl[111]))((ID3D11DeviceContext1*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(114)]
    public HRESULT FinishCommandList(BOOL RestoreDeferredContextState, ID3D11CommandList** ppCommandList)
    {
        return ((delegate* unmanaged<ID3D11DeviceContext1*, BOOL, ID3D11CommandList**, int>)(lpVtbl[114]))((ID3D11DeviceContext1*)Unsafe.AsPointer(ref this), RestoreDeferredContextState, ppCommandList);
    }
}
