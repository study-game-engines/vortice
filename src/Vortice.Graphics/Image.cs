// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

#if TODO
#if WINDOWS
#elif WINDOWS_UWP
using Windows.Graphics.Imaging;
#elif NETSTANDARD2_0 || NET6_0_OR_GREATER
#endif

namespace Vortice.Graphics;

public sealed class Image
{
    internal Image()
    {
    }

    internal Image(int width, int height, Memory<byte> data)
    {
        Width = width;
        Height = height;
        Data = data;
    }

    public int Width { get; }

    public int Height { get; }

    public Memory<byte> Data { get; }

    public static async Task<Image> LoadAsync(string filePath)
    {
        using FileStream stream = File.OpenRead(filePath);
        return await LoadAsync(stream);
    }

    public static async Task<Image> LoadAsync(Stream stream)
    {
#if WINDOWS
        // TODO: WIC
        return new Image();
#elif WINDOWS_UWP
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());

        PixelDataProvider pixelDataProvider = await decoder.GetPixelDataAsync(
            decoder.BitmapPixelFormat, decoder.BitmapAlphaMode, new BitmapTransform(),
            ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);

        byte[] imageBuffer = pixelDataProvider.DetachPixelData();

        PixelFormat pixelFormat = decoder.BitmapPixelFormat switch
        {
            BitmapPixelFormat.Rgba8 => PixelFormat.RGBA8UNorm,
            BitmapPixelFormat.Bgra8 => PixelFormat.BGRA8UNorm,
            BitmapPixelFormat.Rgba16 => PixelFormat.RGBA16UNorm,
            _ => throw new NotSupportedException("This format is not supported.")
        };

        return new Image((int)decoder.OrientedPixelWidth, (int)decoder.OrientedPixelHeight, imageBuffer);
#elif NETSTANDARD2_0 || NET6_0_OR_GREATER
        using (var skiaStream = new SKManagedStream(stream))
        {
            using (SKCodec codec = SKCodec.Create(skiaStream))
            {
                var info = codec.Info;
                using SKBitmap bitmap = new SKBitmap(info.Width, info.Height, info.ColorType, info.IsOpaque ? SKAlphaType.Opaque : SKAlphaType.Premul);

                var result = codec.GetPixels(bitmap.Info, bitmap.GetPixels(out IntPtr length));
                if (result == SKCodecResult.Success || result == SKCodecResult.IncompleteInput)
                {
                    byte[] imageBuffer = new byte[length.ToInt32()];
                    return new Image(bitmap.Width, bitmap.Height, default/*bitmap.GetPixelSpan()*/);
                }

                return default;
            }
        }
#endif
    }
}

#endif
