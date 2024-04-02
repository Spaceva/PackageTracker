using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;
using System.Xml.Linq;

namespace PackageTracker.ApplicationModuleParsers;
internal class DotNetAssemblyParser(IPackagesRepository packagesRepository, ILogger<DotNetAssemblyParser> logger) : ApplicationModuleParser<DotNetAssembly>(packagesRepository, logger)
{
    public override bool CanParse(string fileContent)
    {
        try
        {
            var positionOfHeader = fileContent.IndexOf("<Project");
            var cleanFileContent = fileContent[positionOfHeader..].Trim();
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
        var positionOfHeader = fileContent.IndexOf("<Project");
        var cleanFileContent = fileContent[positionOfHeader..].Trim();
        var csProjectContent = XElement.Parse(cleanFileContent);

        var dotnetVersion = csProjectContent
            .Descendants()
            .Single(IsDotNetElement)
            .Value
            .Replace("coreapp", string.Empty)
            .Replace("net", string.Empty)
            .Replace("standard", "Standard ")
            .Trim();

        if (int.TryParse(dotnetVersion, out var dotNetVersionInt) && dotNetVersionInt >= 400 && dotNetVersionInt < 500)
        {
            dotnetVersion = $"Framework {string.Join(".", dotnetVersion.Select(c => $"{c}"))}";
        }

        var librairiesTasks = csProjectContent
            .Descendants()
            .Where(IsLibraryElement)
            .Select(element => new
            {
                Name = element.Attribute(Constants.Application.DotNet.XMLLibraryNameAttribute)!.Value,
                Version = element.Attribute(Constants.Application.DotNet.XMLLibraryVersionAttribute)!.Value
            })
            .Where(d => Domain.Package.Constants.RegularExpressions.AnyVersionNumber.IsMatch(d.Version))
            .Select(e => ApplicationPackage(e.Name, NormalizeVersion(e.Version), cancellationToken));

        var librairiesVersions = await Task.WhenAll(librairiesTasks);

        return new DotNetAssembly { Name = fileName, FrameworkVersion = dotnetVersion, Packages = [.. librairiesVersions.OrderBy(p => p.PackageName)] };
    }

    private static bool IsLibraryElement(XElement element)
     => element.Name is not null
     && element.Name == Constants.Application.DotNet.XMLLibraryNodeName
     && element.HasAttributes
     && element.Attribute(Constants.Application.DotNet.XMLLibraryNameAttribute) is not null
     && element.Attribute(Constants.Application.DotNet.XMLLibraryVersionAttribute) is not null;

    private static bool IsDotNetElement(XElement element)
     => element.Name is not null
     && element.Name == Constants.Application.DotNet.XMLDotnetVersionNodeName;
}
