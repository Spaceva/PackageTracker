namespace PackageTracker.ApplicationModuleParsers;

internal static partial class Constants
{
    public static class Application
    {
        public static class DotNet
        {
            public const string XMLDotnetVersionNodeName = "TargetFramework";
            public const string XMLLibraryNodeName = "PackageReference";
            public const string XMLLibraryNameAttribute = "Include";
            public const string XMLLibraryVersionAttribute = "Version";
        }

        public static class DotNetFramework
        {
            public const string XMLDotnetVersionNodeName = "TargetFrameworkVersion";
            public const string XMLLibraryNodeName = "Reference";
            public const string XMLLibraryNameAndVersionAttribute = "Include";
            public const string XMLLibraryVersionSubAttribute = "Version";
        }

        public static class Angular
        {
            public const string PackagesProperty = "dependencies";
            public const string DevPackagesProperty = "devDependencies";
            public const string VersionPropertyName = "@angular/cli";
        }

        public static class Php
        {
            public const string PackagesProperty = "require";
            public const string DevPackagesProperty = "require-dev";
            public const string VersionPropertyName = "php";
        }

        public static class Java
        {
            public const string XMLJavaVersionNodeName = "java.version";
            public const string XMLDependencyManagementNodeName = "dependencyManagement";
            public const string XMLDependencyNodeName = "dependency";
            public const string XMLArtifactIdNodeName = "artifactId";
            public const string XMLArtifactVersionNodeName = "version";
            public const string XMLPropertiesNode = "properties";
        }
    }
}
