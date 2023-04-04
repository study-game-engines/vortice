// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Numerics;
using System.Runtime.Serialization;
using JoltPhysicsSharp;
using Vortice.Engine;

namespace Vortice.Physics;

[DefaultEntitySystem(typeof(PhysicsSystem))]
public abstract class PhysicsComponent : EntityComponent
{
    private Body? _joltBody;
}
