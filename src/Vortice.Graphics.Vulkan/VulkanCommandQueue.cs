// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Graphics.Vulkan;

internal unsafe class VulkanCommandQueue : CommandQueue
{
    public readonly VkQueue Handle;

    public VulkanCommandQueue(VulkanGraphicsDevice device, CommandQueueType queueType, uint queueFamilyIndex, uint queueIndex)
        : base(device)
    {
        QueueType = queueType;

        vkGetDeviceQueue(device.NativeDevice, queueFamilyIndex, queueIndex, out Handle);
    }

    // <summary>
    /// Finalizes an instance of the <see cref="VulkanCommandQueue" /> class.
    /// </summary>
    ~VulkanCommandQueue() => Dispose(isDisposing: false);

    /// <inheritdoc />
    public override CommandQueueType QueueType { get; }

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
        }
    }

    /// <inheritdoc />
    public override CommandBuffer BeginCommandBuffer() => default;
}
