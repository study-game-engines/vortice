// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Mathematics;
using Vortice.Graphics;
using static SDL2.SDL;
using static SDL2.SDL.SDL_WindowFlags;
using System.Runtime.InteropServices;

namespace Vortice;

internal unsafe class SDL2GameView : GameView
{
    private readonly IntPtr _window;

    public SDL2GameView()
    {
        SDL_WindowFlags flags = SDL_WINDOW_ALLOW_HIGHDPI | SDL_WINDOW_HIDDEN | SDL_WINDOW_RESIZABLE;

        _window = SDL_CreateWindow("Vortice", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1200, 800, flags);

        SDL_GetWindowSize(_window, out int width, out int height);
        ClientSize = new(width, height);

        // Native handle
        var wmInfo = new SDL_SysWMinfo();
        SDL_GetWindowWMInfo(_window, ref wmInfo);

        // Window handle is selected per subsystem as defined at:
        // https://wiki.libsdl.org/SDL_SysWMinfo
        switch (wmInfo.subsystem)
        {
            case SDL_SYSWM_TYPE.SDL_SYSWM_WINDOWS:
                Surface = SwapChainSurface.CreateWin32(wmInfo.info.win.window);
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_WINRT:
                //Surface = GraphicsSurface.CreateCoreWindow(wmInfo.info.winrt.window);
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_X11:
                //return wmInfo.info.x11.window;
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_COCOA:
                //return wmInfo.info.cocoa.window;
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_UIKIT:
                //return wmInfo.info.uikit.window;
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_WAYLAND:
                //return wmInfo.info.wl.shell_surface;
                break;

            case SDL_SYSWM_TYPE.SDL_SYSWM_ANDROID:
                //return wmInfo.info.android.window;
                break;

            default:
                break;
        }

        SDL_ShowWindow(_window);
    }

    /// <inheritdoc />
    public override SizeI ClientSize { get; }

    /// <inheritdoc />
    public override SwapChainSurface Surface { get; }

    private void OnControlClientSizeChanged(object? sender, EventArgs e)
    {
        OnSizeChanged();
    }
}
