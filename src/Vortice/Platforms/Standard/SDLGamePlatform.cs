// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using static Alimer.Bindings.SDL.SDL;
using static Alimer.Bindings.SDL.SDL.SDL_EventType;
using static Alimer.Bindings.SDL.SDL.SDL_LogPriority;
//using static SDL2.SDL.SDL_WindowEventID;

namespace Vortice;

internal unsafe class SDLGamePlatform : GamePlatform
{
    private const int _eventsPerPeep = 64;
    private readonly SDL_Event* _events = (SDL_Event*)NativeMemory.Alloc(_eventsPerPeep, (nuint)sizeof(SDL_Event));

    private readonly SDLGameWindow _window;
    private bool _exitRequested;

    public SDLGamePlatform()
    {
        SDL_LogSetAllPriority(SDL_LOG_PRIORITY_VERBOSE);
        SDL_LogSetOutputFunction(&OnLog, IntPtr.Zero);

        SDL_GetVersion(out SDL_version version);
        Log.Info($"SDL v{version.major}.{version.minor}.{version.patch}");

        // DPI aware on Windows
        SDL_SetHint(SDL_HINT_WINDOWS_DPI_AWARENESS, "permonitorv2");
        SDL_SetHint(SDL_HINT_WINDOWS_DPI_SCALING, true);

        // Init SDL2
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_GAMECONTROLLER) != 0)
        {
            Log.Error($"Unable to initialize SDL: {SDL_GetError()}");
            throw new Exception("");
        }

        View = (_window = new SDLGameWindow());
    }

    // <inheritdoc />
    public override GameView View { get; }

    public override void RunMainLoop(Action init)
    {
        init();

        _window.Show();

        while (!_exitRequested)
        {
            PollEvents();
            OnTick();
        }

        SDL_Quit();
    }

    private void PollEvents()
    {
        SDL_PumpEvents();
        int eventsRead;

        do
        {
            eventsRead = SDL_PeepEvents(_events, _eventsPerPeep, SDL_eventaction.SDL_GETEVENT, SDL_EventType.SDL_FIRSTEVENT, SDL_EventType.SDL_LASTEVENT);
            for (int i = 0; i < eventsRead; i++)
            {
                HandleSDLEvent(_events[i]);
            }
        } while (eventsRead == _eventsPerPeep);
    }

    private void HandleSDLEvent(SDL_Event evt)
    {
        switch (evt.type)
        {
            case SDL_QUIT:
            case SDL_APP_TERMINATING:
                _exitRequested = true;
                break;

            case SDL_WINDOWEVENT:
                HandleWindowEvent(evt);
                break;
        }
    }

    private void HandleWindowEvent(in SDL_Event evt)
    {
        var window = SDLGameWindow.Get(evt.window.windowID);
        window.HandleEvent(evt);
    }

    [UnmanagedCallersOnly]
    private static unsafe void OnLog(nint userData, int category, SDL_LogPriority priority, sbyte* messagePtr)
    {
        string message = new(messagePtr);
        Log.Info($"SDL: {message}");
    }
}
