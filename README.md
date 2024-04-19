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
    "UseMemoryCache": true
  },
  "Modules": {
    "Fetcher": false,
    "Scanner": false,
    "Monitor": false,
    "ConfluenceExport": false,
    "Discord": false,
    "Telegram": false
  }
}
```

`Persistence > Type` value can be either `MongoDb` either `SqlServer`.

Switch `Modules > *` to true to activate modules.

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
        "MaximumConcurrencyCalls": 10
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
        "MaximumConcurrencyCalls": 10
      },
      {
        "ScannerName": "MyCustomGitHubUser",
        "RepositoryRootLink": "https://github.com/UserName",
        "AccessToken": "TOKEN_HERE",
        "MaximumConcurrencyCalls": 10
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