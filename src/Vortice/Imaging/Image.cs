// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Graphics;
using System.IO;
using static Vortice.NativeApi;

namespace Vortice.Imaging;

public sealed class Image : DisposableObject
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

        return default;
    }

    protected override void Dispose(bool isDisposing)
    {

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
