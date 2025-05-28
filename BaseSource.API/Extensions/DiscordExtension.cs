using BaseSouce.Services.BotChatServices;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace BaseSource.API.Extensions
{
    public static class DiscordExtension
    {
        public static async Task AddDiscordAsync(this WebApplicationBuilder builder)
        {
            builder.Services
                .Configure<DiscordBotChatService.DiscordBotConfigure>(builder.Configuration
                    .GetSection(DiscordBotChatService.DiscordBotConfigure.Configuration));

            DiscordBotChatService.DiscordBotConfigure discordBotConfigure = builder.Configuration
                .GetSection(DiscordBotChatService.DiscordBotConfigure.Configuration)
                .Get<DiscordBotChatService.DiscordBotConfigure>()!;

            builder.Services
                .AddTransient<DiscordBotChatService.DiscordBotConfigure>(x => x
                    .GetRequiredService<IConfiguration>()
                    .GetSection(DiscordBotChatService.DiscordBotConfigure.Configuration)
                    .Get<DiscordBotChatService.DiscordBotConfigure>()!);

            var config = new DiscordRestConfig
            {

            };

            DiscordRestClient streamClient = new DiscordRestClient(config);
            await streamClient.LoginAsync(TokenType.Bot, discordBotConfigure.Token_Stream, true);
            DiscordRestClient botClient = new DiscordRestClient(config);
            await botClient.LoginAsync(TokenType.Bot, discordBotConfigure.Token_Bot, true);

            DiscordBotChatService.DiscordContainer discordContainer = new DiscordBotChatService.DiscordContainer()
            {
                Stream = streamClient,
                Bot = botClient
            };
            builder.Services.AddSingleton<DiscordBotChatService.IDiscordContainer>(discordContainer);
            builder.Services.AddTransient<IDiscordBotChatService, DiscordBotChatService>();
        }
    }
}