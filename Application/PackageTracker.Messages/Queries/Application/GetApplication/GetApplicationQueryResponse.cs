using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages.Queries;

public class GetApplicationQueryResponse
{
    public Application Application { get; init; } = default!;
}