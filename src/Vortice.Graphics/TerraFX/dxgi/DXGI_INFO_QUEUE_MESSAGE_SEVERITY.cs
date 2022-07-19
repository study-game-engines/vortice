// Copyright � Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/dxgidebug.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright � Microsoft. All rights reserved.

using System.Runtime.Versioning;

namespace TerraFX.Interop.DirectX;

[SupportedOSPlatform("windows8.0")]
internal enum DXGI_INFO_QUEUE_MESSAGE_SEVERITY
{
    DXGI_INFO_QUEUE_MESSAGE_SEVERITY_CORRUPTION = 0,
    DXGI_INFO_QUEUE_MESSAGE_SEVERITY_ERROR = (DXGI_INFO_QUEUE_MESSAGE_SEVERITY_CORRUPTION + 1),
    DXGI_INFO_QUEUE_MESSAGE_SEVERITY_WARNING = (DXGI_INFO_QUEUE_MESSAGE_SEVERITY_ERROR + 1),
    DXGI_INFO_QUEUE_MESSAGE_SEVERITY_INFO = (DXGI_INFO_QUEUE_MESSAGE_SEVERITY_WARNING + 1),
    DXGI_INFO_QUEUE_MESSAGE_SEVERITY_MESSAGE = (DXGI_INFO_QUEUE_MESSAGE_SEVERITY_INFO + 1),
}
