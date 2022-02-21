// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from um/Unknwnbase.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TerraFX.Interop.Windows;

internal partial struct BOOL : IComparable, IComparable<BOOL>, IEquatable<BOOL>, IFormattable
{
    public readonly int Value;

    public BOOL(int value)
    {
        Value = value;
    }

    public static BOOL FALSE => new(0);

    public static BOOL TRUE => new(1);

    public static bool operator ==(BOOL left, BOOL right) => left.Value == right.Value;

    public static bool operator !=(BOOL left, BOOL right) => left.Value != right.Value;

    public static bool operator <(BOOL left, BOOL right) => left.Value < right.Value;

    public static bool operator <=(BOOL left, BOOL right) => left.Value <= right.Value;

    public static bool operator >(BOOL left, BOOL right) => left.Value > right.Value;

    public static bool operator >=(BOOL left, BOOL right) => left.Value >= right.Value;

    public static implicit operator bool(BOOL value) => value.Value != 0;

    public static implicit operator BOOL(bool value) => new BOOL(value ? 1 : 0);

    public static bool operator false(BOOL value) => value.Value == 0;

    public static bool operator true(BOOL value) => value.Value != 0;

    public static implicit operator BOOL(byte value) => new(value);

    public static explicit operator byte(BOOL value) => (byte)(value.Value);

    public static implicit operator BOOL(short value) => new(value);

    public static explicit operator short(BOOL value) => (short)(value.Value);

    public static implicit operator BOOL(int value) => new(value);

    public static implicit operator int(BOOL value) => value.Value;

    public static explicit operator BOOL(long value) => new((int)(value));

    public static implicit operator long(BOOL value) => value.Value;

    public static explicit operator BOOL(nint value) => new((int)(value));

    public static implicit operator nint(BOOL value) => value.Value;

    public static implicit operator BOOL(sbyte value) => new(value);

    public static explicit operator sbyte(BOOL value) => (sbyte)(value.Value);

    public static implicit operator BOOL(ushort value) => new(value);

    public static explicit operator ushort(BOOL value) => (ushort)(value.Value);

    public static explicit operator BOOL(uint value) => new((int)(value));

    public static explicit operator uint(BOOL value) => (uint)(value.Value);

    public static explicit operator BOOL(ulong value) => new((int)(value));

    public static explicit operator ulong(BOOL value) => (ulong)(value.Value);

    public static explicit operator BOOL(nuint value) => new((int)(value));

    public static explicit operator nuint(BOOL value) => (nuint)(value.Value);

