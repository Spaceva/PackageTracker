using System.Text.Json;
using System.Text.RegularExpressions;

namespace PackageTracker.Domain.Extensions;

internal static class RegexExtensions
{
    public static T MatchTo<T>(this Regex regex, string input)
    {
        var namedCaptures = regex.MatchNamedCaptures(input);
        if (namedCaptures.Count == 0)
        {
            throw new ArgumentException("No match");
        }

        var jsonText = JsonSerializer.Serialize(namedCaptures);

        return JsonSerializer.Deserialize<T>(jsonText)!;
    }

    public static bool TryMatchTo<T>(this Regex regex, string input, out T? value)
    {
        try
        {
            value = regex.MatchTo<T>(input);
            return true;
        }
        catch (Exception)
        {
            value = default;
            return false;
        }
    }

    private static IDictionary<string, string> MatchNamedCaptures(this Regex regex, string input)
    {
        var namedCaptureDictionary = new Dictionary<string, string>();
        var groups = regex.Match(input).Groups;
        var groupNames = regex.GetGroupNames();
        foreach (string groupName in groupNames.Skip(1).Where(groupName => groups[groupName].Captures.Count > 0))
        {
            namedCaptureDictionary.Add(groupName, groups[groupName].Value);
        }

        return namedCaptureDictionary;
    }
}
