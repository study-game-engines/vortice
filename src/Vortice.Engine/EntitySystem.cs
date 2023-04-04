// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Engine;

/// <summary>
/// An <see cref="Entity"/> game system.
/// </summary>
public abstract class EntitySystem : IGameSystem
{
    protected EntitySystem()
    {
    }

    protected EntitySystem(Type? mainComponentType)
    {
        MainComponentType = mainComponentType;
    }

    protected EntitySystem(Type? mainComponentType, params Type[] requiredComponentTypes)
    {
        MainComponentType = mainComponentType;

        foreach (Type type in requiredComponentTypes)
        {
            RequiredComponentTypes.Add(type);
        }
    }

    public Type? MainComponentType { get; }
    public IList<Type> RequiredComponentTypes { get; } = new List<Type>();

    public virtual void Update(GameTime gameTime)
    {
    }

    public virtual void BeginDraw()
    {
    }

    public virtual void Draw(GameTime gameTime)
    {
    }

    public virtual void EndDraw()
    {
    }
}

public abstract class EntitySystem<TComponent> : EntitySystem where TComponent : EntityComponent
{
    protected EntitySystem()
        : base(typeof(TComponent))
    {
    }

    protected EntitySystem(params Type[] requiredComponentTypes)
        : base(typeof(TComponent), requiredComponentTypes)
    {
    }

}
