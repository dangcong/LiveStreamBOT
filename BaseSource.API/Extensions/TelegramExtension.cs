using BaseSouce.Services.BotChatServices;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace BaseSource.API.Extensions
{
    public static class TelegramExtension
    {
        public static void AddTelegram(this WebApplicationBuilder builder)
        {
            builder.Services
                .Configure<TelegramBotChatService.TelegramBotConfiguration>(builder.Configuration
                    .GetSection(TelegramBotChatService.TelegramBotConfiguration.Configuration));
            builder.Services.AddHttpClient("telegram_bot_client")
                            .AddTypedClient<TelegramBotChatService.ITelegramContainer>((HttpClient httpClient, IServiceProvider sp) =>
                            {
                                TelegramBotChatService.TelegramBotConfiguration? botConfig = sp.GetRequiredService<IOptions<TelegramBotChatService.TelegramBotConfiguration>>()?.Value;
                                if (string.IsNullOrWhiteSpace(botConfig?.Token_Stream)) throw new NullReferenceException($"Telegram Token_Stream is empty");
                                if (string.IsNullOrWhiteSpace(botConfig?.Token_Bot)) throw new NullReferenceException($"Telegram Token_Bot is empty");

                                return new TelegramBotChatService.TelegramContainer()
                                {
                                    Stream = new TelegramBotClient(botConfig.Token_Stream, httpClient),
                                    Bot = new TelegramBotClient(botConfig.Token_Bot, httpClient),
                                };
                            });
            builder.Services.AddTransient<ITelegramBotChatService, TelegramBotChatService>();
        }
    }
}