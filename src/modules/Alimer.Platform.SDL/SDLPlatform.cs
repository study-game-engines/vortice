// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using Alimer.Input;
using static SDL2.SDL;
using static SDL2.SDL.SDL_LogPriority;
using static SDL2.SDL.SDL_EventType;

namespace Alimer.Platform.SDL;

internal unsafe class SDLPlatform : AppPlatform
{
    private const string SDL_HINT_WINDOWS_DPI_AWARENESS = "SDL_WINDOWS_DPI_AWARENESS";
    private const string SDL_HINT_WINDOWS_DPI_SCALING = "SDL_WINDOWS_DPI_SCALING";

    private const int _eventsPerPeep = 64;
    private readonly SDL_Event[] _events = new SDL_Event[_eventsPerPeep];
    //private readonly SDL_Event* _events = (SDL_Event*)NativeMemory.Alloc(_eventsPerPeep, (nuint)sizeof(SDL_Event));

    private readonly SDLInput _input;
    private readonly SDLWindow _window;
    private Dictionary<uint, SDLWindow> _idLookup = new();
    private bool _exitRequested;

    public override string ApiName { get; }

    public override Version ApiVersion { get; }

    // <inheritdoc />
    public override InputManager Input => _input;

    // <inheritdoc />
    public override bool SupportsMultipleWindows => true;

    // <inheritdoc />
    public override Window MainWindow { get; }

    public SDLPlatform()
    {
        ApiName = "SDL";

        SDL_LogSetAllPriority(SDL_LOG_PRIORITY_VERBOSE);
        //SDL_LogSetOutputFunction(&OnLog, IntPtr.Zero);

        SDL_GetVersion(out SDL_version version);
        ApiVersion = new Version(version.major, version.minor, version.patch);

        // DPI aware on Windows
        SDL_SetHint(SDL_HINT_WINDOWS_DPI_AWARENESS, "permonitorv2");
        SDL_SetHint(SDL_HINT_WINDOWS_DPI_SCALING, "1");

        // Init SDL2
        if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_TIMER | SDL_INIT_GAMECONTROLLER) != 0)
        {
            Log.Error($"Unable to initialize SDL: {SDL_GetError()}");
            throw new Exception("");
        }

        _input = new SDLInput(this);
        MainWindow = (_window = new SDLWindow(this));
        _idLookup.Add(_window.Id, _window);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="SDLPlatform" /> class.
    /// </summary>
    ~SDLPlatform() => Dispose(disposing: false);

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            SDL_Quit();
        }
    }

    /// <inheritdoc />
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
        if (_idLookup.TryGetValue(evt.window.windowID, out SDLWindow? window))
        {
            window.HandleEvent(evt);
        }
    }

    internal void WindowClosed(uint windowID)
    {
        _idLookup.Remove(windowID);
    }

    //[UnmanagedCallersOnly]
    //private static unsafe void OnLog(nint userData, int category, SDL_LogPriority priority, sbyte* messagePtr)
    //{
    //    string message = new(messagePtr);
    //    Log.Info($"SDL: {message}");
    //}
}
