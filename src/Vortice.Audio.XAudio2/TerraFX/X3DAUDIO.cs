// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/x3daudio.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using TerraFX.Interop.Windows;

namespace TerraFX.Interop.DirectX;

[StructLayout(LayoutKind.Explicit, Size = X3DAUDIO.X3DAUDIO_HANDLE_BYTESIZE)]
internal readonly struct X3DAUDIO_HANDLE
{
}

internal static unsafe partial class X3DAUDIO
{
    [NativeTypeName("#define X3DAUDIO_HANDLE_BYTESIZE 20")]
    public const int X3DAUDIO_HANDLE_BYTESIZE = 20;

    [NativeTypeName("#define X3DAUDIO_PI 3.141592654f")]
    public const float X3DAUDIO_PI = 3.141592654f;

    [NativeTypeName("#define X3DAUDIO_2PI 6.283185307f")]
    public const float X3DAUDIO_2PI = 6.283185307f;

    [NativeTypeName("#define X3DAUDIO_SPEED_OF_SOUND 343.5f")]
    public const float X3DAUDIO_SPEED_OF_SOUND = 343.5f;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_MATRIX 0x00000001")]
    public const int X3DAUDIO_CALCULATE_MATRIX = 0x00000001;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_DELAY 0x00000002")]
    public const int X3DAUDIO_CALCULATE_DELAY = 0x00000002;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_LPF_DIRECT 0x00000004")]
    public const int X3DAUDIO_CALCULATE_LPF_DIRECT = 0x00000004;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_LPF_REVERB 0x00000008")]
    public const int X3DAUDIO_CALCULATE_LPF_REVERB = 0x00000008;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_REVERB 0x00000010")]
    public const int X3DAUDIO_CALCULATE_REVERB = 0x00000010;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_DOPPLER 0x00000020")]
    public const int X3DAUDIO_CALCULATE_DOPPLER = 0x00000020;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_EMITTER_ANGLE 0x00000040")]
    public const int X3DAUDIO_CALCULATE_EMITTER_ANGLE = 0x00000040;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_ZEROCENTER 0x00010000")]
    public const int X3DAUDIO_CALCULATE_ZEROCENTER = 0x00010000;

    [NativeTypeName("#define X3DAUDIO_CALCULATE_REDIRECT_TO_LFE 0x00020000")]
    public const int X3DAUDIO_CALCULATE_REDIRECT_TO_LFE = 0x00020000;

    [DllImport("XAudio2_9", ExactSpelling = true)]
    public static extern HRESULT X3DAudioInitialize([NativeTypeName("UINT32")] uint SpeakerChannelMask, [NativeTypeName("FLOAT32")] float SpeedOfSound, [NativeTypeName("X3DAUDIO_HANDLE")] out X3DAUDIO_HANDLE Instance);

    //[DllImport("XAudio2_9", ExactSpelling = true)]
    //public static extern void X3DAudioCalculate([NativeTypeName("const X3DAUDIO_HANDLE")] byte* Instance, [NativeTypeName("const X3DAUDIO_LISTENER *")] X3DAUDIO_LISTENER* pListener, [NativeTypeName("const X3DAUDIO_EMITTER *")] X3DAUDIO_EMITTER* pEmitter, [NativeTypeName("UINT32")] uint Flags, X3DAUDIO_DSP_SETTINGS* pDSPSettings);
}
