// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Vortice.Audio;
using Vortice.Graphics;
using Vortice.Input;
using Vortice.Platform;

namespace Vortice;

public abstract class Application : DisposableObject, IGame
{
    private readonly object _tickLock = new();
    private readonly Stopwatch _stopwatch = new();

    public event EventHandler<EventArgs>? Activated;
    public event EventHandler<EventArgs>? Deactivated;

    /// <summary>
    /// Gets the Application name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the Application Version.
    /// </summary>
    public virtual Version Version { get; } = new Version(0, 1, 0);

    /// <summary>
    /// A list of all registered modules.
    /// </summary>
    public ModuleList Modules { get; } = new ModuleList();

    /// <summary>
    /// Gets the platform module.
    /// </summary>
    public AppPlatform Platform => Modules.Get<AppPlatform>();

    /// <summary>
    /// Gets the main window, automatically created by the <see cref="AppPlatform"/> module.
    /// </summary>
    public Window MainWindow => Platform.MainWindow;

    /// <summary>
    /// Gets the system input, crated by the <see cref="AppPlatform"/> module.
    /// </summary>
    public InputManager Input => Platform.Input;

    /// <summary>
    /// Gets the <see cref="AudioModule"/> instance.
    /// </summary>
    public AudioModule? Audio { get; private set; }

    public bool IsRunning { get; private set; }
    public bool IsExiting { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Application" /> class.
    /// </summary>
    /// <param name="name">The optional name of the application.</param>
    protected Application(string? name = default)
    {
        Name = name ?? GetType().Name;
        Version? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        if (assemblyVersion is not null)
        {
            Version = assemblyVersion;
        }

        Log.Info($"Version: {Version}");
        Log.Info($"Platform: {RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})");
        Log.Info($"Framework: {RuntimeInformation.FrameworkDescription}");

        ConfigureModules();

        GraphicsDeviceDescription deviceDescription = new()
        {
            //PreferredBackend = GraphicsBackend.Vulkan,
#if DEBUG
            ValidationMode = ValidationMode.Enabled,
#endif
        };
        GraphicsDevice = GraphicsDevice.CreateDefault(deviceDescription);
    }

    public GameTime Time { get; } = new GameTime();


    public GraphicsDevice GraphicsDevice { get; }


    public IList<IGameSystem> GameSystems { get; } = new List<IGameSystem>();

    /// <inheritdoc />
    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            GraphicsDevice.WaitIdle();
            MainWindow.SwapChain?.Dispose();
            GraphicsDevice.Dispose();

            //AudioDevice?.Dispose();
        }
    }

    protected virtual void ConfigureModules()
    {
        //services.AddSingleton<IGame>(this);
        //services.AddSingleton<IContentManager, ContentManager>();
    }

    public void Run(Action? callback = null)
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("This application is already running.");
        }

        if (IsExiting)
        {
            throw new InvalidOperationException("App is still exiting");
        }

#if DEBUG
        Launch();
#else
        try
        {
            Launch();
        }
        catch (Exception e)
        {
            string path = AppPlatform.DefaultUserDirectory(Name);
            if (Modules.Has<AppPlatform>())
            {
                path = Modules.Get<AppPlatform>().UserDirectory(Name);
            }

            Log.Error(e.Message);
            // TODO - this call was broken with the logging updates!
            //Log.AppendToFile(Name, Path.Combine(path, "ErrorLog.txt"));
            throw;
        }
#endif
        void Launch()
        {
            // init modules
            Modules.ApplicationStarted();

            if (!Modules.Has<AppPlatform>())
            {
                throw new Exception($"App requires a {nameof(AppPlatform)} Module to be registered before it can Start");
            }


            // Try to get optional modules.
            if (Modules.TryGet(out AudioModule? audioModule))
            {
                Audio = audioModule;
            }

            // Startup application
            Platform.RunMainLoop(() =>
            {
                Platform.TickRequested += OnTickRequested;
                InitializeBeforeRun();
                callback?.Invoke();
            });
        }

        IsRunning = true;
    }

    protected virtual void Initialize()
    {
    }

    private void InitializeBeforeRun()
    {
        // Create our primary Window
        //s_primaryWindow = new Window(System, title, width, height, flags);
        //Modules.FirstWindowCreated();

        IsRunning = true;
        Modules.Startup();
        MainWindow.CreateSwapChain(GraphicsDevice);

        Initialize();

        _stopwatch.Start();
        Time.Update(_stopwatch.Elapsed, TimeSpan.Zero);

        BeginRun();
    }

    private void OnTickRequested(object? sender, EventArgs e)
    {
        Tick();
    }

    public void Tick()
    {
        lock (_tickLock)
        {
            if (IsExiting)
            {
                CheckEndRun();
                return;
            }

            try
            {
                TimeSpan elapsedTime = _stopwatch.Elapsed - Time.Total;
                Time.Update(_stopwatch.Elapsed, elapsedTime);

                Update(Time);

                BeginDraw();
                Draw(Time);
            }
            finally
            {
                EndDraw();

                CheckEndRun();
            }
        }
    }

    public virtual void BeginRun()
    {
    }

    public virtual void EndRun()
    {
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (IGameSystem system in GameSystems)
        {
            system.Update(gameTime);
        }
    }

    public virtual void BeginDraw()
    {
        foreach (IGameSystem system in GameSystems)
        {
            system.BeginDraw();
        }
    }

    public virtual void Draw(GameTime gameTime)
    {
        foreach (IGameSystem system in GameSystems)
        {
            system.Draw(gameTime);
        }

        //GraphicsDevice.Frame();
    }

    public virtual void EndDraw()
    {
        foreach (IGameSystem system in GameSystems)
        {
            system.EndDraw();
        }

        GraphicsDevice.CommitFrame();
    }

    /// <summary>
    /// Raises the <see cref="Activated"/> event. Override this method to add code to handle when the game gains focus.
    /// </summary>
    /// <param name="sender">The Game.</param>
    /// <param name="args">Arguments for the Activated event.</param>
    protected virtual void OnActivated(object sender, EventArgs args)
    {
        Activated?.Invoke(this, args);
    }

    /// <summary>
    /// Raises the <see cref="Deactivated"/> event. Override this method to add code to handle when the game loses focus.
    /// </summary>
    /// <param name="sender">The Game.</param>
    /// <param name="args">Arguments for the Deactivated event.</param>
    protected virtual void OnDeactivated(object sender, EventArgs args)
    {
        Deactivated?.Invoke(this, args);
    }

    private void CheckEndRun()
    {
        if (IsExiting && IsRunning)
        {
            EndRun();

            _stopwatch.Stop();

            IsRunning = false;
            IsExiting = false;
        }
    }

    #region GamePlatform Events
    private void GamePlatform_Activated(object? sender, EventArgs e)
    {
        if (!IsActive)
        {
            IsActive = true;
            OnActivated(this, EventArgs.Empty);
        }
    }

    private void GamePlatform_Deactivated(object? sender, EventArgs e)
    {
        if (IsActive)
        {
            IsActive = false;
            OnDeactivated(this, EventArgs.Empty);
        }
    }
    #endregion
}
