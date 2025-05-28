using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Discord;
using Telegram.Bot.Types;
using static BaseSouce.Services.BotChatServices.BotChatService;

namespace BaseSouce.Services.BotChatServices
{
    public interface IBotChatService
    {
        Task SendMessageAsync(string channelId, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
        Task SendMessageAsync(List<string> channelIds, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
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

        public async Task SendMessageAsync(string channelId, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default)
        {
            try
            {
                _ = _telegramBotChatService.SendMessageAsync(channelId, text, sendType, cancellationToken);
                _ = _discordBotChatService.SendMessageAsync(channelId, text, sendType, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendMessageAsync");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelIds">telegram user/channel discord</param>
        /// <param name="text"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendMessageAsync(List<string> channelIds, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default)
        {
            foreach (var channelId in channelIds)
            {
                await SendMessageAsync(channelId, text, sendType, cancellationToken);
            }
        }
    }
}
