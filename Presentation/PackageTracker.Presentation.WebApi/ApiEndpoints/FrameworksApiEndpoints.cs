using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using PackageTracker.Domain.Framework;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Queries;
using PackageTracker.Presentation.WebApi.DTOs.Framework;
using PackageTracker.Presentation.WebApi.Mappers;
using System.Web;

namespace PackageTracker.Presentation.WebApi;

internal static class FrameworksApiEndpoints
{
    public static class EndpointNames
    {
        public const string GetAll = "Get All Frameworks";
        public const string GetAllActive = "Get All Active Frameworks";
        public const string GetByName = "Get Framework by Name";
        public const string Delete = "Delete Framework";
        public const string Search = "Search Frameworks";
    }

    public static IEndpointConventionBuilder MapFrameworksApiEndpoints(this RouteGroupBuilder route)
    {
        route.MapGet("/", GetAll).WithDisplayNameAndSummary(EndpointNames.GetAll);
        route.MapGet("/active", GetAllActive).WithDisplayNameAndSummary(EndpointNames.GetAllActive);
        route.MapGet("/{name}", GetByName).WithDisplayNameAndSummary(EndpointNames.GetByName);
        route.MapPost("/search", Search).WithDisplayNameAndSummary(EndpointNames.Search);
        route.MapPost("/delete", Delete).WithDisplayNameAndSummary(EndpointNames.Delete);
        return route.WithTags("Frameworks");
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
        var queryResponse = await mediator.Send(new GetFrameworksQuery { SearchCriteria = new FrameworkSearchCriteria() { Name = HttpUtility.UrlDecode(name) } }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Framework, FrameworkDto>(queryResponse.Frameworks));
    }

    private static async Task<Ok<IReadOnlyCollection<FrameworkDto>>> GetAllActive(IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Send(new GetFrameworksQuery { SearchCriteria = new FrameworkSearchCriteria { Status = [FrameworkStatus.Active, FrameworkStatus.LongTermSupport] } }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Framework, FrameworkDto>(queryResponse.Frameworks));
    }

    private static async Task<Ok> Delete(DeleteFrameworkCommand deleteFrameworkCommand, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(deleteFrameworkCommand, cancellationToken);
        return TypedResults.Ok();
    }
}
