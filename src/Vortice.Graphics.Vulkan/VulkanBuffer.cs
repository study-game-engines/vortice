// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Graphics.Vulkan;

internal unsafe class VulkanBuffer : GraphicsBuffer
{
    private readonly VkBuffer _handle;

    public VulkanBuffer(VulkanGraphicsDevice device, in BufferDescription description, IntPtr initialData)
        : base(device, description)
    {
        VkBufferCreateInfo createInfo = new VkBufferCreateInfo()
        {
            sType = VkStructureType.BufferCreateInfo
        };
        vkCreateBuffer(device.NativeDevice, &createInfo, null, out _handle);
    }

    // <summary>
    /// Finalizes an instance of the <see cref="VulkanBuffer" /> class.
    /// </summary>
    ~VulkanBuffer() => Dispose(isDisposing: false);

    public VkBuffer Handle => _handle;

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            if (!_handle.IsNull)
            {
                //const u64 frameCount = device->GetFrameCount();
            }
        }
    }
}
