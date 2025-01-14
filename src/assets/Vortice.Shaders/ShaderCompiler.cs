// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.DirectX.DXC_OUT_KIND;
using static TerraFX.Interop.Windows.CLSID;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.DirectX.DXC;

namespace Vortice.Shaders;

public sealed partial class ShaderCompiler
{
    /// <summary>
    /// The thread local <see cref="ShaderCompiler"/> instance.
    /// This is necessary because the DXC library is strictly single-threaded.
    /// </summary>
    [ThreadStatic]
    private static ShaderCompiler? s_instance;

    /// <summary>
    /// The <see cref="IDxcCompiler"/> instance to use to create the bytecode for HLSL sources.
    /// </summary>
    private readonly ComPtr<IDxcCompiler3> _dxcCompiler;

    /// <summary>
    /// The <see cref="IDxcUtils"/> instance to use to work with <see cref="_dxcCompiler"/>.
    /// </summary>
    private readonly ComPtr<IDxcUtils> _dxcUtils;

    /// <summary>
    /// The <see cref="IDxcIncludeHandler"/> instance used to compile shaders with <see cref="_dxcCompiler"/>.
    /// </summary>
    private readonly ComPtr<IDxcIncludeHandler> _dxcDefaultIncludeHandler;

    /// <summary>
    /// Creates a new <see cref="ShaderCompiler"/> instance.
    /// </summary>
    private unsafe ShaderCompiler()
    {
        using ComPtr<IDxcUtils> dxcUtils = default;
        using ComPtr<IDxcCompiler3> dxcCompiler = default;
        using ComPtr<IDxcIncludeHandler> dxcIncludeHandler = default;

        ThrowIfFailed(DxcCreateInstance(
           (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in CLSID_DxcLibrary)),
            __uuidof<IDxcUtils>(),
            dxcUtils.GetVoidAddressOf())
            );
        ThrowIfFailed(DxcCreateInstance(
            (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in CLSID_DxcCompiler)),
            __uuidof<IDxcCompiler3>(),
            dxcCompiler.GetVoidAddressOf())
            );

        ThrowIfFailed(
            dxcUtils.Get()->CreateDefaultIncludeHandler(dxcIncludeHandler.GetAddressOf())
            );

        _dxcCompiler = dxcCompiler.Move();
        _dxcUtils = dxcUtils.Move();
        _dxcDefaultIncludeHandler = dxcIncludeHandler.Move();
    }

    /// <summary>
    /// Destroys the current <see cref="ShaderCompiler"/> instance.
    /// </summary>
    ~ShaderCompiler()
    {
        _dxcCompiler.Dispose();
        _dxcUtils.Dispose();
        _dxcDefaultIncludeHandler.Dispose();
    }

    /// <summary>
    /// Gets a <see cref="ShaderCompiler"/> instance to use.
    /// </summary>
    public static ShaderCompiler Instance => s_instance ??= new();

    public unsafe ShaderCompilationResult Compile(ReadOnlySpan<char> source, ReadOnlySpan<char> entryPoint)
    {
        using ComPtr<IDxcBlobEncoding> dxcBlobEncoding = default;
        using ComPtr<IDxcResult> results = default;

        // Get the encoded blob from the source code
        fixed (char* pSource = source)
        {
            ThrowIfFailed(_dxcUtils.Get()->CreateBlobFromPinned(
                pSource,
                (uint)source.Length * 2,
                DXC_CP_UTF16,
                dxcBlobEncoding.GetAddressOf())
                );
        }

        DxcBuffer buffer = new()
        {
            Ptr = dxcBlobEncoding.Get()->GetBufferPointer(),
            Size = dxcBlobEncoding.Get()->GetBufferSize(),
            Encoding = (uint)DXC_CP_ACP,
        };

        fixed (char* shaderName = "")
        fixed (char* pEntryPointName = "-E")
        fixed (char* pEntryPoint = entryPoint)
        fixed (char* pShaderProfileName = "-T")
        fixed (char* pShaderProfile = "cs_6_0")
        //fixed (char* optimization = "-O3")
        //fixed (char* rowMajor = "-Zpr")
        //fixed (char* warningsAsErrors = "-Werror")
        {
            uint argCount = 5;
            char** arguments = stackalloc char*[5] {
                shaderName,
                pEntryPointName,
                pEntryPoint,
                pShaderProfileName,
                pShaderProfile/*,
                //optimization,
                //rowMajor,
                //warningsAsErrors*/
            };

            HRESULT hr = _dxcCompiler.Get()->Compile(
                &buffer,
                (ushort**)arguments,
                argCount,
                _dxcDefaultIncludeHandler.Get(),
                __uuidof<IDxcResult>(),
                results.GetVoidAddressOf()
                );

            if (hr.FAILED)
            {
                return new DxcShaderCompilationResult($"Compile failed with HRESULT {hr}");
            }

            //
            // Print errors if present.
            //
            using ComPtr<IDxcBlobUtf8> errors = default;
            results.Get()->GetOutput(DXC_OUT_ERRORS,
                __uuidof<IDxcBlobUtf8>(),
                errors.GetVoidAddressOf(),
                null
                );
            // Note that d3dcompiler would return null if no errors or warnings are present.
            // IDxcCompiler3::Compile will always return an error buffer, but its length
            // will be zero if there are no warnings or errors.
            if (errors.Get() is not null && errors.Get()->GetStringLength() != 0)
            {
                //wprintf(L"Warnings and Errors:\n%S\n", pErrors->GetStringPointer());
            }

            // Quit if the compilation failed.
            HRESULT hrStatus;
            results.Get()->GetStatus(&hrStatus);
            if (hrStatus.FAILED)
            {
                return new DxcShaderCompilationResult($"Compile failed with HRESULT {hrStatus}");
            }

            using ComPtr<IDxcBlob> byteCode = default;
            using ComPtr<IDxcBlobUtf16> pShaderName = default;
            results.Get()->GetOutput(DXC_OUT_OBJECT,
                __uuidof<IDxcBlob>(),
                byteCode.GetVoidAddressOf(),
                pShaderName.GetAddressOf()
                );
            if (byteCode.Get() is null)
            {
                return new DxcShaderCompilationResult("The compiled shader is invalid");
            }

            return new DxcShaderCompilationResult(byteCode);
        }
    }
}
