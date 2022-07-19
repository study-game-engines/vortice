// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11sdklayers.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[Guid("6543DBB6-1B48-42F5-AB82-E97EC74326F6")]
[NativeTypeName("struct ID3D11InfoQueue : IUnknown")]
[NativeInheritance("IUnknown")]
internal unsafe partial struct ID3D11InfoQueue
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
    [VtblIndex(12)]
    public HRESULT AddStorageFilterEntries(D3D11_INFO_QUEUE_FILTER* pFilter)
    {
        return ((delegate* unmanaged<ID3D11InfoQueue*, D3D11_INFO_QUEUE_FILTER*, int>)(lpVtbl[12]))((ID3D11InfoQueue*)Unsafe.AsPointer(ref this), pFilter);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(15)]
    public HRESULT PushEmptyStorageFilter()
    {
        return ((delegate* unmanaged<ID3D11InfoQueue*, int>)(lpVtbl[15]))((ID3D11InfoQueue*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(31)]
    public HRESULT SetBreakOnSeverity(D3D11_MESSAGE_SEVERITY Severity, BOOL bEnable)
    {
        return ((delegate* unmanaged<ID3D11InfoQueue*, D3D11_MESSAGE_SEVERITY, BOOL, int>)(lpVtbl[31]))((ID3D11InfoQueue*)Unsafe.AsPointer(ref this), Severity, bEnable);
    }
}
