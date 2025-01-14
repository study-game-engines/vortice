// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

public readonly struct GraphicsAdapterInfo
{
    public readonly GpuVendorId VendorId { get; init; }

    public readonly uint DeviceId { get; init; }
    public readonly string AdapterName { get; init; }
    public readonly string DriverDescription { get; init; }
    public readonly GpuAdapterType AdapterType { get; init; }
}
