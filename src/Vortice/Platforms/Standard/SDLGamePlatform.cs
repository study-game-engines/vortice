// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using static SDL2.SDL;
using static SDL2.SDL.SDL_EventType;
using static SDL2.SDL.SDL_WindowEventID;
//using Vortice.Content;

namespace Vortice;

internal class SDLGamePlatform : GamePlatform
{
    private const int _eventsPerPeep = 64;
    private readonly SDL_Event[] _events = new SDL_Event[_eventsPerPeep];

    private readonly SDLGameWindow _window;
    private bool _exitRequested;

    public SDLGamePlatform()
    {
        // Init SDL2
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_GAMECONTROLLER) != 0)
        {
            SDL_Log($"Unable to initialize SDL: {SDL_GetError()}");
            throw new Exception("");
        }

        View = (_window = new SDLGameWindow(this));
    }

    // <inheritdoc />
    public override GameView View { get; }

    public override void RunMainLoop(Action init)
    {
        init();

        _window.Show();

        while (!_exitRequested)
        {
            PollSDLEvents();
            RunOneFrame();
        }

        SDL_Quit();
    }

    private void PollSDLEvents()
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
        switch (evt.window.windowEvent)
        {
            case SDL_WINDOWEVENT_SIZE_CHANGED:
                //updateWindowSize();
                break;

        }
    }

    private void RunOneFrame()
    {
        Tick();
    }
}
