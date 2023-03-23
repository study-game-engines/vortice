// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi1_5.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.IID;

#pragma warning disable CS0649

namespace TerraFX.Interop.DirectX;

/// <include file='IDXGIFactory5.xml' path='doc/member[@name="IDXGIFactory5"]/*' />
[Guid("7632E1F5-EE65-4DCA-87FD-84CD75F8838D")]
[NativeTypeName("struct IDXGIFactory5 : IDXGIFactory4")]
[NativeInheritance("IDXGIFactory4")]
internal unsafe partial struct IDXGIFactory5 : IDXGIFactory5.Interface, INativeGuid
{
    static Guid* INativeGuid.NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in IID_IDXGIFactory5));

    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, Guid*, void**, int>)(lpVtbl[0]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IDXGIFactory5*, uint>)(lpVtbl[1]))((IDXGIFactory5*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IDXGIFactory5*, uint>)(lpVtbl[2]))((IDXGIFactory5*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IDXGIObject.SetPrivateData" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(3)]
    public HRESULT SetPrivateData([NativeTypeName("const GUID &")] Guid* Name, uint DataSize, [NativeTypeName("const void *")] void* pData)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, Guid*, uint, void*, int>)(lpVtbl[3]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), Name, DataSize, pData);
    }

    /// <inheritdoc cref="IDXGIObject.SetPrivateDataInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(4)]
    public HRESULT SetPrivateDataInterface([NativeTypeName("const GUID &")] Guid* Name, [NativeTypeName("const IUnknown *")] IUnknown* pUnknown)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, Guid*, IUnknown*, int>)(lpVtbl[4]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), Name, pUnknown);
    }

    /// <inheritdoc cref="IDXGIObject.GetPrivateData" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(5)]
    public HRESULT GetPrivateData([NativeTypeName("const GUID &")] Guid* Name, uint* pDataSize, void* pData)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, Guid*, uint*, void*, int>)(lpVtbl[5]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), Name, pDataSize, pData);
    }

    /// <inheritdoc cref="IDXGIObject.GetParent" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(6)]
    public HRESULT GetParent([NativeTypeName("const IID &")] Guid* riid, void** ppParent)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, Guid*, void**, int>)(lpVtbl[6]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), riid, ppParent);
    }

    /// <inheritdoc cref="IDXGIFactory.MakeWindowAssociation" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(8)]
    public HRESULT MakeWindowAssociation(HWND WindowHandle, uint Flags)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, HWND, uint, int>)(lpVtbl[8]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), WindowHandle, Flags);
    }

    /// <inheritdoc cref="IDXGIFactory1.EnumAdapters1" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(12)]
    public HRESULT EnumAdapters1(uint Adapter, IDXGIAdapter1** ppAdapter)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, uint, IDXGIAdapter1**, int>)(lpVtbl[12]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), Adapter, ppAdapter);
    }

    /// <inheritdoc cref="IDXGIFactory1.IsCurrent" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(13)]
    public BOOL IsCurrent()
    {
        return ((delegate* unmanaged<IDXGIFactory5*, int>)(lpVtbl[13]))((IDXGIFactory5*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IDXGIFactory2.IsWindowedStereoEnabled" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(14)]
    public BOOL IsWindowedStereoEnabled()
    {
        return ((delegate* unmanaged<IDXGIFactory5*, int>)(lpVtbl[14]))((IDXGIFactory5*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IDXGIFactory2.CreateSwapChainForHwnd" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(15)]
    public HRESULT CreateSwapChainForHwnd(IUnknown* pDevice, HWND hWnd, [NativeTypeName("const DXGI_SWAP_CHAIN_DESC1 *")] DXGI_SWAP_CHAIN_DESC1* pDesc, [NativeTypeName("const DXGI_SWAP_CHAIN_FULLSCREEN_DESC *")] DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pFullscreenDesc, IDXGIOutput* pRestrictToOutput, IDXGISwapChain1** ppSwapChain)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, IUnknown*, HWND, DXGI_SWAP_CHAIN_DESC1*, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, IDXGIOutput*, IDXGISwapChain1**, int>)(lpVtbl[15]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), pDevice, hWnd, pDesc, pFullscreenDesc, pRestrictToOutput, ppSwapChain);
    }

    /// <inheritdoc cref="IDXGIFactory2.CreateSwapChainForCoreWindow" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(16)]
    public HRESULT CreateSwapChainForCoreWindow(IUnknown* pDevice, IUnknown* pWindow, [NativeTypeName("const DXGI_SWAP_CHAIN_DESC1 *")] DXGI_SWAP_CHAIN_DESC1* pDesc, IDXGIOutput* pRestrictToOutput, IDXGISwapChain1** ppSwapChain)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, IUnknown*, IUnknown*, DXGI_SWAP_CHAIN_DESC1*, IDXGIOutput*, IDXGISwapChain1**, int>)(lpVtbl[16]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), pDevice, pWindow, pDesc, pRestrictToOutput, ppSwapChain);
    }

    /// <inheritdoc cref="IDXGIFactory2.GetSharedResourceAdapterLuid" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(17)]
    public HRESULT GetSharedResourceAdapterLuid(HANDLE hResource, LUID* pLuid)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, HANDLE, LUID*, int>)(lpVtbl[17]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), hResource, pLuid);
    }

    /// <inheritdoc cref="IDXGIFactory2.RegisterStereoStatusWindow" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(18)]
    public HRESULT RegisterStereoStatusWindow(HWND WindowHandle, uint wMsg, [NativeTypeName("DWORD *")] uint* pdwCookie)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, HWND, uint, uint*, int>)(lpVtbl[18]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), WindowHandle, wMsg, pdwCookie);
    }

    /// <inheritdoc cref="IDXGIFactory2.RegisterStereoStatusEvent" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(19)]
    public HRESULT RegisterStereoStatusEvent(HANDLE hEvent, [NativeTypeName("DWORD *")] uint* pdwCookie)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, HANDLE, uint*, int>)(lpVtbl[19]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), hEvent, pdwCookie);
    }

    /// <inheritdoc cref="IDXGIFactory2.UnregisterStereoStatus" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(20)]
    public void UnregisterStereoStatus([NativeTypeName("DWORD")] uint dwCookie)
    {
        ((delegate* unmanaged<IDXGIFactory5*, uint, void>)(lpVtbl[20]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), dwCookie);
    }

    /// <inheritdoc cref="IDXGIFactory2.RegisterOcclusionStatusWindow" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(21)]
    public HRESULT RegisterOcclusionStatusWindow(HWND WindowHandle, uint wMsg, [NativeTypeName("DWORD *")] uint* pdwCookie)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, HWND, uint, uint*, int>)(lpVtbl[21]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), WindowHandle, wMsg, pdwCookie);
    }

    /// <inheritdoc cref="IDXGIFactory2.RegisterOcclusionStatusEvent" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(22)]
    public HRESULT RegisterOcclusionStatusEvent(HANDLE hEvent, [NativeTypeName("DWORD *")] uint* pdwCookie)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, HANDLE, uint*, int>)(lpVtbl[22]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), hEvent, pdwCookie);
    }

    /// <inheritdoc cref="IDXGIFactory2.UnregisterOcclusionStatus" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(23)]
    public void UnregisterOcclusionStatus([NativeTypeName("DWORD")] uint dwCookie)
    {
        ((delegate* unmanaged<IDXGIFactory5*, uint, void>)(lpVtbl[23]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), dwCookie);
    }

    /// <inheritdoc cref="IDXGIFactory2.CreateSwapChainForComposition" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(24)]
    public HRESULT CreateSwapChainForComposition(IUnknown* pDevice, [NativeTypeName("const DXGI_SWAP_CHAIN_DESC1 *")] DXGI_SWAP_CHAIN_DESC1* pDesc, IDXGIOutput* pRestrictToOutput, IDXGISwapChain1** ppSwapChain)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, IUnknown*, DXGI_SWAP_CHAIN_DESC1*, IDXGIOutput*, IDXGISwapChain1**, int>)(lpVtbl[24]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), pDevice, pDesc, pRestrictToOutput, ppSwapChain);
    }

    /// <inheritdoc cref="IDXGIFactory3.GetCreationFlags" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(25)]
    public uint GetCreationFlags()
    {
        return ((delegate* unmanaged<IDXGIFactory5*, uint>)(lpVtbl[25]))((IDXGIFactory5*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IDXGIFactory4.EnumAdapterByLuid" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(26)]
    public HRESULT EnumAdapterByLuid(LUID AdapterLuid, [NativeTypeName("const IID &")] Guid* riid, void** ppvAdapter)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, LUID, Guid*, void**, int>)(lpVtbl[26]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), AdapterLuid, riid, ppvAdapter);
    }

    /// <inheritdoc cref="IDXGIFactory4.EnumWarpAdapter" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(27)]
    public HRESULT EnumWarpAdapter([NativeTypeName("const IID &")] Guid* riid, void** ppvAdapter)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, Guid*, void**, int>)(lpVtbl[27]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), riid, ppvAdapter);
    }

    /// <include file='IDXGIFactory5.xml' path='doc/member[@name="IDXGIFactory5.CheckFeatureSupport"]/*' />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(28)]
    public HRESULT CheckFeatureSupport(DXGI_FEATURE Feature, void* pFeatureSupportData, uint FeatureSupportDataSize)
    {
        return ((delegate* unmanaged<IDXGIFactory5*, DXGI_FEATURE, void*, uint, int>)(lpVtbl[28]))((IDXGIFactory5*)Unsafe.AsPointer(ref this), Feature, pFeatureSupportData, FeatureSupportDataSize);
    }

    public interface Interface : IDXGIFactory4.Interface
    {
        [VtblIndex(28)]
        HRESULT CheckFeatureSupport(DXGI_FEATURE Feature, void* pFeatureSupportData, uint FeatureSupportDataSize);
    }
}
