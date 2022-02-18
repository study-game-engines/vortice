// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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

internal unsafe partial struct HANDLE : IComparable, IComparable<HANDLE>, IEquatable<HANDLE>, IFormattable
{
    public readonly void* Value;

    public HANDLE(void* value)
    {
        Value = value;
    }

    public static HANDLE INVALID_VALUE => new HANDLE((void*)(-1));

    public static HANDLE NULL => new HANDLE(null);

    public static bool operator ==(HANDLE left, HANDLE right) => left.Value == right.Value;

    public static bool operator !=(HANDLE left, HANDLE right) => left.Value != right.Value;

    public static bool operator <(HANDLE left, HANDLE right) => left.Value < right.Value;

    public static bool operator <=(HANDLE left, HANDLE right) => left.Value <= right.Value;

    public static bool operator >(HANDLE left, HANDLE right) => left.Value > right.Value;

    public static bool operator >=(HANDLE left, HANDLE right) => left.Value >= right.Value;

    public static explicit operator HANDLE(void* value) => new HANDLE(value);

    public static implicit operator void*(HANDLE value) => value.Value;

    public static explicit operator HANDLE(byte value) => new HANDLE((void*)(value));

    public static explicit operator byte(HANDLE value) => (byte)(value.Value);

    public static explicit operator HANDLE(short value) => new HANDLE((void*)(value));

    public static explicit operator short(HANDLE value) => (short)(value.Value);

    public static explicit operator HANDLE(int value) => new HANDLE((void*)(value));

    public static explicit operator int(HANDLE value) => (int)(value.Value);

    public static explicit operator HANDLE(long value) => new HANDLE((void*)(value));

    public static explicit operator long(HANDLE value) => (long)(value.Value);

    public static explicit operator HANDLE(nint value) => new HANDLE((void*)(value));

    public static implicit operator nint(HANDLE value) => (nint)(value.Value);

    public static explicit operator HANDLE(sbyte value) => new HANDLE((void*)(value));

    public static explicit operator sbyte(HANDLE value) => (sbyte)(value.Value);

    public static explicit operator HANDLE(ushort value) => new HANDLE((void*)(value));

    public static explicit operator ushort(HANDLE value) => (ushort)(value.Value);

    public static explicit operator HANDLE(uint value) => new HANDLE((void*)(value));

    public static explicit operator uint(HANDLE value) => (uint)(value.Value);

    public static explicit operator HANDLE(ulong value) => new HANDLE((void*)(value));

    public static explicit operator ulong(HANDLE value) => (ulong)(value.Value);

    public static explicit operator HANDLE(nuint value) => new HANDLE((void*)(value));

    public static implicit operator nuint(HANDLE value) => (nuint)(value.Value);

    public int CompareTo(object? obj)
    {
        if (obj is HANDLE other)
        {
            return CompareTo(other);
        }

        return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of HANDLE.");
    }

    public int CompareTo(HANDLE other) => ((nuint)(Value)).CompareTo((nuint)(other.Value));

    public override bool Equals(object? obj) => (obj is HANDLE other) && Equals(other);

    public bool Equals(HANDLE other) => ((nuint)(Value)).Equals((nuint)(other.Value));

    public override int GetHashCode() => ((nuint)(Value)).GetHashCode();

    public override string ToString() => ((nuint)(Value)).ToString((sizeof(nint) == 4) ? "X8" : "X16");

    public string ToString(string? format, IFormatProvider? formatProvider) => ((nuint)(Value)).ToString(format, formatProvider);
}

internal readonly partial struct LUID
{
    [NativeTypeName("DWORD")]
    public readonly uint LowPart;
    [NativeTypeName("LONG")]
    public readonly int HighPart;
}
