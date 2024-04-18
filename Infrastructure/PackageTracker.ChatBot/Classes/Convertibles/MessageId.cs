namespace PackageTracker.ChatBot;

public class MessageId : IConvertible, IComparable<MessageId>, IEqualityComparer<MessageId>, IEquatable<MessageId>, IComparer<MessageId>
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

    public override string ToString() => id;

    public TypeCode GetTypeCode() => TypeCode.String;

    public bool ToBoolean(IFormatProvider? _) => bool.Parse(id);

    public char ToChar(IFormatProvider? _) => char.Parse(id);

    public sbyte ToSByte(IFormatProvider? _) => sbyte.Parse(id);

    public byte ToByte(IFormatProvider? _) => byte.Parse(id);

    public short ToInt16(IFormatProvider? _) => short.Parse(id);

    public ushort ToUInt16(IFormatProvider? _) => ushort.Parse(id);

    public int ToInt32(IFormatProvider? _) => int.Parse(id);

    public uint ToUInt32(IFormatProvider? _) => uint.Parse(id);

    public long ToInt64(IFormatProvider? _) => long.Parse(id);

    public ulong ToUInt64(IFormatProvider? _) => ulong.Parse(id);

    public float ToSingle(IFormatProvider? _) => float.Parse(id);

    public double ToDouble(IFormatProvider? _) => double.Parse(id);

    public decimal ToDecimal(IFormatProvider? _) => decimal.Parse(id);

    public DateTime ToDateTime(IFormatProvider? _) => throw new NotImplementedException();

    public string ToString(IFormatProvider? _) => id;

    public object ToType(Type conversionType, IFormatProvider? _) => Convert.ChangeType(id, conversionType);

    public override bool Equals(object? obj)
    {
        return Equals(obj as MessageId);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public int CompareTo(MessageId? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return id.CompareTo(other.id);
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

    public int GetHashCode(MessageId? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.id.GetHashCode();
    }

    public bool Equals(MessageId? other)
    {
        if (other is null)
        {
            return this is null;
        }

        return id.Equals(other.id);
    }

    public int Compare(MessageId? x, MessageId? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        return x.id.CompareTo(y.id);
    }
}

