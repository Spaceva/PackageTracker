namespace PackageTracker.ChatBot;

public class ChatPermission : IConvertible, IComparable<ChatPermission>, IEqualityComparer<ChatPermission>, IEquatable<ChatPermission>, IComparer<ChatPermission>
{
    private readonly string id;

    public ChatPermission(string id)
    {
        this.id = id;
    }

    public ChatPermission(int id)
    {
        this.id = id.ToString();
    }

    public ChatPermission(long id)
    {
        this.id = id.ToString();
    }

    public ChatPermission(ulong id)
    {
        this.id = id.ToString();
    }

    public static implicit operator string(ChatPermission m)
    {
        return m.id;
    }

    public static implicit operator ChatPermission(string s)
    {
        return new ChatPermission(s);
    }

    public static implicit operator int(ChatPermission m)
    {
        return int.Parse(m.id);
    }

    public static implicit operator ChatPermission(int i)
    {
        return new ChatPermission(i);
    }

    public static implicit operator long(ChatPermission m)
    {
        return long.Parse(m.id);
    }

    public static implicit operator ChatPermission(long l)
    {
        return new ChatPermission(l);
    }

    public static implicit operator ulong(ChatPermission m)
    {
        return ulong.Parse(m.id);
    }

    public static implicit operator ChatPermission(ulong u)
    {
        return new ChatPermission(u);
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
        return Equals(obj as ChatPermission);
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }

    public int CompareTo(ChatPermission? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return id.CompareTo(other.id);
    }

    public bool Equals(ChatPermission? x, ChatPermission? y)
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

    public int GetHashCode(ChatPermission? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return obj.id.GetHashCode();
    }

    public bool Equals(ChatPermission? other)
    {
        if (other is null)
        {
            return this is null;
        }

        return id.Equals(other.id);
    }

    public int Compare(ChatPermission? x, ChatPermission? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        return x.id.CompareTo(y.id);
    }
}
