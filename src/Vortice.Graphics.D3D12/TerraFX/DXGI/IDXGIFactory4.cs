// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi1_4.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

/// <include file='IDXGIFactory4.xml' path='doc/member[@name="IDXGIFactory4"]/*' />
[Guid("1BC6EA02-EF36-464F-BF0C-21CA39E5168A")]
[NativeTypeName("struct IDXGIFactory4 : IDXGIFactory3")]
[NativeInheritance("IDXGIFactory3")]
internal unsafe partial struct IDXGIFactory4 
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IDXGIFactory4*, Guid*, void**, int>)(lpVtbl[0]))((IDXGIFactory4*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IDXGIFactory4*, uint>)(lpVtbl[1]))((IDXGIFactory4*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IDXGIFactory4*, uint>)(lpVtbl[2]))((IDXGIFactory4*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(8)]
    public HRESULT MakeWindowAssociation(IntPtr WindowHandle, uint Flags)
    {
        return ((delegate* unmanaged<IDXGIFactory4*, IntPtr, uint, int>)(lpVtbl[8]))((IDXGIFactory4*)Unsafe.AsPointer(ref this), WindowHandle, Flags);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(12)]
    public HRESULT EnumAdapters1(uint Adapter, IDXGIAdapter1** ppAdapter)
    {
        return ((delegate* unmanaged<IDXGIFactory4*, uint, IDXGIAdapter1**, int>)(lpVtbl[12]))((IDXGIFactory4*)Unsafe.AsPointer(ref this), Adapter, ppAdapter);
    }

    /// <inheritdoc cref="IDXGIFactory1.IsCurrent" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(13)]
    public BOOL IsCurrent()
    {
        return ((delegate* unmanaged<IDXGIFactory4*, int>)(lpVtbl[13]))((IDXGIFactory4*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IDXGIFactory2.IsWindowedStereoEnabled" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(14)]
    public BOOL IsWindowedStereoEnabled()
    {
        return ((delegate* unmanaged<IDXGIFactory4*, int>)(lpVtbl[14]))((IDXGIFactory4*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IDXGIFactory2.CreateSwapChainForHwnd" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(15)]
    public HRESULT CreateSwapChainForHwnd(IUnknown* pDevice, IntPtr hWnd, [NativeTypeName("const DXGI_SWAP_CHAIN_DESC1 *")] DXGI_SWAP_CHAIN_DESC1* pDesc, [NativeTypeName("const DXGI_SWAP_CHAIN_FULLSCREEN_DESC *")] DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pFullscreenDesc, IDXGIOutput* pRestrictToOutput, IDXGISwapChain1** ppSwapChain)
    {
        return ((delegate* unmanaged<IDXGIFactory4*, IUnknown*, IntPtr, DXGI_SWAP_CHAIN_DESC1*, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, IDXGIOutput*, IDXGISwapChain1**, int>)(lpVtbl[15]))((IDXGIFactory4*)Unsafe.AsPointer(ref this), pDevice, hWnd, pDesc, pFullscreenDesc, pRestrictToOutput, ppSwapChain);
    }

    /// <inheritdoc cref="IDXGIFactory2.CreateSwapChainForCoreWindow" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(16)]
    public HRESULT CreateSwapChainForCoreWindow(IUnknown* pDevice, IUnknown* pWindow, [NativeTypeName("const DXGI_SWAP_CHAIN_DESC1 *")] DXGI_SWAP_CHAIN_DESC1* pDesc, IDXGIOutput* pRestrictToOutput, IDXGISwapChain1** ppSwapChain)
    {
        return ((delegate* unmanaged<IDXGIFactory4*, IUnknown*, IUnknown*, DXGI_SWAP_CHAIN_DESC1*, IDXGIOutput*, IDXGISwapChain1**, int>)(lpVtbl[16]))((IDXGIFactory4*)Unsafe.AsPointer(ref this), pDevice, pWindow, pDesc, pRestrictToOutput, ppSwapChain);
    }

    /// <include file='IDXGIFactory4.xml' path='doc/member[@name="IDXGIFactory4.EnumAdapterByLuid"]/*' />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(26)]
    public HRESULT EnumAdapterByLuid(LUID AdapterLuid, [NativeTypeName("const IID &")] Guid* riid, void** ppvAdapter)
    {
        return ((delegate* unmanaged<IDXGIFactory4*, LUID, Guid*, void**, int>)(lpVtbl[26]))((IDXGIFactory4*)Unsafe.AsPointer(ref this), AdapterLuid, riid, ppvAdapter);
    }

    /// <include file='IDXGIFactory4.xml' path='doc/member[@name="IDXGIFactory4.EnumWarpAdapter"]/*' />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(27)]
    public HRESULT EnumWarpAdapter([NativeTypeName("const IID &")] Guid* riid, void** ppvAdapter)
    {
        return ((delegate* unmanaged<IDXGIFactory4*, Guid*, void**, int>)(lpVtbl[27]))((IDXGIFactory4*)Unsafe.AsPointer(ref this), riid, ppvAdapter);
    }
}
