using Microsoft.Extensions.Logging;
using PackageTracker.Database.MongoDb.Core;
using PackageTracker.Database.MongoDb.Model;
using PackageTracker.Database.MongoDb.Repositories.Base;
using PackageTracker.Domain.Modules;

namespace PackageTracker.Database.MongoDb.Repositories;

internal class ModulesDbRepository(MongoDbContext dbContext, ILogger<ModulesDbRepository> logger) : BaseDbRepository<ModuleDbModel>(dbContext, logger), IModuleManager
{
    public async Task DisableAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        var module = await GetAsync(Filter.Eq(m => m.Name, moduleName), cancellationToken) ?? throw new ModuleNotFoundException();
        module.IsEnabled = false;
        await UpdateAsync(Filter.Eq(m => m.Name, moduleName), module, cancellationToken);
    }

    public async Task EnableAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        var module = await GetAsync(Filter.Eq(m => m.Name, moduleName), cancellationToken) ?? throw new ModuleNotFoundException();
        module.IsEnabled = true;
        await UpdateAsync(Filter.Eq(m => m.Name, moduleName), module, cancellationToken);
    }

    public async ValueTask<bool> ToggleAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        var module = await GetAsync(Filter.Eq(m => m.Name, moduleName), cancellationToken) ?? throw new ModuleNotFoundException();
        module.IsEnabled = !module.IsEnabled;
        await UpdateAsync(Filter.Eq(m => m.Name, moduleName), module, cancellationToken);
        return module.IsEnabled;
    }

    public async ValueTask<bool> IsEnabledAsync(string moduleName, CancellationToken cancellationToken = default)
    {
        var module = await GetAsync(Filter.Eq(m => m.Name, moduleName), cancellationToken);
        return module?.IsEnabled ?? false;
    }

    public async Task TryRegisterAsync(string moduleName, bool initialState = false, CancellationToken cancellationToken = default)
    {
        var module = await GetAsync(Filter.Eq(m => m.Name, moduleName), cancellationToken);
        if (module is not null)
        {
            return;
        }

        await InsertAsync(new ModuleDbModel(new Module { Name = moduleName, IsEnabled = initialState }), cancellationToken);
    }

    public async Task<IReadOnlyCollection<Module>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var modulesDb = await FindAsync(Filter.Empty, cancellationToken);
        return [.. modulesDb.Select(m => m.ToModule())];
    }
}
