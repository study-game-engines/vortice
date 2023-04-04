// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using Vortice.Graphics;
using Alimer.Bindings.SDL;
using static Alimer.Bindings.SDL.SDL;
using static Alimer.Bindings.SDL.SDL_EventType;
using static Alimer.Bindings.SDL.SDL_WindowFlags;

namespace Vortice.Platform.SDL;

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

        SDL_WindowFlags flags = SDL_WINDOW_RESIZABLE | SDL_WINDOW_HIDDEN;

        Handle = SDL_CreateWindowWithPosition("Vortice", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1200, 800, flags);
        Id = SDL_GetWindowID(Handle);

        // Native handle
        SDL_SysWMinfo wmInfo = default;
        SDL_GetWindowWMInfo(Handle, &wmInfo);

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
        switch (evt.window.type)
        {
            case SDL_EVENT_WINDOW_MINIMIZED:
                _minimized = true;
                _clientSize = new(evt.window.data1, evt.window.data2);
                OnSizeChanged();
                break;

            case SDL_EVENT_WINDOW_MAXIMIZED:
            case SDL_EVENT_WINDOW_RESTORED:
                _minimized = false;
                _clientSize = new(evt.window.data1, evt.window.data2);
                OnSizeChanged();
                break;

            case SDL_EVENT_WINDOW_RESIZED:
                _minimized = false;
                _clientSize = new(evt.window.data1, evt.window.data2);
                OnSizeChanged();
                break;

            //case SDL_EVENT_WINDOW_CHANGED:
            //    _minimized = false;
            //    _clientSize = new(evt.window.data1, evt.window.data2);
            //    OnSizeChanged();
            //    break;

            case SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                //DestroySurface(window);
                _platform.WindowClosed(evt.window.windowID);
                SDL_DestroyWindow(Handle);
                break;
        }
    }
}
