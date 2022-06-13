// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using SharpGen.Runtime;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Vortice.Graphics.D3DCommon;
using Vortice.Mathematics;

namespace Vortice.Graphics.D3D11;

internal sealed class D3D11CommandBuffer : CommandBuffer, IDisposable
{
    private readonly List<D3D11SwapChain> _swapChains = new();

    public D3D11CommandBuffer(D3D11GraphicsDevice device)
        : base(device)
    {
        Context = device.NativeDevice.CreateDeferredContext1();
        UserDefinedAnnotation = Context.QueryInterface<ID3DUserDefinedAnnotation>();
    }

    public ID3D11DeviceContext1 Context { get; }
    public ID3DUserDefinedAnnotation UserDefinedAnnotation { get; }
    internal ID3D11CommandList? CommandList { get; private set; }
    public bool IsRecording { get; internal set; }
    public bool HasLabel { get; internal set; }

    // <summary>
    /// Finalizes an instance of the <see cref="D3D11Texture" /> class.
    /// </summary>
    ~D3D11CommandBuffer() => Dispose(disposing: false);

    /// <inheritdoc />
    public void Dispose() => Dispose(true);

    /// <inheritdoc />
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            CommandList?.Dispose();
            UserDefinedAnnotation.Dispose();
            Context.Dispose();
        }
    }

    public void Reset()
    {
        CommandList?.Dispose();
        CommandList = default;

        _swapChains.Clear();
    }

    public void End()
    {
        // Serialize the commands into a command list 
        CommandList = Context.FinishCommandList(false);
    }

    /// <inheritdoc />
    public override void PushDebugGroup(string groupLabel)
    {
        _ = UserDefinedAnnotation.BeginEvent(groupLabel);
    }


    /// <inheritdoc />
    public override void PopDebugGroup()
    {
        _ = UserDefinedAnnotation.EndEvent();
    }

    /// <inheritdoc />
    public override void InsertDebugMarker(string debugLabel)
    {
        UserDefinedAnnotation.SetMarker(debugLabel);
    }

    /// <inheritdoc />
    public override Texture? AcquireSwapchainTexture(SwapChain swapChain)
    {
        D3D11SwapChain d3dSwapChain = (D3D11SwapChain)swapChain;

        if (d3dSwapChain.NeedResize())
        {

        }

        _swapChains.Add(d3dSwapChain);
        return d3dSwapChain.BackbufferTexture;
    }

    public void PresentSwapChains()
    {
        for (int i = 0; i < _swapChains.Count; ++i)
        {
            D3D11SwapChain swapChain = _swapChains[i];

            PresentFlags presentFlags = 0;
            bool fullscreen = swapChain.Handle.IsFullscreen;

            if (swapChain.SyncInterval == 0 && !fullscreen)
            {
                presentFlags = PresentFlags.AllowTearing;
            }

            Result result = swapChain.Handle.Present(swapChain.SyncInterval, presentFlags);

            // If the device was reset we must completely reinitialize the renderer.
            if (result == DXGI.ResultCode.DeviceRemoved || result == DXGI.ResultCode.DeviceReset)
            {
#if DEBUG
                //Result logResult = (result == DXGI.ResultCode.DeviceRemoved) ? Device.d3d.DeviceRemovedReason : result;
                //System.Diagnostics.Debug.WriteLine($"Device Lost on Present: Reason code {logResult}");
#endif
                //HandleDeviceLost();
            }

            _swapChains.Clear();
        }
    }
}
