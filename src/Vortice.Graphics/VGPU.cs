// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using System.Text;
using Vortice.Mathematics;

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

    public struct DeviceDesc
    {
        public IntPtr label;
        public GraphicsBackend preferredBackend;
        public ValidationMode validationMode;
    }

    public readonly struct AdapterProperties
    {
        public readonly uint vendorID;
        public readonly uint deviceID;
        private readonly IntPtr _name;
        private readonly IntPtr _driverDescription;
        public readonly GpuAdapterType adapterType;

        public readonly string name => FromUTF8(_name);
        public readonly string driverDescription => FromUTF8(_driverDescription);
    }

    public readonly struct SwapChainDesc
    {
        public readonly uint width { get; init; }
        public readonly uint height { get; init; }
        public readonly TextureFormat format { get; init; }
        public readonly PresentMode presentMode { get; init; }
        public readonly Bool32 isFullscreen { get; init; }
    }

    public readonly struct BufferDesc
    {
        public readonly IntPtr label { get; init; }
        public readonly ulong size { get; init; }
        public readonly BufferUsage usage { get; init; }
        public readonly CpuAccessMode cpuAccess { get; init; }
    }

    public readonly struct TextureDesc
    {
        public readonly IntPtr label { get; init; }
        public readonly TextureDimension type { get; init; }
        public readonly TextureFormat format { get; init; }
        public readonly TextureUsage usage { get; init; }
        public readonly uint width { get; init; }
        public readonly uint height { get; init; }
        public readonly uint depthOrArraySize { get; init; }
        public readonly uint mipLevelCount { get; init; }
        public readonly uint sampleCount { get; init; }
    }

    public readonly struct RenderPassColorAttachment
    {
        public readonly IntPtr texture { get; init; }
        public readonly uint level{ get; init; }
        public readonly uint slice { get; init; }
        public readonly LoadAction loadOp { get; init; }
        public readonly StoreAction storeOp { get; init; }
        public readonly Color4 clearColor { get; init; }
    }
    public struct RenderPassDepthStencilAttachment
    {
        public IntPtr texture;
        public uint level;
        public uint slice;
        public LoadAction depthLoadOp;
        public StoreAction depthStoreOp;
        public float clearDepth;
        public LoadAction stencilLoadOp;
        public StoreAction stencilStoreOp;
        public byte clearStencil;
    }

    public struct RenderPassDesc
    {
        public uint width;
        public uint height;
        public uint colorAttachmentCount;
        public RenderPassColorAttachment* colorAttachments;
        public RenderPassDepthStencilAttachment* depthStencilAttachment;
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
        vgpuSetLogCallback(callback);
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
    public delegate void VGPULogCallback(LogLevel level, sbyte* message);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuSetLogCallback(VGPULogCallback? callback);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Bool32 vgpuIsSupported(GraphicsBackend backend);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr vgpuCreateDevice(DeviceDesc* callback);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuDestroyDevice(IntPtr device);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong vgpuFrame(IntPtr device);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuWaitIdle(IntPtr device);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern GraphicsBackend vgpuGetBackendType(IntPtr device);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern Bool32 vgpuQueryFeature(IntPtr device, Feature feature, void* pInfo, uint infoSize);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuGetAdapterProperties(IntPtr device, out AdapterProperties properties);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuGetLimits(IntPtr device, out GraphicsDeviceLimits limits);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr vgpuBeginCommandBuffer(IntPtr device, string? label);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuPushDebugGroup(IntPtr commandBuffer, string groupLabel);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuPopDebugGroup(IntPtr commandBuffer);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuInsertDebugMarker(IntPtr commandBuffer, string debugLabel);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr vgpuAcquireSwapchainTexture(IntPtr commandBuffer, IntPtr swapChain, out int pWidth, out int pHeight);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuBeginRenderPass(IntPtr commandBuffer, RenderPassDesc* desc);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuEndRenderPass(IntPtr commandBuffer);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuSetViewports(IntPtr commandBuffer, int count, Viewport* viewports);
    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuSetScissorRects(IntPtr commandBuffer, int count, RectI* scissorRects);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuSubmit(IntPtr device, IntPtr* commandBuffers, uint count);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr vgpuCreateSwapChain(IntPtr device, IntPtr windowHandle, SwapChainDesc* desc);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuDestroySwapChain(IntPtr device, IntPtr swapChain);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern TextureFormat vgpuSwapChainGetFormat(IntPtr device, IntPtr swapChain);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr vgpuCreateBuffer(IntPtr device, BufferDesc* desc, void* pInitialData);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuDestroyBuffer(IntPtr device, IntPtr buffer);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr vgpuCreateTexture(IntPtr device, TextureDesc* desc/*, void* pInitialData*/);

    [DllImport(NativeLibName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void vgpuDestroyTexture(IntPtr device, IntPtr texture);
}
