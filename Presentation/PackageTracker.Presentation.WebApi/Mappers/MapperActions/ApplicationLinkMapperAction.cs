using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Presentation.WebApi.DTOs.Application;
using PackageTracker.Presentation.WebApi.Mappers.MapperActions;

namespace PackageTracker.Presentation.WebApi.Mappers;

internal class ApplicationLinkMapperAction(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider) : BaseLinkMapperAction(apiDescriptionGroupCollectionProvider), IMappingAction<Application, ApplicationDto>
{
    public void Process(Application source, ApplicationDto destination, ResolutionContext context)
    {
        var searchEndpoint = FindEndpoint(ApplicationsApiEndpoints.SearchEndpointName);
        var deleteEndpoint = FindEndpoint(ApplicationsApiEndpoints.DeleteEndpointName);
        var decommissionEndpoint = FindEndpoint(ApplicationsApiEndpoints.DecomissionEndpointName);
        var cancelDecommissionEndpoint = FindEndpoint(ApplicationsApiEndpoints.CancelDecommissionEndpointName);
        var unmarkDeadLinkEndpointName = FindEndpoint(ApplicationsApiEndpoints.UnmarkDeadLinkEndpointName);

        destination.Links = [
            Link(searchEndpoint, "search"),
            Link(deleteEndpoint, "delete"),
            Link(decommissionEndpoint, "decommission"),
            Link(cancelDecommissionEndpoint, "cancelDecommission"),
            Link(unmarkDeadLinkEndpointName, "unmarkDeadLink"),
        ];
    }
}