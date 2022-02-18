// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/winerror.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.Diagnostics;

namespace TerraFX.Interop.DirectX;

internal static partial class DXGI
{
    [NativeTypeName("const GUID")]
    public static ref readonly Guid DXGI_DEBUG_ALL
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[] {
                0x83, 0xE2, 0x8A, 0xE4,
                0x80, 0xDA,
                0x0B, 0x49,
                0x87,
                0xE6,
                0x43,
                0xE9,
                0xA9,
                0xCF,
                0xDA,
                0x08
            };

            Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    [NativeTypeName("const GUID")]
    public static ref readonly Guid DXGI_DEBUG_DX
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[] {
                0xFC, 0xD7, 0xCD, 0x35,
                0xB2, 0x13,
                0x1D, 0x42,
                0xA5,
                0xD7,
                0x7E,
                0x44,
                0x51,
                0x28,
                0x7D,
                0x64
            };

            Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    [NativeTypeName("const GUID")]
    public static ref readonly Guid DXGI_DEBUG_DXGI
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[] {
                0xA4, 0xDA, 0xCD, 0x25,
                0xC6, 0xB1,
                0xE1, 0x47,
                0xAC,
                0x3E,
                0x98,
                0x87,
                0x5B,
                0x5A,
                0x2E,
                0x2A
            };

            Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    [NativeTypeName("const GUID")]
    public static ref readonly Guid DXGI_DEBUG_APP
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> data = new byte[] {
                0x01, 0x6E, 0xCD, 0x06,
                0x19, 0x42,
                0xBD, 0x4E,
                0x87,
                0x09,
                0x27,
                0xED,
                0x23,
                0x36,
                0x0C,
                0x62
            };

