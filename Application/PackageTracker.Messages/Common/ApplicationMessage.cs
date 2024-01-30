using PackageTracker.Domain.Application.Model;

namespace PackageTracker.Messages;

public abstract class ApplicationMessage
{
    protected ApplicationMessage() { }

    protected ApplicationMessage(ApplicationMessage applicationMessage)
    {
        Application = applicationMessage.Application;
    }

    public Application Application { get; init; } = default!;
}
