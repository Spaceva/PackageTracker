﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PackageTracker.Domain.Framework.Model;
using PackageTracker.Presentation.WebApi.DTOs.Framework;
using PackageTracker.Presentation.WebApi.Mappers.MapperActions;

namespace PackageTracker.Presentation.WebApi.Mappers;

internal class FrameworkLinkMapperAction(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider) : BaseLinkMapperAction(apiDescriptionGroupCollectionProvider), IMappingAction<Framework, FrameworkDto>
{
    public void Process(Framework source, FrameworkDto destination, ResolutionContext context)
    {
        var searchEndpoint = FindEndpoint(FrameworksApiEndpoints.SearchEndpointName);
        var allEndpoint = FindEndpoint(FrameworksApiEndpoints.GetAllEndpointName);
        var allActiveEndpoint = FindEndpoint(FrameworksApiEndpoints.GetAllActiveEndpointName);
        var nameEndpoint = FindEndpoint(FrameworksApiEndpoints.GetByNameEndpointName);

        destination.Links = [
            Link(searchEndpoint, "search"),
            Link(allEndpoint, "collection"),
            Link(allActiveEndpoint, "collection/active"),
            ParameteredLink(source, nameEndpoint, "self", s => s.Name)
        ];
    }
}