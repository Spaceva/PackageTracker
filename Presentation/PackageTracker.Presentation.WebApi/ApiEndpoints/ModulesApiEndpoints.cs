using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Queries;
using PackageTracker.Domain.Modules;
using PackageTracker.Presentation.WebApi.Mappers;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Presentation.WebApi;

internal static class ModulesApiEndpoints
{
    public static class EndpointNames
    {
        public const string GetAll = "Get All Modules";
        public const string Enable = "Enable Module";
        public const string Disable = "Disable Module";
        public const string Toggle = "Toggle Module";
    }

    public static IEndpointConventionBuilder MapModulesApiEndpoints(this RouteGroupBuilder route)
    {
        route.MapGet("/", GetAll).WithDisplayNameAndSummary(EndpointNames.GetAll);
        route.MapPost("/{name}/enable", Enable).WithDisplayNameAndSummary(EndpointNames.Enable);
        route.MapPost("/{name}/disable", Disable).WithDisplayNameAndSummary(EndpointNames.Disable);
        route.MapPost("/{name}/toggle", Toggle).WithDisplayNameAndSummary(EndpointNames.Toggle);
        return route.WithTags("Modules");
    }

    private static async Task<Ok<IReadOnlyCollection<Module>>> GetAll(IMediator mediator, CancellationToken cancellationToken)
    {
        var response = await mediator.Query<GetModulesQuery, GetModulesQueryResponse>(new GetModulesQuery(), cancellationToken);
        return TypedResults.Ok(response.Modules);
    }

    private static async Task<Ok> Enable(string name, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new EnableModuleCommand { Name = name }, cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Ok> Disable(string name, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DisableModuleCommand { Name = name }, cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Ok> Toggle(string name, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new ToggleModuleCommand { Name = name }, cancellationToken);
        return TypedResults.Ok();
    }
}
