using BaseSouce.Data.EF;
using BaseSouce.Services.Services.BOT;
using BaseSouce.Services.Services.Stream;
using EFCore.UnitOfWork;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graph.Models;
using Microsoft.Extensions.Logging;
using static BaseSouce.Services.BotMessageServices.BotMessage;

#if TELEGRAM
using Telegram.Bot;
#elif DISCORD
using Discord;
using Discord.WebSocket;
#endif

namespace BaseSouce.Services.BotMessageServices
{
    public class BotMessage
    {
        public enum SendType
        {
            Live,
            Bot
        }


        readonly ILogger<BotMessage> _logger;
#if TELEGRAM
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly ITelegramBotClient _telegramBotClient2;
        public BotMessage(ITelegramBotClient telegramBotClient,ILogger<BotMessage> logger)
        {
            this._telegramBotClient = telegramBotClient;
            this._telegramBotClient2 = new TelegramBotClient(_configuration["TelegramBotConfigure:BotToken2"]);
            this._logger = logger;
        }
#elif DISCORD
        public class DiscordBotConfigure
        {
            public static readonly string Configuration = "DiscordBotConfigure";
            public string BotToken { get; init; } = default!;
            public ulong ServerId { get; set; }
            public ulong ChannelLive { get; set; }
            public ulong ChannelBot { get; set; }
        }


        private readonly DiscordSocketClient _discordSocketClient;
        private readonly DiscordBotConfigure _discordBotConfigure;
        public BotMessage(
            DiscordSocketClient discordSocketClient,
            BotMessage.DiscordBotConfigure discordBotConfigure,
            ILogger<BotMessage> logger
            )
        {
            this._discordSocketClient = discordSocketClient;
            this._discordBotConfigure = discordBotConfigure;
            this._logger = logger;
        }
#endif


        public async Task SendMessageAsync(string text, SendType sendType = SendType.Live, CancellationToken cancellationToken = default)
        {
            IMessageChannel? messageChannel = sendType switch
            {
                SendType.Bot => await _discordSocketClient.GetChannelAsync(_discordBotConfigure.ChannelBot) as IMessageChannel,
                SendType.Live => await _discordSocketClient.GetChannelAsync(_discordBotConfigure.ChannelLive) as IMessageChannel,
                _ => null,
            };
            if(messageChannel is not null)
            {
                await messageChannel.SendMessageAsync(text);
            }
        }

        public async Task SendMessageAsync(string channelId, string text, SendType sendType = SendType.Live, CancellationToken cancellationToken = default)
        {
            try
            {
#if TELEGRAM
                await _telegramBotClient.SendTextMessageAsync(new ChatId(idTele.Trim()), text);
#elif DISCORD
                if (ulong.TryParse(channelId, out ulong result))
                {
                    var channel = await _discordSocketClient.GetChannelAsync(result) as IMessageChannel;
                    if (channel != null)
                    {
                        await channel.SendMessageAsync(text);
                        return;
                    }
                }
#endif
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
        public async Task SendMessageAsync(List<string> channelIds, string text, SendType sendType = SendType.Live, CancellationToken cancellationToken = default)
        {
            foreach (var channelId in channelIds)
            {
                await SendMessageAsync(channelId, text, sendType, cancellationToken);
            }
        }
    }
}
