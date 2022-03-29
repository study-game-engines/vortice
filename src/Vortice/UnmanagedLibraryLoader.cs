// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Implementation based on: https://github.com/mellinoe/nativelibraryloader
// Implementation based on Silk.NET.Core: https://github.com/dotnet/Silk.NET/blob/main/LICENSE.md

#if NET5_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

using System.Runtime.InteropServices;
using Microsoft.Toolkit.Diagnostics;

namespace Vortice;

/// <summary>
/// Exposes functionality for loading native libraries and function pointers.
/// </summary>
public abstract class UnmanagedLibraryLoader
{
    /// <summary>
    /// Loads a native library by name and returns an operating system handle to it.
    /// </summary>
    /// <param name="name">The name of the library to open.</param>
    /// <returns>The operating system handle for the shared library.</returns>
    public nint LoadNativeLibrary(string name)
    {
        return LoadNativeLibrary(name, UnmanagedLibraryPathResolver.Default);
    }

    /// <summary>
    /// Loads a native library by name and returns an operating system handle to it.
    /// </summary>
    /// <param name="name">The name of the library to open.</param>
    /// <param name="pathResolver">The path resolver to use.</param>
    /// <returns>The operating system handle for the shared library.</returns>
    public nint LoadNativeLibrary(string name, UnmanagedLibraryPathResolver pathResolver)
    {
        Guard.IsNotNullOrEmpty(name, nameof(name));

        nint result = LoadWithResolver(name, pathResolver);

        if (result == 0)
        {
            ThrowLibNotFound(name, pathResolver);
            return default;
        }

        return result;
    }

    /// <summary>
    /// Loads a native library by name and returns an operating system handle to it.
    /// </summary>
    /// <param name="names">An ordered list of names. Each name is tried in turn, until the library is successfully loaded.
    /// <returns>The operating system handle for the shared library.</returns>
    public nint LoadNativeLibrary(string[] names)
    {
        return LoadNativeLibrary(names, UnmanagedLibraryPathResolver.Default);
    }

    /// <summary>
    /// Loads a native library by name and returns an operating system handle to it.
    /// </summary>
    /// <param name="names">An ordered list of names. Each name is tried in turn, until the library is successfully loaded.
    /// <param name="pathResolver">The path resolver to use.</param>
    /// <returns>The operating system handle for the shared library.</returns>
    public nint LoadNativeLibrary(string[] names, UnmanagedLibraryPathResolver pathResolver)
    {
        Guard.IsNotNull(names, nameof(names));
        Guard.IsTrue(names.Length > 0, nameof(names));

        nint result = 0;
        foreach (string name in names)
        {
            result = LoadWithResolver(name, pathResolver);
            if (result != 0)
            {
                break;
            }
        }

        if (result == 0)
        {
            ThrowLibNotFoundAny(names, pathResolver);
            return default;
        }

        return result;
    }

    private nint LoadWithResolver(string name, UnmanagedLibraryPathResolver pathResolver)
    {
        if (name == "__Internal")
        {
            return CoreLoadNativeLibrary(null);
        }


        if (Path.IsPathRooted(name))
        {
            return CoreLoadNativeLibrary(name);
        }

        foreach (string loadTarget in pathResolver.EnumeratePossibleLibraryLoadTargets(name))
        {
            try
            {
                nint ret = CoreLoadNativeLibrary(loadTarget);
                if (ret != 0)
                {
                    return ret;
                }
            }
            catch (FileNotFoundException)
            {
                // do nothing
            }
        }

        return IntPtr.Zero;
    }

    private static void ThrowLibNotFound(string name, UnmanagedLibraryPathResolver resolver)
    {
        throw new FileNotFoundException(
            $"Could not find or load the native library: {name} Attempted: {string.Join(", ", resolver.EnumeratePossibleLibraryLoadTargets(name).Select(x => "\"" + x + "\""))}",
            name);
    }

    private static void ThrowLibNotFoundAny(string[] names, UnmanagedLibraryPathResolver pathResolver)
    {
        throw new FileNotFoundException
            ($"Could not find or load the native library from any name: [ {string.Join(", ", names.Select(x => x + " Attempted: (" + string.Join(", ", pathResolver.EnumeratePossibleLibraryLoadTargets(x).Select(x2 => "\"" + x2 + "\"")) + ")"))} ]",
            names[0]);
    }


    /// <summary>
    /// Returns a default library loader for the running operating system.
    /// </summary>
    /// <returns>A LibraryLoader suitable for loading libraries.</returns>
    public static UnmanagedLibraryLoader GetPlatformDefaultLoader()
    {
#if NET5_0_OR_GREATER
        return new NetNativeLibraryLoader();
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new Win32LibraryLoader();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new UnixLibraryLoader();
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ||
            RuntimeInformation.OSDescription.ToUpper().Contains("BSD"))
        {
            return new BsdLibraryLoader();
        }

        throw new PlatformNotSupportedException("This platform cannot load native libraries.");
