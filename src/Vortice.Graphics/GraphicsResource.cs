// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Microsoft.Toolkit.Diagnostics;

namespace Vortice.Graphics;

public abstract class GraphicsResource : DisposableObject
{
    protected string? _label;

    protected GraphicsResource(GraphicsDevice device, IntPtr handle, string? label = default)
    {
        Guard.IsNotNull(device, nameof(device));

        Device = device;
        Handle = handle;
        _label = label;
    }

    /// <summary>
    /// Get the <see cref="GraphicsDevice"/> object that created the resource.
    /// </summary>
    public GraphicsDevice Device { get; }

    public IntPtr Handle { get; protected set; }

    /// <summary>
    /// Gets or set the label that identifies the resource.
    /// </summary>
    public string? Label
    {
        get => _label;
        set
        {
            _label = value;
            OnLabelChanged();
        }
    }

    protected virtual void OnLabelChanged()
    {
    }
}
