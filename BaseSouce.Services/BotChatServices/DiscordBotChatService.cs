using Microsoft.Extensions.Logging;
using Discord.WebSocket;
using static BaseSouce.Services.BotChatServices.BotChatService;
using Discord;
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
                IUser user = await discordClient.GetUserAsync(userId);
                IDMChannel dm = await user.CreateDMChannelAsync();
                IUserMessage userMessage = await dm.SendMessageAsync(text);


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
                        await channel.SendMessageAsync(text);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(SendMessageAsync));
            }
        }
    }
}
