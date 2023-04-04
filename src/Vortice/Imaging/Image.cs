// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Graphics;
using System.IO;
//using static Alimer.NativeApi;

namespace Vortice.Imaging;

public sealed class Image 
{
    public int Width { get; }

    public int Height { get; }

    public Memory<byte> Data { get; }

    internal Image(int width, int height, Memory<byte> data)
    {
        Width = width;
        Height = height;
        Data = data;
    }

    public static Image? FromFile(string filePath)
    {
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            return FromStream(stream);
        }
    }

    public static Image? FromStream(Stream stream)
    {
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, (int)stream.Length);
        return FromMemory(data);
    }

    public static Image? FromMemory(byte[] data)
    {
#if TODO
        if (IsKTX1(data) || IsKTX2(data))
        {
            unsafe
            {
                fixed (byte* dataPtr = data)
                {
                    KtxTexture ktx_texture = ktx_load_from_memory(dataPtr, (uint)data.Length, out KtxErrorCode code);

                    byte* texture_data;
                    nuint length;
                    ktx_get_data(ktx_texture.Handle, out texture_data, out length);
                    byte[] imageData = new byte[length];
                    Image image = new Image((int)ktx_texture.BaseWidth, (int)ktx_texture.BaseHeight, imageData);
                    ktx_destroy_texture(ktx_texture.Handle);
                }
            }
        } 
#endif

        return default;
    }

    private static bool IsKTX1(byte[] data)
    {
        if (data.Length <= 12)
        {
            return false;
        }

        Span<byte> id = stackalloc byte[] { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x31, 0x31, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A };
        for (var i = 0; i < 12; i++)
        {
            if (data[i] != id[i])
                return false;
        }

        return true;
    }

    private static bool IsKTX2(byte[] data)
    {
        if (data.Length <= 12)
        {
            return false;
        }

        Span<byte> id = stackalloc byte[] { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x32, 0x30, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A };
        for (var i = 0; i < 12; i++)
        {
            if (data[i] != id[i])
                return false;
        }

        return true;
    }
}

// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

#if TODO
#if WINDOWS
#elif WINDOWS_UWP
using Windows.Graphics.Imaging;
#elif NETSTANDARD2_0 || NET6_0_OR_GREATER
#endif

namespace Alimer.Graphics;

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
