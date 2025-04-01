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

        public static class NodeJs
        {
            public const string NameProperty = "name";
            public const string PackagesProperty = "dependencies";
            public const string DevPackagesProperty = "devDependencies";
            public const string AngularVersionPropertyName = "@angular/cli";
            public const string ReactVersionPropertyName = "react";
        }

        public static class Php
        {
            public const string NameProperty = "name";
            public const string PackagesProperty = "require";
            public const string DevPackagesProperty = "require-dev";
            public const string VersionPropertyName = "php";
        }

        public static class Java
        {
            public const string XMLJavaVersionNodeName = "java.version";
            public const string XMLJavaVersionFallback1NodeName = "maven.compiler.target";
            public const string XMLJavaVersionFallback2NodeName = "maven.compiler.source";
            public const string XMLDependencyManagementNodeName = "dependencyManagement";
            public const string XMLDependencyNodeName = "dependency";
            public const string XMLArtifactIdNodeName = "artifactId";
            public const string XMLGroupIdNodeName = "groupId";
            public const string XMLArtifactVersionNodeName = "version";
            public const string XMLPropertiesNode = "properties";
        }
    }
}
