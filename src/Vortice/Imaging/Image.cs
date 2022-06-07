// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Graphics;
using static Vortice.NativeApi;

namespace Vortice.Imaging;

public sealed class Image : DisposableObject
{
    private const string NativeLibName = "vortice_image";

    public Image()
    {

    }

    public static unsafe Image FromMemory(byte[] data)
    {
        fixed (byte* dataPtr = data)
        {
            IntPtr handle = ktx_load_from_memory(dataPtr, (uint)data.Length, out KtxErrorCode code);
        }

        return new Image();
    }

    protected override void Dispose(bool isDisposing)
    {

    }
}
