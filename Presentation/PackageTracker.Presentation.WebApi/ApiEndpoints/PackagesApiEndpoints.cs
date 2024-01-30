using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Queries;
using PackageTracker.Presentation.WebApi.DTOs.Package;
using PackageTracker.Presentation.WebApi.Mappers;

namespace PackageTracker.Presentation.WebApi;

internal static class PackagesApiEndpoints
{
    public const string GetAllEndpointName = "Get All Packages";
    public const string GetByNameEndpointName = "Get Package by Name";
    public const string SearchEndpointName = "Search Packages";
    public const string FetchEndpointName = "Fetch Packages";
    
    public static IEndpointConventionBuilder MapPackagesApiEndpoints(this RouteGroupBuilder route)
    {
        route.MapGet("/", GetAll).WithDisplayName(GetAllEndpointName);
        route.MapGet("/{name}", GetByName).WithDisplayName(GetByNameEndpointName);
        route.MapPost("/search", Search).WithDisplayName(SearchEndpointName);

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
        var queryResponse = await mediator.Send(new GetPackagesQuery() { SearchCriteria = new PackageSearchCriteria { Name = name } }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Package, PackageDto>(queryResponse.Packages));
    }
}
