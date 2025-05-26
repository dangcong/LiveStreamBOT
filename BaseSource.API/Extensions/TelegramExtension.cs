#if TELEGRAM
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace BaseSource.API.Extensions
{
    public static class TelegramExtension
    {
        public static void AddTelegram(this WebApplicationBuilder builder)
        {
            //builder.Services.Configure<TelegramBotConfiguration>(builder.Configuration.GetSection(TelegramBotConfiguration.Configuration));
            builder.Services.AddHttpClient("telegram_bot_client")
                            .AddTypedClient<ITelegramBotClient>((HttpClient httpClient, IServiceProvider sp) =>
                            {
                                TelegramBotConfiguration? botConfig = sp.GetService<IOptions<TelegramBotConfiguration>>()?.Value;
                                if (string.IsNullOrWhiteSpace(botConfig?.BotToken)) throw new NullReferenceException($"Telegram BotToken is empty");
                               // TelegramBotClientOptions options = new(botConfig.BotToken);
                                return new TelegramBotClient(botConfig.BotToken, httpClient);
                            });
        }
    }
    public class TelegramBotConfiguration
    {
        public static readonly string Configuration = "TelegramBotConfigure";
        public string BotToken { get; init; } = default!;
    }
}
#endif