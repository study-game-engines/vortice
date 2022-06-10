// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.IO;

namespace Vortice;

public static class LibraryLoader
{
    private static readonly ILibraryLoader s_platformLoader = GetPlatformDefaultLoader();

    static LibraryLoader()
    {
        if (PlatformInfo.IsWindows)
            Extension = ".dll";
        else if (PlatformInfo.IsMacOS)
            Extension = ".dylib";
        else
            Extension = ".so";
    }

    public static string Extension { get; }

    public static IEnumerable<string> RuntimeIdentifiers
    {
        get
        {
#if NET6_0_OR_GREATER
            yield return RuntimeInformation.RuntimeIdentifier;
#endif
            string archName = PlatformInfo.Is64Bit
                ? PlatformInfo.IsArm ? "arm64" : "x64"
                : PlatformInfo.IsArm ? "arm" : "x86";

            if (PlatformInfo.IsWindows)
            {
                yield return $"win-{archName}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                yield return $"linux-{archName}";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                yield return $"osx-{archName}";
            }

            yield break;
        }
    }

    public static IntPtr Load(string libraryName)
    {
        string libraryPath = GetLibraryPath(libraryName);

        IntPtr handle = LoadPlatformLibrary(libraryPath);
        if (handle == IntPtr.Zero)
            throw new DllNotFoundException($"Unable to load library '{libraryName}'.");

        return handle;

        static string GetLibraryPath(string libraryName)
        {
            List<string> runtimeIdentifiers = RuntimeIdentifiers.ToList();

            string libWithExt = libraryName;
            if (!libraryName.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
            {
                libWithExt += Extension;
            }

            // 1. try alongside managed assembly
            if (!string.IsNullOrEmpty(AppContext.BaseDirectory))
            {
                foreach (string rid in runtimeIdentifiers)
                {
                    if (CheckLibraryPath(AppContext.BaseDirectory, rid, libWithExt, out string? localLib))
                    {
                        return localLib!;
                    }
                }
            }

            // 2. try nuget native path
            foreach (string rid in runtimeIdentifiers)
            {
                string nugetNativeLibsPath = Path.Combine(AppContext.BaseDirectory, $@"runtimes\{rid}\native");
                bool isNuGetRuntimeLibrariesDirectoryPresent = Directory.Exists(nugetNativeLibsPath);

                if (isNuGetRuntimeLibrariesDirectoryPresent)
                {
                    string libraryPath = Path.Combine(AppContext.BaseDirectory, $@"runtimes\{rid}\native\{libWithExt}");
                    // Windows has multiple fallback win10 -> win
                    if (PlatformInfo.IsWindows)
                    {
                        if (LoadPlatformLibrary(libraryPath) != IntPtr.Zero)
                        {
                            return libraryPath;
                        }
                    }
                    else
                    {
                        return libraryPath;
                    }
                }
            }

            // 3. try current directory
            foreach (string rid in runtimeIdentifiers)
            {
                if (CheckLibraryPath(Directory.GetCurrentDirectory(), rid, libWithExt, out string? lib))
                {
                    return lib!;
                }
            }

            // 4. try app domain
            try
            {
                if (AppDomain.CurrentDomain is AppDomain domain)
                {
                    foreach (string rid in runtimeIdentifiers)
                    {
                        // 4.1 RelativeSearchPath
                        if (CheckLibraryPath(domain.RelativeSearchPath, rid, libWithExt, out string? lib))
                            return lib!;

                        // 4.2 BaseDirectory
                        if (CheckLibraryPath(domain.BaseDirectory, rid, libWithExt, out lib))
                            return lib!;
                    }
                }
            }
            catch
            {
                // no-op as there may not be any domain or path
            }

            // 4. use PATH or default loading mechanism
            return libWithExt;
        }

        static bool CheckLibraryPath(string? root, string arch, string libWithExt, out string? foundPath)
        {
            if (!string.IsNullOrEmpty(root))
            {
                // a. in generic platform sub dir
                string searchLib = Path.Combine(root, arch, libWithExt);
                if (File.Exists(searchLib))
                {
                    foundPath = searchLib;
                    return true;
                }

                // b. in root
                searchLib = Path.Combine(root, libWithExt);
                if (File.Exists(searchLib))
                {
                    foundPath = searchLib;
                    return true;
                }
            }

            // c. nothing
            foundPath = null;
            return false;
        }
    }

    private static IntPtr LoadPlatformLibrary(string libraryName)
    {
        if (string.IsNullOrEmpty(libraryName))
        {
            throw new ArgumentNullException(nameof(libraryName));
        }

        return s_platformLoader.Load(libraryName);
    }

    public static IntPtr GetExport(IntPtr handle, string symbolName)
    {
        return s_platformLoader.GetExport(handle, symbolName);
    }

    public static T GetSymbolDelegate<T>(IntPtr library, string name) where T : Delegate
    {
        var symbol = GetExport(library, name);
        if (symbol == IntPtr.Zero)
            throw new EntryPointNotFoundException($"Unable to load symbol '{name}'.");

        return Marshal.GetDelegateForFunctionPointer<T>(symbol);
    }

    private static ILibraryLoader GetPlatformDefaultLoader()
    {
#if NET6_0_OR_GREATER
        return new NetNativeLibraryLoader();
#else
        if (PlatformInfo.IsWindows)
        {
            return new Win32LibraryLoader();
        }

        if (PlatformInfo.IsLinux)
        {
            return new UnixLibraryLoader();
        }

        if (PlatformInfo.IsMacOS ||
            PlatformInfo.IsFreeBSD)
        {
            return new BsdLibraryLoader();
        }

        throw new PlatformNotSupportedException("This platform cannot load native libraries.");
#endif
    }



    public interface ILibraryLoader
    {
        IntPtr Load(string name);
        void Free(IntPtr handle);
        IntPtr GetExport(IntPtr handle, string symbolName);
    }

#if NET6_0_OR_GREATER
    private class NetNativeLibraryLoader : ILibraryLoader
    {
        public IntPtr Load(string name)
        {
            if (NativeLibrary.TryLoad(name, out IntPtr lib))
            {
                return lib;
            }

            return IntPtr.Zero;
        }

        public void Free(IntPtr handle)
        {
            NativeLibrary.Free(handle);
        }

        public IntPtr GetExport(IntPtr handle, string symbolName)
        {
            if (NativeLibrary.TryGetExport(handle, symbolName, out IntPtr ptr))
            {
                return ptr;
            }

            return IntPtr.Zero;
        }
    }
#else
    private class Win32LibraryLoader : ILibraryLoader
    {
        public IntPtr Load(string name)
        {
            return LoadLibrary(name);
        }

        public void Free(IntPtr handle)
        {
            FreeLibrary(handle);
        }

        public IntPtr GetExport(IntPtr handle, string functionName)
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

    private class UnixLibraryLoader : ILibraryLoader
    {
        private const string SystemLibrary = "libdl.so";
        private const string SystemLibrary2 = "libdl.so.2"; // newer Linux distros use this

        private const int RTLD_LAZY = 1;
        private const int RTLD_NOW = 2;

        private static bool UseSystemLibrary2 = true;

        public IntPtr Load(string name)
        {
            try
            {
                return dlopen2(name, RTLD_LAZY);
            }
            catch (DllNotFoundException)
            {
                UseSystemLibrary2 = false;
                return dlopen1(name, RTLD_LAZY);
            }
        }

        public void Free(IntPtr handle)
        {
            if (UseSystemLibrary2)
                dlclose2(handle);
            else
                dlclose1(handle);
        }

        public IntPtr GetExport(IntPtr handle, string symbolName)
        {
            return UseSystemLibrary2 ? dlsym2(handle, symbolName) : dlsym1(handle, symbolName);
        }


        [DllImport(SystemLibrary, EntryPoint = "dlopen")]
        private static extern IntPtr dlopen1(string path, int mode);

        [DllImport(SystemLibrary, EntryPoint = "dlsym")]
        private static extern IntPtr dlsym1(IntPtr handle, string symbol);

        [DllImport(SystemLibrary, EntryPoint = "dlclose")]
        private static extern void dlclose1(IntPtr handle);

        [DllImport(SystemLibrary2, EntryPoint = "dlopen")]
        private static extern IntPtr dlopen2(string path, int mode);

        [DllImport(SystemLibrary2, EntryPoint = "dlsym")]
        private static extern IntPtr dlsym2(IntPtr handle, string symbol);

        [DllImport(SystemLibrary2, EntryPoint = "dlclose")]
        private static extern void dlclose2(IntPtr handle);
    }

    private class BsdLibraryLoader : ILibraryLoader
    {
        private const int RtldNow = 0x002;

        public nint Load(string name)
        {
            return dlopen(name, RtldNow);
        }

        public void Free(nint handle)
        {
            dlclose(handle);
        }

        public nint GetExport(nint handle, string functionName)
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
