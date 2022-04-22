// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Graphics;

/// <summary>
/// Defines a platform specific surface used for <see cref="SwapChain"/> creation.
/// </summary>
public abstract class GraphicsSurface
{
    protected GraphicsSurface()
    {

    }

    public abstract SwapChainSourceType Type { get; }

    /// <summary>
    /// Creates a new <see cref="GraphicsSurface"/> for a Win32 window.
    /// </summary>
    /// <param name="hInstance">The Win32 instance handle.</param>
    /// <param name="hwnd">The Win32 window handle.</param>
    /// <returns>A new <see cref="GraphicsSurface"/> which can be used to create a <see cref="SwapChain"/> for the given Win32 window.
    /// </returns>
    public static GraphicsSurface CreateWin32(IntPtr hInstance, IntPtr hwnd) => new Win32SwapChainSource(hInstance, hwnd);

    /// <summary>
    /// Creates a new <see cref="GraphicsSurface"/> for a CoreWindow window.
    /// </summary>
    /// <param name="coreWindow">The CoreWindow handle.</param>
    /// <returns>A new <see cref="GraphicsSurface"/> which can be used to create a <see cref="SwapChain"/> for the given Win32 window.
    /// </returns>
    public static GraphicsSurface CreateCoreWindow(IntPtr coreWindow) => new CoreWindowChainSource(coreWindow);
}

public sealed class Win32SwapChainSource : GraphicsSurface
{
    internal Win32SwapChainSource(IntPtr hInstance, IntPtr hwnd)
    {
        HInstance = hInstance;
        Hwnd = hwnd;
    }

    public IntPtr HInstance { get; }
    public IntPtr Hwnd { get; }

    /// <inheritdoc />
    public override SwapChainSourceType Type => SwapChainSourceType.Win32;
}

public sealed class CoreWindowChainSource : GraphicsSurface
{
    internal CoreWindowChainSource(IntPtr coreWindow)
    {
        CoreWindow = coreWindow;
    }

    public IntPtr CoreWindow { get; }

    /// <inheritdoc />
    public override SwapChainSourceType Type => SwapChainSourceType.CoreWindow;
}
