// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Implementation based on: https://github.com/mellinoe/nativelibraryloader
// Implementation based on Silk.NET.Core: https://github.com/dotnet/Silk.NET/blob/main/LICENSE.md

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyModel;

namespace Vortice;

/// <summary>
/// Enumerates possible library load <see cref="UnmanagedLibrary"/>.
/// </summary>
public abstract class UnmanagedLibraryPathResolver
{
    /// <summary>
    /// Gets a default path resolver.
    /// </summary>
    public static UnmanagedLibraryPathResolver Default { get; } = new DefaultPathResolver();

    /// <summary>
    /// Returns an enumerator which yields possible library load targets, in priority order.
    /// </summary>
    /// <param name="name">The name of the library to load.</param>
    /// <returns>An enumerator yielding load targets.</returns>
    public abstract IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name);
}

/// <summary>
/// Enumerates possible library load targets. This default implementation returns the following load targets:
/// First: The simple name, unchanged.
/// Second: The library contained in the applications base folder.
/// Third: The library as resolved via the default DependencyContext, in the default nuget package cache folder.
/// </summary>
public class DefaultPathResolver : UnmanagedLibraryPathResolver
{
    // <inheritdoc />
    public override IEnumerable<string> EnumeratePossibleLibraryLoadTargets(string name)
    {
        return CoreEnumeratePossibleLibraryLoadTargets(name);
    }

    private IEnumerable<string> CoreEnumeratePossibleLibraryLoadTargets(string name, bool skipVersionTraverse = false)
    {
        yield return name;

        if (!string.IsNullOrEmpty(AppContext.BaseDirectory))
        {
            yield return Path.Combine(AppContext.BaseDirectory, name);
            if (TryLocateNativeAssetInRuntimesFolder(name, AppContext.BaseDirectory, out string? result))
            {
                yield return result!;
            }
        }

        if (TryLocateNativeAssetFromDeps(name, out string? appLocalNativePath, out string? depsResolvedPath))
        {
            yield return appLocalNativePath!;
            yield return depsResolvedPath!;
        }

        string? mainModFname = Process.GetCurrentProcess().MainModule?.FileName;
        if (AppContext.BaseDirectory != Process.GetCurrentProcess().MainModule?.FileName &&
            mainModFname is not null)
        {
            mainModFname = Path.GetDirectoryName(mainModFname);
            if (mainModFname is not null)
            {
                yield return Path.Combine(mainModFname, name);
            }

            if (TryLocateNativeAssetInRuntimesFolder(name, mainModFname, out var result))
            {
                yield return result;
            }
        }

        if (!skipVersionTraverse)
        {
            foreach (var linuxName in GetLinuxPossibilities(name))
            {
                foreach (var possibleLoadTarget in CoreEnumeratePossibleLibraryLoadTargets(linuxName, true))
                {
                    yield return possibleLoadTarget;
                }
            }

            foreach (var macName in GetMacPossibilities(name))
            {
                foreach (var possibleLoadTarget in CoreEnumeratePossibleLibraryLoadTargets(macName, false))
                {
                    yield return possibleLoadTarget;
                }
            }
        }
    }

