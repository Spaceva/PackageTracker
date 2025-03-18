namespace PackageTracker.Domain.Modules;

public interface IModuleManager
{
    public ValueTask<bool> IsEnabledAsync(string moduleName, CancellationToken cancellationToken = default);
    public Task EnableAsync(string moduleName, CancellationToken cancellationToken = default);
    public Task DisableAsync(string moduleName, CancellationToken cancellationToken = default);
    public ValueTask<bool> ToggleAsync(string moduleName, CancellationToken cancellationToken = default);
    public Task TryRegisterAsync(string moduleName, bool initialState = false, CancellationToken cancellationToken = default);

    public Task<IReadOnlyCollection<Module>> GetAllAsync(CancellationToken cancellationToken = default);
}
