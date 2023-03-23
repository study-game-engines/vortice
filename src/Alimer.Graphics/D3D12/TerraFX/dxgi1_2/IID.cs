// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi1_2.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TerraFX.Interop.Windows;

internal static partial class IID
{

    [NativeTypeName("const GUID")]
    public static ref readonly Guid IID_IDXGIFactory2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[] {
                0x1C, 0x3A, 0xC8, 0x50,
                0x72, 0xE0,
                0x48, 0x4C,
                0x87,
                0xB0,
                0x36,
                0x30,
                0xFA,
                0x36,
                0xA6,
                0xD0
            };

            Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    [NativeTypeName("const GUID")]
    public static ref readonly Guid IID_IDXGISwapChain1
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[] {
                0xF7, 0x45, 0x0A, 0x79,
                0x42, 0x0D,
                0x76, 0x48,
                0x98,
                0x3A,
                0x0A,
                0x55,
                0xCF,
                0xE6,
                0xF4,
                0xAA
            };

            Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }
}
