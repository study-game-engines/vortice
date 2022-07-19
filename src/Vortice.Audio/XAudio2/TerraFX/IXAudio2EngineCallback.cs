// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/xaudio2.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Runtime.CompilerServices;
using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

internal unsafe partial struct IXAudio2EngineCallback 
{
    public void** lpVtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public void OnProcessingPassStart()
    {
        ((delegate* unmanaged<IXAudio2EngineCallback*, void>)(lpVtbl[0]))((IXAudio2EngineCallback*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    public void OnProcessingPassEnd()
    {
        ((delegate* unmanaged<IXAudio2EngineCallback*, void>)(lpVtbl[1]))((IXAudio2EngineCallback*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    public void OnCriticalError(HRESULT Error)
    {
        ((delegate* unmanaged<IXAudio2EngineCallback*, HRESULT, void>)(lpVtbl[2]))((IXAudio2EngineCallback*)Unsafe.AsPointer(ref this), Error);
    }
}
