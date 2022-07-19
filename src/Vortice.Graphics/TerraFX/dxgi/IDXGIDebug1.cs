// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/dxgidebug.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[Guid("C5A05F0C-16F2-4ADF-9F4D-A8C4D58AC550")]
[NativeTypeName("struct IDXGIDebug1 : IDXGIDebug")]
[NativeInheritance("IDXGIDebug")]
[SupportedOSPlatform("windows8.1")]
internal unsafe partial struct IDXGIDebug1
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IDXGIDebug1*, Guid*, void**, int>)(lpVtbl[0]))((IDXGIDebug1*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IDXGIDebug1*, uint>)(lpVtbl[1]))((IDXGIDebug1*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IDXGIDebug1*, uint>)(lpVtbl[2]))((IDXGIDebug1*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IDXGIDebug.ReportLiveObjects" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(3)]
    public HRESULT ReportLiveObjects(Guid apiid, DXGI_DEBUG_RLO_FLAGS flags)
    {
        return ((delegate* unmanaged<IDXGIDebug1*, Guid, DXGI_DEBUG_RLO_FLAGS, int>)(lpVtbl[3]))((IDXGIDebug1*)Unsafe.AsPointer(ref this), apiid, flags);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(4)]
    public void EnableLeakTrackingForThread()
    {
        ((delegate* unmanaged<IDXGIDebug1*, void>)(lpVtbl[4]))((IDXGIDebug1*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(5)]
    public void DisableLeakTrackingForThread()
    {
        ((delegate* unmanaged<IDXGIDebug1*, void>)(lpVtbl[5]))((IDXGIDebug1*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(6)]
    public BOOL IsLeakTrackingEnabledForThread()
    {
        return ((delegate* unmanaged<IDXGIDebug1*, int>)(lpVtbl[6]))((IDXGIDebug1*)Unsafe.AsPointer(ref this));
    }
}
