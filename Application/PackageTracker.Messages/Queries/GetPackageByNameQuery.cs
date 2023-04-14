namespace PackageTracker.Messages.Queries;

using MediatR;

public class GetPackageByNameQuery : IRequest<GetPackageByNameQueryResponse>
{
    public string PackageName { get; init; } = default!;
}
