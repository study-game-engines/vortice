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
    public static VkFormat ToVulkanFormat(this TextureFormat format)
    {
        return format switch
        {
            // 8-bit formats
            TextureFormat.R8UNorm => VkFormat.R8UNorm,
            TextureFormat.R8SNorm => VkFormat.R8SNorm,
            TextureFormat.R8UInt => VkFormat.R8UInt,
            TextureFormat.R8SInt => VkFormat.R8SInt,
            // 16-bit formats
            TextureFormat.R16UNorm => VkFormat.R16UNorm,
            TextureFormat.R16SNorm => VkFormat.R16SNorm,
            TextureFormat.R16UInt => VkFormat.R16UInt,
            TextureFormat.R16SInt => VkFormat.R16SInt,
            TextureFormat.R16Float => VkFormat.R16SFloat,
            TextureFormat.RG8UNorm => VkFormat.R8G8UNorm,
            TextureFormat.RG8SNorm => VkFormat.R8G8SNorm,
            TextureFormat.RG8UInt => VkFormat.R8G8UInt,
            TextureFormat.RG8SInt => VkFormat.R8G8SInt,
            // 32-bit formats
            TextureFormat.R32UInt => VkFormat.R32UInt,
            TextureFormat.R32SInt => VkFormat.R32SInt,
            TextureFormat.R32Float => VkFormat.R32SFloat,
            TextureFormat.RG16UNorm => VkFormat.R16G16UNorm,
            TextureFormat.RG16SNorm => VkFormat.R16G16SNorm,
            TextureFormat.RG16UInt => VkFormat.R16G16UInt,
            TextureFormat.RG16SInt => VkFormat.R16G16SInt,
            TextureFormat.RG16Float => VkFormat.R16G16SFloat,
            TextureFormat.RGBA8UNorm => VkFormat.R8G8B8A8UNorm,
            TextureFormat.RGBA8UNormSrgb => VkFormat.R8G8B8A8SRgb,
            TextureFormat.RGBA8SNorm => VkFormat.R8G8B8A8SNorm,
            TextureFormat.RGBA8UInt => VkFormat.R8G8B8A8UInt,
            TextureFormat.RGBA8SInt => VkFormat.R8G8B8A8SInt,
            TextureFormat.BGRA8UNorm => VkFormat.B8G8R8A8UNorm,
            TextureFormat.BGRA8UNormSrgb => VkFormat.B8G8R8A8SRgb,
            // Packed 32-Bit formats
            TextureFormat.RGB10A2UNorm => VkFormat.A2B10G10R10UNormPack32,
            TextureFormat.RG11B10Float => VkFormat.B10G11R11UFloatPack32,
            TextureFormat.RGB9E5Float => VkFormat.E5B9G9R9UFloatPack32,
            // 64-Bit formats
            TextureFormat.RG32UInt => VkFormat.R32G32UInt,
            TextureFormat.RG32SInt => VkFormat.R32G32SInt,
            TextureFormat.RG32Float => VkFormat.R32G32SFloat,
            TextureFormat.RGBA16UNorm => VkFormat.R16G16B16A16UNorm,
            TextureFormat.RGBA16SNorm => VkFormat.R16G16B16A16SNorm,
            TextureFormat.RGBA16UInt => VkFormat.R16G16B16A16UInt,
            TextureFormat.RGBA16SInt => VkFormat.R16G16B16A16SInt,
            TextureFormat.RGBA16Float => VkFormat.R16G16B16A16SFloat,
            // 128-Bit formats
            TextureFormat.RGBA32UInt => VkFormat.R32G32B32A32UInt,
            TextureFormat.RGBA32SInt => VkFormat.R32G32B32A32SInt,
            TextureFormat.RGBA32Float => VkFormat.R32G32B32A32SFloat,
            // Depth-stencil formats
            TextureFormat.Depth16UNorm => VkFormat.D16UNorm,
            TextureFormat.Depth32Float => VkFormat.D32SFloat,
            TextureFormat.Depth24UNormStencil8 => VkFormat.D24UNormS8UInt,
            TextureFormat.Depth32FloatStencil8 => VkFormat.D32SFloatS8UInt,
            // Compressed BC formats
            TextureFormat.BC1RGBAUNorm => VkFormat.BC1RGBAUNormBlock,
            TextureFormat.BC1RGBAUNormSrgb => VkFormat.BC1RGBASRgbBlock,
            TextureFormat.BC2RGBAUNorm => VkFormat.BC2UNormBlock,
            TextureFormat.BC2RGBAUNormSrgb => VkFormat.BC2SRgbBlock,
            TextureFormat.BC3RGBAUNorm => VkFormat.BC3UNormBlock,
            TextureFormat.BC3RGBAUNormSrgb => VkFormat.BC3SRgbBlock,
            TextureFormat.BC4RSNorm => VkFormat.BC4SNormBlock,
            TextureFormat.BC4RUNorm => VkFormat.BC4UNormBlock,
            TextureFormat.BC5RGSNorm => VkFormat.BC5SNormBlock,
            TextureFormat.BC5RGUNorm => VkFormat.BC5UNormBlock,
            TextureFormat.BC6HRGBUFloat => VkFormat.BC6HUFloatBlock,
            TextureFormat.BC6HRGBFloat => VkFormat.BC6HSFloatBlock,
            TextureFormat.BC7RGBAUNorm => VkFormat.BC7UNormBlock,
            TextureFormat.BC7RGBAUNormSrgb => VkFormat.BC7SRgbBlock,
            _ => ThrowHelper.ThrowArgumentException<VkFormat>("Invalid texture format"),
        };
    }

    public static VkMemoryHeap GetMemoryHeap(this VkPhysicalDeviceMemoryProperties memoryProperties, uint index)
    {
        return (&memoryProperties.memoryHeaps_0)[index];
    }
}

public struct VkPhysicalDeviceExtensions
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
