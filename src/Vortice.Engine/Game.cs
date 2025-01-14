// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Vortice.Engine;

/// <summary>
/// Defines a Game <see cref="Application"/>.
/// </summary>
public abstract class Game : Application
{
    protected Game(string? name = default)
        : base(name)
    {
    }
}
