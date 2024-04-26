using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;
using System.Xml.Linq;

namespace PackageTracker.ApplicationModuleParsers;
internal class DotNetFrameworkAssemblyParser(IPackagesRepository packagesRepository, ILogger<DotNetAssemblyParser> logger) : ApplicationModuleParser<DotNetAssembly>(packagesRepository, logger)
{
    public override bool CanParse(string fileContent)
    {
        try
        {
            var projectPosition = fileContent.IndexOf("<Project");
            if (projectPosition < 0)
            {
                return false;
            }

            var cleanFileContent = fileContent[projectPosition..].Trim();
            var csProjectContent = XElement.Parse(cleanFileContent);
            var hasDotnetVersion = csProjectContent
                .Descendants()
                .SingleOrDefault(IsDotNetElement) is not null;

            return hasDotnetVersion;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override async Task<DotNetAssembly> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken)
    {
        var projectPosition = fileContent.IndexOf("<Project");
        var cleanFileContent = fileContent[projectPosition..].Trim();
        var csProjectContent = XElement.Parse(cleanFileContent);

        var dotnetVersion = csProjectContent
            .Descendants()
            .Single(IsDotNetElement)
            .Value
            .Replace("v", "Framework ")
            .Trim();

        var librairiesTasks = csProjectContent
            .Descendants()
            .Where(IsLibraryElement)
            .Select(element => ParseLibraryElement(element.Attribute(Constants.Application.DotNetFramework.XMLLibraryNameAndVersionAttribute)!.Value))
            .Select(e => ApplicationPackage(e.Name, NormalizeVersion(e.Version), cancellationToken));

        var librairiesVersions = await Task.WhenAll(librairiesTasks);

        return new DotNetAssembly { Name = fileName, FrameworkVersion = dotnetVersion, Packages = [.. librairiesVersions.OrderBy(p => p.PackageName)] };
    }

    private static (string Name, string Version) ParseLibraryElement(string value)
    {
        var splitByCommas = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var name = splitByCommas[0].Trim();
        var version = splitByCommas.Single(v => v.Trim().StartsWith(Constants.Application.DotNetFramework.XMLLibraryVersionSubAttribute + "="))
                                   .Replace(Constants.Application.DotNetFramework.XMLLibraryVersionSubAttribute + "=", string.Empty)
                                   .Trim();
        return (name, version);
    }

    private static bool IsLibraryElement(XElement element)
     => element.Name?.LocalName == Constants.Application.DotNetFramework.XMLLibraryNodeName
     && element.HasAttributes
     && element.Attribute(Constants.Application.DotNetFramework.XMLLibraryNameAndVersionAttribute)?.Value is not null
     && element.Attribute(Constants.Application.DotNetFramework.XMLLibraryNameAndVersionAttribute)!.Value.Trim().Contains($"{Constants.Application.DotNetFramework.XMLLibraryVersionSubAttribute}=");

    private static bool IsDotNetElement(XElement element)
     => element.Name?.LocalName == Constants.Application.DotNetFramework.XMLDotnetVersionNodeName;
}
