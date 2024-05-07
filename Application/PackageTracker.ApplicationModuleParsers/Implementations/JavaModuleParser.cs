using Microsoft.Extensions.Logging;
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
            var javaVersionPosition = FindJavaVersionPosition(fileContent);
            if (javaVersionPosition < 0)
            {
                return false;
            }

            var cleanFileContent = fileContent.Trim();
            var povFile = XElement.Parse(cleanFileContent);
            var hasJavaVersion = povFile
                .Descendants()
                .FirstOrDefault(IsJavaElement) is not null;

            return hasJavaVersion;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override async Task<JavaModule> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken)
    {
        var javaVersionPosition = FindJavaVersionPosition(fileContent);
        var cleanFileContent = fileContent.Trim();
        var pomFileRootNode = XElement.Parse(cleanFileContent);

        var propertiesNode = pomFileRootNode
                .Descendants()
                .Single(IsPropertiesElement);

        var javaVersion = GetJavaVersion(propertiesNode.Descendants().Where(IsJavaElement));

        var artifactName = pomFileRootNode.Descendants().Single(d => d.Name?.LocalName == Constants.Application.Java.XMLArtifactIdNodeName
        && d.Parent is not null 
        && d.Parent.Name.Equals(pomFileRootNode.Name)).Value;

        var librairiesTasks = pomFileRootNode
            .Descendants()
            .Where(IsLibraryElement)
            .Select(element => ParseLibraryElement(element, propertiesNode))
            .Where(package => package.Name is not null && package.Version is not null)
            .Select((package) => ApplicationPackage(package.Name!, package.Version!, cancellationToken));

        var librairiesVersions = await Task.WhenAll(librairiesTasks);

        return new JavaModule { Name = artifactName, FrameworkVersion = javaVersion, Packages = [.. librairiesVersions.OrderBy(p => p.PackageName)] };
    }

    private static string GetJavaVersion(IEnumerable<XElement> nodes)
    {
        var node = nodes.SingleOrDefault(e => e.Name?.LocalName == Constants.Application.Java.XMLJavaVersionNodeName)
         ?? nodes.SingleOrDefault(e => e.Name?.LocalName == Constants.Application.Java.XMLJavaVersionFallback1NodeName)
         ?? nodes.Single(e => e.Name?.LocalName == Constants.Application.Java.XMLJavaVersionFallback2NodeName);
        return node.Value.Trim();
    }

    private static (string? Name, string? Version) ParseLibraryElement(XElement dependencyNode, XElement propertiesNode)
    {
        var name = dependencyNode.Descendants().SingleOrDefault(d => d.Name.LocalName.Equals(Constants.Application.Java.XMLArtifactIdNodeName, StringComparison.OrdinalIgnoreCase))?.Value;
        var group = dependencyNode.Descendants().SingleOrDefault(d => d.Name.LocalName.Equals(Constants.Application.Java.XMLGroupIdNodeName, StringComparison.OrdinalIgnoreCase))?.Value;
        var version = dependencyNode.Descendants().SingleOrDefault(d => d.Name.LocalName.Equals(Constants.Application.Java.XMLArtifactVersionNodeName, StringComparison.OrdinalIgnoreCase))?.Value;
        if (name is null || group is null || version is null)
        {
            return (null, null);
        }

        if (!PackageVersion.IsValid(version))
        {
            var matchingProperty = propertiesNode.Descendants().SingleOrDefault(n => n.Name.LocalName.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (matchingProperty is null)
            {
                return (null, null);
            }

            version = matchingProperty.Value;
        }

        return ($"{group}.{name}", version);
    }

    private static bool IsLibraryElement(XElement element)
     => element.Name?.LocalName == Constants.Application.Java.XMLDependencyNodeName;

    private static bool IsPropertiesElement(XElement element)
     => element.Name?.LocalName == Constants.Application.Java.XMLPropertiesNode;

    private static bool IsJavaElement(XElement element)
     => element.Name?.LocalName == Constants.Application.Java.XMLJavaVersionNodeName
        || element.Name?.LocalName == Constants.Application.Java.XMLJavaVersionFallback1NodeName
        || element.Name?.LocalName == Constants.Application.Java.XMLJavaVersionFallback2NodeName;

    private static int FindJavaVersionPosition(string fileContent)
    {
        var positionOfHeader = fileContent.IndexOf($"<{Constants.Application.Java.XMLJavaVersionNodeName}>");
        if (positionOfHeader < 0)
        {
            positionOfHeader = fileContent.IndexOf($"<{Constants.Application.Java.XMLJavaVersionFallback1NodeName}>");
            if (positionOfHeader < 0)
            {
                positionOfHeader = fileContent.IndexOf($"<{Constants.Application.Java.XMLJavaVersionFallback2NodeName}>");
            }
        }

        return positionOfHeader;
    }
}
