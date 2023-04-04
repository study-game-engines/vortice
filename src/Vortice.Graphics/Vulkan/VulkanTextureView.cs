// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;

namespace Vortice.Graphics.Vulkan;

internal unsafe class VulkanTextureView : TextureView
{
    public VkImageView Handle { get; }

    public VulkanTextureView(VulkanTexture texture, in TextureViewDescription description)
        : base(texture, description)
    {
    }

    // <summary>
    /// Finalizes an instance of the <see cref="VulkanTextureView" /> class.
    /// </summary>
    ~VulkanTextureView() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    /// <inheritdoc />
    protected override void OnLabelChanged(string newLabel)
    {
    }
}
