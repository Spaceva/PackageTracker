namespace PackageTracker.Domain.Framework.Model;

public class Framework
{
    public string Name { get; set; } = default!;

    public string Version { get; set; } = default!;

    public string Channel { get; set; } = default!;

    public string? CodeName { get; set; }

    public FrameworkStatus Status { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public DateTime? EndOfLife { get; set; }

    public bool IsEnded => EndOfLife is not null && EndOfLife < DateTime.UtcNow;
}