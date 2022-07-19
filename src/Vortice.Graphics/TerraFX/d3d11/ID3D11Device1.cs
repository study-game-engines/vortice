// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11_1.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[Guid("A04BFB29-08EF-43D6-A49C-A9BDBDCBE686")]
[NativeTypeName("struct ID3D11Device1 : ID3D11Device")]
[NativeInheritance("ID3D11Device")]
internal unsafe partial struct ID3D11Device1
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
    [VtblIndex(3)]
    public HRESULT CreateBuffer([NativeTypeName("const D3D11_BUFFER_DESC *")] D3D11_BUFFER_DESC* pDesc, [NativeTypeName("const D3D11_SUBRESOURCE_DATA *")] D3D11_SUBRESOURCE_DATA* pInitialData, ID3D11Buffer** ppBuffer)
    {
        return ((delegate* unmanaged<ID3D11Device1*, D3D11_BUFFER_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Buffer**, int>)(lpVtbl[3]))((ID3D11Device1*)Unsafe.AsPointer(ref this), pDesc, pInitialData, ppBuffer);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[VtblIndex(4)]
    //public HRESULT CreateTexture1D([NativeTypeName("const D3D11_TEXTURE1D_DESC *")] D3D11_TEXTURE1D_DESC* pDesc, [NativeTypeName("const D3D11_SUBRESOURCE_DATA *")] D3D11_SUBRESOURCE_DATA* pInitialData, ID3D11Texture1D** ppTexture1D)
    //{
    //    return ((delegate* unmanaged<ID3D11Device1*, D3D11_TEXTURE1D_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Texture1D**, int>)(lpVtbl[4]))((ID3D11Device1*)Unsafe.AsPointer(ref this), pDesc, pInitialData, ppTexture1D);
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(5)]
    public HRESULT CreateTexture2D([NativeTypeName("const D3D11_TEXTURE2D_DESC *")] D3D11_TEXTURE2D_DESC* pDesc, [NativeTypeName("const D3D11_SUBRESOURCE_DATA *")] D3D11_SUBRESOURCE_DATA* pInitialData, ID3D11Texture2D** ppTexture2D)
    {
        return ((delegate* unmanaged<ID3D11Device1*, D3D11_TEXTURE2D_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Texture2D**, int>)(lpVtbl[5]))((ID3D11Device1*)Unsafe.AsPointer(ref this), pDesc, pInitialData, ppTexture2D);
    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //[VtblIndex(6)]
    //public HRESULT CreateTexture3D([NativeTypeName("const D3D11_TEXTURE3D_DESC *")] D3D11_TEXTURE3D_DESC* pDesc, [NativeTypeName("const D3D11_SUBRESOURCE_DATA *")] D3D11_SUBRESOURCE_DATA* pInitialData, ID3D11Texture3D** ppTexture3D)
    //{
    //    return ((delegate* unmanaged<ID3D11Device1*, D3D11_TEXTURE3D_DESC*, D3D11_SUBRESOURCE_DATA*, ID3D11Texture3D**, int>)(lpVtbl[6]))((ID3D11Device1*)Unsafe.AsPointer(ref this), pDesc, pInitialData, ppTexture3D);
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(33)]
    public HRESULT CheckFeatureSupport(D3D11_FEATURE Feature, void* pFeatureSupportData, uint FeatureSupportDataSize)
    {
        return ((delegate* unmanaged<ID3D11Device*, D3D11_FEATURE, void*, uint, int>)(lpVtbl[33]))((ID3D11Device*)Unsafe.AsPointer(ref this), Feature, pFeatureSupportData, FeatureSupportDataSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(44)]
    public HRESULT CreateDeferredContext1(uint ContextFlags, ID3D11DeviceContext1** ppDeferredContext)
    {
        return ((delegate* unmanaged<ID3D11Device1*, uint, ID3D11DeviceContext1**, int>)(lpVtbl[44]))((ID3D11Device1*)Unsafe.AsPointer(ref this), ContextFlags, ppDeferredContext);
    }
}
