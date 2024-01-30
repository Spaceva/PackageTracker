using Microsoft.AspNetCore.Mvc.ApiExplorer;
using PackageTracker.Presentation.WebApi.DTOs;

namespace PackageTracker.Presentation.WebApi.Mappers.MapperActions;

internal abstract class BaseLinkMapperAction(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
{
    protected bool TryFindEndpoint(string displayName, out ApiDescription? endpoint)
    {
        endpoint = MaybeFindEndpoint(displayName);
        return endpoint is not null;
    }

    protected static LinkDto Link(ApiDescription endpoint, string rel)
    {
        return new LinkDto
        {
            Method = endpoint.HttpMethod!,
            Rel = rel,
            Href = endpoint.RelativePath!,
        };
    }

    protected static LinkDto ParameteredLink<TSource>(TSource source, ApiDescription endpoint, string rel, params Func<TSource, string>[] valueSelectors)
    {
        if (valueSelectors.Length != endpoint.ParameterDescriptions.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(valueSelectors), $"Endpoint has {endpoint.ParameterDescriptions.Count} parameters, got ${valueSelectors.Length} value selector(s).");
        }

        var url = endpoint.RelativePath ?? string.Empty;
        for (var index = 0; index < endpoint.ParameterDescriptions.Count; index++)
        {
            var valueSelector = valueSelectors[index];
            var parameter = endpoint.ParameterDescriptions[index];
            var value = valueSelector(source);

            url = url.Replace($"{{{parameter.Name}}}", value);
        }

        return new LinkDto
        {
            Method = endpoint.HttpMethod!,
            Rel = rel,
            Href = url!,
        };
    }

    protected ApiDescription FindEndpoint(string displayName)
    {
        return MaybeFindEndpoint(displayName) ?? throw new ArgumentException(displayName, nameof(displayName));
    }

    private ApiDescription? MaybeFindEndpoint(string displayName)
    {
        return apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items.SelectMany(g => g.Items).SingleOrDefault(endpoint => endpoint.ActionDescriptor.DisplayName is not null && endpoint.ActionDescriptor.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
    }
}