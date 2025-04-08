using Microsoft.Extensions.Logging;
using PackageTracker.Domain;
using PackageTracker.Domain.Application;
using PackageTracker.Domain.Application.Model;
using PackageTracker.Domain.Package.Model;
using PackageTracker.Messages.Commands;
using PackageTracker.Messages.Events;
using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.Handlers;

internal class UpdateApplicationCommandHandler(IMediator mediator, IApplicationsRepository applicationsRepository, ILogger<UpdateApplicationCommandHandler> logger) : BaseHandler<UpdateApplicationCommand>(mediator)
{
    public override async Task Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var newApplication = request.Application;

            var dbApplication = await applicationsRepository.GetAsync(newApplication.Name, newApplication.Type, newApplication.RepositoryLink, cancellationToken);

            var messages = BuildNotificationFromChangesBetween(newApplication, dbApplication);

            if (messages.Length == 0)
            {
                return;
            }

            await applicationsRepository.SaveAsync(newApplication, cancellationToken);

            await NotifyParallelAsync(messages, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to handle {Command}.", nameof(UpdateApplicationCommand));
        }
    }

    private static INotification[] BuildNotificationFromChangesBetween(Application updatedApplication, Application updatingApplication)
    {
        var delta = new CollectionsDelta<ApplicationBranch>(updatedApplication.Branchs, updatingApplication.Branchs, new ApplicationBranchNameComparer());
        var notifications = new List<INotification>();
        if (delta.AddedEntities.Count != 0)
        {
            notifications.AddRange(delta.AddedEntities.Select(branch => ApplicationBranchAddedEvent(updatedApplication, branch)));
        }

        if (delta.RemovedEntities.Count != 0)
        {
            notifications.AddRange(delta.RemovedEntities.Select(branch => ApplicationBranchDeletedEvent(updatedApplication, branch)));
        }

        if (updatedApplication.IsSoonDecommissioned != updatingApplication.IsSoonDecommissioned
            || updatedApplication.IsDeadLink != updatingApplication.IsDeadLink)
        {
            notifications.Add(ApplicationUpdatedEvent(updatedApplication));
        }

        return [.. notifications.Union(BuildNotificationFromChangesBetween(updatedApplication, delta.UpdatedEntitiesCommon, delta.UpdatingEntitiesCommon))];
    }

    private static IEnumerable<INotification> BuildNotificationFromChangesBetween(Application application, IEnumerable<ApplicationBranch> updatedBranchs, IEnumerable<ApplicationBranch> updatingBranchs)
    {
        var notifications = new List<INotification>();
        foreach (var branch in updatedBranchs)
        {
            var matchingBranch = updatingBranchs.Single(b => b.Name.Equals(branch.Name));
            notifications.AddRange(BuildNotificationFromChangesBetween(application, branch, matchingBranch));
        }

        return notifications.AsEnumerable();
    }

    private static IEnumerable<INotification> BuildNotificationFromChangesBetween(Application application, ApplicationBranch updatedBranch, ApplicationBranch updatingBranch)
    {
        var delta = new CollectionsDelta<ApplicationModule>(updatedBranch.Modules, updatingBranch.Modules, new ApplicationModuleNameComparer());
        var notifications = new List<INotification>();
        if (delta.AddedEntities.Count != 0)
        {
            notifications.AddRange(delta.AddedEntities.Select(module => ApplicationModuleAddedEvent(application, updatedBranch, module)));
        }

        if (delta.RemovedEntities.Count != 0)
        {
            notifications.AddRange(delta.RemovedEntities.Select(module => ApplicationModuleDeletedEvent(application, updatedBranch, module)));
        }

        if (!updatedBranch.LastCommit.Equals(updatingBranch))
        {
            notifications.Add(new ApplicationBranchCommittedEvent { ApplicationName = application.Name, BranchName = updatedBranch.Name, CommitDate = updatedBranch.LastCommit, Type = application.Type });
        }

        return notifications.Union(BuildNotificationFromChangesBetween(application, updatedBranch, delta.UpdatedEntitiesCommon, delta.UpdatingEntitiesCommon));
    }

    private static IEnumerable<INotification> BuildNotificationFromChangesBetween(Application application, ApplicationBranch branch, IEnumerable<ApplicationModule> updatedModules, IEnumerable<ApplicationModule> updatingModules)
    {
        var notifications = new List<INotification>();
        foreach (var module in updatedModules)
        {
            var matchingModule = updatingModules.Single(b => b.Name.Equals(module.Name));
            notifications.AddRange(BuildNotificationFromChangesBetween(application, branch, module, matchingModule));
        }

        return notifications.AsEnumerable();
    }

    private static IEnumerable<INotification> BuildNotificationFromChangesBetween(Application application, ApplicationBranch branch, ApplicationModule updatedModule, ApplicationModule updatingModule)
    {
        var delta = new CollectionsDelta<ApplicationPackage>(updatedModule.Packages, updatingModule.Packages, new ApplicationPackageNameComparer());
        var notifications = new List<INotification>();
        if (delta.AddedEntities.Count != 0)
        {
            notifications.AddRange(delta.AddedEntities.Select(package => ApplicationPackageVersionAddedEvent(application, branch, updatedModule, package)));
        }

        if (delta.RemovedEntities.Count != 0)
        {
            notifications.AddRange(delta.RemovedEntities.Select(package => ApplicationPackageVersionDeletedEvent(application, branch, updatedModule, package)));
        }

        return notifications.Union(BuildNotificationFromChangesBetween(application, branch, updatedModule, delta.UpdatedEntitiesCommon, delta.UpdatingEntitiesCommon));
    }

    private static IEnumerable<INotification> BuildNotificationFromChangesBetween(Application application, ApplicationBranch branch, ApplicationModule module, IEnumerable<ApplicationPackage> updatedPackages, IEnumerable<ApplicationPackage> updatingPackages)
    {
        var notifications = new List<INotification>();
        foreach (var package in updatedPackages)
        {
            var matchingPackage = updatingPackages.Single(b => b.PackageName.Equals(package.PackageName));
            if (package.PackageVersion.Equals(matchingPackage.PackageVersion))
            {
                continue;
            }
            notifications.Add(ApplicationPackageVersionUpdatedEvent(application, branch, module, package, matchingPackage.PackageVersion));
        }

        return notifications.AsEnumerable();
    }

    private static ApplicationUpdatedEvent ApplicationUpdatedEvent(Application updatedApplication)
     => new()
     {
         ApplicationType = updatedApplication.Type,
         IsSoonDecommissionned = updatedApplication.IsSoonDecommissioned,
         Name = updatedApplication.Name,
         Path = updatedApplication.Path,
         RepositoryType = updatedApplication.RepositoryType,
         IsDeadLink = updatedApplication.IsDeadLink,
     };

    private static ApplicationBranchDeletedEvent ApplicationBranchDeletedEvent(Application application, ApplicationBranch branch)
        => new() { ApplicationName = application.Name, BranchName = branch.Name, Type = application.Type };

    private static ApplicationBranchAddedEvent ApplicationBranchAddedEvent(Application application, ApplicationBranch branch)
        => new() { ApplicationName = application.Name, BranchName = branch.Name, Modules = [.. branch.Modules.Select(module => ApplicationModuleAddedEvent(application, branch, module))], Type = application.Type };

    private static ApplicationModuleAddedEvent ApplicationModuleAddedEvent(Application application, ApplicationBranch branch, ApplicationModule module)
        => new() { ApplicationName = application.Name, BranchName = branch.Name, ModuleName = module.Name, Type = application.Type, PackageVersions = [.. module.Packages.Select(package => ApplicationPackageVersionAddedEvent(application, branch, module, package))] };

    private static ApplicationModuleDeletedEvent ApplicationModuleDeletedEvent(Application application, ApplicationBranch branch, ApplicationModule module)
        => new() { ApplicationName = application.Name, BranchName = branch.Name, ModuleName = module.Name, Type = application.Type };

    private static ApplicationPackageVersionAddedEvent ApplicationPackageVersionAddedEvent(Application application, ApplicationBranch branch, ApplicationModule module, ApplicationPackage package)
        => new() { ApplicationName = application.Name, BranchName = branch.Name, ModuleName = module.Name, Type = application.Type, PackageName = package.PackageName, PackageVersionLabel = package.PackageVersion.ToString(), PackageLink = package.TrackedPackage?.Link };

    private static ApplicationPackageVersionDeletedEvent ApplicationPackageVersionDeletedEvent(Application application, ApplicationBranch branch, ApplicationModule module, ApplicationPackage package)
        => new() { ApplicationName = application.Name, BranchName = branch.Name, ModuleName = module.Name, Type = application.Type, PackageName = package.PackageName, PackageVersionLabel = package.PackageVersion.ToString(), PackageLink = package.TrackedPackage?.Link };

    private static ApplicationPackageVersionUpdatedEvent ApplicationPackageVersionUpdatedEvent(Application application, ApplicationBranch branch, ApplicationModule module, ApplicationPackage package, string oldVersion)
        => new() { ApplicationName = application.Name, BranchName = branch.Name, ModuleName = module.Name, Type = application.Type, PackageName = package.PackageName, PackageVersionLabel = package.PackageVersion.ToString(), PackageLink = package.TrackedPackage?.Link, OldPackageVersionLabel = oldVersion };
}
