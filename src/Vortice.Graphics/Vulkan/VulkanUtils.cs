// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using Microsoft.Toolkit.Diagnostics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Graphics.Vulkan;

internal static unsafe class VulkanUtils
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    private static bool CheckIsSupported()
    {
        try
        {
            VkResult result = vkInitialize();
            if (result != VkResult.Success)
            {
                return false;
            }

            int count = 0;
            result = vkEnumerateInstanceExtensionProperties((byte*)null, &count, null);
            if (result != VkResult.Success || count == 0)
            {
                return false;
            }

            // TODO: Should we enumerate physical devices and try to create instance?

            return true;
        }
        catch
        {
            return false;
        }
    }


    public static bool IsSupported() => s_isSupported.Value;

    public static string[] EnumerateInstanceExtensions()
    {
        if (!IsSupported())
        {
            return Array.Empty<string>();
        }

        int count = 0;
        VkResult result = vkEnumerateInstanceExtensionProperties((byte*)null, &count, null);
        if (result != VkResult.Success)
        {
            return Array.Empty<string>();
        }

        if (count == 0)
        {
            return Array.Empty<string>();
        }

        VkExtensionProperties* properties = stackalloc VkExtensionProperties[count];
        vkEnumerateInstanceExtensionProperties((byte*)null, &count, properties).CheckResult();

        string[] resultExt = new string[count];
        for (int i = 0; i < count; i++)
        {
            resultExt[i] = properties[i].GetExtensionName();
        }

        return resultExt;
    }

    public static string[] EnumerateInstanceLayers()
    {
        if (!IsSupported())
        {
            return Array.Empty<string>();
        }

        int count = 0;
        VkResult result = vkEnumerateInstanceLayerProperties(&count, null);
        if (result != VkResult.Success)
        {
            return Array.Empty<string>();
        }

        if (count == 0)
        {
            return Array.Empty<string>();
        }

        VkLayerProperties* properties = stackalloc VkLayerProperties[count];
        vkEnumerateInstanceLayerProperties(&count, properties).CheckResult();

        string[] resultExt = new string[count];
        for (int i = 0; i < count; i++)
        {
            resultExt[i] = properties[i].GetLayerName();
        }

        return resultExt;
    }

    public static string[] GetOptimalValidationLayers(HashSet<string> supportedInstanceLayers)
    {
        // The preferred validation layer is "VK_LAYER_KHRONOS_validation"
        string[] requiredLayers = new[] { "VK_LAYER_KHRONOS_validation" };
        if (ValidateLayers(requiredLayers, supportedInstanceLayers))
        {
            return requiredLayers;
        }
        //Log.Warn("Couldn't enable validation layers (see log for error) - falling back");

        // Otherwise we fallback to using the LunarG meta layer
        requiredLayers = new[] { "VK_LAYER_LUNARG_standard_validation" };
        if (ValidateLayers(requiredLayers, supportedInstanceLayers))
        {
            return requiredLayers;
        }
        //Log.Warn("Couldn't enable validation layers (see log for error) - falling back");

        // Otherwise we attempt to enable the individual layers that compose the LunarG meta layer since it doesn't exist
        requiredLayers = new[] {
            "VK_LAYER_GOOGLE_threading",
            "VK_LAYER_LUNARG_parameter_validation",
            "VK_LAYER_LUNARG_object_tracker",
            "VK_LAYER_LUNARG_core_validation",
            "VK_LAYER_GOOGLE_unique_objects"
        };
        if (ValidateLayers(requiredLayers, supportedInstanceLayers))
        {
            return requiredLayers;
        }
        //Log.Warn("Couldn't enable validation layers (see log for error) - falling back");

        // Otherwise as a last resort we fallback to attempting to enable the LunarG core layer
        requiredLayers = new[] { "VK_LAYER_LUNARG_core_validation" };
        if (ValidateLayers(requiredLayers, supportedInstanceLayers))
        {
            return requiredLayers;
        }

        // Else return nothing
        return Array.Empty<string>();
    }

    private static bool ValidateLayers(string[] required, HashSet<string> supportedInstanceLayers)
    {
        for (int i = 0; i < required.Length; ++i)
        {
            bool found = false;
            foreach (string availableLayer in supportedInstanceLayers)
            {
                if (availableLayer == required[i])
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                //Log.Warn("Validation Layer '{}' not found", layer);
                return false;
            }
        }

        return true;
    }

    public static VkPhysicalDeviceExtensions QueryPhysicalDeviceExtensions(VkPhysicalDevice physicalDevice)
    {
        int count = 0;
        VkResult result = vkEnumerateDeviceExtensionProperties(physicalDevice, null, &count, null);
        if (result != VkResult.Success)
            return default;

        VkExtensionProperties* vk_extensions = stackalloc VkExtensionProperties[count];
        vkEnumerateDeviceExtensionProperties(physicalDevice, null, &count, vk_extensions);

        VkPhysicalDeviceExtensions extensions = default;

        for (int i = 0; i < count; ++i)
        {
            string extensionName = vk_extensions[i].GetExtensionName();

            if (extensionName == VK_KHR_SWAPCHAIN_EXTENSION_NAME)
            {
                extensions.swapchain = true;
            }
            else if (extensionName == VK_EXT_DEPTH_CLIP_ENABLE_EXTENSION_NAME)
            {
                extensions.depth_clip_enable = true;
            }
            else if (extensionName == VK_EXT_MEMORY_BUDGET_EXTENSION_NAME)
            {
                extensions.memory_budget = true;
            }
            else if (extensionName == VK_KHR_PERFORMANCE_QUERY_EXTENSION_NAME)
            {
                extensions.performance_query = true;
            }
            else if (extensionName == VK_KHR_DEFERRED_HOST_OPERATIONS_EXTENSION_NAME)
            {
                extensions.deferred_host_operations = true;
            }
            else if (extensionName == VK_KHR_CREATE_RENDERPASS_2_EXTENSION_NAME)
            {
                extensions.renderPass2 = true;
            }
            else if (extensionName == VK_KHR_ACCELERATION_STRUCTURE_EXTENSION_NAME)
            {
                extensions.accelerationStructure = true;
            }
            else if (extensionName == VK_KHR_RAY_TRACING_PIPELINE_EXTENSION_NAME)
            {
                extensions.raytracingPipeline = true;
            }
            else if (extensionName == VK_KHR_RAY_QUERY_EXTENSION_NAME)
            {
                extensions.rayQuery = true;
            }
            else if (extensionName == VK_KHR_FRAGMENT_SHADING_RATE_EXTENSION_NAME)
            {
                extensions.fragment_shading_rate = true;
            }
            else if (extensionName == VK_NV_MESH_SHADER_EXTENSION_NAME)
            {
                extensions.NV_mesh_shader = true;
            }
            else if (extensionName == VK_EXT_CONDITIONAL_RENDERING_EXTENSION_NAME)
            {
                extensions.EXT_conditional_rendering = true;
            }
            else if (extensionName == VK_EXT_VERTEX_ATTRIBUTE_DIVISOR_EXTENSION_NAME)
            {
                extensions.vertex_attribute_divisor = true;
            }
            else if (extensionName == VK_EXT_EXTENDED_DYNAMIC_STATE_EXTENSION_NAME)
            {
                extensions.extended_dynamic_state = true;
            }
            else if (extensionName == VK_EXT_VERTEX_INPUT_DYNAMIC_STATE_EXTENSION_NAME)
            {
                extensions.vertex_input_dynamic_state = true;
            }
            else if (extensionName == VK_EXT_EXTENDED_DYNAMIC_STATE_2_EXTENSION_NAME)
            {
                extensions.extended_dynamic_state2 = true;
            }
            else if (extensionName == VK_KHR_DYNAMIC_RENDERING_EXTENSION_NAME)
            {
                extensions.dynamic_rendering = true;
            }
            else if (extensionName == "VK_EXT_full_screen_exclusive")
            {
                extensions.win32_full_screen_exclusive = true;
            }
        }

        vkGetPhysicalDeviceProperties(physicalDevice, out VkPhysicalDeviceProperties properties);

        // Core 1.2
        if (properties.apiVersion >= VkVersion.Version_1_2)
        {
            extensions.renderPass2 = true;
        }

        // Core 1.3
        if (properties.apiVersion >= VkVersion.Version_1_3)
        {
        }

        return extensions;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VkFormat ToVulkanFormat(PixelFormat format)
    {
        return format switch
        {
            // 8-bit formats
            PixelFormat.R8UNorm => VkFormat.R8UNorm,
            PixelFormat.R8SNorm => VkFormat.R8SNorm,
            PixelFormat.R8UInt => VkFormat.R8UInt,
            PixelFormat.R8SInt => VkFormat.R8SInt,
            // 16-bit formats
            PixelFormat.R16UNorm => VkFormat.R16UNorm,
            PixelFormat.R16SNorm => VkFormat.R16SNorm,
            PixelFormat.R16UInt => VkFormat.R16UInt,
            PixelFormat.R16SInt => VkFormat.R16SInt,
            PixelFormat.R16Float => VkFormat.R16SFloat,
            PixelFormat.RG8UNorm => VkFormat.R8G8UNorm,
            PixelFormat.RG8SNorm => VkFormat.R8G8SNorm,
            PixelFormat.RG8UInt => VkFormat.R8G8UInt,
            PixelFormat.RG8SInt => VkFormat.R8G8SInt,
            // 32-bit formats
            PixelFormat.R32UInt => VkFormat.R32UInt,
            PixelFormat.R32SInt => VkFormat.R32SInt,
            PixelFormat.R32Float => VkFormat.R32SFloat,
            PixelFormat.RG16UNorm => VkFormat.R16G16UNorm,
            PixelFormat.RG16SNorm => VkFormat.R16G16SNorm,
            PixelFormat.RG16UInt => VkFormat.R16G16UInt,
            PixelFormat.RG16SInt => VkFormat.R16G16SInt,
            PixelFormat.RG16Float => VkFormat.R16G16SFloat,
            PixelFormat.RGBA8UNorm => VkFormat.R8G8B8A8UNorm,
            PixelFormat.RGBA8UNormSrgb => VkFormat.R8G8B8A8SRgb,
            PixelFormat.RGBA8SNorm => VkFormat.R8G8B8A8SNorm,
            PixelFormat.RGBA8UInt => VkFormat.R8G8B8A8UInt,
            PixelFormat.RGBA8SInt => VkFormat.R8G8B8A8SInt,
            PixelFormat.BGRA8UNorm => VkFormat.B8G8R8A8UNorm,
            PixelFormat.BGRA8UNormSrgb => VkFormat.B8G8R8A8SRgb,
            // Packed 32-Bit formats
            PixelFormat.RGB10A2UNorm => VkFormat.A2B10G10R10UNormPack32,
            PixelFormat.RG11B10Float => VkFormat.B10G11R11UFloatPack32,
            PixelFormat.RGB9E5Float => VkFormat.E5B9G9R9UFloatPack32,
            // 64-Bit formats
            PixelFormat.RG32UInt => VkFormat.R32G32UInt,
            PixelFormat.RG32SInt => VkFormat.R32G32SInt,
            PixelFormat.RG32Float => VkFormat.R32G32SFloat,
            PixelFormat.RGBA16UNorm => VkFormat.R16G16B16A16UNorm,
            PixelFormat.RGBA16SNorm => VkFormat.R16G16B16A16SNorm,
            PixelFormat.RGBA16UInt => VkFormat.R16G16B16A16UInt,
            PixelFormat.RGBA16SInt => VkFormat.R16G16B16A16SInt,
            PixelFormat.RGBA16Float => VkFormat.R16G16B16A16SFloat,
            // 128-Bit formats
            PixelFormat.RGBA32UInt => VkFormat.R32G32B32A32UInt,
            PixelFormat.RGBA32SInt => VkFormat.R32G32B32A32SInt,
            PixelFormat.RGBA32Float => VkFormat.R32G32B32A32SFloat,
            // Depth-stencil formats
            PixelFormat.Depth16UNorm => VkFormat.D16UNorm,
            PixelFormat.Depth32Float => VkFormat.D32SFloat,
            PixelFormat.Depth24UNormStencil8 => VkFormat.D24UNormS8UInt,
            PixelFormat.Depth32FloatStencil8 => VkFormat.D32SFloatS8UInt,
            // Compressed BC formats
            PixelFormat.BC1RGBAUNorm => VkFormat.BC1RGBAUNormBlock,
            PixelFormat.BC1RGBAUNormSrgb => VkFormat.BC1RGBASRgbBlock,
            PixelFormat.BC2RGBAUNorm => VkFormat.BC2UNormBlock,
            PixelFormat.BC2RGBAUNormSrgb => VkFormat.BC2SRgbBlock,
            PixelFormat.BC3RGBAUNorm => VkFormat.BC3UNormBlock,
            PixelFormat.BC3RGBAUNormSrgb => VkFormat.BC3SRgbBlock,
            PixelFormat.BC4RSNorm => VkFormat.BC4SNormBlock,
            PixelFormat.BC4RUNorm => VkFormat.BC4UNormBlock,
            PixelFormat.BC5RGSNorm => VkFormat.BC5SNormBlock,
            PixelFormat.BC5RGUNorm => VkFormat.BC5UNormBlock,
            PixelFormat.BC6HRGBUFloat => VkFormat.BC6HUFloatBlock,
            PixelFormat.BC6HRGBFloat => VkFormat.BC6HSFloatBlock,
            PixelFormat.BC7RGBAUNorm => VkFormat.BC7UNormBlock,
            PixelFormat.BC7RGBAUNormSrgb => VkFormat.BC7SRgbBlock,
            _ => ThrowHelper.ThrowArgumentException<VkFormat>("Invalid texture format"),
        };
    }
}

internal struct VkPhysicalDeviceExtensions
{
    public bool swapchain;
    public bool depth_clip_enable;
    public bool memory_budget;
    public bool performance_query;
    public bool deferred_host_operations;
    public bool renderPass2;
    public bool accelerationStructure;
    public bool raytracingPipeline;
    public bool rayQuery;
    public bool fragment_shading_rate;
    public bool NV_mesh_shader;
    public bool EXT_conditional_rendering;
    public bool win32_full_screen_exclusive;
    public bool vertex_attribute_divisor;
    public bool extended_dynamic_state;
    public bool vertex_input_dynamic_state;
    public bool extended_dynamic_state2;
    public bool dynamic_rendering;
};
