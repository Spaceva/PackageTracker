﻿using Microsoft.Extensions.Logging;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package;
using PackageTracker.Domain.Package.Model;
using System.Xml.Linq;

namespace PackageTracker.ApplicationModuleParsers;
internal class JavaModuleParser(IPackagesRepository packagesRepository, ILogger<JavaModuleParser> logger) : ApplicationModuleParser<JavaModule>(packagesRepository, logger)
{
    public override bool CanParse(string fileContent)
    {
        try
        {
            var positionOfHeader = fileContent.IndexOf($"<{Constants.Application.Java.XMLJavaVersionNodeName}>");
            var cleanFileContent = fileContent[positionOfHeader..].Trim();
            var povFile = XElement.Parse(cleanFileContent);
            var hasJavaVersion = povFile
                .Descendants()
                .SingleOrDefault(IsPropertiesElement)
                ?.Descendants()
                ?.SingleOrDefault(IsJavaElement) is not null;

            return hasJavaVersion;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override async Task<JavaModule> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken)
    {
        var positionOfHeader = fileContent.IndexOf($"<{Constants.Application.Java.XMLJavaVersionNodeName}>");
        var cleanFileContent = fileContent[positionOfHeader..].Trim();
        var povFile = XElement.Parse(cleanFileContent);

        var propertiesNode = povFile
                .Descendants()
                .Single(IsPropertiesElement);

        var javaVersion = propertiesNode
                .Descendants()
                .Single(IsJavaElement)
                .Value
                .Trim();

        var librairiesTasks = povFile
            .Descendants()
            .Where(IsLibraryElement)
            .SelectMany(element => element.Descendants())
            .Select(element => ParseLibraryElement(element, propertiesNode))
            .Where(package => package.Name is not null && package.Version is not null)
            .Select((package) => ApplicationPackage(package.Name!, package.Version!, cancellationToken));

        var librairiesVersions = await Task.WhenAll(librairiesTasks);

        return new JavaModule { Name = fileName, FrameworkVersion = javaVersion, Packages = [.. librairiesVersions.OrderBy(p => p.PackageName)] };
    }

    private static (string? Name, string? Version) ParseLibraryElement(XElement dependencyNode, XElement propertiesNode)
    {
        var name = dependencyNode.Descendants().Single(d => d.Name.LocalName.Equals(Constants.Application.Java.XMLArtifactIdNodeName, StringComparison.OrdinalIgnoreCase)).Value;
        var version = dependencyNode.Descendants().Single(d => d.Name.LocalName.Equals(Constants.Application.Java.XMLArtifactVersionNodeName, StringComparison.OrdinalIgnoreCase)).Value;
        if (!PackageVersion.IsValid(version))
        {
            var matchingProperty = propertiesNode.Descendants().SingleOrDefault(n => n.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (matchingProperty is null)
            {
                return (null, null);
            }

            version = matchingProperty.Value;
        }

        return (name, version);
    }

    private static bool IsLibraryElement(XElement element)
     => element.Name?.LocalName == Constants.Application.Java.XMLDependencyManagementNodeName
     && element.Descendants().All(e => e.Name.LocalName.Equals(Constants.Application.Java.XMLDependencyNodeName));

    private static bool IsPropertiesElement(XElement element)
     => element.Name?.LocalName == Constants.Application.Java.XMLPropertiesNode;

    private static bool IsJavaElement(XElement element)
     => element.Name?.LocalName == Constants.Application.Java.XMLJavaVersionNodeName;
}
