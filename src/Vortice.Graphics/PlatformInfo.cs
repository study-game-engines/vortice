// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;

namespace Vortice.Graphics;

internal static class PlatformInfo
{
    public static bool IsUnix { get; }

    public static bool IsWindows { get; }

    public static bool IsMacOS { get; }

    public static bool IsLinux { get; }

    public static bool IsAndroid { get; }

    public static bool IsArm { get; }

    public static bool Is64Bit { get; }

    static PlatformInfo()
    {
        IsMacOS = OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst() || OperatingSystem.IsIOS() || OperatingSystem.IsTvOS();
        IsLinux = OperatingSystem.IsLinux();
        IsUnix = IsMacOS || IsLinux;
        IsWindows = OperatingSystem.IsWindows();
        IsAndroid = OperatingSystem.IsAndroid();

        var arch = RuntimeInformation.ProcessArchitecture;
        IsArm = arch == Architecture.Arm || arch == Architecture.Arm64;
        Is64Bit = Environment.Is64BitOperatingSystem;
    }
}
