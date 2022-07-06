// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;

namespace Vortice;

internal static unsafe partial class NativeApi
{
    private static readonly IntPtr s_libraryHandle = LoadNativeLibrary();
    public static readonly delegate* unmanaged[Cdecl]<void*, uint, out KtxErrorCode, IntPtr> ktx_load_from_memory = (delegate* unmanaged[Cdecl]<void*, uint, out KtxErrorCode, IntPtr>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_load_from_memory));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint> ktx_get_baseWidth = (delegate* unmanaged[Cdecl]<IntPtr, uint>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_get_baseWidth));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint> ktx_get_baseHeight = (delegate* unmanaged[Cdecl]<IntPtr, uint>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_get_baseHeight));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint> ktx_get_numDimensions = (delegate* unmanaged[Cdecl]<IntPtr, uint>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_get_numDimensions));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint> ktx_get_numLevels = (delegate* unmanaged[Cdecl]<IntPtr, uint>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_get_numLevels));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint> ktx_get_numLayers = (delegate* unmanaged[Cdecl]<IntPtr, uint>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_get_numLayers));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, uint> ktx_get_numFaces = (delegate* unmanaged[Cdecl]<IntPtr, uint>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_get_numFaces));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, byte> ktx_get_isArray = (delegate* unmanaged[Cdecl]<IntPtr, byte>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_get_isArray));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, out byte*, out nuint, void> ktx_get_data = (delegate* unmanaged[Cdecl]<IntPtr, out byte*, out nuint, void>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_get_data));
    public static readonly delegate* unmanaged[Cdecl]<IntPtr, void> ktx_destroy_texture = (delegate* unmanaged[Cdecl]<IntPtr, void>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_destroy_texture));

    private static IntPtr LoadNativeLibrary()
    {
        if (OperatingSystem.IsWindows())
        {
            return LibraryLoader.Load("vortice_image.dll");
        }
        else if (OperatingSystem.IsLinux())
        {
            return LibraryLoader.Load("vortice_image.so");
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            return LibraryLoader.Load("vortice_image.dylib");
        }
        else
        {
            return LibraryLoader.Load("vortice_image");
        }
    }
}

/// <summary>
/// A dispatchable handle.
/// </summary>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
public readonly unsafe partial struct KtxTexture : IEquatable<KtxTexture>
{
    public KtxTexture(nint handle) { Handle = handle; }
    public nint Handle { get; }
    public bool IsNull => Handle == 0;
    public static KtxTexture Null => new KtxTexture(0);

    public uint BaseWidth => NativeApi.ktx_get_baseWidth(Handle);
    public uint BaseHeight => NativeApi.ktx_get_baseHeight(Handle);
    public uint NumDimensions => NativeApi.ktx_get_numDimensions(Handle);
    public uint NumLevels => NativeApi.ktx_get_numLevels(Handle);
    public uint NumLayers => NativeApi.ktx_get_numLayers(Handle);
    public uint NumFaces => NativeApi.ktx_get_numFaces(Handle);
    public bool IsArray => NativeApi.ktx_get_isArray(Handle) == 1;

    public static implicit operator KtxTexture(nint handle) => new KtxTexture(handle);
    public static bool operator ==(KtxTexture left, KtxTexture right) => left.Handle == right.Handle;
    public static bool operator !=(KtxTexture left, KtxTexture right) => left.Handle != right.Handle;
    public static bool operator ==(KtxTexture left, nint right) => left.Handle == right;
    public static bool operator !=(KtxTexture left, nint right) => left.Handle != right;
    public bool Equals(KtxTexture other) => Handle == other.Handle;
    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is KtxTexture handle && Equals(handle);
    /// <inheritdoc/>
    public override int GetHashCode() => Handle.GetHashCode();
    private string DebuggerDisplay => string.Format("KtxTexture [0x{0}]", Handle.ToString("X"));
}

internal enum KtxErrorCode
{
    /// <summary>
    /// Operation was successful.
    /// </summary>
    Success = 0,
    KTX_FILE_DATA_ERROR,     /*!< The data in the file is inconsistent with the spec. */
    KTX_FILE_ISPIPE,         /*!< The file is a pipe or named pipe. */
    KTX_FILE_OPEN_FAILED,    /*!< The target file could not be opened. */
    KTX_FILE_OVERFLOW,       /*!< The operation would exceed the max file size. */
    KTX_FILE_READ_ERROR,     /*!< An error occurred while reading from the file. */
    KTX_FILE_SEEK_ERROR,     /*!< An error occurred while seeking in the file. */
    KTX_FILE_UNEXPECTED_EOF, /*!< File does not have enough data to satisfy request. */
    KTX_FILE_WRITE_ERROR,    /*!< An error occurred while writing to the file. */
    KTX_GL_ERROR,            /*!< GL operations resulted in an error. */
    KTX_INVALID_OPERATION,   /*!< The operation is not allowed in the current state. */
    KTX_INVALID_VALUE,       /*!< A parameter value was not valid */
    KTX_NOT_FOUND,           /*!< Requested key was not found */
    KTX_OUT_OF_MEMORY,       /*!< Not enough memory to complete the operation. */
    KTX_TRANSCODE_FAILED,    /*!< Transcoding of block compressed texture failed. */
    KTX_UNKNOWN_FILE_FORMAT, /*!< The file not a KTX file */
    KTX_UNSUPPORTED_TEXTURE_TYPE, /*!< The KTX file specifies an unsupported texture type. */
    KTX_UNSUPPORTED_FEATURE,  /*!< Feature not included in in-use library or not yet implemented. */
    KTX_LIBRARY_NOT_LINKED,  /*!< Library dependency (OpenGL or Vulkan) not linked into application. */
    KTX_ERROR_MAX_ENUM = KTX_LIBRARY_NOT_LINKED /*!< For safety checks. */
}
