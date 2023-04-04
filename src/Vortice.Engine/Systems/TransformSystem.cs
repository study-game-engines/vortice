// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Engine;

public sealed class TransformSystem : EntitySystem<TransformComponent>
{
    private readonly HashSet<TransformComponent> _transformationRoots = new HashSet<TransformComponent>();

    public TransformSystem()
    {
    }

    public override void BeginDraw()
    {
        UpdateTransformations(_transformationRoots);
    }

    private void UpdateTransformations(IEnumerable<TransformComponent> transformationRoots)
    {
        foreach (TransformComponent transformComponent in transformationRoots)
        {
            UpdateTransformationsRecursive(transformComponent);
        }
    }

    private void UpdateTransformationsRecursive(TransformComponent transformComponent)
    {
        transformComponent.UpdateLocalMatrix();
        transformComponent.UpdateWorldMatrixInternal(false);

        //foreach (TransformComponent child in transformComponent)
        //{
        //    UpdateTransformationsRecursive(child);
        //}
    }
}
