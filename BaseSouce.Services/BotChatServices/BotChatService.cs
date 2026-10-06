using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using static BaseSouce.Services.BotChatServices.BotChatService;
using BaseSource.Shared.Interfaces;
using BaseSource.Shared.Classes;

namespace BaseSouce.Services.BotChatServices
{
    public interface IBotChatService
    {
        Task<bool> SendMessageAndReportAsync(IBotChatId botChatId, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
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

        readonly ITelegramBotChatService _telegramBotChatService;
        readonly ILogger<BotChatService> _logger;
        public BotChatService(
            ITelegramBotChatService telegramBotChatService,
            ILogger<BotChatService> logger
            )
        {
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
        {
            _logger.LogInformation("Dispatch message: SendType={SendType}", sendType);
            return _SendTelegramAsync(botChatId, text, sendType, cancellationToken);
        }

        public Task<bool> SendMessageAndReportAsync(
            IBotChatId botChatId,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Dispatch message with delivery result: SendType={SendType}", sendType);
            return _SendTelegramAsync(botChatId, text, sendType, cancellationToken);
        }

        async Task<bool> _SendTelegramAsync(
            IBotChatId botChatId,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
        {
            // Log số người nhận đã resolve để phân biệt "không có người nhận" (lỗi data) vs "gửi fail" (lỗi delivery)
            var telegramUserNames = botChatId.TelegramUserNames?.ToList() ?? new List<string>();
            _logger.LogInformation("Telegram recipients resolved: Count={Count}, SendType={SendType}", telegramUserNames.Count, sendType);
            var delivered = false;

            foreach (var item in telegramUserNames)
            {
                try
                {
                    // Bỏ qua giá trị rỗng/khoảng trắng để tránh ArgumentException khi tạo ChatId
                    if (string.IsNullOrWhiteSpace(item))
                        continue;

                    // ChatId hợp lệ khi là numeric id hoặc username bắt đầu bằng @; tự thêm @ nếu thiếu
                    var value = long.TryParse(item, out _) || item.StartsWith("@")
                        ? item
                        : "@" + item;

                    delivered |= await _telegramBotChatService.SendMessageAsync(new ChatId(value), text, sendType, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Telegram dispatch error: SendType={SendType}, Item={Item}", sendType, item);
                }
            }

            return delivered;
        }
    }
}
