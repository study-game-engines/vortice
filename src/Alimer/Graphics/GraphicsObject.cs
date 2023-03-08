// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using CommunityToolkit.Diagnostics;

namespace Alimer.Graphics;

/// <summary>
/// An base graphics object that implements <see cref="IDisposable"/> pattern.
/// </summary>
public abstract class GraphicsObject : DisposableObject
{
    protected string _label;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsObject" /> class.
    /// </summary>
    /// <param name="label">The label of the object or <c>null</c> to use <see cref="MemberInfo.Name" />.</param>
    protected GraphicsObject(string? label = default)
    {
        _label = label ?? GetType().Name;
    }

    /// <summary>
    /// Gets or set the label that identifies the resource.
    /// </summary>
    public string Label
    {
        get => _label;
        set
        {
            _label = value ?? GetType().Name;
            OnLabelChanged(_label);
        }
    }

    /// <inheritdoc />
    public override string ToString() => _label;

    protected virtual void OnLabelChanged(string newLabel)
    {
    }
}
