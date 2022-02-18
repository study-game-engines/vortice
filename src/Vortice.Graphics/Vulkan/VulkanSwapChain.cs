// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Graphics;

internal unsafe class VulkanSwapChain : SwapChain
{
    private readonly VkSurfaceKHR _surface = VkSurfaceKHR.Null;

    public VulkanSwapChain(VulkanGraphicsDevice device, in SwapChainSource source, in SwapChainDescriptor descriptor)
        : base(device, descriptor)
    {
        _surface = CreateVkSurface(source);
        Resize(descriptor.Size.Width, descriptor.Size.Height);

        VkSurfaceKHR CreateVkSurface(in SwapChainSource source)
        {
            VkSurfaceKHR surface;

            switch (source.Type)
            {
                case SwapChainSourceType.Win32:
                    Win32SwapChainSource win32Source = (Win32SwapChainSource)source;
                    VkWin32SurfaceCreateInfoKHR surfaceCreateInfo = new VkWin32SurfaceCreateInfoKHR
                    {
                        sType = VkStructureType.Win32SurfaceCreateInfoKHR,
                        hinstance = GetModuleHandleW(lpModuleName: null),
                        hwnd = win32Source.Hwnd
                    };

                    vkCreateWin32SurfaceKHR(device.Instance, &surfaceCreateInfo, null, &surface).CheckResult();
                    break;

                default:
                    throw new GraphicsException("Surface not supported");

            }

            return surface;
        }
    }

    public VkSurfaceKHR Surface { get; }

    public VkSwapchainKHR Handle { get; private set; } = VkSwapchainKHR.Null;

    // <inheritdoc />
    public override Texture? CurrentBackBuffer => null;

    // <inheritdoc />
    public override int CurrentBackBufferIndex => 0;

    // <inheritdoc />
    public override int BackBufferCount => 1;

    public void Resize(int width, int height)
    {
        //VkDevice vkDevice = ((GraphicsDeviceVulkan)Device).NativeDevice;

        var createInfo = new VkSwapchainCreateInfoKHR
        {
            sType = VkStructureType.SwapchainCreateInfoKHR,
            surface = Surface,
            //minImageCount = imageCount,
            //imageFormat = surfaceFormat.format,
            //imageColorSpace = surfaceFormat.colorSpace,
            //imageExtent = Extent,
            //imageArrayLayers = 1,
            //imageUsage = VkImageUsageFlags.ColorAttachment,
            //imageSharingMode = VkSharingMode.Exclusive,
            //preTransform = swapChainSupport.Capabilities.currentTransform,
            //compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque,
            //presentMode = presentMode,
            clipped = true,
            oldSwapchain = Handle
        };

        //vkCreateSwapchainKHR(vkDevice, &createInfo, null, out VkSwapchainKHR swapChain).CheckResult();
        //Handle = swapChain;
    }

    /// <inheritdoc />
    public override void Present()
    {

    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    [DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
    private static extern unsafe IntPtr GetModuleHandleW(ushort* lpModuleName);
}
