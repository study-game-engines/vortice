// Copyright � Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public abstract class CommandBuffer 
{
    protected CommandBuffer()
    {
    }

    public abstract void Commit();
}
