using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Discord;
using Telegram.Bot.Types;
using static BaseSouce.Services.BotChatServices.BotChatService;
using BaseSource.Shared.Interfaces;
using BaseSource.Shared.Classes;
using BaseSouce.Data.EF;
using BaseSource.Data.Entities;
using EFCore.UnitOfWork;

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
        readonly IServiceScopeFactory _scopeFactory;
        readonly ILogger<BotChatService> _logger;
        public BotChatService(
            IDiscordBotChatService discordBotChatService,
            ITelegramBotChatService telegramBotChatService,
            IServiceScopeFactory scopeFactory,
            ILogger<BotChatService> logger
            )
        {
            this._discordBotChatService = discordBotChatService;
            this._telegramBotChatService = telegramBotChatService;
            this._scopeFactory = scopeFactory;
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
        public async Task SendMessageAsync(
            IBotChatId botChatId,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
        {
            _logger.LogInformation("Dispatch message: SendType={SendType}", sendType);

            // Với thông báo luồng (Stream): ghép thêm danh sách admin (ConfigSystem) để admin nhận MỌI thông báo luồng,
            // bên cạnh chủ luồng. Thông báo Bot đã tự thêm admin ở nơi gọi nên không xử lý ở đây.
            IBotChatId recipients = botChatId;
            if (sendType == SendType.Stream)
            {
                IBotChatId? admin = await _GetAdminRecipientAsync(cancellationToken);
                if (admin is not null)
                    recipients = new BotChatId(new IBotChatId[] { botChatId, admin });
            }

            await Task.WhenAll(_SendDiscordAsync(recipients, text, sendType, cancellationToken), _SendTelegramAsync(recipients, text, sendType, cancellationToken));
        }


        async Task _SendDiscordAsync(
            IBotChatId botChatId,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
        {
            // Log số người nhận đã resolve để phân biệt "không có người nhận" (lỗi data) vs "gửi fail" (lỗi delivery)
            var discordUserIds = botChatId.DiscordUserIds?.ToList() ?? new List<ulong>();
            _logger.LogInformation("Discord recipients resolved: Count={Count}, SendType={SendType}", discordUserIds.Count, sendType);

            foreach (var item in discordUserIds)
            {
                try
                {
                    await _discordBotChatService.SendMessageAsync(item, text, sendType, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Discord dispatch error: SendType={SendType}, UserId={UserId}", sendType, item);
                }
            }
        }
        async Task _SendTelegramAsync(
            IBotChatId botChatId,
            string text,
            SendType sendType = SendType.Stream,
            CancellationToken cancellationToken = default
            )
        {
            // Log số người nhận đã resolve để phân biệt "không có người nhận" (lỗi data) vs "gửi fail" (lỗi delivery)
            var telegramUserNames = botChatId.TelegramUserNames?.ToList() ?? new List<string>();
            _logger.LogInformation("Telegram recipients resolved: Count={Count}, SendType={SendType}", telegramUserNames.Count, sendType);

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

                    await _telegramBotChatService.SendMessageAsync(new ChatId(value), text, sendType, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Telegram dispatch error: SendType={SendType}, Item={Item}", sendType, item);
                }
            }
        }

        /// <summary>
        /// Lấy người nhận admin (ConfigSystem.TelegramUserIds/DiscordUserIds) để gắn vào thông báo luồng.
        /// Dùng scope DB riêng vì gửi tin là fire-and-forget, không được dùng chung DbContext với luồng gọi.
        /// </summary>
        async Task<IBotChatId?> _GetAdminRecipientAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<BaseSourceDbContext>>();
                return await unitOfWork.GetRepository<ConfigSystem>().GetFirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Load admin recipients (ConfigSystem) failed");
                return null;
            }
        }
    }
}
