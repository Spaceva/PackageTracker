namespace PackageTracker.Telegram.SDK.Interfaces;

internal interface IAttachment
{
    string? Filename { get; }
    string? Url { get; }
    string? ProxyUrl { get; }
    int Size { get; }
}
