// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/dxcapi.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved. Licensed under the University of Illinois Open Source License.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.IID;

#pragma warning disable CS0649

namespace TerraFX.Interop.DirectX;

/// <include file='IDxcUtils.xml' path='doc/member[@name="IDxcUtils"]/*' />
[Guid("4605C4CB-2019-492A-ADA4-65F20BB7D67F")]
[NativeTypeName("struct IDxcUtils : IUnknown")]
[NativeInheritance("IUnknown")]
internal unsafe partial struct IDxcUtils : IDxcUtils.Interface, INativeGuid
{
    static Guid* INativeGuid.NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in IID_IDxcUtils));

    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IDxcUtils*, Guid*, void**, int>)(lpVtbl[0]))((IDxcUtils*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IDxcUtils*, uint>)(lpVtbl[1]))((IDxcUtils*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IDxcUtils*, uint>)(lpVtbl[2]))((IDxcUtils*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(3)]
    public HRESULT CreateBlobFromBlob(IDxcBlob* pBlob, [NativeTypeName("UINT32")] uint offset, [NativeTypeName("UINT32")] uint length, IDxcBlob** ppResult)
    {
        return ((delegate* unmanaged<IDxcUtils*, IDxcBlob*, uint, uint, IDxcBlob**, int>)(lpVtbl[3]))((IDxcUtils*)Unsafe.AsPointer(ref this), pBlob, offset, length, ppResult);
    }

    /// <include file='IDxcUtils.xml' path='doc/member[@name="IDxcUtils.CreateBlobFromPinned"]/*' />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(4)]
    public HRESULT CreateBlobFromPinned([NativeTypeName("LPCVOID")] void* pData, [NativeTypeName("UINT32")] uint size, [NativeTypeName("UINT32")] uint codePage, IDxcBlobEncoding** pBlobEncoding)
    {
        return ((delegate* unmanaged<IDxcUtils*, void*, uint, uint, IDxcBlobEncoding**, int>)(lpVtbl[4]))((IDxcUtils*)Unsafe.AsPointer(ref this), pData, size, codePage, pBlobEncoding);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(9)]
    public HRESULT CreateDefaultIncludeHandler(IDxcIncludeHandler** ppResult)
    {
        return ((delegate* unmanaged<IDxcUtils*, IDxcIncludeHandler**, int>)(lpVtbl[9]))((IDxcUtils*)Unsafe.AsPointer(ref this), ppResult);
    }

    public interface Interface : IUnknown.Interface
    {
        [VtblIndex(3)]
        HRESULT CreateBlobFromBlob(IDxcBlob* pBlob, [NativeTypeName("UINT32")] uint offset, [NativeTypeName("UINT32")] uint length, IDxcBlob** ppResult);

        [VtblIndex(4)]
        HRESULT CreateBlobFromPinned([NativeTypeName("LPCVOID")] void* pData, [NativeTypeName("UINT32")] uint size, [NativeTypeName("UINT32")] uint codePage, IDxcBlobEncoding** pBlobEncoding);

        [VtblIndex(9)]
        HRESULT CreateDefaultIncludeHandler(IDxcIncludeHandler** ppResult);
    }
}
