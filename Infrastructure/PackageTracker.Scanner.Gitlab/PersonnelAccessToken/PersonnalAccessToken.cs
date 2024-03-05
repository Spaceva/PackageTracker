using System.Text.Json.Serialization;

namespace PackageTracker.Scanner.Gitlab;
internal class PersonnalAccessToken
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;

    [JsonPropertyName("revoked")]
    public bool Revoked { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("scopes")]
    public IReadOnlyCollection<string> Scopes { get; init; } = [];

    [JsonPropertyName("user_id")]
    public int UserId { get; init; }

    [JsonPropertyName("last_used_at")]
    public DateTime LastUsedAt { get; init; }

    [JsonPropertyName("active")]
    public bool Active { get; init; }

    [JsonPropertyName("expires_at")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly ExpiresAt { get; init; }
}
