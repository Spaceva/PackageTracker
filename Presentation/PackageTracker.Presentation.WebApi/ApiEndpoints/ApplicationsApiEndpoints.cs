using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Messages.Queries;
using PackageTracker.Presentation.WebApi.DTOs.Application;
using PackageTracker.Presentation.WebApi.Mappers;

namespace PackageTracker.Presentation.WebApi;

internal static class ApplicationsApiEndpoints
{
    public const string SearchEndpointName = "Search Applications";

    public static IEndpointConventionBuilder MapApplicationsApiEndpoints(this RouteGroupBuilder route)
    {
        route.MapPost("/search", Search).WithRequestTimeout(TimeSpan.FromSeconds(10)).WithDisplayName(SearchEndpointName);

        return route;
    }

    private static async Task<Ok<IReadOnlyCollection<ApplicationDto>>> Search(ApplicationSearchCriteria? applicationSearchCriteria,
        IMediator mediator,
        IMapper mapper,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(applicationSearchCriteria);
        
        var queryResponse = await mediator.Send(new GetApplicationsQuery { SearchCriteria = applicationSearchCriteria }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Application, ApplicationDto>(queryResponse.Applications));
    }
}
