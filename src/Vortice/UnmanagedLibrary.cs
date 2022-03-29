// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Implementation based on: https://github.com/mellinoe/nativelibraryloader
// Implementation based on Silk.NET.Core: https://github.com/dotnet/Silk.NET/blob/main/LICENSE.md

namespace Vortice;

public sealed class UnmanagedLibrary : IDisposable
{
    private static readonly UnmanagedLibraryLoader s_platformDefaultLoader = UnmanagedLibraryLoader.GetPlatformDefaultLoader();
    private readonly UnmanagedLibraryLoader _loader;

    /// <summary>
    /// Constructs a new <see cref="UnmanagedLibrary"/> using the platform's default library loader.
    /// </summary>
    /// <param name="name">The name of the library to load.</param>
    public UnmanagedLibrary(string name)
        : this(name, s_platformDefaultLoader, UnmanagedLibraryPathResolver.Default)
    {
    }

    /// <summary>
    /// Constructs a new <see cref="UnmanagedLibrary"/> using the platform's default library loader.
    /// </summary>
    /// <param name="names">An ordered list of names to attempt to load.</param>
    public UnmanagedLibrary(string[] names)
        : this(names, s_platformDefaultLoader, UnmanagedLibraryPathResolver.Default)
    {
    }

    /// <summary>
    /// Constructs a new <see cref="UnmanagedLibrary"/> using the specified library loader.
    /// </summary>
    /// <param name="name">The name of the library to load.</param>
    /// <param name="loader">The loader used to open and close the library, and to load function pointers.</param>
    public UnmanagedLibrary(string name, UnmanagedLibraryLoader loader)
        : this(name, loader, UnmanagedLibraryPathResolver.Default)
    {
    }

    /// <summary>
    /// Constructs a new <see cref="UnmanagedLibrary"/> using the specified library loader.
    /// </summary>
    /// <param name="names">An ordered list of names to attempt to load.</param>
    /// <param name="loader">The loader used to open and close the library, and to load function pointers.</param>
    public UnmanagedLibrary(string[] names, UnmanagedLibraryLoader loader)
        : this(names, loader, UnmanagedLibraryPathResolver.Default)
    {
    }

    /// <summary>
    /// Constructs a new NativeLibrary using the specified library loader.
    /// </summary>
    /// <param name="name">The name of the library to load.</param>
    /// <param name="loader">The loader used to open and close the library, and to load function pointers.</param>
    /// <param name="pathResolver">The path resolver, used to identify possible load targets for the library.</param>
    public UnmanagedLibrary(string name, UnmanagedLibraryLoader loader, UnmanagedLibraryPathResolver pathResolver)
    {
        _loader = loader;
        Handle = _loader.LoadNativeLibrary(name, pathResolver);
    }

    /// <summary>
    /// Constructs a new <see cref="UnmanagedLibrary"/> using the specified library loader.
    /// </summary>
    /// <param name="names">An ordered list of names to attempt to load.</param>
    /// <param name="loader">The loader used to open and close the library, and to load function pointers.</param>
    /// <param name="pathResolver">The path resolver, used to identify possible load targets for the library.</param>
    public UnmanagedLibrary(string[] names, UnmanagedLibraryLoader loader, UnmanagedLibraryPathResolver pathResolver)
    {
        _loader = loader;
        Handle = _loader.LoadNativeLibrary(names, pathResolver);
    }

    /// <summary>
    /// The operating system handle of the loaded library.
    /// </summary>
    public nint Handle { get; }

    /// <summary>
    /// Frees the native library. Function pointers retrieved from this library will be void.
    /// </summary>
    public void Dispose()
    {
        _loader.FreeNativeLibrary(Handle);
    }

}
