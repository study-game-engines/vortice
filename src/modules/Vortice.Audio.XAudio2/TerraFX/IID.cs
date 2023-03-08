// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/xaudio2.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TerraFX.Interop.Windows;

internal static partial class IID
{
    public static ref readonly Guid IID_IXAudio2
    {
        get
        {
            ReadOnlySpan<byte> data = new byte[] {
                0xCF, 0xE3, 0x02, 0x2B,
                0x0B, 0x2E,
                0xC3, 0x4E,
                0xBE,
                0x45,
                0x1B,
                0x2A,
                0x3F,
                0xE7,
                0x21,
                0x0D
            };

            Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }
}
