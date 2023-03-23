// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/windef.h in the Windows SDK for Windows 10.0.22621.0
// Original source is Copyright © Microsoft. All rights reserved.

namespace TerraFX.Interop.Windows;

internal partial struct RECT
{
    [NativeTypeName("LONG")]
    public int left;

    [NativeTypeName("LONG")]
    public int top;

    [NativeTypeName("LONG")]
    public int right;

    [NativeTypeName("LONG")]
    public int bottom;
}
