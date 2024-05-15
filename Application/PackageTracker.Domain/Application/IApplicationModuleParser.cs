using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Domain.Application;

public interface IApplicationModuleParser
{
    bool CanParse(string fileContent);

    bool IsModuleFile(string fileAbsolutePath);

    Task<ApplicationModule> ParseModuleAsync(string fileContent, string fileName, CancellationToken cancellationToken);
}
