# Package Tracker Application

Track your favorites packages releases and pre-releases.

# Setup
## Configuration

Replace the following values with what you need in the `appsettings.json` file (located in `Host/PackageTracker.Host` before build).

``` json
 "Fetcher": {
    "TimeBetweenEachExecution": "HH:MM:DD",
    "Packages": {
      "Npm": [
        "Package1",
        "Package2",
        "Package3",
      ],
      "Nuget": [
        "Package1",
        "Package2",
        "Package3",
      ]
    }
  }
```
**Caution:** This is **case-sensitive** so be sure to write correctly the name of the package !

## Build

Use `docker build` command with the provided `Dockerfile`.

----
*Demo file*
``` json
 "Fetcher": {
    "TimeBetweenEachExecution": "00:30:00",
    "Packages": {
      "Npm": [
        "@angular/cli",
        "eslint",
        "jasmine-core",
        "jest",
        "karma",
        "rxjs",
        "typescript"
      ],
      "Nuget": [
        "AutoFixture",
        "FluentValidation",
        "FluentAssertions",
        "MassTransit",
        "MediatR",
        "Serilog",
        "xunit"
      ]
    }
  }
```