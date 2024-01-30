using System.Text.Json.Serialization;

namespace PackageTracker.Scanner.Gitlab;
internal class PersonnalAccessToken
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("revoked")]
    public bool Revoked { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("scopes")]
    public IReadOnlyCollection<string> Scopes { get; set; } = Array.Empty<string>();

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("last_used_at")]
    public DateTime LastUsedAt { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("expires_at")]
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly ExpiresAt { get; set; }
}
