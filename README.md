# Package Tracker Application

Track your favorites packages releases and pre-releases.

# Setup
## Application Global Settings
Use the `appsettings.json` configuration file to setup your database.
``` json
{
  "AllowedHosts": "*",
  "Persistence": {
    "Type": "MongoDb | SqlServer | Postgres | InMemory",
    "UseMemoryCache": true,
    "ConnectionString": "YOUR_DATABASE_CONNECTION_STRING_HERE"
  }
}
```

`Persistence > Type` value can be : `MongoDb`, `SqlServer`, `Postgres`, `InMemory`.

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
        ],
        "Packagist": [
          "package1",
          "package2"
        ],
        "MavenCentral": [
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
        "ScannerType": "Gitlab",
        "RepositoryRootLink": "https://gitlab.custom.fr/",
        "AccessToken": "TOKEN_HERE",
        "TokenExpirationWarningThreshold": "DD:HH:MM:SS",
        "MaximumConcurrencyCalls": 10
      },
      {
        "ScannerType": "AzureDevops",
        "RepositoryRootLink": "https://custom.visualstudio.com/",
        "AccessToken": "TOKEN_HERE"
      },
      {
        "ScannerType": "GitHubOrganization",
        "RepositoryRootLink": "https://github.com/MyOrganizationName",
        "AccessToken": "TOKEN_HERE",
        "MaximumConcurrencyCalls": 10
      },
      {
        "ScannerType": "GitHubUser",
        "RepositoryRootLink": "https://github.com/UserName",
        "AccessToken": "TOKEN_HERE",
        "MaximumConcurrencyCalls": 10
      }
    ]
  }
}
```

`Scanner > Applications[] > ScannerType` value can be in the following : [`Gitlab`,`AzureDevops`,`GitHubOrganization`,`GitHubUser`].

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
        "PageName1": 0, 
        "PageName2": 0,
        "PageName3": 0,
        "PageName4": 0,
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
Change the values next to `PageNameXX` to the Confluence ID Page associated.

### Chat Bots
Send notifications to chat bots.

Add objects to `Notifications` to add recipients for the chat bot.

`Notifications > [] > ChatId` is the chat ID of the recipient (usually a `long` number).

`Notifications > [] > Type` value is either `User` or `Channel`.

#### Discord
Use the `discord.json` configuration file.
``` json
{
  "Discord": {
    "Token": "TOKEN_HERE",
    "Notifications": [ 
      {
        "ChatId": 0, 
        "Type": "User | Channel"
      }
    ]
  }
}
```

#### Telegram
Use the `telegram.json` configuration file.
``` json
{
  "Telegram": {
    "Token": "TOKEN_HERE",
    "Notifications": [ 
      {
        "ChatId": 0, 
        "Type": "User | Channel"
      }
    ]
  }
}
```