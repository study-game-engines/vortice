// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Vortice.Mathematics;
using Vortice.Graphics;
using System.Windows.Forms;

namespace Vortice;

internal unsafe class WinFormsGameView : GameView
{
    private readonly WinFormsGamePlatform _platform;

    public WinFormsGameView(WinFormsGamePlatform platform, Control? control = default)
    {
        _platform = platform;
        if (control != null)
        {
            Control = control!;
        }
        else
        {
            Control = new Form()
            {
                Text = "Alimer",
                MaximizeBox = false,
                FormBorderStyle = FormBorderStyle.Fixed3D,
                StartPosition = FormStartPosition.CenterScreen,
                ClientSize = new System.Drawing.Size(1200, 800)
            };
        }

        Surface = SwapChainSurface.CreateWin32(Control.Handle);
        Control.ClientSizeChanged += OnControlClientSizeChanged;
    }

    public Control Control { get; }

    /// <inheritdoc />
    public override SizeI ClientSize => new(Control.ClientSize.Width, Control.ClientSize.Height);

    /// <inheritdoc />
    public override SwapChainSurface Surface { get; }

    internal void Close()
    {
        if (Control is Form form)
        {
            form.Close();
        }
    }

    private void OnControlClientSizeChanged(object? sender, EventArgs e)
    {
        OnSizeChanged();
    }
}
