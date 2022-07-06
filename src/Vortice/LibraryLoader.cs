// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.IO;

namespace Vortice;

public static class LibraryLoader
{
    private static readonly ILibraryLoader s_platformLoader = GetPlatformDefaultLoader();

    static LibraryLoader()
    {
        if (OperatingSystem.IsWindows())
            Extension = ".dll";
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
            Extension = ".dylib";
        else
            Extension = ".so";
    }

    public static string Extension { get; }

    public static IEnumerable<string> RuntimeIdentifiers
    {
        get
        {
            yield return RuntimeInformation.RuntimeIdentifier;

            bool isArm = RuntimeInformation.ProcessArchitecture == Architecture.Arm || RuntimeInformation.ProcessArchitecture == Architecture.Arm64;

            string archName = Environment.Is64BitOperatingSystem
                ? isArm ? "arm64" : "x64"
                : isArm ? "arm" : "x86";

            if (OperatingSystem.IsWindows())
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
                    if (OperatingSystem.IsWindows())
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
        return new NetNativeLibraryLoader();
    }

    public interface ILibraryLoader
    {
        IntPtr Load(string name);
        void Free(IntPtr handle);
        IntPtr GetExport(IntPtr handle, string symbolName);
    }

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
}
