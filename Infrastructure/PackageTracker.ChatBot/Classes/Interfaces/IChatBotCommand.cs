using PackageTracker.SharedKernel.Mediator;

namespace PackageTracker.ChatBot;

public interface IChatBotCommand : IRequest
{
    public IIncomingMessage MessageProperties { get; }
}