    private bool TryLocateNativeAssetInRuntimesFolder(string name, string baseFolder, out string? result)
    {
        static bool Check(string name, string ridFolder, out string result)
        {
            string theoreticalFName = Path.Combine(ridFolder, name);
            if (File.Exists(theoreticalFName))
            {
                result = theoreticalFName;
                return true;
            }

            result = null;
            return false;
        }

        foreach (var rid in GetAllRuntimeIds(GetRuntimeIdentifier(), DependencyContext.Default))
        {
            if (Check(name, Path.Combine(baseFolder, "runtimes", rid, "native", name), out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }

    private bool TryLocateNativeAssetFromDeps(string name, out string? appLocalNativePath, out string? depsResolvedPath)
    {
        try
        {
            DependencyContext defaultContext = DependencyContext.Default;
            Assembly? entAsm = Assembly.GetEntryAssembly();
            if (defaultContext is null && entAsm is not null)
            {
                var json = new DependencyContextJsonReader();
                defaultContext ??= json.Read
                (
                    File.OpenRead
                    (
                        Path.Combine
                        (
                            Path.GetDirectoryName(entAsm.Location),
                            entAsm.GetName().Name + ".deps.json"
                        )
                    )
                );
                defaultContext ??=
                    json.Read
                    (
                        File.OpenRead
                        (
                            Path.Combine(AppContext.BaseDirectory, entAsm.GetName().Name + ".deps.json")
                        )
                    );
            }

            if (defaultContext == null)
            {
                appLocalNativePath = null;
                depsResolvedPath = null;
                return false;
            }

            string currentRid = GetRuntimeIdentifier();
            foreach (var rid in GetAllRuntimeIds(currentRid, defaultContext))
            {
                foreach (var runtimeLib in defaultContext.RuntimeLibraries)
                {
                    foreach (var nativeAsset in runtimeLib.GetRuntimeNativeAssets(defaultContext, rid))
                    {
                        if (Path.GetFileName(nativeAsset) == name || Path.GetFileNameWithoutExtension(nativeAsset) == name)
                        {
                            appLocalNativePath = Path.Combine
                            (
                                AppContext.BaseDirectory,
                                nativeAsset
                            );
                            appLocalNativePath = Path.GetFullPath(appLocalNativePath);

                            depsResolvedPath = Path.Combine
                            (
                                GetNugetPackagesRootDirectory(),
                                runtimeLib.Name.ToLowerInvariant(),
                                runtimeLib.Version,
                                nativeAsset
                            );
                            depsResolvedPath = Path.GetFullPath(depsResolvedPath);

                            return true;
                        }
                    }
                }
            }

            appLocalNativePath = null;
            depsResolvedPath = null;
            return false;
        }
        catch (Exception ex)
        {
            appLocalNativePath = null;
            depsResolvedPath = null;
            return false;
        }
    }

    private static IEnumerable<string> GetLinuxPossibilities(string name)
    {
        var nameSplit = name.Split('.');
        var indexOfSo = Array.LastIndexOf(nameSplit, "so");
        if (indexOfSo != -1)
        {
            // for libglfw.so.3.3 this should return:
            // libglfw.so.3.3
            // libglfw.so.3
            // libglfw.so
            for (var i = nameSplit.Length - 1; i >= indexOfSo; i--)
            {
                yield return string.Join(".", nameSplit, 0, i + 1);
            }
        }
    }

    private static IEnumerable<string> GetMacPossibilities(string name)
    {
        var nameSplit = name.Split('.');
        var indexOfDylib = Array.LastIndexOf(nameSplit, "dylib");
        if (indexOfDylib != -1)
        {
            // for libglfw.3.3.dylib this should return:
            // libglfw.3.3.dylib
            // libglfw.3.dylib
            // libglfw.dylib
            for (var i = indexOfDylib - 1; i >= 0; i--)
            {
                yield return $"{string.Join(".", nameSplit, 0, i + 1)}.dylib";
            }
        }
    }

    private static string GetRuntimeIdentifier()
    {
#if NET5_0_OR_GREATER
        return RuntimeInformation.RuntimeIdentifier;
#else
        return Microsoft.DotNet.PlatformAbstractions.RuntimeEnvironment.GetRuntimeIdentifier();
#endif
    }

    private string GetNugetPackagesRootDirectory()
    {
        // TODO: Handle alternative package directories, if they are configured.
        return Path.Combine(GetUserDirectory(), ".nuget", "packages");
    }

    private string? GetUserDirectory()
    {
#if NET5_0_OR_GREATER
        if (OperatingSystem.IsWindows())
#else
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
#endif
        {
            return Environment.GetEnvironmentVariable("USERPROFILE");
        }

        return Environment.GetEnvironmentVariable("HOME");
    }

    private List<string> GetAllRuntimeIds(string currentRid, DependencyContext ctx)
    {
        var allRiDs = new List<string>();

        // prevent null reference exception on net6.0-android where DependencyContext.Default is null
        if (ctx is not null)
        {
            allRiDs.Add(currentRid);
            if (!AddFallbacks(allRiDs, currentRid, ctx.RuntimeGraph))
            {
                var guessedFallbackRid = GuessFallbackRid(currentRid);
                if (guessedFallbackRid != null)
                {
                    allRiDs.Add(guessedFallbackRid);
                    AddFallbacks(allRiDs, guessedFallbackRid, ctx.RuntimeGraph);
                }
            }
        }

        return allRiDs;
    }

    // from: https://github.com/dotnet/runtime/blob/main/src/libraries/Microsoft.NETCore.Platforms/src/runtime.json
    private static readonly string[] _linuxRiDs =
    {
        "alpine", "android", "arch", "centos", "debian", "exherbo", "fedora", "freebsd", "gentoo", "linux",
        "opensuse", "rhel", "sles", "tizen"
    };

    private string GuessFallbackRid(string actualRuntimeIdentifier)
    {
        if (actualRuntimeIdentifier == "osx.10.13-x64")
        {
            return "osx.10.12-x64";
        }

        var split = actualRuntimeIdentifier.Split('-');
        if (split[0] != "osx" && split[0].StartsWith("osx"))
        {
            return $"osx-{string.Join("-", split.Skip(1))}".TrimEnd('-');
        }

        if (split[0] != "win" && split[0].StartsWith("win"))
        {
            return $"win-{string.Join("-", split.Skip(1))}".TrimEnd('-');
        }

        if (split[0] != "linux" && _linuxRiDs.Any(x => split[0].StartsWith(x)))
        {
            return $"linux-{string.Join("-", split.Skip(1))}".TrimEnd('-');
        }

        return null;
    }

    private bool AddFallbacks(List<string> fallbacks, string rid, IReadOnlyList<RuntimeFallbacks> allFallbacks)
    {
        foreach (var fb in allFallbacks)
        {
            if (fb.Runtime == rid)
            {
                fallbacks.AddRange(fb.Fallbacks);
                return true;
            }
        }

        while (rid is not null)
        {
            if (!fallbacks.Contains(rid))
            {
                fallbacks.Add(rid);
            }

            rid = GuessFallbackRid(rid);
        }

        return false;
    }
}
