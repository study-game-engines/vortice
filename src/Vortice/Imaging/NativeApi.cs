// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice;

internal static unsafe partial class NativeApi
{
    private const string NativeLibName = "vortice_image";

    private static readonly IntPtr s_libraryHandle = LoadNativeLibrary();
    public static readonly delegate* unmanaged[Cdecl]<void*, uint, out KtxErrorCode, IntPtr> ktx_load_from_memory = (delegate* unmanaged[Cdecl]<void*, uint, out KtxErrorCode, IntPtr>)LibraryLoader.GetExport(s_libraryHandle, nameof(ktx_load_from_memory));

    private static IntPtr LoadNativeLibrary()
    {
        if (PlatformInfo.IsWindows)
        {
            return LibraryLoader.Load("vortice_image.dll");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return LibraryLoader.Load("vortice_image.so");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return LibraryLoader.Load("vortice_image.dylib");
        }
        else
        {
            return LibraryLoader.Load("libAlimer");
        }
    }
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
