// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Code based on Foster implementation: https://github.com/NoelFB/Foster/blob/master/LICENSE.txt

namespace Vortice;

/// <summary>
/// A core <see cref="Application"/> ,odule that is created earlier and has specific Application callbacks.
/// </summary>
public abstract class AppModule : Module
{
    public abstract string ApiName { get; }
    public abstract Version ApiVersion { get; }

    protected AppModule(int priority = 10000)
        : base(priority)
    {

    }

    /// <summary>
    /// Called when Application is starting, before the Primary Window is created.
    /// </summary>
    protected internal virtual void ApplicationStarted()
    {
    }

    /// <summary>
    /// Called when the Module is created, before Startup but after the first Window is created
    /// </summary>
    protected internal virtual void FirstWindowCreated()
    {
    }
}
