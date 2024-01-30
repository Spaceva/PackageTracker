using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Messages.Queries;
using PackageTracker.Presentation.WebApi.DTOs.Framework;
using PackageTracker.Presentation.WebApi.Mappers;

namespace PackageTracker.Presentation.WebApi;

internal static class FrameworksApiEndpoints
{
    public const string GetAllEndpointName = "Get All Frameworks";
    public const string GetAllActiveEndpointName = "Get All Active Frameworks";
    public const string GetByNameEndpointName = "Get Framework by Name";
    public const string SearchEndpointName = "Search Frameworks";
    
    public static IEndpointConventionBuilder MapFrameworksApiEndpoints(this RouteGroupBuilder route)
    {
        route.MapGet("/", GetAll).WithDisplayName(GetAllEndpointName);
        route.MapGet("/active", GetAllActive).WithDisplayName(GetAllActiveEndpointName);
        route.MapGet("/{name}", GetByName).WithDisplayName(GetByNameEndpointName);
        route.MapPost("/search", Search).WithDisplayName(SearchEndpointName);
        return route;
    }

    private static async Task<Ok<IReadOnlyCollection<FrameworkDto>>> Search(FrameworkSearchCriteria? frameworkSearchCriteria, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(frameworkSearchCriteria);

        var queryResponse = await mediator.Send(new GetFrameworksQuery { SearchCriteria = frameworkSearchCriteria }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Framework, FrameworkDto>(queryResponse.Frameworks));
    }

    private static async Task<Ok<IReadOnlyCollection<FrameworkDto>>> GetAll(IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Send(new GetFrameworksQuery { SearchCriteria = new FrameworkSearchCriteria() }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Framework, FrameworkDto>(queryResponse.Frameworks));
    }

    private static async Task<Ok<IReadOnlyCollection<FrameworkDto>>> GetByName(string name, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Send(new GetFrameworksQuery { SearchCriteria = new FrameworkSearchCriteria() { Name = name } }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Framework, FrameworkDto>(queryResponse.Frameworks));
    }

    private static async Task<Ok<IReadOnlyCollection<FrameworkDto>>> GetAllActive(IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Send(new GetFrameworksQuery { SearchCriteria = new FrameworkSearchCriteria { Status = [FrameworkStatus.Active, FrameworkStatus.LongTermSupport] } }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Framework, FrameworkDto>(queryResponse.Frameworks));
    }
}