            Debug.Assert(data.Length == Unsafe.SizeOf<Guid>());
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference(data));
        }
    }

    [NativeTypeName("#define DXGI_DEBUG_BINARY_VERSION ( 1 )")]
    public const int DXGI_DEBUG_BINARY_VERSION = (1);

    [NativeTypeName("#define DXGI_INFO_QUEUE_MESSAGE_ID_STRING_FROM_APPLICATION 0")]
    public const int DXGI_INFO_QUEUE_MESSAGE_ID_STRING_FROM_APPLICATION = 0;

    [NativeTypeName("#define DXGI_INFO_QUEUE_DEFAULT_MESSAGE_COUNT_LIMIT 1024")]
    public const int DXGI_INFO_QUEUE_DEFAULT_MESSAGE_COUNT_LIMIT = 1024;

    [NativeTypeName("#define DXGI_STATUS_OCCLUDED _HRESULT_TYPEDEF_(0x087A0001L)")]
    public const int DXGI_STATUS_OCCLUDED = ((int)(0x087A0001));

    [NativeTypeName("#define DXGI_STATUS_CLIPPED _HRESULT_TYPEDEF_(0x087A0002L)")]
    public const int DXGI_STATUS_CLIPPED = ((int)(0x087A0002));

    [NativeTypeName("#define DXGI_STATUS_NO_REDIRECTION _HRESULT_TYPEDEF_(0x087A0004L)")]
    public const int DXGI_STATUS_NO_REDIRECTION = ((int)(0x087A0004));

    [NativeTypeName("#define DXGI_STATUS_NO_DESKTOP_ACCESS _HRESULT_TYPEDEF_(0x087A0005L)")]
    public const int DXGI_STATUS_NO_DESKTOP_ACCESS = ((int)(0x087A0005));

    [NativeTypeName("#define DXGI_STATUS_GRAPHICS_VIDPN_SOURCE_IN_USE _HRESULT_TYPEDEF_(0x087A0006L)")]
    public const int DXGI_STATUS_GRAPHICS_VIDPN_SOURCE_IN_USE = ((int)(0x087A0006));

    [NativeTypeName("#define DXGI_STATUS_MODE_CHANGED _HRESULT_TYPEDEF_(0x087A0007L)")]
    public const int DXGI_STATUS_MODE_CHANGED = ((int)(0x087A0007));

    [NativeTypeName("#define DXGI_STATUS_MODE_CHANGE_IN_PROGRESS _HRESULT_TYPEDEF_(0x087A0008L)")]
    public const int DXGI_STATUS_MODE_CHANGE_IN_PROGRESS = ((int)(0x087A0008));

    [NativeTypeName("#define DXGI_ERROR_INVALID_CALL _HRESULT_TYPEDEF_(0x887A0001L)")]
    public const int DXGI_ERROR_INVALID_CALL = unchecked((int)(0x887A0001));

    [NativeTypeName("#define DXGI_ERROR_NOT_FOUND _HRESULT_TYPEDEF_(0x887A0002L)")]
    public const int DXGI_ERROR_NOT_FOUND = unchecked((int)(0x887A0002));

    [NativeTypeName("#define DXGI_ERROR_MORE_DATA _HRESULT_TYPEDEF_(0x887A0003L)")]
    public const int DXGI_ERROR_MORE_DATA = unchecked((int)(0x887A0003));

    [NativeTypeName("#define DXGI_ERROR_UNSUPPORTED _HRESULT_TYPEDEF_(0x887A0004L)")]
    public const int DXGI_ERROR_UNSUPPORTED = unchecked((int)(0x887A0004));

    [NativeTypeName("#define DXGI_ERROR_DEVICE_REMOVED _HRESULT_TYPEDEF_(0x887A0005L)")]
    public const int DXGI_ERROR_DEVICE_REMOVED = unchecked((int)(0x887A0005));

    [NativeTypeName("#define DXGI_ERROR_DEVICE_HUNG _HRESULT_TYPEDEF_(0x887A0006L)")]
    public const int DXGI_ERROR_DEVICE_HUNG = unchecked((int)(0x887A0006));

    [NativeTypeName("#define DXGI_ERROR_DEVICE_RESET _HRESULT_TYPEDEF_(0x887A0007L)")]
    public const int DXGI_ERROR_DEVICE_RESET = unchecked((int)(0x887A0007));

    [NativeTypeName("#define DXGI_ERROR_WAS_STILL_DRAWING _HRESULT_TYPEDEF_(0x887A000AL)")]
    public const int DXGI_ERROR_WAS_STILL_DRAWING = unchecked((int)(0x887A000A));

    [NativeTypeName("#define DXGI_ERROR_FRAME_STATISTICS_DISJOINT _HRESULT_TYPEDEF_(0x887A000BL)")]
    public const int DXGI_ERROR_FRAME_STATISTICS_DISJOINT = unchecked((int)(0x887A000B));

    [NativeTypeName("#define DXGI_ERROR_GRAPHICS_VIDPN_SOURCE_IN_USE _HRESULT_TYPEDEF_(0x887A000CL)")]
    public const int DXGI_ERROR_GRAPHICS_VIDPN_SOURCE_IN_USE = unchecked((int)(0x887A000C));

    [NativeTypeName("#define DXGI_ERROR_DRIVER_INTERNAL_ERROR _HRESULT_TYPEDEF_(0x887A0020L)")]
    public const int DXGI_ERROR_DRIVER_INTERNAL_ERROR = unchecked((int)(0x887A0020));

    [NativeTypeName("#define DXGI_ERROR_NONEXCLUSIVE _HRESULT_TYPEDEF_(0x887A0021L)")]
    public const int DXGI_ERROR_NONEXCLUSIVE = unchecked((int)(0x887A0021));

    [NativeTypeName("#define DXGI_ERROR_NOT_CURRENTLY_AVAILABLE _HRESULT_TYPEDEF_(0x887A0022L)")]
    public const int DXGI_ERROR_NOT_CURRENTLY_AVAILABLE = unchecked((int)(0x887A0022));

    [NativeTypeName("#define DXGI_ERROR_REMOTE_CLIENT_DISCONNECTED _HRESULT_TYPEDEF_(0x887A0023L)")]
    public const int DXGI_ERROR_REMOTE_CLIENT_DISCONNECTED = unchecked((int)(0x887A0023));

    [NativeTypeName("#define DXGI_ERROR_REMOTE_OUTOFMEMORY _HRESULT_TYPEDEF_(0x887A0024L)")]
    public const int DXGI_ERROR_REMOTE_OUTOFMEMORY = unchecked((int)(0x887A0024));

    [NativeTypeName("#define DXGI_ERROR_ACCESS_LOST _HRESULT_TYPEDEF_(0x887A0026L)")]
    public const int DXGI_ERROR_ACCESS_LOST = unchecked((int)(0x887A0026));

    [NativeTypeName("#define DXGI_ERROR_WAIT_TIMEOUT _HRESULT_TYPEDEF_(0x887A0027L)")]
    public const int DXGI_ERROR_WAIT_TIMEOUT = unchecked((int)(0x887A0027));

    [NativeTypeName("#define DXGI_ERROR_SESSION_DISCONNECTED _HRESULT_TYPEDEF_(0x887A0028L)")]
    public const int DXGI_ERROR_SESSION_DISCONNECTED = unchecked((int)(0x887A0028));

    [NativeTypeName("#define DXGI_ERROR_RESTRICT_TO_OUTPUT_STALE _HRESULT_TYPEDEF_(0x887A0029L)")]
    public const int DXGI_ERROR_RESTRICT_TO_OUTPUT_STALE = unchecked((int)(0x887A0029));

    [NativeTypeName("#define DXGI_ERROR_CANNOT_PROTECT_CONTENT _HRESULT_TYPEDEF_(0x887A002AL)")]
    public const int DXGI_ERROR_CANNOT_PROTECT_CONTENT = unchecked((int)(0x887A002A));

    [NativeTypeName("#define DXGI_ERROR_ACCESS_DENIED _HRESULT_TYPEDEF_(0x887A002BL)")]
    public const int DXGI_ERROR_ACCESS_DENIED = unchecked((int)(0x887A002B));

    [NativeTypeName("#define DXGI_ERROR_NAME_ALREADY_EXISTS _HRESULT_TYPEDEF_(0x887A002CL)")]
    public const int DXGI_ERROR_NAME_ALREADY_EXISTS = unchecked((int)(0x887A002C));

    [NativeTypeName("#define DXGI_ERROR_SDK_COMPONENT_MISSING _HRESULT_TYPEDEF_(0x887A002DL)")]
    public const int DXGI_ERROR_SDK_COMPONENT_MISSING = unchecked((int)(0x887A002D));

    [NativeTypeName("#define DXGI_ERROR_NOT_CURRENT _HRESULT_TYPEDEF_(0x887A002EL)")]
    public const int DXGI_ERROR_NOT_CURRENT = unchecked((int)(0x887A002E));

    [NativeTypeName("#define DXGI_ERROR_HW_PROTECTION_OUTOFMEMORY _HRESULT_TYPEDEF_(0x887A0030L)")]
    public const int DXGI_ERROR_HW_PROTECTION_OUTOFMEMORY = unchecked((int)(0x887A0030));

    [NativeTypeName("#define DXGI_ERROR_DYNAMIC_CODE_POLICY_VIOLATION _HRESULT_TYPEDEF_(0x887A0031L)")]
    public const int DXGI_ERROR_DYNAMIC_CODE_POLICY_VIOLATION = unchecked((int)(0x887A0031));

    [NativeTypeName("#define DXGI_ERROR_NON_COMPOSITED_UI _HRESULT_TYPEDEF_(0x887A0032L)")]
    public const int DXGI_ERROR_NON_COMPOSITED_UI = unchecked((int)(0x887A0032));

    [NativeTypeName("#define DXGI_STATUS_UNOCCLUDED _HRESULT_TYPEDEF_(0x087A0009L)")]
    public const int DXGI_STATUS_UNOCCLUDED = ((int)(0x087A0009));

    [NativeTypeName("#define DXGI_STATUS_DDA_WAS_STILL_DRAWING _HRESULT_TYPEDEF_(0x087A000AL)")]
    public const int DXGI_STATUS_DDA_WAS_STILL_DRAWING = ((int)(0x087A000A));

    [NativeTypeName("#define DXGI_ERROR_MODE_CHANGE_IN_PROGRESS _HRESULT_TYPEDEF_(0x887A0025L)")]
    public const int DXGI_ERROR_MODE_CHANGE_IN_PROGRESS = unchecked((int)(0x887A0025));

    [NativeTypeName("#define DXGI_STATUS_PRESENT_REQUIRED _HRESULT_TYPEDEF_(0x087A002FL)")]
    public const int DXGI_STATUS_PRESENT_REQUIRED = ((int)(0x087A002F));

    [NativeTypeName("#define DXGI_ERROR_CACHE_CORRUPT _HRESULT_TYPEDEF_(0x887A0033L)")]
    public const int DXGI_ERROR_CACHE_CORRUPT = unchecked((int)(0x887A0033));

    [NativeTypeName("#define DXGI_ERROR_CACHE_FULL _HRESULT_TYPEDEF_(0x887A0034L)")]
    public const int DXGI_ERROR_CACHE_FULL = unchecked((int)(0x887A0034));

    [NativeTypeName("#define DXGI_ERROR_CACHE_HASH_COLLISION _HRESULT_TYPEDEF_(0x887A0035L)")]
    public const int DXGI_ERROR_CACHE_HASH_COLLISION = unchecked((int)(0x887A0035));

    [NativeTypeName("#define DXGI_ERROR_ALREADY_EXISTS _HRESULT_TYPEDEF_(0x887A0036L)")]
    public const int DXGI_ERROR_ALREADY_EXISTS = unchecked((int)(0x887A0036));

    [NativeTypeName("#define DXGI_DDI_ERR_WASSTILLDRAWING _HRESULT_TYPEDEF_(0x887B0001L)")]
    public const int DXGI_DDI_ERR_WASSTILLDRAWING = unchecked((int)(0x887B0001));

    [NativeTypeName("#define DXGI_DDI_ERR_UNSUPPORTED _HRESULT_TYPEDEF_(0x887B0002L)")]
    public const int DXGI_DDI_ERR_UNSUPPORTED = unchecked((int)(0x887B0002));

    [NativeTypeName("#define DXGI_DDI_ERR_NONEXCLUSIVE _HRESULT_TYPEDEF_(0x887B0003L)")]
    public const int DXGI_DDI_ERR_NONEXCLUSIVE = unchecked((int)(0x887B0003));

    [NativeTypeName("#define DXGI_USAGE_SHADER_INPUT 0x00000010UL")]
    public const uint DXGI_USAGE_SHADER_INPUT = 0x00000010U;

    [NativeTypeName("#define DXGI_USAGE_RENDER_TARGET_OUTPUT 0x00000020UL")]
    public const uint DXGI_USAGE_RENDER_TARGET_OUTPUT = 0x00000020U;

    [NativeTypeName("#define DXGI_USAGE_BACK_BUFFER 0x00000040UL")]
    public const uint DXGI_USAGE_BACK_BUFFER = 0x00000040U;

    [NativeTypeName("#define DXGI_USAGE_SHARED 0x00000080UL")]
    public const uint DXGI_USAGE_SHARED = 0x00000080U;

    [NativeTypeName("#define DXGI_USAGE_READ_ONLY 0x00000100UL")]
    public const uint DXGI_USAGE_READ_ONLY = 0x00000100U;

    [NativeTypeName("#define DXGI_USAGE_DISCARD_ON_PRESENT 0x00000200UL")]
    public const uint DXGI_USAGE_DISCARD_ON_PRESENT = 0x00000200U;

    [NativeTypeName("#define DXGI_USAGE_UNORDERED_ACCESS 0x00000400UL")]
    public const uint DXGI_USAGE_UNORDERED_ACCESS = 0x00000400U;

    [NativeTypeName("#define DXGI_RESOURCE_PRIORITY_MINIMUM ( 0x28000000 )")]
    public const int DXGI_RESOURCE_PRIORITY_MINIMUM = (0x28000000);

    [NativeTypeName("#define DXGI_RESOURCE_PRIORITY_LOW ( 0x50000000 )")]
    public const int DXGI_RESOURCE_PRIORITY_LOW = (0x50000000);

    [NativeTypeName("#define DXGI_RESOURCE_PRIORITY_NORMAL ( 0x78000000 )")]
    public const int DXGI_RESOURCE_PRIORITY_NORMAL = (0x78000000);

    [NativeTypeName("#define DXGI_RESOURCE_PRIORITY_HIGH ( 0xa0000000 )")]
    public const uint DXGI_RESOURCE_PRIORITY_HIGH = (0xa0000000);

    [NativeTypeName("#define DXGI_RESOURCE_PRIORITY_MAXIMUM ( 0xc8000000 )")]
    public const uint DXGI_RESOURCE_PRIORITY_MAXIMUM = (0xc8000000);

    [NativeTypeName("#define DXGI_MAP_READ ( 1UL )")]
    public const uint DXGI_MAP_READ = (1U);

    [NativeTypeName("#define DXGI_MAP_WRITE ( 2UL )")]
    public const uint DXGI_MAP_WRITE = (2U);

    [NativeTypeName("#define DXGI_MAP_DISCARD ( 4UL )")]
    public const uint DXGI_MAP_DISCARD = (4U);

    [NativeTypeName("#define DXGI_ENUM_MODES_INTERLACED ( 1UL )")]
    public const uint DXGI_ENUM_MODES_INTERLACED = (1U);

    [NativeTypeName("#define DXGI_ENUM_MODES_SCALING ( 2UL )")]
    public const uint DXGI_ENUM_MODES_SCALING = (2U);

    [NativeTypeName("#define DXGI_MAX_SWAP_CHAIN_BUFFERS ( 16 )")]
    public const int DXGI_MAX_SWAP_CHAIN_BUFFERS = (16);

    [NativeTypeName("#define DXGI_PRESENT_TEST 0x00000001UL")]
    public const uint DXGI_PRESENT_TEST = 0x00000001U;

    [NativeTypeName("#define DXGI_PRESENT_DO_NOT_SEQUENCE 0x00000002UL")]
    public const uint DXGI_PRESENT_DO_NOT_SEQUENCE = 0x00000002U;

    [NativeTypeName("#define DXGI_PRESENT_RESTART 0x00000004UL")]
    public const uint DXGI_PRESENT_RESTART = 0x00000004U;

    [NativeTypeName("#define DXGI_PRESENT_DO_NOT_WAIT 0x00000008UL")]
    public const uint DXGI_PRESENT_DO_NOT_WAIT = 0x00000008U;

    [NativeTypeName("#define DXGI_PRESENT_STEREO_PREFER_RIGHT 0x00000010UL")]
    public const uint DXGI_PRESENT_STEREO_PREFER_RIGHT = 0x00000010U;

    [NativeTypeName("#define DXGI_PRESENT_STEREO_TEMPORARY_MONO 0x00000020UL")]
    public const uint DXGI_PRESENT_STEREO_TEMPORARY_MONO = 0x00000020U;

    [NativeTypeName("#define DXGI_PRESENT_RESTRICT_TO_OUTPUT 0x00000040UL")]
    public const uint DXGI_PRESENT_RESTRICT_TO_OUTPUT = 0x00000040U;

    [NativeTypeName("#define DXGI_PRESENT_USE_DURATION 0x00000100UL")]
    public const uint DXGI_PRESENT_USE_DURATION = 0x00000100U;

    [NativeTypeName("#define DXGI_PRESENT_ALLOW_TEARING 0x00000200UL")]
    public const uint DXGI_PRESENT_ALLOW_TEARING = 0x00000200U;

    [NativeTypeName("#define DXGI_MWA_NO_WINDOW_CHANGES ( 1 << 0 )")]
    public const int DXGI_MWA_NO_WINDOW_CHANGES = (1 << 0);

    [NativeTypeName("#define DXGI_MWA_NO_ALT_ENTER ( 1 << 1 )")]
    public const int DXGI_MWA_NO_ALT_ENTER = (1 << 1);

    [NativeTypeName("#define DXGI_MWA_NO_PRINT_SCREEN ( 1 << 2 )")]
    public const int DXGI_MWA_NO_PRINT_SCREEN = (1 << 2);

    [NativeTypeName("#define DXGI_MWA_VALID ( 0x7 )")]
    public const int DXGI_MWA_VALID = (0x7);
}
