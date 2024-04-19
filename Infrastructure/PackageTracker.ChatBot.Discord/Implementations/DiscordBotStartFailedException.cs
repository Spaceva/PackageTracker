namespace PackageTracker.ChatBot.Discord;

public class DiscordBotStartFailedException(string botName) : Exception($"{botName} failed to start.")
{
}
