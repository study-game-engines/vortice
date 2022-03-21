// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Reflection;
using System.Runtime.InteropServices;
using Vortice.Mathematics;
using Vortice.Graphics;

namespace Vortice;

internal unsafe class WinFormsGameView : GameView
{
    private readonly Control _control;

    public WinFormsGameView(Control control)
    {
        _control = control;

        Surface = GraphicsSurface.CreateWin32(GetModuleHandleW(lpModuleName: null), _control.Handle);

        _control.ClientSizeChanged += OnControlClientSizeChanged;
    }

    /// <inheritdoc />
    public override SizeI ClientSize => new SizeI(_control.ClientSize.Width, _control.ClientSize.Height);

    /// <inheritdoc />
    public override GraphicsSurface Surface { get; }

    private void OnControlClientSizeChanged(object? sender, EventArgs e)
    {
        OnSizeChanged();
    }

    [DllImport("kernel32", ExactSpelling = true, SetLastError = true)]
    private static extern unsafe IntPtr GetModuleHandleW(ushort* lpModuleName);
}
