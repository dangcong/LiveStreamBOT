using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Services.Services.BOT;
using BaseSouce.Services.Services.Stream;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.BotClient;
using BaseSouce.SharedSignalrData.Server;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.ManagerBOT;
using EFCore.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BaseSouce.Services.HubServices
{
    public class BotHub : Hub<IBotClientHub>, IServerHub
    {
        private readonly IManagerBOTService _managerBOTService;
        private readonly IStreamClientService _streamClientService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITelegramBotClient _telegramBotClient;

        public BotHub(IManagerBOTService managerBOTService,
            IStreamClientService streamClientService,
            IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ITelegramBotClient telegramBotClient)
        {
            _managerBOTService = managerBOTService;
            _streamClientService = streamClientService;
            _unitOfWork = unitOfWork;
            _telegramBotClient = telegramBotClient;

        }

        public async Task PingAsync(ClientInfo clientInfo)
        {
            if (this.Context.GetHttpContext()?.Request?.Headers?.TryGetValue("BotId", out StringValues strings) == true)
            {
                string key = strings.ToString();
                await _managerBOTService.CreateBOTAsync(new ManagerBotCreateDto
                {
                    BotId = key,
                    ConnectionId = Context.ConnectionId,
                    Bandwidth = clientInfo.Bandwidth,
                    CpuUsage = clientInfo.CpuUsage,
                    RamUsage = clientInfo.RamUsage
                });
            }
        }

        public async Task RenderUpdateAsync(RenderMessage renderMessage)
        {
            await _streamClientService.UpdateStreamAsync(renderMessage);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            //  Trace.WriteLine(Context.ConnectionId + ' - disconnected');

            try
            {
                var _repositoryBot = _unitOfWork.GetRepository<ManagerBOT>();
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var botCurrent = await _repositoryBot.GetFirstOrDefaultAsync(
                    predicate: x => x.ConnectionId == Context.ConnectionId, disableTracking: false);
                if (botCurrent != null)
                {
                    botCurrent.Status = ManagerBOTStatus.Disconnected;

                    var streamsCurrents = await _repositoryStream.Queryable()
                        .Include(x => x.AppUser)
                        .Where(x => x.ManagerBOTId == botCurrent.Id && x.Status == StreamStatus.Streaming).ToListAsync();

                    if (streamsCurrents.Any())
                    {
                        foreach (var item in streamsCurrents)
                        {

                            item.Status = StreamStatus.Stop;

                            if (!string.IsNullOrEmpty(item.AppUser.TelegramAPI))
                            {
                                var liveType = item.EndTimeLive != null ? "11h" : "24/7";
                                var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                                _ = _telegramBotClient
                                .SendTextMessageAsync(new ChatId(item.AppUser.TelegramAPI),
                                $"User: {item.AppUser.UserName}{Environment.NewLine}"
                                + $"Tên luồng:{item.StreamName}{Environment.NewLine}"
                                + $"Live:{item.LiveType} {Environment.NewLine}"
                                + $"Kiểu live: {liveType}{Environment.NewLine}"
                                + $"Dừng:{timer}", ParseMode.Default, true);
                            }

                        }
                        _repositoryStream.Update(streamsCurrents);
                    }
                    _repositoryBot.Update(botCurrent);
                    await _unitOfWork.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                //_logger.LogError($"Disconnect BOT failed by error:[{ex}]");

            }
            // _ = _managerBOTService.DisconnectBotAsync(Context.ConnectionId);
            //await base.OnDisconnectedAsync(exception);
        }
    }
}