#endif
    }

    /// <summary>
    /// Frees the library represented by the given operating system handle.
    /// </summary>
    /// <param name="handle">The handle of the open shared library.</param>
    public void FreeNativeLibrary(nint handle)
    {
        if (handle == 0)
        {
            throw new ArgumentException("Parameter must not be zero.", nameof(handle));
        }

        CoreFreeNativeLibrary(handle);
    }

    /// <summary>
    /// Loads a native library by name and returns an operating system handle to it.
    /// </summary>
    /// <param name="name">The name of the library to open. This parameter must not be null or empty.</param>
    /// <returns>The operating system handle for the shared library.
    /// If the library cannot be loaded, IntPtr.Zero should be returned.</returns>
    protected abstract nint CoreLoadNativeLibrary(string name);

    /// <summary>
    /// Frees the library represented by the given operating system handle.
    /// </summary>
    /// <param name="handle">The handle of the open shared library. This must not be zero.</param>
    protected abstract void CoreFreeNativeLibrary(nint handle);

    /// <summary>
    /// Loads a function pointer out of the given library by name.
    /// </summary>
    /// <param name="handle">The operating system handle of the opened shared library. This must not be zero.</param>
    /// <param name="functionName">The name of the exported function to load. This must not be null or empty.</param>
    /// <returns>A pointer to the loaded function.</returns>
    protected abstract nint CoreLoadFunctionPointer(nint handle, string functionName);

#if NET5_0_OR_GREATER
    private class NetNativeLibraryLoader : UnmanagedLibraryLoader
    {
        protected override nint CoreLoadNativeLibrary(string name)
        {
            if (NativeLibrary.TryLoad(name, out IntPtr lib))
            {
                return lib;
            }

            return 0;
        }

        protected override void CoreFreeNativeLibrary(nint handle)
        {
            NativeLibrary.Free(handle);
        }

        protected override nint CoreLoadFunctionPointer(nint handle, string functionName)
        {
            if (NativeLibrary.TryGetExport(handle, functionName, out var ptr))
            {
                return ptr;
            }

            return 0;
        }
    }
#else
    private class Win32LibraryLoader : UnmanagedLibraryLoader
    {
        protected override nint CoreLoadNativeLibrary(string name)
        {
            return LoadLibrary(name);
        }

        protected override void CoreFreeNativeLibrary(nint handle)
        {
            FreeLibrary(handle);
        }

        protected override nint CoreLoadFunctionPointer(nint handle, string functionName)
        {
            return GetProcAddress(handle, functionName);
        }

        [DllImport("kernel32")]
        private static extern nint LoadLibrary(string fileName);

        [DllImport("kernel32")]
        private static extern int FreeLibrary(nint module);

        [DllImport("kernel32")]
        private static extern nint GetProcAddress(nint module, string procName);
    }

    private class UnixLibraryLoader : UnmanagedLibraryLoader
    {
        private const int RtldNow = 0x002;

        protected override nint CoreLoadNativeLibrary(string name)
        {
            return dlopen(name, RtldNow);
        }

        protected override void CoreFreeNativeLibrary(nint handle)
        {
            dlclose(handle);
        }

        protected override nint CoreLoadFunctionPointer(nint handle, string functionName)
        {
            return dlsym(handle, functionName);
        }


        [DllImport("libdl")]
        private static extern nint dlopen(string fileName, int flags);

        [DllImport("libdl")]
        private static extern int dlclose(nint handle);

        [DllImport("libdl")]
        private static extern nint dlsym(nint handle, string name);

        [DllImport("libdl")]
        private static extern string dlerror();
    }

    private class BsdLibraryLoader : UnmanagedLibraryLoader
    {
        private const int RtldNow = 0x002;

        protected override nint CoreLoadNativeLibrary(string name)
        {
            return dlopen(name, RtldNow);
        }

        protected override void CoreFreeNativeLibrary(nint handle)
        {
            dlclose(handle);
        }

        protected override nint CoreLoadFunctionPointer(nint handle, string functionName)
        {
            return dlsym(handle, functionName);
        }

        [DllImport("libc")]
        private static extern nint dlopen(string fileName, int flags);

        [DllImport("libc")]
        private static extern nint dlsym(nint handle, string name);

        [DllImport("libc")]
        private static extern int dlclose(nint handle);

        [DllImport("libc")]
        private static extern string dlerror();
    }

#endif
}
