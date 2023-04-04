// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Code based on Foster implementation: https://github.com/NoelFB/Foster/blob/master/LICENSE.txt

using System.Collections;
using Alimer.Platform;

namespace Alimer;

/// <summary>
/// List of <see cref="Module"/>
/// </summary>
public sealed class ModuleList : IEnumerable<Module>
{
    private readonly Dictionary<Type, Func<Module>> _registeredFactories = new();
    private readonly List<Module?> _modules = new();
    private readonly Dictionary<Type, Module> _modulesByType = new();
    private bool immediateInit;
    private bool immediateStart;

    /// <summary>
    /// Registers a Module
    /// </summary>
    public void Register<T>() where T : Module, new()
    {
        Register(typeof(T), () => new T());
    }

    /// <summary>
    /// Registers a Module
    /// </summary>
    public void Register(Type type, Func<Module> factory)
    {
        if (immediateInit)
        {
            Module module = Instantiate(type, factory);

            if (immediateStart)
                StartupModule(module, true);
        }
        else
        {
            _registeredFactories.Add(type, factory);
        }
    }

    /// <summary>
    /// Registers a Module
    /// </summary>
    private Module Instantiate(Type type, Func<Module> factory)
    {
        Module module = factory();

        // add Module to lookup
        while (type != typeof(Module) && type != typeof(AppModule))
        {
            if (!_modulesByType.ContainsKey(type))
                _modulesByType[type] = module;

            if (type.BaseType == null)
                break;

            type = type.BaseType;
        }

        // insert in order
        int insert = 0;
        while (insert < _modules.Count && (_modules[insert]?.Priority ?? int.MinValue) <= module.Priority)
        {
            insert++;
        }
        _modules.Insert(insert, module);

        // registered
        module.IsRegistered = true;
        module.MainThreadId = Environment.CurrentManagedThreadId;
        return module;
    }

    /// <summary>
    /// Removes a Module
    /// Note: Removing core modules (such as System) will make everything break
    /// </summary>
    public void Remove(Module module)
    {
        if (!module.IsRegistered)
            throw new Exception("Module is not already registered");

        module.Shutdown();
        module.Disposed();

        var index = _modules.IndexOf(module);
        _modules[index] = null;

        var type = module.GetType();
        while (type != typeof(Module))
        {
            if (_modulesByType[type] == module)
                _modulesByType.Remove(type);

            if (type.BaseType == null)
                break;

            type = type.BaseType;
        }

        module.IsRegistered = false;
    }

    /// <summary>
    /// Tries to get the first <see cref="Module"/> of the given type.
    /// </summary>
    public bool TryGet<T>(out T? module) where T : Module
    {
        if (_modulesByType.TryGetValue(typeof(T), out Module? m))
        {
            module = (T)m;
            return true;
        }

        module = null;
        return false;
    }

    /// <summary>
    /// Tries to get the First Module of the given type
    /// </summary>
    public bool TryGet(Type type, out Module? module)
    {
        if (_modulesByType.TryGetValue(type, out var m))
        {
            module = m;
            return true;
        }

        module = null;
        return false;
    }

    /// <summary>
    /// Gets the First Module of the given type, if it exists, or throws an Exception
    /// </summary>
    public T Get<T>() where T : Module
    {
        if (!_modulesByType.TryGetValue(typeof(T), out var module))
            throw new Exception($"App is does not have a {typeof(T).Name} Module registered");

        return (T)module;
    }

    /// <summary>
    /// Gets the First Module of the given type, if it exists, or throws an Exception
    /// </summary>
    public Module Get(Type type)
    {
        if (!_modulesByType.TryGetValue(type, out var module))
            throw new Exception($"App is does not have a {type.Name} Module registered");

        return module;
    }

