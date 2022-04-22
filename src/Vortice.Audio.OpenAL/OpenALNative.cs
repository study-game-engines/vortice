// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Audio.OpenAL;

internal unsafe static class OpenALNative
{
    private static readonly IntPtr s_nativeLibrary;
    private static readonly delegate* unmanaged[Cdecl]<sbyte*, IntPtr> alcOpenDevice_ptr;

    public static IntPtr alcOpenDevice(sbyte* name) => alcOpenDevice_ptr(name);

    static OpenALNative()
    {
        //s_nativeLibrary = NativeLibrary.Load("OpenAL32.dll");
        //alcOpenDevice_ptr = (delegate* unmanaged[Cdecl]<sbyte*, IntPtr>)NativeLibrary.GetExport(s_nativeLibrary, nameof(alcOpenDevice));
    }
}
