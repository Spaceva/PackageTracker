namespace PackageTracker.ChatBot.Discord;

internal class DiscordBotStartFailedException(string botName) : Exception($"{botName} failed to start.")
{
}