    /// <summary>
    /// Checks if a Module of the given type exists
    /// </summary>
    public bool Has<T>() where T : Module
    {
        return _modulesByType.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Checks if a Module of the given type exists
    /// </summary>
    public bool Has(Type type)
    {
        return _modulesByType.ContainsKey(type);
    }

    internal void ApplicationStarted()
    {
        // Create Application Modules.
        List<Type> toRemove = new();
        foreach (KeyValuePair<Type, Func<Module>> pair in _registeredFactories)
        {
            if (typeof(AppModule).IsAssignableFrom(pair.Key))
            {
                Instantiate(pair.Key, pair.Value);
                toRemove.Add(pair.Key);
            }
        }

        foreach (Type type in toRemove)
        {
            _registeredFactories.Remove(type);
        }

        for (int i = 0; i < _modules.Count; i++)
        {
            if (_modules[i] is AppModule module)
                module.ApplicationStarted();
        }
    }

    internal void FirstWindowCreated()
    {
        for (int i = 0; i < _modules.Count; i++)
        {
            if (_modules[i] is AppModule module)
            {
                module.FirstWindowCreated();
            }
        }
    }

    internal void Startup()
    {
        // this method is a little strange because it makes sure all App Modules have
        // had their Startup methods called BEFORE instantiating normal Modules
        // Thus it has to iterate over modules and call Startup twice

        // run startup on on App Modules
        for (int i = 0; i < _modules.Count; i++)
        {
            StartupModule(_modules[i], false);
        }

        // Instantiate remaining modules that are registered
        foreach (KeyValuePair<Type, Func<Module>> pair in _registeredFactories)
        {
            Instantiate(pair.Key, pair.Value);
        }

        // further modules will be instantiated immediately
        immediateInit = true;

        // call started on all modules
        for (int i = 0; i < _modules.Count; i++)
        {
            StartupModule(_modules[i], true);
        }

        // further modules will have Startup called immediately
        immediateStart = true;
    }

    private static void StartupModule(Module? module, bool callAppMethods)
    {
        if (module != null && !module.IsStarted)
        {
            module.IsStarted = true;

            if (module is AppModule appModule && callAppMethods)
            {
                appModule.ApplicationStarted();
                appModule.FirstWindowCreated();
            }

            module.Startup();
        }
    }

    internal void Shutdown()
    {
        for (int i = _modules.Count - 1; i >= 0; i--)
            _modules[i]?.Shutdown();

        for (int i = _modules.Count - 1; i >= 0; i--)
            _modules[i]?.Disposed();

        _registeredFactories.Clear();
        _modules.Clear();
        _modules.Clear();
    }

    internal void FrameStart()
    {
        // remove null module entries
        int toRemove;
        while ((toRemove = _modules.IndexOf(null)) >= 0)
            _modules.RemoveAt(toRemove);

        for (int i = 0; i < _modules.Count; i++)
            _modules[i]?.FrameStart();
    }

    internal void FixedUpdate()
    {
        for (int i = 0; i < _modules.Count; i++)
            _modules[i]?.FixedUpdate();
    }

    internal void Update()
    {
        for (int i = 0; i < _modules.Count; i++)
            _modules[i]?.Update();
    }

    internal void FrameEnd()
    {
        for (int i = 0; i < _modules.Count; i++)
            _modules[i]?.FrameEnd();
    }

    internal void BeforeRender()
    {
        for (int i = 0; i < _modules.Count; i++)
            _modules[i]?.BeforeRender();
    }

    internal void AfterRender()
    {
        for (int i = 0; i < _modules.Count; i++)
            _modules[i]?.AfterRender();
    }

    internal void BeforeRenderWindow(Window window)
    {
        for (int i = 0; i < _modules.Count; i++)
            _modules[i]?.BeforeRenderWindow(window);
    }
    
    internal void AfterRenderWindow(Window window)
    {
        for (int i = 0; i < _modules.Count; i++)
            _modules[i]?.AfterRenderWindow(window);
    }

    public IEnumerator<Module> GetEnumerator()
    {
        for (int i = 0; i < _modules.Count; i++)
        {
            Module? module = _modules[i];
            if (module is not null)
                yield return module;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < _modules.Count; i++)
        {
            Module? module = _modules[i];
            if (module is not null)
                yield return module;
        }
    }
}
