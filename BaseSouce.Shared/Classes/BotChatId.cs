using BaseSource.Shared.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace BaseSource.Shared.Classes
{
    public class BotChatId : IBotChatId
    {
        public BotChatId()
        {

        }

        [SetsRequiredMembers]
        public BotChatId(IEnumerable<IBotChatId> botChatIds)
        {
            TelegramUserNames = botChatIds.SelectMany(x => x.TelegramUserNames).Distinct().ToList();
            DiscordUserIds = botChatIds.SelectMany(x => x.DiscordUserIds).Distinct().ToList();
        }

        public required IEnumerable<string> TelegramUserNames { get; init; }

        public required IEnumerable<ulong> DiscordUserIds { get; init; }
    }
}
