using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PackageTracker.ApplicationModuleParsers;

internal abstract class NodeModuleParser(IPackagesRepository packagesRepository, ILogger logger) : ApplicationModuleParser(packagesRepository, logger)
{
    protected abstract string FrameworkPackageName { get; }

    public override bool CanParse(string fileContent)
    {
        try
        {
            var jsonObject = JsonNode.Parse(fileContent, new JsonNodeOptions { PropertyNameCaseInsensitive = true }, new JsonDocumentOptions { AllowTrailingCommas = true }) ?? throw new JsonException("Parsing failed.");
            var librairiesProperties = Dependencies(jsonObject);
            return Array.Exists(librairiesProperties, l => l.Name == FrameworkPackageName);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override bool IsModuleFile(string fileAbsolutePath)
    => fileAbsolutePath.EndsWith("package.json", StringComparison.OrdinalIgnoreCase)
        && !fileAbsolutePath.Contains("public", StringComparison.OrdinalIgnoreCase)
        && !fileAbsolutePath.Contains("resource", StringComparison.OrdinalIgnoreCase);

    public override async Task<ApplicationModule> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken)
    {
        var jsonObject = JsonNode.Parse(fileContent, new JsonNodeOptions { PropertyNameCaseInsensitive = true }, new JsonDocumentOptions { AllowTrailingCommas = true }) ?? throw new JsonException("Parsing failed.");
        var moduleName = jsonObject[Constants.Application.NodeJs.NameProperty]?.AsValue()?.GetValue<string>() ?? fileName;
        var dependencies = Dependencies(jsonObject);

        var frameworkVersion = dependencies.SingleOrDefault(l => l.Name == FrameworkPackageName).Version ?? throw new JsonException($"Missing {FrameworkPackageName} package.");
        var packagesTask = dependencies.Select(l => ApplicationPackage(l.Name, l.Version, cancellationToken));
        var packages = await Task.WhenAll(packagesTask);

        return ApplicationModule(moduleName, frameworkVersion, packages ?? []);
    }

    protected abstract ApplicationModule ApplicationModule(string moduleName, string frameworkVersion, ApplicationPackage[] packages);

    private static (string Name, string Version)[] Dependencies(JsonNode jsonObject)
    {
        var dependencies = jsonObject[Constants.Application.NodeJs.PackagesProperty]?.AsObject() ?? [];
        var devDependencies = jsonObject[Constants.Application.NodeJs.DevPackagesProperty]?.AsObject() ?? [];

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
