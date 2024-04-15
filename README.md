# Package Tracker Application

Track your favorites packages releases and pre-releases.

# Setup
## Application Global Settings
Use the `appsettings.json` configuration file to setup your database and qwactivate modules.

``` json
{
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "Database": "YOUR_DATABASE_CONNECTION_STRING_HERE"
  },
  "Persistence": {
    "Type": "MongoDb | SqlServer",
    "UseMemoryCache": boolean
  },
  "Modules": {
    "Fetcher": boolean,
    "Scanner": boolean,
    "Monitor": boolean,
    "ConfluenceExport": boolean
  }
}
```

## Modules
### Fetcher
Packages fetching
Use the `fetcher.json` configuration file.

``` json
{
  "Fetcher": {
    "ProxyUrl": "LeaveEmptyIfNoProxy",
    "TimeBetweenEachExecution": "HH:MM:SS",
    "Packages": {
      "Public": {
        "Npm": [
          "package1",
          "package2"
        ],
        "Nuget": [
          "package1",
          "package2"
        ]
      },
      "Privates": [
        {
          "FetcherName": "MyNugetServer",
          "RepositoryLink": "http://my-private-server/",
          "PackagesName": [
          "package1",
          "package2"
          ]
        }
      ]
    }
  }
}
```

### Monitor
Frameworks monitoring
Use the `monitor.json` configuration file.

``` json
{
  "Monitor": {
    "ProxyUrl": "LeaveEmptyIfNoProxy",
    "TimeBetweenEachExecution": "HH:MM:SS",
    "Frameworks": [
      {
        "MonitorName": "NodeJSGitHubMonitor",
        "Url": "https://api.github.com/repos/nodejs/Release/contents/schedule.json"
      },
      {
        "MonitorName": "DotNetGitHubMonitor",
        "Url": "https://api.github.com/repos/dotnet/core/contents/release-notes/releases-index.json"
      }
    ]
  }
}
```

## Scanner
Applications scanning
Use the `scanner.json` configuration file.

``` json
{
  "Scanner": {
    "ProxyUrl": "LeaveEmptyIfNoProxy",
    "TimeBetweenEachExecution": "HH:MM:SS",
    "Applications": [
      {
        "ScannerName": "MyCustomAngularGitlab",
        "RepositoryRootLink": "https://gitlab.custom.fr/",
        "AccessToken": "TOKEN_HERE",
        "TokenExpirationWarningThreshold": "DD:HH:MM:SS",
        "MaximumConcurrencyCalls": NumberHere
      },
      {
        "ScannerName": "MyCustomAzureDevops",
        "RepositoryRootLink": "https://custom.visualstudio.com/",
        "AccessToken": "TOKEN_HERE"
      },
      {
        "ScannerName": "MyCustomGitHubOrganization",
        "RepositoryRootLink": "https://github.com/MyOrganizationName",
        "AccessToken": "TOKEN_HERE",
        "MaximumConcurrencyCalls": NumberHere
      },
      {
        "ScannerName": "MyCustomGitHubUser",
        "RepositoryRootLink": "https://github.com/UserName",
        "AccessToken": "TOKEN_HERE",
        "MaximumConcurrencyCalls": NumberHere
      }
    ]
  }
}
```

### Confluence Export
Confluence exporting
Use the `confluence.json` configuration file.

``` json
{
  "Confluence": {
    "ProxyUrl": "LeaveEmptyIfNoProxy",
    "TimeBetweenEachExecution": "HH:MM:SS",
    "Domain": "",
    "Username": "",
    "AccessToken": "",
    "Pages": {
    "PageName1": PageName1NumberId,
    "PageName2": PageName2NumberId,
    "PageName3": PageName3NumberId,
    "PageName4": PageName4NumberId,
    },
    "Credentials": [
      {
        "Domain": "",
        "AccessToken": ""
      }
    ]
  }
}
```