    public int CompareTo(object? obj)
    {
        if (obj is BOOL other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of BOOL.");
    }

    public int CompareTo(BOOL other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is BOOL other) && Equals(other);

    public bool Equals(BOOL other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString();

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}

internal unsafe readonly struct HRESULT : IComparable, IComparable<HRESULT>, IEquatable<HRESULT>, IFormattable
{
    public readonly int Value;

    public bool FAILED => Value < 0;

    public bool SUCCEEDED => Value >= 0;

    public HRESULT(int value)
    {
        Value = value;
    }

    public static bool operator ==(HRESULT left, HRESULT right) => left.Value == right.Value;

    public static bool operator !=(HRESULT left, HRESULT right) => left.Value != right.Value;

    public static bool operator <(HRESULT left, HRESULT right) => left.Value < right.Value;

    public static bool operator <=(HRESULT left, HRESULT right) => left.Value <= right.Value;

    public static bool operator >(HRESULT left, HRESULT right) => left.Value > right.Value;

    public static bool operator >=(HRESULT left, HRESULT right) => left.Value >= right.Value;

    public static implicit operator HRESULT(byte value) => new(value);

    public static explicit operator byte(HRESULT value) => (byte)(value.Value);

    public static implicit operator HRESULT(short value) => new(value);

    public static explicit operator short(HRESULT value) => (short)(value.Value);

    public static implicit operator HRESULT(int value) => new(value);

    public static implicit operator int(HRESULT value) => value.Value;

    public static explicit operator HRESULT(long value) => new((int)(value));

    public static implicit operator long(HRESULT value) => value.Value;

    public static explicit operator HRESULT(nint value) => new((int)(value));

    public static implicit operator nint(HRESULT value) => value.Value;

    public static implicit operator HRESULT(sbyte value) => new(value);

    public static explicit operator sbyte(HRESULT value) => (sbyte)(value.Value);

    public static implicit operator HRESULT(ushort value) => new(value);

    public static explicit operator ushort(HRESULT value) => (ushort)(value.Value);

    public static explicit operator HRESULT(uint value) => new((int)(value));

    public static explicit operator uint(HRESULT value) => (uint)(value.Value);

    public static explicit operator HRESULT(ulong value) => new((int)(value));

    public static explicit operator ulong(HRESULT value) => (ulong)(value.Value);

    public static explicit operator HRESULT(nuint value) => new((int)(value));

    public static explicit operator nuint(HRESULT value) => (nuint)(value.Value);

    public int CompareTo(object? obj)
    {
        if (obj is HRESULT other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of HRESULT.");
    }

    public int CompareTo(HRESULT other) => Value.CompareTo(other.Value);

    public override bool Equals(object? obj) => (obj is HRESULT other) && Equals(other);

    public bool Equals(HRESULT other) => Value.Equals(other.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value.ToString("X8");

    public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
}

internal static unsafe partial class Windows
{
    [NativeTypeName("#define S_OK ((HRESULT)0L)")]
    public const int S_OK = 0;

    [NativeTypeName("#define S_FALSE ((HRESULT)1L)")]
    public const int S_FALSE = 0;

    [DoesNotReturn]
    internal static void ThrowExternalException(string methodName, int errorCode)
    {
        var message = string.Format("'{0}' failed with an error code of '{1}'", methodName, errorCode);
        throw new ExternalException(message, errorCode);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ThrowIfFailed(HRESULT value, [CallerArgumentExpression("value")] string? valueExpression = null)
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
        private readonly Guid* riid;

        internal UuidOfType(Guid* riid)
        {
            this.riid = riid;
        }

        /// <summary>Reads a <see cref="Guid"/> value from the GUID buffer for a given <see cref="UuidOfType"/> instance.</summary>
        /// <param name="guid">The input <see cref="UuidOfType"/> instance to read data for.</param>
        public static implicit operator Guid(UuidOfType guid) => *guid.riid;

        /// <summary>Returns the <see cref="Guid"/>* pointer to the GUID buffer for a given <see cref="UuidOfType"/> instance.</summary>
        /// <param name="guid">The input <see cref="UuidOfType"/> instance to read data for.</param>
        public static implicit operator Guid*(UuidOfType guid) => guid.riid;
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
            var p = (Guid*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), sizeof(Guid));
            *p = typeof(T).GUID;
            return p;
        }
    }
}

[Guid("00000000-0000-0000-C000-000000000046")]
internal unsafe partial struct IUnknown
{
    public void** lpVtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(0)]
    public HRESULT QueryInterface([NativeTypeName("const IID &")] Guid* riid, void** ppvObject)
    {
        return ((delegate* unmanaged<IUnknown*, Guid*, void**, int>)(lpVtbl[0]))((IUnknown*)Unsafe.AsPointer(ref this), riid, ppvObject);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(1)]
    [return: NativeTypeName("ULONG")]
    public uint AddRef()
    {
        return ((delegate* unmanaged<IUnknown*, uint>)(lpVtbl[1]))((IUnknown*)Unsafe.AsPointer(ref this));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [VtblIndex(2)]
    [return: NativeTypeName("ULONG")]
    public uint Release()
    {
        return ((delegate* unmanaged<IUnknown*, uint>)(lpVtbl[2]))((IUnknown*)Unsafe.AsPointer(ref this));
    }
}
