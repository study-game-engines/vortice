// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

namespace Alimer.Graphics;

public enum TextureViewDimension
{
    Undefined,
    /// <summary>
    /// A one-dimensional TextureView.
    /// </summary>
    View1D,
    /// <summary>
    /// A one-dimensional array TextureView.
    /// </summary>
    View1DArray,
    /// <summary>
    /// A two-dimensional TextureView.
    /// </summary>
    View2D,
    /// <summary>
    /// A two-dimensional array TextureView.
    /// </summary>
    View2DArray,
    /// <summary>
    /// A three-dimensional TextureView.
    /// </summary>
    View3D,
    /// <summary>
    /// A cubemap TextureView.
    /// </summary>
    ViewCube,
    /// <summary>
    /// A cubemap arraya TextureView.
    /// </summary>
    ViewCubeArray,
}
