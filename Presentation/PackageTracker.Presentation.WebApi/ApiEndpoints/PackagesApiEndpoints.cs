using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Queries;
using PackageTracker.Presentation.WebApi.DTOs.Package;
using PackageTracker.Presentation.WebApi.Mappers;
using System.Web;

namespace PackageTracker.Presentation.WebApi;

internal static class PackagesApiEndpoints
{
    public const string GetAllEndpointName = "Get All Packages";
    public const string GetByNameEndpointName = "Get Package by Name";
    public const string SearchEndpointName = "Search Packages";
    public const string TrackEndpointName = "Track Package";
    public const string DeleteEndpointName = "Untrack Package";
    public const string FetchEndpointName = "Fetch Packages";

    public static IEndpointConventionBuilder MapPackagesApiEndpoints(this RouteGroupBuilder route)
    {
        route.MapGet("/", GetAll).WithDisplayName(GetAllEndpointName);
        route.MapGet("/{name}", GetByName).WithDisplayName(GetByNameEndpointName);
        route.MapPost("/search", Search).WithDisplayName(SearchEndpointName);
        route.MapPost("/track", Track).WithDisplayName(TrackEndpointName);
        route.MapDelete("/{name}", Delete).WithDisplayName(DeleteEndpointName);
        return route;
    }

    private static async Task<Ok<IReadOnlyCollection<PackageDto>>> Search(PackageSearchCriteria? packageSearchCriteria, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(packageSearchCriteria);

        var queryResponse = await mediator.Send(new GetPackagesQuery { SearchCriteria = packageSearchCriteria }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Package, PackageDto>(queryResponse.Packages));
    }

    private static async Task<Ok<IReadOnlyCollection<PackageDto>>> GetAll(IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Send(new GetPackagesQuery { SearchCriteria = new PackageSearchCriteria() }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Package, PackageDto>(queryResponse.Packages));
    }

    private static async Task<Ok<IReadOnlyCollection<PackageDto>>> GetByName(string name, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Send(new GetPackagesQuery() { SearchCriteria = new PackageSearchCriteria { Name = HttpUtility.UrlDecode(name) } }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Package, PackageDto>(queryResponse.Packages));
    }

    private static async Task<Ok> Track(TrackPackageRequestDto trackRequest, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new FetchPackageCommand(trackRequest.Name, trackRequest.Type), cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Ok> Delete(string name, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeletePackageCommand { Name = name }, cancellationToken);
        return TypedResults.Ok();
    }
}
