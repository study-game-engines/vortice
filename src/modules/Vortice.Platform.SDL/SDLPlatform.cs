// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.InteropServices;
using Vortice.Input;
using static SDL2.SDL;
using static SDL2.SDL.SDL_EventType;
using static SDL2.SDL.SDL_LogPriority;

namespace Vortice.Platform.SDL;

public static class SDLPlatformExtensions
{
    public static ModuleList UseSDL(this ModuleList builder)
    {
        builder.Register<SDLPlatform>();
        return builder;
    }
}

internal unsafe class SDLPlatform : AppPlatform
{
    private const string SDL_HINT_WINDOWS_DPI_AWARENESS = "SDL_WINDOWS_DPI_AWARENESS";
    private const string SDL_HINT_WINDOWS_DPI_SCALING = "SDL_WINDOWS_DPI_SCALING";

    private const int _eventsPerPeep = 64;
    private unsafe readonly SDL_Event* _events = (SDL_Event*)NativeMemory.Alloc(_eventsPerPeep, (nuint)sizeof(SDL_Event));

    public override string ApiName { get; }

    public override Version ApiVersion { get; }

    // <inheritdoc />
    public override bool SupportsMultipleWindows => true;

    // <inheritdoc />
    public override Window MainWindow { get; }

    // <inheritdoc />
    public override InputManager Input => throw new NotImplementedException();

    public SDLPlatform()
    {
        ApiName = "SDL2";

        SDL_LogSetAllPriority(SDL_LOG_PRIORITY_VERBOSE);
        //SDL_LogSetOutputFunction(&OnLog, IntPtr.Zero);

        SDL_GetVersion(out SDL_version version);
        Log.Info($"SDL v{version.major}.{version.minor}.{version.patch}");
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

    }

    //[UnmanagedCallersOnly]
    //private static unsafe void OnLog(nint userData, int category, SDL_LogPriority priority, sbyte* messagePtr)
    //{
    //    string message = new(messagePtr);
    //    Log.Info($"SDL: {message}");
    //}
}
