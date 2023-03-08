// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/winnt.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

namespace TerraFX.Interop.Windows;

internal readonly unsafe partial struct LUID
{
    [NativeTypeName("DWORD")]
    public readonly uint LowPart;

    [NativeTypeName("LONG")]
    public readonly int HighPart;

    public LUID(uint lowPart, int highPart)
    {
        LowPart = lowPart;
        HighPart = highPart;
    }

    [return: NativeTypeName("INT64")]
    public static long Int64FromLuid([NativeTypeName("const LUID &")] LUID* Luid)
    {
        LARGE_INTEGER val = new LARGE_INTEGER();
        val.Anonymous.LowPart = Luid->LowPart;
        val.Anonymous.HighPart = Luid->HighPart;
        return val.QuadPart;
    }

    public static LUID LuidFromInt64([NativeTypeName("INT64")] long Int64)
    {
        LARGE_INTEGER val = new();
        val.QuadPart = Int64;

        return new(val.Anonymous.LowPart, val.Anonymous.HighPart);
    }
}
