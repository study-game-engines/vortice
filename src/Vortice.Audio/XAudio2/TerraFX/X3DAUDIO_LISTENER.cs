// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/x3daudio.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Numerics;
using System.Runtime.InteropServices;

namespace TerraFX.Interop.DirectX;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal unsafe partial struct X3DAUDIO_LISTENER
{
    [NativeTypeName("X3DAUDIO_VECTOR")]
    public Vector3 OrientFront;

    [NativeTypeName("X3DAUDIO_VECTOR")]
    public Vector3 OrientTop;

    [NativeTypeName("X3DAUDIO_VECTOR")]
    public Vector3 Position;

    [NativeTypeName("X3DAUDIO_VECTOR")]
    public Vector3 Velocity;

    public X3DAUDIO_CONE* pCone;
}
