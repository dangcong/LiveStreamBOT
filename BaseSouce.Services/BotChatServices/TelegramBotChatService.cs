using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using static BaseSouce.Services.BotChatServices.BotChatService;

namespace BaseSouce.Services.BotChatServices
{
    public interface ITelegramBotChatService
    {
        Task<bool> SendMessageAsync(ChatId chatId, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
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

        public async Task<bool> SendMessageAsync(ChatId chatId, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default)
        {
            try
            {
                ITelegramBotClient telegramBotClient = sendType switch
                {
                    SendType.Stream => _telegramContainer.Stream,
                    SendType.Bot => _telegramContainer.Bot,
                    _ => throw new NotSupportedException(sendType.ToString())
                };

                _logger.LogInformation("Telegram sending: SendType={SendType}, ChatId={ChatId}", sendType, chatId);
                Message sent = await telegramBotClient.SendTextMessageAsync(chatId, text, cancellationToken: cancellationToken);
                _logger.LogInformation("Telegram sent OK: SendType={SendType}, ChatId={ChatId}, MessageId={MessageId}", sendType, chatId, sent.MessageId);
                return true;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (ApiRequestException ex)
            {
                // Lỗi do Telegram API trả về (vd 403 Forbidden: bot can't initiate conversation,
                // 400 Bad Request: chat not found...). Đây là nguyên nhân hay gặp nhất khi không nhận được tin.
                _logger.LogError(ex, "Telegram API error: SendType={SendType}, ChatId={ChatId}, ErrorCode={ErrorCode}, Message={ApiMessage}", sendType, chatId, ex.ErrorCode, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                // Lỗi khác: mạng tới api.telegram.org, token sai, TLS...
                _logger.LogError(ex, "Telegram send failed: SendType={SendType}, ChatId={ChatId}", sendType, chatId);
                return false;
            }
        }
    }
}
