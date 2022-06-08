// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

#if WINDOWS_UWP
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
#endif

namespace Vortice.Graphics;

/// <summary>
/// Defines a platform specific surface used for <see cref="SwapChain"/> creation.
/// </summary>
public abstract class SwapChainSurface
{
    protected SwapChainSurface()
    {

    }

    public abstract SwapChainSurfaceType Type { get; }

#if !WINDOWS_UWP
    /// <summary>
    /// Creates a new <see cref="GraphicsSurface"/> for a Win32 window.
    /// </summary>
    /// <param name="hwnd">The Win32 window handle.</param>
    /// <returns>A new <see cref="SwapChainSurface"/> which can be used to create a <see cref="SwapChain"/> for the given Win32 window.
    /// </returns>
    public static SwapChainSurface CreateWin32(IntPtr hwnd) => new Win32SwapChainSurface(hwnd);
#else
    /// <summary>
    /// Creates a new <see cref="SwapChainSurface"/> for a <see cref="CoreWindow"/>.
    /// </summary>
    /// <param name="coreWindow">The <see cref="CoreWindow"/> handle.</param>
    /// <returns>A new <see cref="SwapChainSurface"/> which can be used to create a <see cref="SwapChain"/> for the given Win32 window.
    /// </returns>
    public static SwapChainSurface CreateCoreWindow(CoreWindow coreWindow) => new CoreWindowSwapChainSurface(coreWindow);

    /// <summary>
    /// Creates a new <see cref="SwapChainSurface"/> for a <see cref="SwapChainPanel"/>.
    /// </summary>
    /// <param name="panel">The <see cref="SwapChainPanel"/> instance.</param>
    /// <returns>A new <see cref="SwapChainSurface"/> which can be used to create a <see cref="SwapChain"/> for the given SwapChainPanel.
    /// </returns>
    public static SwapChainSurface CreateSwapChainPanel(SwapChainPanel panel) => new SwapChainPanelSwapChainSurface(panel);
#endif
}

#if !WINDOWS_UWP
internal class Win32SwapChainSurface : SwapChainSurface
{
    public Win32SwapChainSurface(IntPtr hwnd)
    {
        Hwnd = hwnd;
    }

    public IntPtr HInstance { get; }
    public IntPtr Hwnd { get; }

    /// <inheritdoc />
    public override SwapChainSurfaceType Type => SwapChainSurfaceType.Win32;
}
#else
internal class CoreWindowSwapChainSurface : SwapChainSurface
{
    public CoreWindowSwapChainSurface(CoreWindow coreWindow)
    {
        CoreWindow = coreWindow;
    }

    public CoreWindow CoreWindow { get; }

    /// <inheritdoc />
    public override SwapChainSurfaceType Type => SwapChainSurfaceType.CoreWindow;
}

internal class SwapChainPanelSwapChainSurface : SwapChainSurface
{
    public SwapChainPanel Panel { get; }

    public SwapChainPanelSwapChainSurface(SwapChainPanel panel)
    {
        Panel = panel;
    }

    /// <inheritdoc />
    public override SwapChainSurfaceType Type => SwapChainSurfaceType.SwapChainPanel;
}

#endif
