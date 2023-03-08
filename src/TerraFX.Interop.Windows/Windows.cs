// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TerraFX.Interop.Windows;

internal static unsafe partial class Windows
{
    [NativeTypeName("#define S_OK ((HRESULT)0L)")]
    public const int S_OK = 0;

    [NativeTypeName("#define S_FALSE ((HRESULT)1L)")]
    public const int S_FALSE = 0;

    /// <summary>Raised whenever a native library is loaded by TerraFX.Interop.Windows. Handlers can be added to this event to customize how libraries are loaded, and they will be used first whenever a new native library is being resolved.</summary>
    public static event DllImportResolver? ResolveLibrary;

    static Windows()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OnDllImport);
    }

    /// <summary>The default <see cref="DllImportResolver"/> for TerraFX.Interop.Windows.</summary>
    /// <inheritdoc cref="DllImportResolver"/>
    private static IntPtr OnDllImport(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (TryResolveLibrary(libraryName, assembly, searchPath, out IntPtr nativeLibrary))
        {
            return nativeLibrary;
        }

        return NativeLibrary.Load(libraryName, assembly, searchPath);
    }

    /// <summary>Tries to resolve a native library using the handlers for the <see cref="ResolveLibrary"/> event.</summary>
    /// <param name="libraryName">The native library to resolve.</param>
    /// <param name="assembly">The assembly requesting the resolution.</param>
    /// <param name="searchPath">The <see cref="DllImportSearchPath"/> value on the P/Invoke or assembly, or <see langword="null"/>.</param>
    /// <param name="nativeLibrary">The loaded library, if one was resolved.</param>
    /// <returns>Whether or not the requested library was successfully loaded.</returns>
    private static bool TryResolveLibrary(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, out IntPtr nativeLibrary)
    {
        var resolveLibrary = ResolveLibrary;

        if (resolveLibrary != null)
        {
            var resolvers = resolveLibrary.GetInvocationList();

            foreach (DllImportResolver resolver in resolvers)
            {
                nativeLibrary = resolver(libraryName, assembly, searchPath);

                if (nativeLibrary != IntPtr.Zero)
                {
                    return true;
                }
            }
        }

        nativeLibrary = IntPtr.Zero;
        return false;
    }

    [DoesNotReturn]
    public static void ThrowExternalException(string methodName, int errorCode)
    {
        string message = string.Format("'{0}' failed with an error code of '{1}'", methodName, errorCode);
        throw new ExternalException(message, errorCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ThrowIfFailed(HRESULT value, [CallerArgumentExpression("value")] string? valueExpression = null)
    {
        if (value.FAILED)
        {
            ThrowExternalException(valueExpression ?? "Method", value);
        }
    }

    /// <summary>Retrieves the GUID of of a specified type.</summary>
    /// <param name="value">A value of type <typeparamref name="T"/>.</param>
    /// <typeparam name="T">The type to retrieve the GUID for.</typeparam>
    /// <returns>A <see cref="UuidOfType"/> value wrapping a pointer to the GUID data for the input type. This value can be either converted to a <see cref="Guid"/> pointer, or implicitly assigned to a <see cref="Guid"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe UuidOfType __uuidof<T>(T value) // for type inference similar to C++'s __uuidof
        where T : unmanaged
    {
        return new UuidOfType(UUID<T>.RIID);
    }

    /// <summary>Retrieves the GUID of of a specified type.</summary>
    /// <param name="value">A pointer to a value of type <typeparamref name="T"/>.</param>
    /// <typeparam name="T">The type to retrieve the GUID for.</typeparam>
    /// <returns>A <see cref="UuidOfType"/> value wrapping a pointer to the GUID data for the input type. This value can be either converted to a <see cref="Guid"/> pointer, or implicitly assigned to a <see cref="Guid"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe UuidOfType __uuidof<T>(T* value) // for type inference similar to C++'s __uuidof
        where T : unmanaged
    {
        return new UuidOfType(UUID<T>.RIID);
    }

    /// <summary>Retrieves the GUID of of a specified type.</summary>
    /// <typeparam name="T">The type to retrieve the GUID for.</typeparam>
    /// <returns>A <see cref="UuidOfType"/> value wrapping a pointer to the GUID data for the input type. This value can be either converted to a <see cref="Guid"/> pointer, or implicitly assigned to a <see cref="Guid"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe UuidOfType __uuidof<T>()
        where T : unmanaged
    {
        return new UuidOfType(UUID<T>.RIID);
    }

    /// <summary>A proxy type that wraps a pointer to GUID data. Values of this type can be implicitly converted to and assigned to <see cref="Guid"/>* or <see cref="Guid"/> parameters.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public readonly unsafe ref struct UuidOfType
    {
        private readonly Guid* _riid;

        internal UuidOfType(Guid* riid)
        {
            _riid = riid;
        }

        /// <summary>Reads a <see cref="Guid"/> value from the GUID buffer for a given <see cref="UuidOfType"/> instance.</summary>
        /// <param name="guid">The input <see cref="UuidOfType"/> instance to read data for.</param>
        public static implicit operator Guid(UuidOfType guid) => *guid._riid;

        /// <summary>Returns the <see cref="Guid"/>* pointer to the GUID buffer for a given <see cref="UuidOfType"/> instance.</summary>
        /// <param name="guid">The input <see cref="UuidOfType"/> instance to read data for.</param>
        public static implicit operator Guid*(UuidOfType guid) => guid._riid;
    }

    /// <summary>A helper type to provide static GUID buffers for specific types.</summary>
    /// <typeparam name="T">The type to allocate a GUID buffer for.</typeparam>
    private static unsafe class UUID<T> where T : unmanaged
    {
        /// <summary>The pointer to the <see cref="Guid"/> value for the current type.</summary>
        /// <remarks>The target memory area should never be written to.</remarks>
        public static readonly Guid* RIID = CreateRIID();

        /// <summary>Allocates memory for a <see cref="Guid"/> value and initializes it.</summary>
        /// <returns>A pointer to memory holding the <see cref="Guid"/> value for the current type.</returns>
        private static Guid* CreateRIID()
        {
#if NET5_0_OR_GREATER
            var p = (Guid*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), sizeof(Guid));
#else
            var p = (Guid*)Marshal.AllocHGlobal(sizeof(Guid));
#endif
            *p = typeof(T).GUID;
            return p;
        }
    }
}
