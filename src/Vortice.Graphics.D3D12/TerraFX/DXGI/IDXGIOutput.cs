// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[Guid("AE02EEDB-C735-4690-8D52-5A8DC20213AA")]
[NativeTypeName("struct IDXGIOutput : IDXGIObject")]
[NativeInheritance("IDXGIObject")]
internal unsafe partial struct IDXGIOutput 
{
    public void** lpVtbl;

    /// <inheritdoc cref="IUnknown.QueryInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IDXGIOutput*, Guid*, void**, int>)(lpVtbl[0]))((IDXGIOutput*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    /// <inheritdoc cref="IUnknown.AddRef" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IDXGIOutput*, uint>)(lpVtbl[1]))((IDXGIOutput*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IUnknown.Release" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IDXGIOutput*, uint>)(lpVtbl[2]))((IDXGIOutput*)Unsafe.AsPointer(ref this));
    }

    /// <inheritdoc cref="IDXGIObject.SetPrivateData" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(3)]
    public HRESULT SetPrivateData([NativeTypeName("const GUID &")] Guid* Name, uint DataSize, [NativeTypeName("const void *")] void* pData)
    {
        return ((delegate* unmanaged<IDXGIOutput*, Guid*, uint, void*, int>)(lpVtbl[3]))((IDXGIOutput*)Unsafe.AsPointer(ref this), Name, DataSize, pData);
    }

    /// <inheritdoc cref="IDXGIObject.SetPrivateDataInterface" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(4)]
    public HRESULT SetPrivateDataInterface([NativeTypeName("const GUID &")] Guid* Name, [NativeTypeName("const IUnknown *")] IUnknown* pUnknown)
    {
        return ((delegate* unmanaged<IDXGIOutput*, Guid*, IUnknown*, int>)(lpVtbl[4]))((IDXGIOutput*)Unsafe.AsPointer(ref this), Name, pUnknown);
    }

    /// <inheritdoc cref="IDXGIObject.GetPrivateData" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(5)]
    public HRESULT GetPrivateData([NativeTypeName("const GUID &")] Guid* Name, uint* pDataSize, void* pData)
    {
        return ((delegate* unmanaged<IDXGIOutput*, Guid*, uint*, void*, int>)(lpVtbl[5]))((IDXGIOutput*)Unsafe.AsPointer(ref this), Name, pDataSize, pData);
    }

    /// <inheritdoc cref="IDXGIObject.GetParent" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(6)]
    public HRESULT GetParent([NativeTypeName("const IID &")] Guid* riid, void** ppParent)
    {
        return ((delegate* unmanaged<IDXGIOutput*, Guid*, void**, int>)(lpVtbl[6]))((IDXGIOutput*)Unsafe.AsPointer(ref this), riid, ppParent);
    }
}
