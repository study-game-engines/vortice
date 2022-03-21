// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Graphics.Vulkan;

internal sealed unsafe class VulkanGraphicsDevice : GraphicsDevice
{
    private static readonly VkString s_engineName = new("Vortice");

    private readonly bool _debugUtils;
    private readonly VkInstance _instance;
    private readonly VkDebugUtilsMessengerEXT _debugMessenger = VkDebugUtilsMessengerEXT.Null;

    private readonly VkPhysicalDeviceExtensions _supportedExtensions;
    private readonly VkPhysicalDevice _physicalDevice;

    private readonly bool _dynamicRendering; // Beta
    private readonly ulong _minAllocationAlignment;

    private readonly VkDevice _handle;
    private readonly GraphicsDeviceCaps _caps;

    public VulkanGraphicsDevice(in GraphicsDeviceDescriptor descriptor)
    {
        if (!VulkanUtils.IsSupported())
        {
            throw new InvalidOperationException("Vulkan is not supported");
        }

        // Create instance and debug utils
        {
            VkVersion apiVersion = vkEnumerateInstanceVersion();

            HashSet<string> availableInstanceExtensions = new(VulkanUtils.EnumerateInstanceExtensions());
            HashSet<string> availableInstanceLayers = new(VulkanUtils.EnumerateInstanceLayers());

            List<string> instanceExtensions = new();
            List<string> instanceLayers = new();

            if (availableInstanceExtensions.Contains(VK_EXT_DEBUG_UTILS_EXTENSION_NAME))
            {
                _debugUtils = true;
                instanceExtensions.Add(VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
            }

            if (availableInstanceExtensions.Contains(VK_KHR_GET_PHYSICAL_DEVICE_PROPERTIES_2_EXTENSION_NAME))
            {
                instanceExtensions.Add(VK_KHR_GET_PHYSICAL_DEVICE_PROPERTIES_2_EXTENSION_NAME);
            }
            if (availableInstanceExtensions.Contains(VK_EXT_SWAPCHAIN_COLOR_SPACE_EXTENSION_NAME))
            {
                instanceExtensions.Add(VK_EXT_SWAPCHAIN_COLOR_SPACE_EXTENSION_NAME);
            }

            instanceExtensions.Add(VK_KHR_SURFACE_EXTENSION_NAME);

            if (OperatingSystem.IsWindows())
            {
                instanceExtensions.Add(VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
            }
            else if (OperatingSystem.IsAndroid())
            {
                instanceExtensions.Add(VK_KHR_ANDROID_SURFACE_EXTENSION_NAME);
            }
            else if (OperatingSystem.IsIOS() || OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
            {
                instanceExtensions.Add(VK_EXT_METAL_SURFACE_EXTENSION_NAME);
            }
            else if (OperatingSystem.IsLinux())
            {
                if (availableInstanceExtensions.Contains(VK_KHR_XCB_SURFACE_EXTENSION_NAME))
                {
                    instanceExtensions.Add(VK_KHR_XCB_SURFACE_EXTENSION_NAME);
                }
                if (availableInstanceExtensions.Contains(VK_KHR_XLIB_SURFACE_EXTENSION_NAME))
                {
                    instanceExtensions.Add(VK_KHR_XLIB_SURFACE_EXTENSION_NAME);
                }
                if (availableInstanceExtensions.Contains(KHRWaylandSurfaceExtensionName))
                {
                    instanceExtensions.Add(KHRWaylandSurfaceExtensionName);
                }
            }

            if (descriptor.ValidationMode != ValidationMode.Disabled)
            {
                // Determine the optimal validation layers to enable that are necessary for useful debugging
                string[] optimalValidationLyers = VulkanUtils.GetOptimalValidationLayers(availableInstanceLayers);
                instanceLayers.AddRange(optimalValidationLyers);
            }

            using VkString appName = new(string.IsNullOrEmpty(descriptor.Name) ? "Vortice" : descriptor.Name);

            VkApplicationInfo appInfo = new()
            {
                sType = VkStructureType.ApplicationInfo,
                pApplicationName = appName,
                //applicationVersion = new VkVersion(1, 0, 0),
                pEngineName = s_engineName,
                engineVersion = new VkVersion(1, 0, 0),
                apiVersion = VkVersion.Version_1_2
            };

            using var vkLayerNames = new VkStringArray(instanceLayers);
            using VkStringArray vkInstanceExtensions = new(instanceExtensions);

            VkInstanceCreateInfo createInfo = new()
            {
                sType = VkStructureType.InstanceCreateInfo,
                pApplicationInfo = &appInfo,
                enabledLayerCount = vkLayerNames.Length,
                ppEnabledLayerNames = vkLayerNames,
                enabledExtensionCount = vkInstanceExtensions.Length,
                ppEnabledExtensionNames = vkInstanceExtensions
            };

            VkDebugUtilsMessengerCreateInfoEXT debugUtilsCreateInfo = new()
            {
                sType = VkStructureType.DebugUtilsMessengerCreateInfoEXT
            };

            if (descriptor.ValidationMode != ValidationMode.Disabled && _debugUtils)
            {
                debugUtilsCreateInfo.messageSeverity = VkDebugUtilsMessageSeverityFlagsEXT.Warning | VkDebugUtilsMessageSeverityFlagsEXT.Error;
                debugUtilsCreateInfo.messageType = VkDebugUtilsMessageTypeFlagsEXT.Validation | VkDebugUtilsMessageTypeFlagsEXT.Performance;

                if (descriptor.ValidationMode == ValidationMode.Verbose)
                {
                    debugUtilsCreateInfo.messageSeverity |= VkDebugUtilsMessageSeverityFlagsEXT.Verbose | VkDebugUtilsMessageSeverityFlagsEXT.Info;
                }

#if NET6_0_OR_GREATER
                debugUtilsCreateInfo.pfnUserCallback = &DebugMessengerCallback;
#else
            debugUtilsCreateInfo.pfnUserCallback = Marshal.GetFunctionPointerForDelegate(DebugMessagerCallbackDelegate);
#endif
                createInfo.pNext = &debugUtilsCreateInfo;
            }

            vkCreateInstance(&createInfo, null, out _instance).CheckResult();
            vkLoadInstance(_instance);

            if (instanceLayers.Count > 0)
            {
                vkCreateDebugUtilsMessengerEXT(_instance, &debugUtilsCreateInfo, null, out _debugMessenger).CheckResult();
            }

            //Log.Info($"Created VkInstance with version: {appInfo.apiVersion.Major}.{appInfo.apiVersion.Minor}.{appInfo.apiVersion.Patch}");
            //if (instanceLayers.Count > 0)
            //{
            //    foreach (var layer in instanceLayers)
            //    {
            //        Log.Info($"Instance layer '{layer}'");
            //    }
            //}
            //
            //foreach (string extension in instanceExtensions)
            //{
            //    Log.Info($"Instance extension '{extension}'");
            //}
        }

        // Find physical device, setup queue's and create device.
        int physicalDevicesCount = 0;
        vkEnumeratePhysicalDevices(_instance, &physicalDevicesCount, null).CheckResult();

        if (physicalDevicesCount == 0)
        {
            throw new GraphicsException("Vulkan: Failed to find GPUs with Vulkan support");
        }

        VkPhysicalDevice* physicalDevices = stackalloc VkPhysicalDevice[physicalDevicesCount];
        vkEnumeratePhysicalDevices(_instance, &physicalDevicesCount, physicalDevices).CheckResult();

        List<string> enabledDeviceExtensions = new List<string>();

        VkPhysicalDeviceFeatures2 features2 = default;
        VkPhysicalDeviceProperties2 properties2 = default;
        VkPhysicalDeviceVulkan11Features features_1_1 = default;
        VkPhysicalDeviceVulkan12Features features_1_2 = default;

        for (int i = 0; i < physicalDevicesCount; i++)
        {
            VkPhysicalDevice physicalDevice = physicalDevices[i];

            vkGetPhysicalDeviceProperties(physicalDevice, out VkPhysicalDeviceProperties properties);
            if (properties.apiVersion < VkVersion.Version_1_1)
            {
                continue;
            }

            VkPhysicalDeviceExtensions physicalDeviceExt = VulkanUtils.QueryPhysicalDeviceExtensions(physicalDevice);
            bool suitable = physicalDeviceExt.swapchain && physicalDeviceExt.depth_clip_enable;

            if (!suitable)
            {
                continue;
            }

            features2 = new VkPhysicalDeviceFeatures2
            {
                sType = VkStructureType.PhysicalDeviceFeatures2
            };

            features_1_1 = new VkPhysicalDeviceVulkan11Features
            {
                sType = VkStructureType.PhysicalDeviceVulkan11Features
            };

            features_1_2 = new VkPhysicalDeviceVulkan12Features
            {
                sType = VkStructureType.PhysicalDeviceVulkan12Features
            };

            VkPhysicalDevicePerformanceQueryFeaturesKHR perf_counter_features = default;

            features2.pNext = &features_1_1;
            features_1_1.pNext = &features_1_2;
            void** features_chain = &features_1_2.pNext;

            //acceleration_structure_features = { };
            //raytracing_features = { };
            //raytracing_query_features = { };
            //fragment_shading_rate_features = { };
            //mesh_shader_features = { };
            VkPhysicalDeviceDepthClipEnableFeaturesEXT depth_clip_enable_features = default;

            properties2 = new VkPhysicalDeviceProperties2
            {
                sType = VkStructureType.PhysicalDeviceProperties2
            };

            VkPhysicalDeviceVulkan11Properties properties_1_1 = new VkPhysicalDeviceVulkan11Properties
            {
                sType = VkStructureType.PhysicalDeviceVulkan11Properties
            };

            VkPhysicalDeviceVulkan12Properties properties_1_2 = new VkPhysicalDeviceVulkan12Properties
            {
                sType = VkStructureType.PhysicalDeviceVulkan12Properties
            };

            properties2.pNext = &properties_1_1;
            properties_1_1.pNext = &properties_1_2;
            void** properties_chain = &properties_1_2.pNext;

            //sampler_minmax_properties = { };
            //acceleration_structure_properties = { };
            //raytracing_properties = { };
            //fragment_shading_rate_properties = { };
            //mesh_shader_properties = { };

            enabledDeviceExtensions.Clear();
            enabledDeviceExtensions.Add(VK_KHR_SWAPCHAIN_EXTENSION_NAME);

            // Core in 1.2
            //{
            //    // Required by VK_KHR_spirv_1_4
            //    enabledDeviceExtensions.Add(VK_KHR_SHADER_FLOAT_CONTROLS_EXTENSION_NAME);
            //
            //    // Required for VK_KHR_ray_tracing_pipeline
            //    enabledDeviceExtensions.Add(VK_KHR_SPIRV_1_4_EXTENSION_NAME);
            //
            //    enabledDeviceExtensions.Add(VK_EXT_SHADER_VIEWPORT_INDEX_LAYER_EXTENSION_NAME);
            //
            //    // Required by VK_KHR_acceleration_structure
            //    enabledDeviceExtensions.Add(VK_KHR_BUFFER_DEVICE_ADDRESS_EXTENSION_NAME);
            //
            //    // Required by VK_KHR_acceleration_structure
            //    enabledDeviceExtensions.Add(VK_EXT_DESCRIPTOR_INDEXING_EXTENSION_NAME);
            //
            //    // Required by VK_KHR_fragment_shading_rate
            //    enabledDeviceExtensions.Add(VK_KHR_CREATE_RENDERPASS_2_EXTENSION_NAME);
            //}

            // For performance queries, we also use host query reset since queryPool resets cannot live in the same command buffer as beginQuery
            if (physicalDeviceExt.performance_query)
            {
                perf_counter_features.sType = VkStructureType.PhysicalDevicePerformanceQueryFeaturesKHR;
                enabledDeviceExtensions.Add(VK_KHR_PERFORMANCE_QUERY_EXTENSION_NAME);
                *features_chain = &perf_counter_features;
                features_chain = &perf_counter_features.pNext;
            }

            if (physicalDeviceExt.memory_budget)
            {
                enabledDeviceExtensions.Add(VK_EXT_MEMORY_BUDGET_EXTENSION_NAME);
            }

            if (physicalDeviceExt.depth_clip_enable)
            {
                depth_clip_enable_features.sType = VkStructureType.PhysicalDeviceDepthClipEnableFeaturesEXT;
                enabledDeviceExtensions.Add(VK_EXT_DEPTH_CLIP_ENABLE_EXTENSION_NAME);
                *features_chain = &depth_clip_enable_features;
                features_chain = &depth_clip_enable_features.pNext;
            }

            if (physicalDeviceExt.deferred_host_operations)
            {
                // Required by VK_KHR_acceleration_structure
                enabledDeviceExtensions.Add(VK_KHR_DEFERRED_HOST_OPERATIONS_EXTENSION_NAME);
            }

            vkGetPhysicalDeviceProperties2(physicalDevice, &properties2);

            bool discrete = properties2.properties.deviceType == VkPhysicalDeviceType.DiscreteGpu;
            if (discrete || _physicalDevice.IsNull)
            {
                _supportedExtensions = physicalDeviceExt;
                _physicalDevice = physicalDevice;
                if (discrete)
                {
                    break; // if this is discrete GPU, look no further (prioritize discrete GPU)
                }
            }
        }

        {
            ReadOnlySpan<VkQueueFamilyProperties> queueFamilies = vkGetPhysicalDeviceQueueFamilyProperties(PhysicalDevice);

            float priority = 1.0f;
            VkDeviceQueueCreateInfo queueCreateInfo = new VkDeviceQueueCreateInfo
            {
                sType = VkStructureType.DeviceQueueCreateInfo,
                queueFamilyIndex = 0, // queueFamilies.graphicsFamily,
                queueCount = 1,
                pQueuePriorities = &priority
            };

            using var deviceExtensionNames = new VkStringArray(enabledDeviceExtensions);

            VkDeviceCreateInfo createInfo = new VkDeviceCreateInfo
            {
                sType = VkStructureType.DeviceCreateInfo,
                pNext = &features2,
                queueCreateInfoCount = 1,
                pQueueCreateInfos = &queueCreateInfo,
                enabledExtensionCount = deviceExtensionNames.Length,
                ppEnabledExtensionNames = deviceExtensionNames,
                pEnabledFeatures = null,
            };

            VkResult result = vkCreateDevice(PhysicalDevice, &createInfo, null, out _handle);
            if (result != VkResult.Success)
            {
                throw new GraphicsException("Vulkan: Cannot create device");
            }
        }

        // Init caps
        {
            vkGetPhysicalDeviceProperties(PhysicalDevice, out VkPhysicalDeviceProperties properties);
            vkGetPhysicalDeviceFeatures(PhysicalDevice, out VkPhysicalDeviceFeatures features);

            VendorId = (GpuVendorId)properties.vendorID;
            AdapterId = properties.deviceID;
            AdapterName = properties.GetDeviceName();

            switch (properties.deviceType)
            {
                case VkPhysicalDeviceType.IntegratedGpu:
                    AdapterType = GpuAdapterType.IntegratedGPU;
                    break;

                case VkPhysicalDeviceType.DiscreteGpu:
                    AdapterType = GpuAdapterType.DiscreteGPU;
                    break;

                case VkPhysicalDeviceType.Cpu:
                    AdapterType = GpuAdapterType.CPU;
                    break;

                default:
                    AdapterType = GpuAdapterType.Unknown;
                    break;
            }

            _caps = new GraphicsDeviceCaps()
            {
                Features = new GraphicsDeviceFeatures
                {
                    IndependentBlend = features.independentBlend,
                    ComputeShader = true,
                    TessellationShader = features.tessellationShader,
                    MultiViewport = features.multiViewport,
                    IndexUInt32 = features.fullDrawIndexUint32,
                    MultiDrawIndirect = features.multiDrawIndirect,
                    FillModeNonSolid = features.fillModeNonSolid,
                    SamplerAnisotropy = features.samplerAnisotropy,
                    TextureCompressionETC2 = features.textureCompressionETC2,
                    TextureCompressionASTC_LDR = features.textureCompressionASTC_LDR,
                    TextureCompressionBC = features.textureCompressionBC,
                    TextureCubeArray = features.imageCubeArray,
                    Raytracing = false
                },
                Limits = new GraphicsDeviceLimits
                {
                    MaxVertexAttributes = properties.limits.maxVertexInputAttributes,
                    MaxVertexBindings = properties.limits.maxVertexInputBindings,
                    MaxVertexAttributeOffset = properties.limits.maxVertexInputAttributeOffset,
                    MaxVertexBindingStride = properties.limits.maxVertexInputBindingStride,
                    MaxTextureDimension1D = properties.limits.maxImageDimension1D,
                    MaxTextureDimension2D = properties.limits.maxImageDimension2D,
                    MaxTextureDimension3D = properties.limits.maxImageDimension3D,
                    MaxTextureDimensionCube = properties.limits.maxImageDimensionCube,
                    MaxTextureArrayLayers = properties.limits.maxImageArrayLayers,
                    MaxColorAttachments = properties.limits.maxColorAttachments,
                    MaxUniformBufferRange = properties.limits.maxUniformBufferRange,
                    MaxStorageBufferRange = properties.limits.maxStorageBufferRange,
                    MinUniformBufferOffsetAlignment = properties.limits.minUniformBufferOffsetAlignment,
                    MinStorageBufferOffsetAlignment = properties.limits.minStorageBufferOffsetAlignment,
                    MaxSamplerAnisotropy = (uint)properties.limits.maxSamplerAnisotropy,
                    MaxViewports = properties.limits.maxViewports,
                    MaxViewportWidth = properties.limits.maxViewportDimensions[0],
                    MaxViewportHeight = properties.limits.maxViewportDimensions[1],
                    MaxTessellationPatchSize = properties.limits.maxTessellationPatchSize,
                    MaxComputeSharedMemorySize = properties.limits.maxComputeSharedMemorySize,
                    MaxComputeWorkGroupCountX = properties.limits.maxComputeWorkGroupCount[0],
                    MaxComputeWorkGroupCountY = properties.limits.maxComputeWorkGroupCount[1],
                    MaxComputeWorkGroupCountZ = properties.limits.maxComputeWorkGroupCount[2],
                    MaxComputeWorkGroupInvocations = properties.limits.maxComputeWorkGroupInvocations,
                    MaxComputeWorkGroupSizeX = properties.limits.maxComputeWorkGroupSize[0],
                    MaxComputeWorkGroupSizeY = properties.limits.maxComputeWorkGroupSize[1],
                    MaxComputeWorkGroupSizeZ = properties.limits.maxComputeWorkGroupSize[2],
                }
            };
        }
    }

    public VkInstance Instance => _instance;

    public bool DebugUtils => _debugUtils;

    public VkPhysicalDevice PhysicalDevice => _physicalDevice;
    public VkDevice NativeDevice => _handle;

    // <inheritdoc />
    public override GpuBackend BackendType => GpuBackend.Vulkan;

    // <inheritdoc />
    public override GpuVendorId VendorId { get; }

    /// <inheritdoc />
    public override uint AdapterId { get; }

    /// <inheritdoc />
    public override GpuAdapterType AdapterType { get; }

    /// <inheritdoc />
    public override string AdapterName { get; }

    /// <inheritdoc />
    public override GraphicsDeviceCaps Capabilities => _caps;


    /// <inheritdoc />
    protected override void OnDispose()
    {
        WaitIdle();

        if (!_handle.IsNull)
        {
            vkDestroyDevice(_handle, null);
        }

        if (_debugMessenger != VkDebugUtilsMessengerEXT.Null)
        {
            vkDestroyDebugUtilsMessengerEXT(_instance, _debugMessenger, null);
        }

        if (_instance != VkInstance.Null)
        {
            vkDestroyInstance(_instance, null);
        }
    }

    [UnmanagedCallersOnly]
    private static uint DebugMessengerCallback(VkDebugUtilsMessageSeverityFlagsEXT messageSeverity,
        VkDebugUtilsMessageTypeFlagsEXT messageTypes,
        VkDebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* userData)
    {
        string? message = Interop.GetString(pCallbackData->pMessage);
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

            System.Diagnostics.Debug.WriteLine($"[Vulkan]: Validation: {messageSeverity} - {message}");
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

            System.Diagnostics.Debug.WriteLine($"[Vulkan]: {messageSeverity} - {message}");
        }

        return VK_FALSE;
    }


    /// <inheritdoc />
    public override void WaitIdle()
    {
        vkDeviceWaitIdle(_handle).CheckResult();
    }

    /// <inheritdoc />
    public override CommandBuffer BeginCommandBuffer(CommandQueueType queueType = CommandQueueType.Graphics) => default;

    /// <inheritdoc />
    protected override GraphicsBuffer CreateBufferCore(in BufferDescriptor descriptor, IntPtr initialData) => throw new NotImplementedException();
    /// <inheritdoc />
    protected override Texture CreateTextureCore(in TextureDescriptor descriptor) => new VulkanTexture(this, descriptor);
    /// <inheritdoc />
    protected override SwapChain CreateSwapChainCore(in GraphicsSurface surface, in SwapChainDescriptor descriptor) => new VulkanSwapChain(this, surface, descriptor);
}
