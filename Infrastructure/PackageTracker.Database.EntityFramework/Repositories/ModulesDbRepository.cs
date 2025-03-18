using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PackageTracker.Domain.Modules;

namespace PackageTracker.Database.EntityFramework.Repositories;

internal class ModulesDbRepository(IServiceScopeFactory serviceScopeFactory) : IModuleManager
{
    public async Task DisableAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var module = await dbContext.Modules.SingleOrDefaultAsync(x => x.Name == moduleName, cancellationToken) ?? throw new ModuleNotFoundException();
        module.IsEnabled = false;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task EnableAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var module = await dbContext.Modules.SingleOrDefaultAsync(x => x.Name == moduleName, cancellationToken) ?? throw new ModuleNotFoundException();
        module.IsEnabled = true;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask<bool> ToggleAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var module = await dbContext.Modules.SingleOrDefaultAsync(x => x.Name == moduleName, cancellationToken) ?? throw new ModuleNotFoundException();
        module.IsEnabled = !module.IsEnabled;

        await dbContext.SaveChangesAsync(cancellationToken);

        return module.IsEnabled;
    }

    public async ValueTask<bool> IsEnabledAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();
        var module = await dbContext.Modules.SingleOrDefaultAsync(x => x.Name == moduleName, cancellationToken);
        return module?.IsEnabled ?? false;
    }

    public async Task TryRegisterAsync(string moduleName, bool initialState = false, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        var module = await dbContext.Modules.SingleOrDefaultAsync(x => x.Name == moduleName, cancellationToken);
        if(module is not null)
        {
            return;
        }

        dbContext.Modules.Add(new Module { Name = moduleName, IsEnabled = initialState });
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Module>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<PackageTrackerDbContext>();

        return await dbContext.Modules.ToArrayAsync(cancellationToken);
    }
}
