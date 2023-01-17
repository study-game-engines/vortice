// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Vulkan;
using static Vortice.Vulkan.Vulkan;

namespace Vortice.Graphics.Vulkan;

internal unsafe class VulkanTexture : Texture
{
    private readonly VkImage _handle = VkImage.Null;

    public VkFormat VkFormat { get; }
    public VkImage Handle => _handle;

    public VulkanTexture(VulkanGraphicsDevice device, in TextureDescription description)
        : base(device, description)
    {
        VkFormat = description.Format.ToVkFormat();
        bool isDepthStencil = description.Format.IsDepthStencilFormat();
        VkImageCreateFlags flags = VkImageCreateFlags.None;
        VkImageType imageType = description.Dimension.ToVk();
        VkImageUsageFlags usage = VkImageUsageFlags.None;
        uint depth = 1u;
        uint arrayLayers = 1u;

        switch (description.Dimension)
        {
            case TextureDimension.Texture1D:
                arrayLayers = (uint)description.DepthOrArrayLayers;
                break;

            case TextureDimension.Texture2D:
                arrayLayers = (uint)description.DepthOrArrayLayers;

                if (description.Width == description.Height &&
                    description.DepthOrArrayLayers >= 6)
                {
                    flags |= VkImageCreateFlags.CubeCompatible;
                }
                break;
            case TextureDimension.Texture3D:
                flags |= VkImageCreateFlags.Array2DCompatible;
                depth = (uint)description.DepthOrArrayLayers;
                break;
        }

        if ((description.Usage & TextureUsage.ShaderRead) != 0)
        {
            usage |= VkImageUsageFlags.Sampled;
        }
        if ((description.Usage & TextureUsage.ShaderWrite) != 0)
        {
            usage |= VkImageUsageFlags.Storage;

            //if (IsFormatSRGB(texture->desc.format))
            //{
            //    imageInfo.flags |= VK_IMAGE_CREATE_EXTENDED_USAGE_BIT;
            //}
        }

        if ((description.Usage & TextureUsage.RenderTarget) != 0)
        {
            if (isDepthStencil)
            {
                usage |= VkImageUsageFlags.DepthStencilAttachment;
            }
            else
            {
                usage |= VkImageUsageFlags.ColorAttachment;
            }
        }

        if ((description.Usage & TextureUsage.Transient) != 0)
        {
            usage |= VkImageUsageFlags.TransientAttachment;

        }
        else
        {
            usage |= VkImageUsageFlags.TransferSrc | VkImageUsageFlags.TransferDst;
        }

        VkImageCreateInfo imageInfo = new()
        {
            sType = VkStructureType.ImageCreateInfo,
            flags = flags,
            imageType = imageType,
            format = VkFormat,
            extent = new((uint)description.Width, (uint)description.Height, depth),
            mipLevels = (uint)MipLevels,
            arrayLayers = arrayLayers,
            samples = SampleCount.ToVkSampleCount(),
            tiling = VkImageTiling.Linear,
            usage = usage
        };
        vkCreateImage(device.Handle, &imageInfo, null, out _handle).CheckResult();
    }

    public VulkanTexture(GraphicsDevice device, VkImage existingTexture, in TextureDescription description)
        : base(device, description)
    {
        _handle = existingTexture;
    }

    // <summary>
    /// Finalizes an instance of the <see cref="VulkanTexture" /> class.
    /// </summary>
    ~VulkanTexture() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            base.Dispose(disposing);

        }
    }

    /// <inheritdoc />
    protected override void OnLabelChanged(string newLabel)
    {
    }

    protected override TextureView CreateView(in TextureViewDescription description) => new VulkanTextureView(this, description);
}
