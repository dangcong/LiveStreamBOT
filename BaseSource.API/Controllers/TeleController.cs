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
            await _telegramBotClient
                .SendTextMessageAsync(new ChatId(1711431727), $@"User: abc{Environment.NewLine}Tên luồng:123 {Environment.NewLine}Live:1 {Environment.NewLine}Kiểu live: 24/7{Environment.NewLine}Lên lịch:123");

            

            return Ok();
        }
    }
}
