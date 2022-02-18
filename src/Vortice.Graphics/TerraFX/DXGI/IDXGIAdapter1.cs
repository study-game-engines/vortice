// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

/// <include file='IDXGIAdapter1.xml' path='doc/member[@name="IDXGIAdapter1"]/*' />
[Guid("29038F61-3839-4626-91FD-086879011A05")]
[NativeTypeName("struct IDXGIAdapter1 : IDXGIAdapter")]
[NativeInheritance("IDXGIAdapter")]
internal unsafe partial struct IDXGIAdapter1 : IDXGIAdapter1.Interface
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IDXGIAdapter1*, Guid*, void**, int>)(lpVtbl[0]))((IDXGIAdapter1*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IDXGIAdapter1*, uint>)(lpVtbl[1]))((IDXGIAdapter1*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IDXGIAdapter1*, uint>)(lpVtbl[2]))((IDXGIAdapter1*)Unsafe.AsPointer(ref this));
    }

    /// <include file='IDXGIAdapter1.xml' path='doc/member[@name="IDXGIAdapter1.GetDesc1"]/*' />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(10)]
    public HRESULT GetDesc1(DXGI_ADAPTER_DESC1* pDesc)
    {
        return ((delegate* unmanaged<IDXGIAdapter1*, DXGI_ADAPTER_DESC1*, int>)(lpVtbl[10]))((IDXGIAdapter1*)Unsafe.AsPointer(ref this), pDesc);
    }

    public interface Interface : IUnknown.Interface
    {
        [VtblIndex(10)]
        HRESULT GetDesc1(DXGI_ADAPTER_DESC1* pDesc);
    }
}
