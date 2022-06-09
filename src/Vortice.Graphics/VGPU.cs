// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using System.Text;

namespace Vortice.Graphics;

internal static unsafe class VGPU
{
    private const string NativeLibName = "vgpu";

    public enum LogLevel
    {
        Info = 0,
        Warn,
        Error,
    }

    private static VGPULogCallback? _logCallback;

#if NET6_0_OR_GREATER
    public static event DllImportResolver? ResolveLibrary;
    static VGPU()
    {
        ResolveLibrary += static (libraryName, assembly, searchPath) =>
        {
            if (libraryName is not "vgpu")
            {
                return IntPtr.Zero;
            }

            string rid = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "win-x64",
                Architecture.Arm64 => "win-arm64",
                _ => throw new NotSupportedException("Invalid process architecture")
            };

            // Test whether the native libraries are present in the same folder of the executable
            // (which is the case when the program was built with a runtime identifier), or whether
            // they are in the "runtimes\win-x64\native" folder in the executable directory.
            string nugetNativeLibsPath = Path.Combine(AppContext.BaseDirectory, $@"runtimes\{rid}\native");
            bool isNuGetRuntimeLibrariesDirectoryPresent = Directory.Exists(nugetNativeLibsPath);

            if (isNuGetRuntimeLibrariesDirectoryPresent)
            {
                string vpuPath = Path.Combine(AppContext.BaseDirectory, $@"runtimes\{rid}\native\vgpu.dll");

                // Load dependencies first so that vgpu doesn't fail to load it
                if (NativeLibrary.TryLoad(vpuPath, out IntPtr handle))
                {
                    return handle;
                }
            }
            else
            {
                if (NativeLibrary.TryLoad("vgpu", assembly, searchPath, out IntPtr handle))
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        };

        NativeLibrary.SetDllImportResolver(System.Reflection.Assembly.GetExecutingAssembly(), OnDllImport);
    }

    private static IntPtr OnDllImport(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (TryResolveLibrary(libraryName, assembly, searchPath, out IntPtr nativeLibrary))
        {
            return nativeLibrary;
        }

        return NativeLibrary.Load(libraryName, assembly, searchPath);
    }

    /// <summary>Tries to resolve a native library using the handlers for the <see cref="ResolveLibrary"/> event.</summary>
    /// <param name="libraryName">The native library to resolve.</param>
    /// <param name="assembly">The assembly requesting the resolution.</param>
    /// <param name="searchPath">The <see cref="DllImportSearchPath"/> value on the P/Invoke or assembly, or <see langword="null"/>.</param>
    /// <param name="nativeLibrary">The loaded library, if one was resolved.</param>
    /// <returns>Whether or not the requested library was successfully loaded.</returns>
    private static bool TryResolveLibrary(string libraryName, System.Reflection.Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        var resolveLibrary = ResolveLibrary;

        if (resolveLibrary != null)
        {
            var resolvers = resolveLibrary.GetInvocationList();

            foreach (DllImportResolver resolver in resolvers)
            {
                nativeLibrary = resolver(libraryName, assembly, searchPath);

                if (nativeLibrary != IntPtr.Zero)
                {
                    return true;
                }
            }
        }

        nativeLibrary = IntPtr.Zero;
        return false;
    }
#endif

    public static void SetLogCallback(VGPULogCallback? callback)
    {
        _logCallback = callback;
        VGPU_SetLogCallback(callback);
    }

    public static string FromUTF8(IntPtr str)
    {
        if (str == IntPtr.Zero)
        {
            return string.Empty;
        }

        byte* ptr = (byte*)str;
        while (*ptr != 0)
        {
            ptr += 1;
        }

        string result = Encoding.UTF8.GetString((byte*)str, (int)(ptr - (byte*)str));
        return result;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void VGPULogCallback(LogLevel level, IntPtr message);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void VGPU_SetLogCallback(VGPULogCallback? callback);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool vgpuIsSupported(GraphicsBackend backend);
}
