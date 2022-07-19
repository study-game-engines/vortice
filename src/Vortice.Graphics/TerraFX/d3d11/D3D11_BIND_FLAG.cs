// Copyright � Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/d3d11.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright � Microsoft. All rights reserved.

namespace TerraFX.Interop.DirectX;

[Flags]
internal enum D3D11_BIND_FLAG
{
    D3D11_BIND_VERTEX_BUFFER = 0x1,
    D3D11_BIND_INDEX_BUFFER = 0x2,
    D3D11_BIND_CONSTANT_BUFFER = 0x4,
    D3D11_BIND_SHADER_RESOURCE = 0x8,
    D3D11_BIND_STREAM_OUTPUT = 0x10,
    D3D11_BIND_RENDER_TARGET = 0x20,
    D3D11_BIND_DEPTH_STENCIL = 0x40,
    D3D11_BIND_UNORDERED_ACCESS = 0x80,
    D3D11_BIND_DECODER = 0x200,
    D3D11_BIND_VIDEO_ENCODER = 0x400,
}
