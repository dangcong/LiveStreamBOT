using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Services.Services.BOT;
using BaseSouce.Services.Services.Stream;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Interfaces;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.ManagerBOT;
using EFCore.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Telegram.Bot;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types;

namespace BaseSouce.Services.HubServices
{
    public class BotHub : Hub<IBotClientHub>, IServerHub
    {
        private readonly IManagerBOTService _managerBOTService;
        private readonly IStreamClientService _streamClientService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private TelegramBotClient _botClient2;
        private readonly ITelegramBotClient _telegramBotClient;

        public BotHub(IManagerBOTService managerBOTService,
            IStreamClientService streamClientService,
            IUnitOfWork<BaseSourceDbContext> unitOfWork
            , IConfiguration configuration
            , ITelegramBotClient telegramBotClient
            )
        {
            _managerBOTService = managerBOTService;
            _streamClientService = streamClientService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _telegramBotClient = telegramBotClient;
            _botClient2 = new TelegramBotClient(_configuration["TelegramBotConfigure:BotToken2"]);

        }

        public Task PingAsync(ClientInfo clientInfo)
        {
            if (this.Context.GetHttpContext()?.Request?.Headers?.TryGetValue("BotId", out StringValues strings) == true)
            {
                string key = strings.ToString();
                _ = _managerBOTService.CreateBOTAsync(new ManagerBotCreateDto
                {
                    BotId = key,
                    ConnectionId = Context.ConnectionId,
                    Bandwidth = clientInfo.Bandwidth,
                    CpuUsage = clientInfo.CpuUsage,
                    RamUsage = clientInfo.RamUsage,
                    DiskInfo = clientInfo.DiskInfo,
                });
            }
            return Task.CompletedTask;
        }

        public async Task RenderUpdateAsync(RenderMessage renderMessage)
        {
            await _streamClientService.UpdateStreamAsync(renderMessage);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var _repositoryBot = _unitOfWork.GetRepository<ManagerBOT>();
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
                var _repositorySetting = _unitOfWork.GetRepository<ConfigSystem>();

                var botCurrent = await _repositoryBot.GetFirstOrDefaultAsync(
                    predicate: x => x.ConnectionId == Context.ConnectionId
                    , include: x => x.Include(i => i.Manager),
                    disableTracking: false);
                if (botCurrent != null)
                {
                    botCurrent.Status = ManagerBOTStatus.Disconnected;
                    botCurrent.DisconnectedTime = DateTime.Now;
                    botCurrent.IsSendNotification = false;
                    var streamsCurrents = await _repositoryStream.Queryable()
                        .Include(x => x.AppUser)
                        .Where(x => x.ManagerBOTId == botCurrent.Id && x.Status == StreamStatus.Streaming).ToListAsync();

                    if (streamsCurrents.Any())
                    {

                        foreach (var item in streamsCurrents)
                        {
                            item.Status = StreamStatus.Disconnet;
                            item.IsLiveBackup = false;
                            //if (item.DisconnectedTime != null && item.DisconnectedTime < DateTime.Now.AddSeconds(-180))
                            //{
                            //    if (!string.IsNullOrEmpty(item.AppUser.TelegramAPI))
                            //    {
                            //        var liveType = item.EndTimeLive != null ? "11h" : "24/7";
                            //        var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            //        _ = _telegramBotClient
                            //        .SendTextMessageAsync(new ChatId(item.AppUser.TelegramAPI),
                            //        $"User: {item.AppUser.UserName}{Environment.NewLine}"
                            //        + $"Tên luồng:{item.StreamName}{Environment.NewLine}"
                            //        + $"Live:{item.LiveType} {Environment.NewLine}"
                            //        + $"Kiểu live: {liveType}{Environment.NewLine}"
                            //        + $"Disconnected:{timer}");
                            //    }
                            //}
                            item.DisconnectedTime = DateTime.Now;


                        }
                        _repositoryStream.Update(streamsCurrents);
                    }

                    _repositoryBot.Update(botCurrent);
                    await _unitOfWork.SaveChangesAsync();

                    //if (!string.IsNullOrWhiteSpace(botCurrent.Manager.TelegramBOT))
                    //{
                    //    var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    //    var idTeles = botCurrent.Manager.TelegramBOT.Split(',');
                    //    if (idTeles.Length > 0)
                    //    {
                    //        foreach (var item in idTeles)
                    //        {
                    //            _ = _botClient2
                    //           .SendTextMessageAsync(new ChatId(item.Trim()),
                    //           $"BotName: {botCurrent.Name}{Environment.NewLine}"
                    //           + $"Group:{botCurrent.Group}{Environment.NewLine}"
                    //           + $"Manager:{botCurrent.Manager?.UserName} {Environment.NewLine}"
                    //           + $"Live: {botCurrent.LiveType}{Environment.NewLine}"
                    //           + $"Disconnected:{timer}");
                    //        }
                    //    }
                    //}


                    //var setting = await _repositorySetting.GetFirstOrDefaultAsync();
                    //if (!string.IsNullOrEmpty(setting.Telegrams))
                    //{

                    //    var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    //    var idTeles = setting.Telegrams.Split(',');
                    //    if (idTeles.Length > 0)
                    //    {
                    //        foreach (var item in idTeles)
                    //        {
                    //            _ = _botClient2
                    //           .SendTextMessageAsync(new ChatId(item.Trim()),
                    //           $"BotName: {botCurrent.Name}{Environment.NewLine}"
                    //           + $"Group:{botCurrent.Group}{Environment.NewLine}"
                    //           + $"Manager:{botCurrent.Manager?.UserName} {Environment.NewLine}"
                    //           + $"Live: {botCurrent.LiveType}{Environment.NewLine}"
                    //           + $"Disconnected:{timer}");
                    //        }
                    //    }
                    //}
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
