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
        return Equals(obj as ChatId);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public int CompareTo(ChatId? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return id.CompareTo(other.id);
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

    public int GetHashCode(ChatId? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.id.GetHashCode();
    }

    public bool Equals(ChatId? other)
    {
        if (other is null)
        {
            return this is null;
        }

        return id.Equals(other.id);
    }

    public int Compare(ChatId? x, ChatId? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        return x.id.CompareTo(y.id);
    }
}

