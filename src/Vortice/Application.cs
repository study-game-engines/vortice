// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Vortice.Graphics;
using Vortice.Input;
using Vortice.Audio;
using Vortice.Platform;

namespace Vortice;

public abstract class Application : DisposableObject, IGame
{
    private readonly GamePlatform _platform;
    private readonly object _tickLock = new();
    private readonly Stopwatch _stopwatch = new();
    private bool _isExiting;

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

    public bool IsRunning { get; private set; }
    public bool IsExiting { get; private set; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// A list of all registered modules.
    /// </summary>
    public ModuleList Modules { get; } = new ModuleList();

    /// <summary>
    /// Gets the Audio module.
    /// </summary>
    public AudioModule Audio => Modules.Get<AudioModule>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Game" /> class.
    /// </summary>
    /// <param name="platform">The optional <see cref="GamePlatform"/> to handle platform logic..</param>
    protected Application(string? name = default, GamePlatform ? platform = null)
    {
        Name = name ?? GetType().Name;
        Version? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        if (assemblyVersion is not null)
        {
            Version = assemblyVersion;
        }

        _platform = platform ?? GamePlatform.CreateDefault();

        Log.Info($"Version: {Version}");
        Log.Info($"Platform: {RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture})");
        Log.Info($"Framework: {RuntimeInformation.FrameworkDescription}");

        _platform.ConfigureModules(Modules);
        ConfigureModules();

        GraphicsDeviceDescription deviceDescription = new()
        {
            //PreferredBackend = GraphicsBackend.Vulkan,
#if DEBUG
            ValidationMode = ValidationMode.Enabled,
#endif
        };
        GraphicsDevice = GraphicsDevice.CreateDefault(deviceDescription);

        // Get optional services.
        //AudioDevice? audioDevice = _serviceProvider.GetService<AudioDevice>();
        //AudioDevice = audioDevice ?? AudioDevice.CreateDefault();
    }

    public GameTime Time { get; } = new GameTime();

    public Window MainWindow => _platform.MainWindow;

    public InputManager Input => Modules.Get<InputManager>();

    public GraphicsDevice GraphicsDevice { get; }

    //public AudioDevice AudioDevice { get; }

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

        Modules.Register<InputManager>();
    }

    public void Run()
    {
        if (IsRunning)
        {
            throw new InvalidOperationException("This game is already running.");
        }

        IsRunning = true;

        _platform.TickRequested += OnTickRequested;
        _platform.RunMainLoop(InitializeBeforeRun);
    }

    protected virtual void Initialize()
    {
    }

    private void InitializeBeforeRun()
    {
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
            if (_isExiting)
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
        if (_isExiting && IsRunning)
        {
            EndRun();

            _stopwatch.Stop();

            IsRunning = false;
            _isExiting = false;
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
