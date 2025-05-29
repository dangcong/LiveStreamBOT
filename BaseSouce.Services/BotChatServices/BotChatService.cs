using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Discord;
using Telegram.Bot.Types;
using static BaseSouce.Services.BotChatServices.BotChatService;
using BaseSource.Shared.Interfaces;
using BaseSource.Shared.Classes;

namespace BaseSouce.Services.BotChatServices
{
    public interface IBotChatService
    {
        Task SendMessageAsync(IEnumerable<IBotChatId> botChatIds, IEnumerable<string> texts, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
        Task SendMessageAsync(IBotChatId botChatId, IEnumerable<string> texts, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
        Task SendMessageAsync(IEnumerable<IBotChatId> botChatIds, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
        Task SendMessageAsync(IBotChatId botChatId, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
    }
    public class BotChatService : IBotChatService
    {
        public enum SendType
        {
            Stream,
            Bot
        }

        readonly IDiscordBotChatService _discordBotChatService;
        readonly ITelegramBotChatService _telegramBotChatService;
        readonly ILogger<BotChatService> _logger;
        public BotChatService(
            IDiscordBotChatService discordBotChatService,
            ITelegramBotChatService telegramBotChatService,
            ILogger<BotChatService> logger
            )
        {
            this._discordBotChatService = discordBotChatService;
            this._telegramBotChatService = telegramBotChatService;
            this._logger = logger;
        }

        public Task SendMessageAsync(
            IEnumerable<IBotChatId> botChatIds,
            IEnumerable<string> texts,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
            => SendMessageAsync(new BotChatId(botChatIds), string.Join(Environment.NewLine, texts), sendType, cancellationToken);
        public Task SendMessageAsync(
            IBotChatId botChatId,
            IEnumerable<string> texts,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
            => SendMessageAsync(botChatId, string.Join(Environment.NewLine, texts), sendType, cancellationToken);
        public Task SendMessageAsync(
            IEnumerable<IBotChatId> botChatIds,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
            => SendMessageAsync(new BotChatId(botChatIds), text, sendType, cancellationToken);
        public Task SendMessageAsync(
            IBotChatId botChatId,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
            => Task.WhenAll(_SendDiscordAsync(botChatId, text, sendType, cancellationToken), _SendTelegramAsync(botChatId, text, sendType, cancellationToken));


        async Task _SendDiscordAsync(
            IBotChatId botChatId,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
        {
            foreach (var item in botChatId.DiscordUserIds)
            {
                await _discordBotChatService.SendMessageAsync(item, text, sendType, cancellationToken);
            }
        }
        async Task _SendTelegramAsync(
            IBotChatId botChatId,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
        {
            foreach (var item in botChatId.TelegramUserNames)
            {
                await _telegramBotChatService.SendMessageAsync(new ChatId(item), text, sendType, cancellationToken);
            }
        }
    }
}
