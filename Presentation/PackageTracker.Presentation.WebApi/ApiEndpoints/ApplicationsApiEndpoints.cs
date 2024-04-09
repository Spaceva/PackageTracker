using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Exceptions;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Queries;
using PackageTracker.Presentation.WebApi.DTOs.Application;
using PackageTracker.Presentation.WebApi.Mappers;

namespace PackageTracker.Presentation.WebApi;

internal static class ApplicationsApiEndpoints
{
    public const string SearchEndpointName = "Search Applications";
    public const string DeleteEndpointName = "Delete Application";
    public const string DecomissionEndpointName = "Decomission Application";
    public const string CancelDecommissionEndpointName = "Cancel Decomission Application";
    public const string UnmarkDeadLinkEndpointName = "Unmark Dead Link on Application";

    public static IEndpointConventionBuilder MapApplicationsApiEndpoints(this RouteGroupBuilder route)
    {
        route.MapPost("/search", Search).WithDisplayName(SearchEndpointName);
        route.MapPost("/delete", Delete).WithDisplayName(DeleteEndpointName);
        route.MapPost("/decommission", Decommission).WithDisplayName(DecomissionEndpointName);
        route.MapPost("/decommission/cancel", CancelDecommission).WithDisplayName(CancelDecommissionEndpointName);
        route.MapPost("/deadlink/unmark", UnmarkDeadLink).WithDisplayName(UnmarkDeadLinkEndpointName);

        return route;
    }

    private static async Task<Ok<IReadOnlyCollection<ApplicationDto>>> Search(ApplicationSearchCriteria? applicationSearchCriteria, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(applicationSearchCriteria);

        var queryResponse = await mediator.Send(new GetApplicationsQuery { SearchCriteria = applicationSearchCriteria }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Application, ApplicationDto>(queryResponse.Applications));
    }

    private static async Task<Ok> Delete(DeleteApplicationCommand deleteApplicationCommand, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(deleteApplicationCommand, cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Ok> Decommission(ApplicationRequestDto applicationRequest, IMediator mediator, CancellationToken cancellationToken)
    {
        await PatchApplicationAsync(a => a.IsSoonDecommissioned = true, applicationRequest, mediator, cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Ok> CancelDecommission(ApplicationRequestDto applicationRequest, IMediator mediator, CancellationToken cancellationToken)
    {
        await PatchApplicationAsync(a => a.IsSoonDecommissioned = false, applicationRequest, mediator, cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Ok> UnmarkDeadLink(ApplicationRequestDto applicationRequest, IMediator mediator, CancellationToken cancellationToken)
    {
        await PatchApplicationAsync(a => a.IsDeadLink = false, applicationRequest, mediator, cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task PatchApplicationAsync(Action<Application> patch, ApplicationRequestDto applicationRequestBody, IMediator mediator, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Send(new GetApplicationQuery { Name = applicationRequestBody.Name, RepositoryLink = applicationRequestBody.RepositoryLink, Type = applicationRequestBody.Type }, cancellationToken);
        var application = queryResponse.Application ?? throw new ApplicationNotFoundException();
        patch(application);
        await mediator.Send(new UpdateApplicationCommand { Application = application }, cancellationToken);
    }
}
