﻿namespace PackageTracker.Fetcher.PublicRegistries;

internal static class Constants
{
    public static class PublicRegistryUrls
    {
        public const string NUGET_API = "https://api.nuget.org";
        public const string NPM_API = "https://registry.npmjs.org";
        public const string PACKAGIST_API = "https://repo.packagist.org";
        public const string MAVENCENTRAL_API = "https://search.maven.org";

        public const string NUGET_PACKAGE = "https://nuget.org";
        public const string NPM_PACKAGE = "https://npmjs.com";
        public const string PACKAGIST_PACKAGE = "https://packagist.org";
        public const string MAVENCENTRAL_PACKAGE = "https://central.sonatype.com";
    }
}