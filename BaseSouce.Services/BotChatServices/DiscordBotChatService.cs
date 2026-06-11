using Microsoft.Extensions.Logging;
using Discord.WebSocket;
using static BaseSouce.Services.BotChatServices.BotChatService;
using Discord;
using Discord.Net;
using Telegram.Bot.Types;
using Telegram.Bot;
using Discord.Net.Rest;

namespace BaseSouce.Services.BotChatServices
{
    public interface IDiscordBotChatService
    {
        Task SendMessageAsync(ulong userId, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default);
    }
    public class DiscordBotChatService : IDiscordBotChatService
    {
        public class DiscordBotConfigure
        {
            public static readonly string Configuration = "DiscordBotConfigure";
            public string Token_Stream { get; init; } = default!;
            public string Token_Bot { get; init; } = default!;

            public ulong? ChannelLive { get; set; }
            public ulong? ChannelBot { get; set; }
        }
        public interface IDiscordContainer
        {
            public IDiscordClient Stream { get; }
            public IDiscordClient Bot { get; }
        }
        public class DiscordContainer : IDiscordContainer
        {
            public required IDiscordClient Stream { get; init; }
            public required IDiscordClient Bot { get; init; }
        }




        readonly ILogger<DiscordBotChatService> _logger;
        private readonly IDiscordContainer _discordContainer;
        private readonly DiscordBotConfigure _discordBotConfigure;
        public DiscordBotChatService(
            IDiscordContainer discordContainer,
            DiscordBotConfigure discordBotConfigure,
            ILogger<DiscordBotChatService> logger
            )
        {
            this._discordContainer = discordContainer;
            this._discordBotConfigure = discordBotConfigure;
            this._logger = logger;
        }



        public async Task SendMessageAsync(ulong userId, string text, SendType sendType = SendType.Stream, CancellationToken cancellationToken = default)
        {
            try
            {
                text = $"```\r\n{text}\r\n```";

                IDiscordClient discordClient = sendType switch
                {
                    SendType.Stream => _discordContainer.Stream,
                    SendType.Bot => _discordContainer.Bot,
                    _ => throw new NotSupportedException(sendType.ToString())
                };

                // Gửi DM cho user
                _logger.LogInformation("Discord sending DM: SendType={SendType}, UserId={UserId}", sendType, userId);
                IUser user = await discordClient.GetUserAsync(userId);
                if (user is null)
                {
                    // Thường gặp khi user chưa từng chung server với bot, hoặc UserId sai
                    _logger.LogWarning("Discord user not found: SendType={SendType}, UserId={UserId}", sendType, userId);
                }
                else
                {
                    IDMChannel dm = await user.CreateDMChannelAsync();
                    IUserMessage userMessage = await dm.SendMessageAsync(text);
                    _logger.LogInformation("Discord DM sent OK: SendType={SendType}, UserId={UserId}, MessageId={MessageId}", sendType, userId, userMessage.Id);
                }


                // Gửi vào channel server (nếu có cấu hình)
                ulong? serverChannel = sendType switch
                {
                    SendType.Stream => _discordBotConfigure.ChannelLive,
                    SendType.Bot => _discordBotConfigure.ChannelBot,
                    _ => null
                };
                if(serverChannel.HasValue)
                {
                    var channel = await _discordContainer.Stream.GetChannelAsync(serverChannel.Value) as IMessageChannel;
                    if (channel is not null)
                    {
                        IUserMessage channelMessage = await channel.SendMessageAsync(text);
                        _logger.LogInformation("Discord channel sent OK: SendType={SendType}, ChannelId={ChannelId}, MessageId={MessageId}", sendType, serverChannel.Value, channelMessage.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Discord channel not found or not a message channel: SendType={SendType}, ChannelId={ChannelId}", sendType, serverChannel.Value);
                    }
                }
            }
            catch (HttpException ex)
            {
                // Lỗi do Discord API trả về (vd 403 Forbidden khi user chặn DM, 404 unknown user/channel...)
                _logger.LogError(ex, "Discord API error: SendType={SendType}, UserId={UserId}, HttpCode={HttpCode}, DiscordCode={DiscordCode}, Reason={Reason}", sendType, userId, ex.HttpCode, ex.DiscordCode, ex.Reason);
            }
            catch (Exception ex)
            {
                // Lỗi khác: mạng tới Discord, token sai, client chưa login...
                _logger.LogError(ex, "Discord send failed: SendType={SendType}, UserId={UserId}", sendType, userId);
            }
        }
    }
}
