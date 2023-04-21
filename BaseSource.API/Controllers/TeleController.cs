using FFmpegArgs.Filters.VideoFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BaseSource.API.Controllers
{
    public class TeleController : BaseApiController
    {
        private readonly ITelegramBotClient _telegramBotClient;
        public TeleController(ITelegramBotClient telegramBotClient)
        {
            _telegramBotClient = telegramBotClient;
        }
        //@ym997
        [HttpGet]
        [AllowAnonymous]

        public async Task<IActionResult> Index()
        {
            var result = await _telegramBotClient.TestApiAsync();
            for (int i = 0; i < 100; i++)
            {
                await _telegramBotClient.SendTextMessageAsync(new ChatId(1711431727), $"hello {i}", ParseMode.Default, true);
            }
            

            return Ok();
        }
    }
}
