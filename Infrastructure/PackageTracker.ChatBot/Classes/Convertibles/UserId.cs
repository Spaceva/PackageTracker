namespace PackageTracker.ChatBot;

public sealed class UserId : IConvertible, IComparable<UserId>, IEqualityComparer<UserId>, IEquatable<UserId>, IComparer<UserId>
{
    private readonly string id;

    public UserId(string id)
    {
        this.id = id;
    }

    public UserId(int id)
    {
        this.id = id.ToString();
    }

    public UserId(long id)
    {
        this.id = id.ToString();
    }

    public UserId(ulong id)
    {
        this.id = id.ToString();
    }

    public static implicit operator string(UserId m)
    {
        return m.id;
    }

    public static implicit operator UserId(string s)
    {
        return new UserId(s);
    }

    public static implicit operator int(UserId m)
    {
        return int.Parse(m.id);
    }

    public static implicit operator UserId(int i)
    {
        return new UserId(i);
    }

    public static implicit operator long(UserId m)
    {
        return long.Parse(m.id);
    }

    public static implicit operator UserId(long l)
    {
        return new UserId(l);
    }

    public static implicit operator ulong(UserId m)
    {
        return ulong.Parse(m.id);
    }

    public static implicit operator UserId(ulong u)
    {
        return new UserId(u);
    }

    public static bool operator ==(UserId m1, UserId m2) => m1.Equals(m2);

    public static bool operator !=(UserId m1, UserId m2) => !m1.Equals(m2);

    public static bool operator >(UserId m1, UserId m2) => m1.CompareTo(m2) > 0;

    public static bool operator <(UserId m1, UserId m2) => m1.CompareTo(m2) < 0;

    public static bool operator >=(UserId m1, UserId m2) => m1.CompareTo(m2) >= 0;

    public static bool operator <=(UserId m1, UserId m2) => m1.CompareTo(m2) <= 0;

    public override string ToString() => id;

    public TypeCode GetTypeCode() => TypeCode.String;

    public bool ToBoolean(IFormatProvider? provider) => bool.Parse(id);

    public char ToChar(IFormatProvider? provider) => char.Parse(id);

    public sbyte ToSByte(IFormatProvider? provider) => sbyte.Parse(id);

    public byte ToByte(IFormatProvider? provider) => byte.Parse(id);

    public short ToInt16(IFormatProvider? provider) => short.Parse(id);

    public ushort ToUInt16(IFormatProvider? provider) => ushort.Parse(id);

    public int ToInt32(IFormatProvider? provider) => int.Parse(id);

    public uint ToUInt32(IFormatProvider? provider) => uint.Parse(id);

    public long ToInt64(IFormatProvider? provider) => long.Parse(id);

    public ulong ToUInt64(IFormatProvider? provider) => ulong.Parse(id);

    public float ToSingle(IFormatProvider? provider) => float.Parse(id);

    public double ToDouble(IFormatProvider? provider) => double.Parse(id);

    public decimal ToDecimal(IFormatProvider? provider) => decimal.Parse(id);

    public DateTime ToDateTime(IFormatProvider? provider) => throw new NotImplementedException();

    public string ToString(IFormatProvider? provider) => id;

    public object ToType(Type conversionType, IFormatProvider? provider) => Convert.ChangeType(id, conversionType);

    public override bool Equals(object? obj)
    {
        return Equals(obj as UserId);
    }

    public bool Equals(UserId? other)
    {
        if (other is null)
        {
            return this is null;
        }

        return id.Equals(other.id);
    }

    public bool Equals(UserId? x, UserId? y)
    {
        if (x is null)
        {
            return y is null;
        }

        if (y is null)
        {
            return x is null;
        }

        return x.id.Equals(y.id);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public int GetHashCode(UserId? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.id.GetHashCode();
    }

    public int CompareTo(UserId? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return id.CompareTo(other.id);
    }

    public int Compare(UserId? x, UserId? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        return x.id.CompareTo(y.id);
    }
}
