using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using static BaseSouce.Services.BotChatServices.BotChatService;

namespace BaseSouce.Services.BotChatServices
{
    public interface ITelegramBotChatService
    {
        Task SendMessageAsync(string userName, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
    }
    public class TelegramBotChatService : ITelegramBotChatService
    {
        public class TelegramBotConfiguration
        {
            public static readonly string Configuration = "TelegramBotConfigure";
            public string Token_Stream { get; init; } = default!;
            public string Token_Bot { get; init; } = default!;
        }
        public interface ITelegramContainer
        {
            public ITelegramBotClient Stream { get; }
            public ITelegramBotClient Bot { get; }
        }
        public class TelegramContainer : ITelegramContainer
        {
            public required ITelegramBotClient Stream { get; init; }
            public required ITelegramBotClient Bot { get; init; }
        }


        private readonly ITelegramContainer _telegramContainer;
        readonly ILogger<TelegramBotChatService> _logger;
        public TelegramBotChatService(
            ITelegramContainer telegramContainer,
            ILogger<TelegramBotChatService> logger
            )
        {
            this._telegramContainer = telegramContainer;
            this._logger = logger;
        }

        public async Task SendMessageAsync(string userName, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default)
        {
            try
            {
                ITelegramBotClient telegramBotClient = sendType switch
                {
                    SendType.Stream => _telegramContainer.Stream,
                    SendType.Bot => _telegramContainer.Bot,
                    _ => throw new NotSupportedException(sendType.ToString())
                };
                await telegramBotClient.SendTextMessageAsync(new ChatId(userName.Trim()), text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(SendMessageAsync));
            }
        }
    }
}
