using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace PackageTracker.Presentation.WebApi.Mappers;
internal class WebApiExplorer(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider)
{
    public bool TryFindEndpoint(string displayName, out ApiDescription? endpoint)
    {
        endpoint = MaybeFindEndpoint(displayName);
        return endpoint is not null;
    }

    public ApiDescription FindEndpoint(string displayName)
    {
        return MaybeFindEndpoint(displayName) ?? throw new ArgumentException(displayName, nameof(displayName));
    }

    private ApiDescription? MaybeFindEndpoint(string displayName)
    {
        return apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items.SelectMany(g => g.Items).SingleOrDefault(endpoint => endpoint.ActionDescriptor.DisplayName is not null && endpoint.ActionDescriptor.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase));
    }
}
