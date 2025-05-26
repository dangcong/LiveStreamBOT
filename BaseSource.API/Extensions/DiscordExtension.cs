#if DISCORD
using BaseSouce.Services.BotMessageServices;
using Discord;
using Discord.WebSocket;

namespace BaseSource.API.Extensions
{
    public static class DiscordExtension
    {
        public static async Task AddDiscordAsync(this WebApplicationBuilder builder)
        {
            builder.Services
                .Configure<BotMessage.DiscordBotConfigure>(builder.Configuration.GetSection(BotMessage.DiscordBotConfigure.Configuration));

            BotMessage.DiscordBotConfigure discordBotConfigure = builder.Configuration
                .GetSection(BotMessage.DiscordBotConfigure.Configuration)
                .Get<BotMessage.DiscordBotConfigure>();

            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
            };
            DiscordSocketClient client = new DiscordSocketClient(config);
            await client.LoginAsync(TokenType.Bot, discordBotConfigure.BotToken, true);
            await client.StartAsync();
            builder.Services.AddSingleton(client);
        }
    }
}
#endif