namespace PackageTracker.ChatBot;

public class ChatId : IConvertible, IComparable<ChatId>, IEqualityComparer<ChatId>, IEquatable<ChatId>, IComparer<ChatId>
{
    private readonly string id;

    public ChatId(string id)
    {
        this.id = id;
    }

    public ChatId(int id)
    {
        this.id = id.ToString();
    }

    public ChatId(long id)
    {
        this.id = id.ToString();
    }

    public ChatId(ulong id)
    {
        this.id = id.ToString();
    }

    public static implicit operator string(ChatId m)
    {
        return m.id;
    }

    public static implicit operator ChatId(string s)
    {
        return new ChatId(s);
    }

    public static implicit operator int(ChatId m)
    {
        return int.Parse(m.id);
    }

    public static implicit operator ChatId(int i)
    {
        return new ChatId(i);
    }

    public static implicit operator long(ChatId m)
    {
        return long.Parse(m.id);
    }

    public static implicit operator ChatId(long l)
    {
        return new ChatId(l);
    }

    public static implicit operator ulong(ChatId m)
    {
        return ulong.Parse(m.id);
    }

    public static implicit operator ChatId(ulong u)
    {
        return new ChatId(u);
    }

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
        return Equals(obj as ChatId);
    }

    public bool Equals(ChatId? x, ChatId? y)
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

    public bool Equals(ChatId? other)
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

    public int GetHashCode(ChatId? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.id.GetHashCode();
    }

    public int CompareTo(ChatId? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return id.CompareTo(other.id);
    }

    public int Compare(ChatId? x, ChatId? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        return x.id.CompareTo(y.id);
    }
}

