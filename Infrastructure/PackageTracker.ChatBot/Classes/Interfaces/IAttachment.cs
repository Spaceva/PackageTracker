namespace PackageTracker.ChatBot;

public interface IAttachment
{
    string? Filename { get; }
    string? Url { get; }
    string? ProxyUrl { get; }
    int Size { get; }
}
