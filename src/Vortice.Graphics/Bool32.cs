// Copyright © Amer Koleci and Contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vortice.Graphics;

/// <summary>
/// A boolean value stored on 4 bytes (instead of 1 in .NET).
/// </summary>
[StructLayout(LayoutKind.Sequential, Size = 4)]
public readonly struct Bool32 : IEquatable<Bool32>
{
    public static readonly Bool32 True = new(true);
    public static readonly Bool32 False = new(false);

    private readonly uint _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Bool32" /> class.
    /// </summary>
    /// <param name="boolValue">if set to <c>true</c> [bool value].</param>
    public Bool32(bool boolValue)
    {
        _value = boolValue ? 1u : 0u;
    }

    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="other">The other.</param>
    /// <returns>true if <paramref name="other" /> and this instance are the same type and represent the same value; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Bool32 other) => _value == other._value;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Bool32 rawBool && Equals(rawBool);

    /// <inheritdoc/>
    public override int GetHashCode() => _value.GetHashCode();

    /// <summary>
    /// Implements the ==.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Bool32 left, Bool32 right) => left._value.Equals(right._value);

    /// <summary>
    /// Implements the !=.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns>The result of the operator.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Bool32 left, Bool32 right) => !left._value.Equals(right._value);

    /// <summary>
    /// Performs an explicit conversion from <see cref="Bool32"/> to <see cref="bool"/>.
    /// </summary>
    /// <param name="value">The <see cref="Bool32"/> value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator bool(Bool32 value) => value._value != 0;

    /// <summary>
    /// Performs an explicit conversion from <see cref="bool"/> to <see cref="Bool32"/>.
    /// </summary>
    /// <param name="boolValue">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Bool32(bool boolValue) => new(boolValue);

    /// <inheritdoc/>
    public override string ToString() => _value != 0 ? "True" : "False";
}
