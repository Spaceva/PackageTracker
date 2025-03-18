using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Presentation.WebApi.DTOs.Framework;
using PackageTracker.Presentation.WebApi.Mappers.MapperActions;
using System.Web;

namespace PackageTracker.Presentation.WebApi.Mappers;

internal class FrameworkLinkMapperAction(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider) : BaseLinkMapperAction(apiDescriptionGroupCollectionProvider), IMappingAction<Framework, FrameworkDto>
{
    public void Process(Framework source, FrameworkDto destination, ResolutionContext context)
    {
        var searchEndpoint = FindEndpoint(FrameworksApiEndpoints.EndpointNames.Search);
        var allEndpoint = FindEndpoint(FrameworksApiEndpoints.EndpointNames.GetAll);
        var allActiveEndpoint = FindEndpoint(FrameworksApiEndpoints.EndpointNames.GetAllActive);
        var nameEndpoint = FindEndpoint(FrameworksApiEndpoints.EndpointNames.GetByName);
        var deleteEndpoint = FindEndpoint(FrameworksApiEndpoints.EndpointNames.Delete);

        destination.Links = [
            Link(searchEndpoint, "search"),
            Link(allEndpoint, "collection"),
            Link(allActiveEndpoint, "collection/active"),
            ParameteredLink(source, nameEndpoint, "self", s => HttpUtility.UrlEncode(s.Name)),
            Link(deleteEndpoint, "delete"),
        ];
    }
}