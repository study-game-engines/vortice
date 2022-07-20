// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using CommunityToolkit.Diagnostics;
using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Graphics.Vulkan;

internal unsafe class VulkanGraphicsDevice : GraphicsDevice
{
    private static readonly Lazy<bool> s_isSupported = new(CheckIsSupported);

    public static bool IsSupported() => s_isSupported.Value;

    private readonly GraphicsAdapterInfo _adapterInfo;
    //private readonly GraphicsDeviceFeatures _features;
    private readonly GraphicsDeviceLimits _limits;

    public VulkanGraphicsDevice(ValidationMode validationMode)
        : base(GraphicsBackend.Count)
    {
        Guard.IsTrue(IsSupported(), nameof(VulkanGraphicsDevice), "Vulkan is not supported");

        _adapterInfo = new()
        {
        };
    }

    public VkInstance Instance { get; }

    /// <inheritdoc />
    public override GraphicsAdapterInfo AdapterInfo => _adapterInfo;

    /// <inheritdoc />
    //public override GraphicsDeviceFeatures Features => _features;

    /// <inheritdoc />
    public override GraphicsDeviceLimits Limits => _limits;


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
    protected override Texture CreateTextureCore(in TextureDescription description)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override SwapChain CreateSwapChainCore(SwapChainSurface surface, in SwapChainDescription description)
    {
        throw new NotImplementedException();
    }

    private static bool CheckIsSupported()
    {
        try
        {
            return true;
        }
        catch
        {
            return false;
        }
    }
}
