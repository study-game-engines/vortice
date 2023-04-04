// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public abstract class ComputePassEncoder : CommandEncoder
{
    protected ComputePassEncoder()
    {
    }

    public abstract void Dispatch(int groupCountX, int groupCountY, int groupCountZ);

    public void Dispatch1D(int threadCountX, int groupSizeX = 64)
    {
        Dispatch(DivideByMultiple(threadCountX, groupSizeX), 1, 1);
    }

    public void Dispatch2D(int threadCountX, int threadCountY, int groupSizeX = 8, int groupSizeY = 8)
    {
        Dispatch(
            DivideByMultiple(threadCountX, groupSizeX),
            DivideByMultiple(threadCountY, groupSizeX),
            1
        );
    }

    public void Dispatch3D(int threadCountX, int threadCountY, int threadCountZ, int groupSizeX, int groupSizeY, int groupSizeZ)
    {
        Dispatch(
            DivideByMultiple(threadCountX, groupSizeX),
            DivideByMultiple(threadCountY, groupSizeY),
            DivideByMultiple(threadCountZ, groupSizeZ)
        );
    }

    private static int DivideByMultiple(int value, int alignment)
    {
        return (value + alignment - 1) / alignment;
    }
}
