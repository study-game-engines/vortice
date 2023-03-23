// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/dxgi1_5.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TerraFX.Interop.Windows;

internal static partial class IID
{
    [NativeTypeName("const GUID")]
    public static ref readonly Guid IID_IDXGIFactory5
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[] {
                0xF5, 0xE1, 0x32, 0x76,
                0x65, 0xEE,
                0xCA, 0x4D,
                0x87,
                0xFD,
                0x84,
                0xCD,
                0x75,
                0xF8,
                0x83,
                0x8D
            };

            Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }
}
