// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using CommunityToolkit.Diagnostics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Alimer.Graphics.Vulkan;

internal unsafe class VulkanGraphicsDevice : GraphicsDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    private readonly bool _debugUtils;
    private readonly bool _hasPortability;
    private readonly VkInstance _instance;
    private readonly VkDebugUtilsMessengerEXT _debugMessenger = VkDebugUtilsMessengerEXT.Null;
    private readonly VkDevice _handle = VkDevice.Null;
    private readonly VkPipelineCache _pipelineCache = VkPipelineCache.Null;
    private bool _shuttingDown;

    private readonly GraphicsAdapterInfo _adapterInfo;
    //private readonly GraphicsDeviceFeatures _features;
    private readonly GraphicsDeviceLimits _limits;

    public override string ApiName => "Vulkan";

    public override Version ApiVersion { get; }

    public bool DebugUtils => _debugUtils;
    public VkInstance Instance => _instance;
    public VkPhysicalDevice PhysicalDevice { get; }
    public VkDevice Handle => _handle;
    public VkPipelineCache PipelineCache => _pipelineCache;

    /// <inheritdoc />
    public override GraphicsAdapterInfo AdapterInfo => _adapterInfo;

    /// <inheritdoc />
    //public override GraphicsDeviceFeatures Features => _features;

    /// <inheritdoc />
    public override GraphicsDeviceLimits Limits => _limits;

    public static bool IsSupported() => s_isSupported.Value;

    public VulkanGraphicsDevice(in GraphicsDeviceDescription description)
        : base(GraphicsBackend.Vulkan)
    {
        Guard.IsTrue(IsSupported(), nameof(VulkanGraphicsDevice), "Vulkan is not supported");

        ApiVersion = new Version(1, 3, 0);
        VkResult result = VkResult.Success;

        // Create instance first.
        {
            int instanceLayerCount = 0;
            vkEnumerateInstanceLayerProperties(&instanceLayerCount, null).CheckResult();
            VkLayerProperties* availableInstanceLayers = stackalloc VkLayerProperties[instanceLayerCount];
            vkEnumerateInstanceLayerProperties(&instanceLayerCount, availableInstanceLayers).CheckResult();

            int extensionCount = 0;
            vkEnumerateInstanceExtensionProperties(null, &extensionCount, null).CheckResult();
            VkExtensionProperties* availableInstanceExtensions = stackalloc VkExtensionProperties[extensionCount];
            vkEnumerateInstanceExtensionProperties(null, &extensionCount, availableInstanceExtensions).CheckResult();

            List<string> instanceExtensions = new();
            List<string> instanceLayers = new();

            for (int i = 0; i < extensionCount; i++)
            {
                string extensionName = availableInstanceExtensions[i].GetExtensionName();
                if (extensionName == VK_EXT_DEBUG_UTILS_EXTENSION_NAME)
                {
                    _debugUtils = true;
                    instanceExtensions.Add(VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
                }
                else if (extensionName == VK_KHR_GET_PHYSICAL_DEVICE_PROPERTIES_2_EXTENSION_NAME)
                {
                    instanceExtensions.Add(VK_KHR_GET_PHYSICAL_DEVICE_PROPERTIES_2_EXTENSION_NAME);
                }
                else if (extensionName == VK_KHR_PORTABILITY_ENUMERATION_EXTENSION_NAME)
                {
                    _hasPortability = true;
                    instanceExtensions.Add(VK_KHR_PORTABILITY_ENUMERATION_EXTENSION_NAME);
                }
                else if (extensionName == VK_EXT_SWAPCHAIN_COLOR_SPACE_EXTENSION_NAME)
                {
                    instanceExtensions.Add(VK_EXT_SWAPCHAIN_COLOR_SPACE_EXTENSION_NAME);
                }
            }

            instanceExtensions.Add(VK_KHR_SURFACE_EXTENSION_NAME);

            // Enable surface extensions depending on os
            if (OperatingSystem.IsAndroid())
            {
                instanceExtensions.Add(VK_KHR_ANDROID_SURFACE_EXTENSION_NAME);
            }
            else if (OperatingSystem.IsWindows())
            {
                instanceExtensions.Add(VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
            }
            else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
            {
                instanceExtensions.Add(VK_EXT_METAL_SURFACE_EXTENSION_NAME);
            }
            else
            {
                instanceExtensions.Add(VK_KHR_XCB_SURFACE_EXTENSION_NAME);
            }

            if (description.ValidationMode != ValidationMode.Disabled)
            {
                // Determine the optimal validation layers to enable that are necessary for useful debugging
                GetOptimalValidationLayers(availableInstanceLayers, instanceLayerCount, instanceLayers);
            }

            var debugUtilsCreateInfo = new VkDebugUtilsMessengerCreateInfoEXT
            {
                sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT
            };

            fixed (sbyte* pApplicationName = description.Label.GetUtf8Span())
            {
                fixed (sbyte* pEngineName = "Alimer".GetUtf8Span())
                {
                    var appInfo = new VkApplicationInfo
                    {
                        pApplicationName = pApplicationName,
                        applicationVersion = new VkVersion(1, 0, 0),
                        pEngineName = pEngineName,
                        engineVersion = new VkVersion(1, 0, 0),
                        apiVersion = VkVersion.Version_1_3
                    };

                    using VkStringArray vkLayerNames = new(instanceLayers);
                    using VkStringArray vkExtensionNames = new(instanceExtensions);

                    var instanceCreateInfo = new VkInstanceCreateInfo
                    {
                        sType = VkStructureType.InstanceCreateInfo,
                        pApplicationInfo = &appInfo,
                        enabledLayerCount = vkLayerNames.Length,
                        ppEnabledLayerNames = vkLayerNames,
                        enabledExtensionCount = vkExtensionNames.Length,
                        ppEnabledExtensionNames = vkExtensionNames
                    };

                    if (_debugUtils)
                    {
                        debugUtilsCreateInfo.messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Error | VkDebugUtilsMessageSeverityFlagsEXT.Warning;
                        debugUtilsCreateInfo.messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance;
#if NET6_0_OR_GREATER
                        debugUtilsCreateInfo.pfnUserCallback = &DebugMessengerCallback;
#else
                        debugUtilsCreateInfo.pfnUserCallback = Marshal.GetFunctionPointerForDelegate(DebugMessagerCallbackDelegate);
#endif
                        instanceCreateInfo.pNext = &debugUtilsCreateInfo;
                    }

                    if (_hasPortability)
                    {
                        instanceCreateInfo.flags |= VkInstanceCreateFlags.EnumeratePortabilityKHR;
                    }

                    result = vkCreateInstance(&instanceCreateInfo, null, out _instance);
                    if (result != VkResult.Success)
                    {
                        throw new InvalidOperationException($"Failed to create vulkan instance: {result}");
                    }

                    vkLoadInstanceOnly(_instance);

                    if (_debugUtils)
                    {
                        vkCreateDebugUtilsMessengerEXT(_instance, &debugUtilsCreateInfo, null, out _debugMessenger).CheckResult();
                    }
                }
            }
        }

        _adapterInfo = new()
        {
        };

        static void GetOptimalValidationLayers(VkLayerProperties* availableLayers, int count, List<string> instanceLayers)
        {
            // The preferred validation layer is "VK_LAYER_KHRONOS_validation"
            List<string> validationLayers = new()
            {
                "VK_LAYER_KHRONOS_validation"
            };

            if (ValidateLayers(validationLayers, availableLayers, count))
            {
                instanceLayers.AddRange(validationLayers);
                return;
            }

            // Otherwise we fallback to using the LunarG meta layer
            validationLayers = new()
            {
                "VK_LAYER_LUNARG_standard_validation"
            };
            if (ValidateLayers(validationLayers, availableLayers, count))
            {
                instanceLayers.AddRange(validationLayers);
                return;
            }

            // Otherwise we attempt to enable the individual layers that compose the LunarG meta layer since it doesn't exist
            validationLayers = new()
            {
                "VK_LAYER_GOOGLE_threading",
                "VK_LAYER_LUNARG_parameter_validation",
                "VK_LAYER_LUNARG_object_tracker",
                "VK_LAYER_LUNARG_core_validation",
                "VK_LAYER_GOOGLE_unique_objects",
            };

            if (ValidateLayers(validationLayers, availableLayers, count))
            {
                instanceLayers.AddRange(validationLayers);
                return;
            }

            // Otherwise as a last resort we fallback to attempting to enable the LunarG core layer
            validationLayers = new()
            {
                "VK_LAYER_LUNARG_core_validation"
            };

            if (ValidateLayers(validationLayers, availableLayers, count))
            {
                instanceLayers.AddRange(validationLayers);
                return;
            }
        }

        static bool ValidateLayers(List<string> required, VkLayerProperties* availableLayers, int count)
        {
            foreach (string layer in required)
            {
                bool found = false;
                for (int i = 0; i < count; i++)
                {
                    string availableLayer = availableLayers[i].GetLayerName();

                    if (availableLayer == layer)
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
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="VulkanGraphicsDevice" /> class.
    /// </summary>
    ~VulkanGraphicsDevice() => Dispose(isDisposing: false);

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            vkDestroyInstance(Instance);
        }
    }

    /// <inheritdoc />
    public override bool QueryFeature(Feature feature)
    {
        return false;
    }

    /// <inheritdoc />
    public override void WaitIdle()
    {
    }

    /// <inheritdoc />
    public override CommandBuffer BeginCommandBuffer(string? label = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override void SubmitCommandBuffers(CommandBuffer[] commandBuffers, int count)
    {
    }

    /// <inheritdoc />
    protected override GraphicsBuffer CreateBufferCore(in BufferDescription description, void* initialData)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override Texture CreateTextureCore(in TextureDescription description, void* initialData)
    {
        return new VulkanTexture(this, description, initialData);
    }

    /// <inheritdoc />
    protected override SwapChain CreateSwapChainCore(SwapChainSurface surface, in SwapChainDescription description)
    {
        throw new NotImplementedException();
    }

    [UnmanagedCallersOnly]
    private static uint DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageTypes,
        VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* userData)
    {
        string message = new(pCallbackData->pMessage);
        if (messageTypes == VkDebugUtilsMessageTypeFlagsEXT.Validation)
        {
            if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Error)
            {
                //Log.Error($"[Vulkan]: Validation: {messageSeverity} - {message}");
            }
            else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Warning)
            {
                //Log.Warn($"[Vulkan]: Validation: {messageSeverity} - {message}");
            }

            Debug.WriteLine($"[Vulkan]: Validation: {messageSeverity} - {message}");
        }
        else
        {
            if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Error)
            {
                //Log.Error($"[Vulkan]: {messageSeverity} - {message}");
            }
            else if (messageSeverity == VkDebugUtilsMessageSeverityFlagsEXT.Warning)
            {
                //Log.Warn($"[Vulkan]: {messageSeverity} - {message}");
            }

            Debug.WriteLine($"[Vulkan]: {messageSeverity} - {message}");
        }

        return VK_FALSE;
    }

    private static bool CheckIsSupported()
    {
        try
        {
            VkResult result = vkInitialize();
            if (result != VkResult.Success)
            {
                return false;
            }

            VkVersion apiVersion = vkEnumerateInstanceVersion();
            if (apiVersion < VkVersion.Version_1_2)
            {
                //Log.Warn("Vulkan 1.2 is required!");
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
