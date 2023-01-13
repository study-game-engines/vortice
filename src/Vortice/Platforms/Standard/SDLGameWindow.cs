// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using Vortice.Graphics;
using Vortice.Mathematics;
using static SDL2.SDL;
using static SDL2.SDL.SDL_WindowFlags;

namespace Vortice;

internal class SDLGameWindow : GameView
{
    private readonly SDLGamePlatform _platform;

    public unsafe SDLGameWindow(SDLGamePlatform platform)
    {
        _platform = platform;

        SDL_WindowFlags flags = SDL_WINDOW_ALLOW_HIGHDPI | SDL_WINDOW_HIDDEN | SDL_WINDOW_RESIZABLE;

        Handle = SDL_CreateWindow("Vortice", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1200, 800, flags);

        // Native handle
        var wmInfo = new SDL_SysWMinfo();
        SDL_GetWindowWMInfo(Handle, ref wmInfo);

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
    }

    public IntPtr Handle { get; }

    /// <inheritdoc />
    public override Size ClientSize
    {
        get
        {
            SDL_GetWindowSize(Handle, out int width, out int height);
            return new(width, height);
        }
    }

    /// <inheritdoc />
    public override SwapChainSurface Surface { get; }

    public void Show()
    {
        SDL_ShowWindow(Handle);
    }
}
