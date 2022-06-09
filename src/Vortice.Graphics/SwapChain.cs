// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static Vortice.Graphics.VGPU;

namespace Vortice.Graphics;

public unsafe sealed class SwapChain : GraphicsResource
{
    public SwapChain(GraphicsDevice device, SwapChainSurface surface, in SwapChainDescription description)
        : base(device, IntPtr.Zero, description.Label)
    {
        SwapChainDesc nativeDesc = description.ToVGPU();
#if WINDOWS_UWP
        IntPtr handle = Marshal.GetIUnknownForObject(((CoreWindowSwapChainSurface)surface).CoreWindow);
        Handle = vgpuCreateSwapChain(device.Handle, handle, &nativeDesc);
#else
        Handle = vgpuCreateSwapChain(device.Handle, ((Win32SwapChainSurface)surface).Hwnd, &nativeDesc);
#endif

        PresentMode = description.PresentMode;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="SwapChain" /> class.
    /// </summary>
    ~SwapChain() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Handle != IntPtr.Zero)
            {
                vgpuDestroySwapChain(Device.Handle, Handle);
            }
        }
    }

    public TextureFormat ColorFormat => vgpuSwapChainGetFormat(Device.Handle, Handle);
    public PresentMode PresentMode { get; }

    //public void Resize(int newWidth, int newHeight);
}
