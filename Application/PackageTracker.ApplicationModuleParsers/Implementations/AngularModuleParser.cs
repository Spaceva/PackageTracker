using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PackageTracker.ApplicationModuleParsers;

internal class AngularModuleParser(IPackagesRepository packagesRepository, ILogger<AngularModuleParser> logger) : ApplicationModuleParser<AngularModule>(packagesRepository, logger)
{
    public override bool CanParse(string fileContent)
    {
        try
        {
            var librairiesProperties = Dependencies(fileContent);
            return Array.Exists(librairiesProperties, l => l.Name == Constants.Application.Angular.VersionPropertyName);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override async Task<AngularModule> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken)
    {
        var jsonObject = JsonNode.Parse(fileContent, new JsonNodeOptions { PropertyNameCaseInsensitive = true }, new JsonDocumentOptions { AllowTrailingCommas = true }) ?? throw new JsonException("Parsing failed.");
        var moduleName = jsonObject[Constants.Application.Angular.NameProperty]?.AsValue()?.GetValue<string>() ?? fileName;
        var dependencies = Dependencies(jsonObject);

        var angularVersion = dependencies.SingleOrDefault(l => l.Name == Constants.Application.Angular.VersionPropertyName).Version ?? throw new JsonException("Missing Angular package");
        var packagesTask = dependencies.Select(l => ApplicationPackage(l.Name, l.Version, cancellationToken));
        var packages = await Task.WhenAll(packagesTask);

        return new AngularModule { Name = moduleName, FrameworkVersion = angularVersion, Packages = packages };
    }

    private static (string Name, string Version)[] Dependencies(JsonNode jsonObject)
    {
        var dependencies = jsonObject[Constants.Application.Angular.PackagesProperty]?.AsObject() ?? [];
        var devDependencies = jsonObject[Constants.Application.Angular.DevPackagesProperty]?.AsObject() ?? [];

        return dependencies
                .Union(devDependencies)
                .Where(d => d.Value is not null)
                .Select(dependency => (Name: dependency.Key, Version: dependency.Value!.GetValue<string>()))
                .Select(dependency => (dependency.Name, Version: TransformVersion(dependency.Version)))
                .Where(d => Domain.Package.Constants.RegularExpressions.AnyVersionNumber.IsMatch(d.Version))
                .ToArray();
    }

    private static string TransformVersion(string version)
     => version.Replace("^", string.Empty).Replace("~", string.Empty);
}
