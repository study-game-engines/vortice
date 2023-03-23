// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from dxgicommon.h in microsoft/DirectX-Headers tag v1.606.4
// Original source is Copyright © Microsoft. Licensed under the MIT license

namespace TerraFX.Interop.DirectX;

internal static partial class DXGI
{
    [NativeTypeName("#define DXGI_STANDARD_MULTISAMPLE_QUALITY_PATTERN 0xffffffff")]
    public const uint DXGI_STANDARD_MULTISAMPLE_QUALITY_PATTERN = 0xffffffff;

    [NativeTypeName("#define DXGI_CENTER_MULTISAMPLE_QUALITY_PATTERN 0xfffffffe")]
    public const uint DXGI_CENTER_MULTISAMPLE_QUALITY_PATTERN = 0xfffffffe;
}
