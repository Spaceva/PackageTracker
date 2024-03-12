using MediatR;
using PackageTracker.Domain.Framework;
using PackageTracker.Messages.Queries;

namespace PackageTracker.Handlers.Queries;

internal class GetFrameworksQueryHandler(IFrameworkRepository frameworkRepository) : IRequestHandler<GetFrameworksQuery, GetFrameworksQueryResponse>
{
    public async Task<GetFrameworksQueryResponse> Handle(GetFrameworksQuery request, CancellationToken cancellationToken)
    {
        var take = request.PageSize;
        var skip = request.PageNumber;
        if (take is not null && skip is not null)
        {
            skip *= take.Value;
        }

        var frameworks = await frameworkRepository.SearchAsync(request.SearchCriteria, skip, take, cancellationToken);
        return new GetFrameworksQueryResponse
        {
            Frameworks = [.. frameworks.OrderBy(f => f.Name)
                                   .ThenByDescending(f => int.TryParse(f.Channel, out var channel) ? channel.ToString("00.##") : f.Channel)
                                   .ThenByDescending(f => f.Version)]
        };
    }
}
