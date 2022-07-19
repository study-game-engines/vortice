// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

/// <include file='IDXGIDevice.xml' path='doc/member[@name="IDXGIDevice"]/*' />
[Guid("54EC77FA-1377-44E6-8C32-88FD5F44C84C")]
[NativeTypeName("struct IDXGIDevice : IDXGIObject")]
[NativeInheritance("IDXGIObject")]
internal unsafe partial struct IDXGIDevice 
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IDXGIDevice*, Guid*, void**, int>)(lpVtbl[0]))((IDXGIDevice*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IDXGIDevice*, uint>)(lpVtbl[1]))((IDXGIDevice*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IDXGIDevice*, uint>)(lpVtbl[2]))((IDXGIDevice*)Unsafe.AsPointer(ref this));
    }
}
