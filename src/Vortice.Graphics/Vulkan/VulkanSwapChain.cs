// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;
using static Vortice.Mathematics.MathHelper;
using static Vortice.Vulkan.Vulkan;
using static Vortice.Graphics.Vulkan.VulkanUtils;

namespace Vortice.Graphics.Vulkan;

internal unsafe class VulkanSwapChain : SwapChain
{
    private VkSurfaceKHR _surface = VkSurfaceKHR.Null;

    public VulkanSwapChain(VulkanGraphicsDevice device, in GraphicsSurface surface, in SwapChainDescriptor descriptor)
        : base(device, surface, descriptor)
    {
        _surface = CreateVkSurface(surface);
        Resize(descriptor.Size.Width, descriptor.Size.Height);

        VkSurfaceKHR CreateVkSurface(in GraphicsSurface source)
        {
            VkSurfaceKHR surface = default;

            switch (source.Type)
            {
                case SwapChainSourceType.Win32:
                    Win32SwapChainSource win32Source = (Win32SwapChainSource)source;
                    VkWin32SurfaceCreateInfoKHR surfaceCreateInfo = new VkWin32SurfaceCreateInfoKHR
                    {
                        sType = VkStructureType.Win32SurfaceCreateInfoKHR,
                        hinstance = win32Source.Hwnd,
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

    public VkSwapchainKHR Handle { get; private set; } = VkSwapchainKHR.Null;

    // <inheritdoc />
    public override Texture? CurrentBackBuffer => null;

    // <inheritdoc />
    public override int CurrentBackBufferIndex => 0;

    // <inheritdoc />
    public override int BackBufferCount => 1;

    // <inheritdoc />
    public override void Resize(int newWidth, int newHeight)
    {
        VkPhysicalDevice physicalDevice = ((VulkanGraphicsDevice)Device).PhysicalDevice;
        VkDevice vkDevice = ((VulkanGraphicsDevice)Device).NativeDevice;

        vkGetPhysicalDeviceSurfaceCapabilitiesKHR(physicalDevice,
            _surface,
            out VkSurfaceCapabilitiesKHR surfaceCapabilities).CheckResult();

        int formatCount;
        vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, _surface, &formatCount, null).CheckResult();

        VkSurfaceFormatKHR* swapchainFormats = stackalloc VkSurfaceFormatKHR[formatCount];
        vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, _surface, &formatCount, swapchainFormats).CheckResult();

        // Determine the number of images
        uint imageCount = surfaceCapabilities.minImageCount + 1;
        if ((surfaceCapabilities.maxImageCount > 0) && (imageCount > surfaceCapabilities.maxImageCount))
        {
            imageCount = surfaceCapabilities.maxImageCount;
        }

        VkSurfaceFormatKHR surfaceFormat = default;
        surfaceFormat.format = ToVulkanFormat(ColorFormat);
        surfaceFormat.colorSpace = VkColorSpaceKHR.SrgbNonLinear;
        bool valid = false;
        bool allowHDR = true;

        for (int i = 0; i < formatCount; ++i)
        {
            if (!allowHDR && swapchainFormats[i].colorSpace != VkColorSpaceKHR.SrgbNonLinear)
                continue;

            if (swapchainFormats[i].format == surfaceFormat.format)
            {
                surfaceFormat = swapchainFormats[i];
                valid = true;
                break;
            }
        }
        if (!valid)
        {
            surfaceFormat.format = VkFormat.B8G8R8A8UNorm;
            surfaceFormat.colorSpace = VkColorSpaceKHR.SrgbNonLinear;
        }

        uint width = (uint)Size.Width;
        uint height = (uint)Size.Height;
        if (surfaceCapabilities.currentExtent.width != 0xFFFFFFFF
            && surfaceCapabilities.currentExtent.width != 0xFFFFFFFF)
        {
            width = surfaceCapabilities.currentExtent.width;
            height = surfaceCapabilities.currentExtent.height;
        }
        else
        {
            width = Max(surfaceCapabilities.minImageExtent.width, Min(surfaceCapabilities.maxImageExtent.width, width));
            height = Max(surfaceCapabilities.minImageExtent.height, Min(surfaceCapabilities.maxImageExtent.height, height));
        }

        VkPresentModeKHR presentMode = VkPresentModeKHR.Fifo;

        var createInfo = new VkSwapchainCreateInfoKHR
        {
            sType = VkStructureType.SwapchainCreateInfoKHR,
            surface = _surface,
            minImageCount = imageCount,
            imageFormat = surfaceFormat.format,
            imageColorSpace = surfaceFormat.colorSpace,
            imageExtent = new VkExtent2D(width, height),
            imageArrayLayers = 1,
            imageUsage = VkImageUsageFlags.ColorAttachment | VkImageUsageFlags.TransferSrc,
            imageSharingMode = VkSharingMode.Exclusive,
            preTransform = surfaceCapabilities.currentTransform,
            compositeAlpha = VkCompositeAlphaFlagsKHR.Opaque,
            presentMode = presentMode,
            clipped = true,
            oldSwapchain = Handle
        };

        vkCreateSwapchainKHR(vkDevice, &createInfo, null, out VkSwapchainKHR swapChain).CheckResult();
        Handle = swapChain;

        Size = new((int)width, (int)height);
    }

    /// <inheritdoc />
    public override void Present()
    {

    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        if (!Handle.IsNull)
        {
            vkDestroySwapchainKHR(((VulkanGraphicsDevice)Device).NativeDevice, Handle);
            Handle = VkSwapchainKHR.Null;
        }

        if (!_surface.IsNull)
        {
            vkDestroySurfaceKHR(((VulkanGraphicsDevice)Device).Instance, _surface);
            _surface = VkSurfaceKHR.Null;
        }
    }
}
