// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Drawing;
using Vortice.Graphics;
using Alimer.Bindings.SDL;
using static Alimer.Bindings.SDL.SDL;
using static Alimer.Bindings.SDL.SDL.SDL_WindowEventID;
using static Alimer.Bindings.SDL.SDL.SDL_WindowFlags;

namespace Vortice;

internal unsafe class SDLGameWindow : GameView
{
    private static Dictionary<uint, SDLGameWindow> _idLookup = new();

    public readonly SDL_Window Handle;
    public readonly uint Id;
    private Size _clientSize;
    private bool _minimized;
    private bool _isFullscreen;

    public SDLGameWindow()
    {
        SDL_WindowFlags flags = SDL_WINDOW_ALLOW_HIGHDPI | SDL_WINDOW_HIDDEN | SDL_WINDOW_RESIZABLE;

        Handle = SDL_CreateWindow("Vortice", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1200, 800, flags);
        Id = SDL_GetWindowID(Handle);
        _idLookup.Add(Id, this);

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

    /// <inheritdoc />
    public override bool IsMinimized => _minimized;

    /// <inheritdoc />
    public override Size ClientSize => _clientSize;

    /// <inheritdoc />
    public override SwapChainSurface Surface { get; }

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
                _idLookup.Remove(evt.window.windowID);
                SDL_DestroyWindow(Handle);
                break;
        }
    }

    internal static SDLGameWindow Get(uint windowID)
    {
        return _idLookup[windowID];
    }
}
