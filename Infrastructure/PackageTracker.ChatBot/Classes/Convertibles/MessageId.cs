namespace PackageTracker.ChatBot;

public sealed class MessageId : IConvertible, IComparable<MessageId>, IEqualityComparer<MessageId>, IEquatable<MessageId>, IComparer<MessageId>
{
    private readonly string id;

    public MessageId(string id)
    {
        this.id = id;
    }

    public MessageId(int id)
    {
        this.id = id.ToString();
    }

    public MessageId(long id)
    {
        this.id = id.ToString();
    }

    public MessageId(ulong id)
    {
        this.id = id.ToString();
    }

    public static implicit operator string(MessageId m)
    {
        return m.id;
    }

    public static implicit operator MessageId(string s)
    {
        return new MessageId(s);
    }

    public static implicit operator int(MessageId m)
    {
        return int.Parse(m.id);
    }

    public static implicit operator MessageId(int i)
    {
        return new MessageId(i);
    }

    public static implicit operator long(MessageId m)
    {
        return long.Parse(m.id);
    }

    public static implicit operator MessageId(long l)
    {
        return new MessageId(l);
    }

    public static implicit operator ulong(MessageId m)
    {
        return ulong.Parse(m.id);
    }

    public static implicit operator MessageId(ulong u)
    {
        return new MessageId(u);
    }

    public static bool operator ==(MessageId m1, MessageId m2) => m1.Equals(m2);

    public static bool operator !=(MessageId m1, MessageId m2) => !m1.Equals(m2);

    public static bool operator >(MessageId m1, MessageId m2) => m1.CompareTo(m2) > 0;

    public static bool operator <(MessageId m1, MessageId m2) => m1.CompareTo(m2) < 0;

    public static bool operator >=(MessageId m1, MessageId m2) => m1.CompareTo(m2) >= 0;

    public static bool operator <=(MessageId m1, MessageId m2) => m1.CompareTo(m2) <= 0;

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
        return Equals(obj as MessageId);
    }

    public bool Equals(MessageId? x, MessageId? y)
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

    public bool Equals(MessageId? other)
    {
        if (other is null)
        {
            return this is null;
        }

        return id.Equals(other.id);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public int GetHashCode(MessageId? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.id.GetHashCode();
    }

    public int CompareTo(MessageId? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return id.CompareTo(other.id);
    }

    public int Compare(MessageId? x, MessageId? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        return x.id.CompareTo(y.id);
    }
}

