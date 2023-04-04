// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using Win32;
using Win32.Graphics.Direct3D11;
using Win32.Graphics.Dxgi;
using static Win32.Apis;
using static Win32.Graphics.Dxgi.Apis;

namespace Vortice.Graphics.D3D11;

internal unsafe class D3D11CommandBuffer : CommandBuffer, IDisposable
{
    private readonly ComPtr<ID3D11DeviceContext1> _handle = default;
    private readonly ComPtr<ID3DUserDefinedAnnotation> _userDefinedAnnotation = default;
    private readonly ComPtr<ID3D11CommandList> _commandList;
    private readonly List<D3D11SwapChain> _swapChains = new();
    private bool _activeEncoder = false;
    private readonly D3D11RenderPassEncoder _renderPass;
    private readonly D3D11ComputePassEncoder _computePass;

    public D3D11CommandBuffer(D3D11GraphicsDevice device)
        : base(device)
    {
        ThrowIfFailed(device.NativeDevice->CreateDeferredContext1(0u, _handle.GetAddressOf()));
        _handle.CopyTo(_userDefinedAnnotation.GetAddressOf());

        _renderPass = new D3D11RenderPassEncoder(this);
        _computePass = new D3D11ComputePassEncoder(this);
    }

    public ID3D11DeviceContext1* Handle => _handle;
    public ID3DUserDefinedAnnotation* UserDefinedAnnotation => _userDefinedAnnotation;
    public ID3D11CommandList* CommandList => _commandList;
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
            _commandList.Dispose();
            _userDefinedAnnotation.Dispose();
            _handle.Dispose();
        }
    }

    public void Reset()
    {
        _commandList.Dispose();
        _swapChains.Clear();
        _activeEncoder = false;
    }

    public void End()
    {
        if (_activeEncoder)
        {
            throw new InvalidOperationException("Cannot submit with active command encoders");
        }

        // Serialize the commands into a command list 
        ThrowIfFailed(_handle.Get()->FinishCommandList(false, _commandList.GetAddressOf()));
    }

    /// <inheritdoc />
    public override void PushDebugGroup(string groupLabel)
    {
        fixed (char* groupLabelPtr = groupLabel)
        {
            _ = _userDefinedAnnotation.Get()->BeginEvent((ushort*)groupLabelPtr);
        }
    }


    /// <inheritdoc />
    public override void PopDebugGroup()
    {
        _ = _userDefinedAnnotation.Get()->EndEvent();
    }

    /// <inheritdoc />
    public override void InsertDebugMarker(string debugLabel)
    {
        fixed (char* debugLabelPtr = debugLabel)
        {
            _userDefinedAnnotation.Get()->SetMarker((ushort*)debugLabelPtr);
        }
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

    public override RenderPassEncoder BeginRenderPass(in RenderPassDescription renderPass)
    {
        _renderPass.Begin(renderPass);
        _activeEncoder = true;
        return _renderPass;
    }

    public override ComputePassEncoder BeginComputePass()
    {
        _computePass.Begin();
        _activeEncoder = true;
        return _computePass;
    }

    public void EndEncoder()
    {
        _activeEncoder = false;
    }

    public void PresentSwapChains()
    {
        for (int i = 0; i < _swapChains.Count; ++i)
        {
            D3D11SwapChain swapChain = _swapChains[i];

            Bool32 fullscreen = false;
            swapChain.Handle->GetFullscreenState(&fullscreen, null);

            PresentFlags presentFlags = 0;
            if (swapChain.SyncInterval == 0 && !fullscreen)
            {
                presentFlags = PresentFlags.AllowTearing;
            }

            HResult result = swapChain.Handle->Present(swapChain.SyncInterval, presentFlags);

            // If the device was reset we must completely reinitialize the renderer.
            if (result == DXGI_ERROR_DEVICE_REMOVED || result == DXGI_ERROR_DEVICE_RESET)
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
