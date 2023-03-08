// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Code based on Foster implementation: https://github.com/NoelFB/Foster/blob/master/LICENSE.txt

using Vortice.Platform;

namespace Vortice;

/// <summary>
/// Base module class, used to get callbacks during Application Events.
/// </summary>
public abstract class Module : DisposableObject
{
    /// <summary>
    /// Gets the name of the Module.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the priority of the Module.
    /// </summary>
    /// <remarks>A lower priority is run first</remarks>
    public int Priority { get; }

    /// <summary>
    /// The Application Main Thread ID
    /// </summary>
    public int MainThreadId { get; internal set; }

    /// <summary>
    /// Whether the Module has been Registered by the Application
    /// </summary>
    public bool IsRegistered { get; internal set; }

    /// <summary>
    /// Whether the Module has been Started by the Application
    /// </summary>
    public bool IsStarted { get; internal set; }

    /// <summary>
    /// Creates the Module with the given Priority
    /// </summary>
    protected Module(int priority = 10000, string? name = default)
    {
        Name = name ?? GetType().Name;
        Priority = priority;
    }

    /// <summary>
    /// Called when the Application begins, after the Primary Window is created.
    /// If the Application has already started when the module is Registered, this will be called immediately.
    /// </summary>
    protected internal virtual void Startup() { }

    /// <summary>
    /// Called when the Application shuts down, or the Module is Removed
    /// </summary>
    protected internal virtual void Shutdown() { }

    /// <summary>
    /// Called after the Shutdown method when the Module should be fully Disposed
    /// </summary>
    protected internal virtual void Disposed() { }

    /// <summary>
    /// Called at the start of the frame, before Update or Fixed Update.
    /// </summary>
    protected internal virtual void FrameStart() { }

    /// <summary>
    /// Called every fixed step
    /// </summary>
    protected internal virtual void FixedUpdate() { }

    /// <summary>
    /// Called every variable step
    /// </summary>
    protected internal virtual void Update() { }

    /// <summary>
    /// Called at the end of the frame, after Update and Fixed Update.
    /// </summary>
    protected internal virtual void FrameEnd() { }

    /// <summary>
    /// Called before any rendering
    /// </summary>
    protected internal virtual void BeforeRender() { }

    /// <summary>
    /// Called when a Window is being rendered to, before the Window.OnRender callback
    /// </summary>
    protected internal virtual void BeforeRenderWindow(Window window)
    {
    }

    /// <summary>
    /// Called when a Window is being rendered to, after the Window.OnRender callback
    /// </summary>
    protected internal virtual void AfterRenderWindow(Window window)
    {
    }

    /// <summary>
    /// Called after all rendering
    /// </summary>
    protected internal virtual void AfterRender() { }

}
