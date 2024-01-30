using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Domain.Application;

public interface IApplicationModuleParser<TApplicationModule> where TApplicationModule : ApplicationModule
{
    Task<TApplicationModule> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken);

    bool CanParse(string fileContent);
}
