using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Presentation.WebApi.DTOs.Package;
using PackageTracker.Presentation.WebApi.Mappers.MapperActions;
using System.Web;

namespace PackageTracker.Presentation.WebApi.Mappers;

internal class PackageLinkMapperAction(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider) : BaseLinkMapperAction(apiDescriptionGroupCollectionProvider), IMappingAction<Package, PackageDto>
{
    public void Process(Package source, PackageDto destination, ResolutionContext context)
    {
        var searchEndpoint = FindEndpoint(PackagesApiEndpoints.SearchEndpointName);
        var nameEndpoint = FindEndpoint(PackagesApiEndpoints.GetByNameEndpointName);
        var deleteEndpoint = FindEndpoint(PackagesApiEndpoints.DeleteEndpointName);
        var allEndpoint = FindEndpoint(PackagesApiEndpoints.GetAllEndpointName);

        destination.Links = [
            Link(searchEndpoint, "search"),
            Link(allEndpoint, "collection"),
            ParameteredLink(source, nameEndpoint, "self", s => HttpUtility.UrlEncode(s.Name)),
            ParameteredLink(source, deleteEndpoint, "delete", s => HttpUtility.UrlEncode(s.Name))
        ];
    }
}