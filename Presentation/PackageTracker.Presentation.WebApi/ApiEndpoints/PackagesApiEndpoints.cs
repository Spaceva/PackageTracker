using AutoMapper;
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
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Presentation.WebApi;

internal static class PackagesApiEndpoints
{
    public static class EndpointNames
    {
        public const string GetAll = "Get All Packages";
        public const string GetByName = "Get Package by Name";
        public const string Search = "Search Packages";
        public const string Track = "Track Package";
        public const string Delete = "Untrack Package";
    }

    public static IEndpointConventionBuilder MapPackagesApiEndpoints(this RouteGroupBuilder route)
    {
        route.MapGet("/", GetAll).WithDisplayNameAndSummary(EndpointNames.GetAll);
        route.MapGet("/{name}", GetByName).WithDisplayNameAndSummary(EndpointNames.GetByName);
        route.MapPost("/search", Search).WithDisplayNameAndSummary(EndpointNames.Search);
        route.MapPost("/track", Track).WithDisplayNameAndSummary(EndpointNames.Track);
        route.MapDelete("/{name}", Delete).WithDisplayNameAndSummary(EndpointNames.Delete);
        return route.WithTags("Packages");
    }

    private static async Task<Ok<IReadOnlyCollection<PackageDto>>> Search(PackageSearchCriteria? packageSearchCriteria, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(packageSearchCriteria);

        var queryResponse = await mediator.Query<GetPackagesQuery, GetPackagesQueryResponse>(new GetPackagesQuery { SearchCriteria = packageSearchCriteria }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Package, PackageDto>(queryResponse.Packages));
    }

    private static async Task<Ok<IReadOnlyCollection<PackageDto>>> GetAll(IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Query<GetPackagesQuery, GetPackagesQueryResponse>(new GetPackagesQuery { SearchCriteria = new PackageSearchCriteria() }, cancellationToken);
        return TypedResults.Ok(mapper.MapCollection<Package, PackageDto>(queryResponse.Packages));
    }

    private static async Task<Ok<IReadOnlyCollection<PackageDto>>> GetByName(string name, IMediator mediator, IMapper mapper, CancellationToken cancellationToken)
    {
        var queryResponse = await mediator.Query<GetPackagesQuery, GetPackagesQueryResponse>(new GetPackagesQuery() { SearchCriteria = new PackageSearchCriteria { Name = HttpUtility.UrlDecode(name) } }, cancellationToken);
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
