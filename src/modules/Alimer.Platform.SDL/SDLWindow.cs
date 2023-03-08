// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using Alimer.Graphics;
using static SDL2.SDL;
using static SDL2.SDL.SDL_WindowEventID;
using static SDL2.SDL.SDL_WindowFlags;

namespace Alimer.Platform.SDL;

internal unsafe class SDLWindow : Window
{
    private readonly SDLPlatform _platform;
    private Size _clientSize;
    private bool _minimized;
    private bool _isFullscreen;

    public readonly nint Handle;
    public readonly uint Id;

    /// <inheritdoc />
    public override bool IsMinimized => _minimized;

    /// <inheritdoc />
    public override Size ClientSize => _clientSize;

    /// <inheritdoc />
    public override SwapChainSurface Surface { get; }

    public SDLWindow(SDLPlatform platform)
    {
        _platform = platform;

        SDL_WindowFlags flags = SDL_WINDOW_ALLOW_HIGHDPI | SDL_WINDOW_HIDDEN | SDL_WINDOW_RESIZABLE;

        Handle = SDL_CreateWindow("Vortice", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1200, 800, flags);
        Id = SDL_GetWindowID(Handle);

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
                //Surface = SwapChainSurface.CreateCoreWindow(wmInfo.info.winrt.window);
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

    public void Destroy()
    {
        SDL_DestroyWindow(Handle);
    }

    public void Show()
    {
        SDL_ShowWindow(Handle);
    }

    public void HandleEvent(in SDL_Event evt)
    {
        switch (evt.window.windowEvent)
        {
            case SDL_WINDOWEVENT_MINIMIZED:
                _minimized = true;
                _clientSize = new(evt.window.data1, evt.window.data2);
                OnSizeChanged();
                break;

            case SDL_WINDOWEVENT_MAXIMIZED:
            case SDL_WINDOWEVENT_RESTORED:
                _minimized = false;
                _clientSize = new(evt.window.data1, evt.window.data2);
                OnSizeChanged();
                break;

            case SDL_WINDOWEVENT_RESIZED:
                _minimized = false;
                _clientSize = new(evt.window.data1, evt.window.data2);
                OnSizeChanged();
                break;

            case SDL_WINDOWEVENT_SIZE_CHANGED:
                _minimized = false;
                _clientSize = new(evt.window.data1, evt.window.data2);
                OnSizeChanged();
                break;

            case SDL_WINDOWEVENT_CLOSE:
                //DestroySurface(window);
                _platform.WindowClosed(evt.window.windowID);
                SDL_DestroyWindow(Handle);
                break;
        }
    }
}
