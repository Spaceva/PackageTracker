using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Scanner;
public class UntypedApplication : Application
{
    public override ApplicationType Type => ApplicationType.Unknown;
}
