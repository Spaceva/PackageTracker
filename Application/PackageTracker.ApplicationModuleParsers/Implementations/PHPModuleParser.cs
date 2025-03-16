using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;
using System.Text.Json.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PackageTracker.ApplicationModuleParsers;

internal class PhpModuleParser(IPackagesRepository packagesRepository, ILogger<PhpModuleParser> logger) : ApplicationModuleParser(packagesRepository, logger)
{
    public override bool CanParse(string fileContent)
    {
        try
        {
            var jsonObject = JsonNode.Parse(fileContent, new JsonNodeOptions { PropertyNameCaseInsensitive = true }, new JsonDocumentOptions { AllowTrailingCommas = true }) ?? throw new JsonException("Parsing failed.");
            var librairiesProperties = Dependencies(jsonObject);

            return Array.Exists(librairiesProperties, l => l.Name == Constants.Application.Php.VersionPropertyName);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override bool IsModuleFile(string fileAbsolutePath)
     => fileAbsolutePath.EndsWith("composer.json", StringComparison.OrdinalIgnoreCase)
        && !fileAbsolutePath.Contains("public", StringComparison.OrdinalIgnoreCase)
        && !fileAbsolutePath.Contains("resource", StringComparison.OrdinalIgnoreCase);

    public override async Task<ApplicationModule> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken)
    {
        var jsonObject = JsonNode.Parse(fileContent, new JsonNodeOptions { PropertyNameCaseInsensitive = true }, new JsonDocumentOptions { AllowTrailingCommas = true }) ?? throw new JsonException("Parsing failed.");
        var moduleName = jsonObject[Constants.Application.Php.NameProperty]?.AsValue()?.GetValue<string>() ?? fileName;
        var dependencies = Dependencies(jsonObject);

        var phpVersion = dependencies.SingleOrDefault(l => l.Name == Constants.Application.Php.VersionPropertyName).Version ?? throw new JsonException("Missing PHP package");
        var packagesTask = dependencies.Select(l => ApplicationPackage(l.Name, l.Version, cancellationToken));
        var packages = await Task.WhenAll(packagesTask);

        return new PhpModule { Name = moduleName, FrameworkVersion = phpVersion, Packages = packages };
    }

    private static (string Name, string Version)[] Dependencies(JsonNode jsonObject)
    {
        var dependencies = jsonObject[Constants.Application.Php.PackagesProperty]?.AsObject() ?? [];
        var devDependencies = jsonObject[Constants.Application.Php.DevPackagesProperty]?.AsObject() ?? [];

        return [.. dependencies
                .Union(devDependencies)
                .Where(d => d.Value is not null)
                .Select(dependency => (Name: dependency.Key, Version: dependency.Value!.GetValue<string>()))
                .Select(dependency => (dependency.Name, Version: NormalizeVersion(CleanPackagistVersion(dependency.Version))))
                .Where(d => Domain.Package.Constants.RegularExpressions.AnyVersionNumber.IsMatch(d.Version))];
    }

    private static string CleanPackagistVersion(string version)
        => version.Replace(">=", string.Empty).Replace(">", string.Empty).Replace("^", string.Empty).Replace("~", string.Empty).Replace("*", "0").Split(',')[0].Split("||")[^1].Split("|")[^1];
}
