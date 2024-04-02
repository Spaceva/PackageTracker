namespace PackageTracker.Database.MongoDb.Core;

internal static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        return char.ToLowerInvariant(str[0]) + str[1..];
    }

    public static string AsCollectionName(this Type t)
    {
        var collectionName = t.Name.Replace("dbmodel", string.Empty, StringComparison.OrdinalIgnoreCase).ToCamelCase();

        return collectionName.EndsWith('s') ? collectionName : string.Concat(collectionName, "s");
    }
}
