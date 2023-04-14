using PackageTracker.Telegram.SDK.Interfaces;

namespace PackageTracker.Telegram.SDK.Base;

internal class TelegramAttachment : IAttachment
{
    public string? Filename { get; init; }

    public string? Url { get; init; }

    public string? ProxyUrl { get; init; }

    public int Size { get; init; }
}
