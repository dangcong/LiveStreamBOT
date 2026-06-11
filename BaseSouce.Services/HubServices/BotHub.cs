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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BaseSouce.Services.HubServices
{
    public class BotHub : Hub<IBotClientHub>, IServerHub
    {
        private readonly IManagerBOTService _managerBOTService;
        private readonly IStreamClientService _streamClientService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BotHub> _logger;

        public BotHub(IManagerBOTService managerBOTService,
            IStreamClientService streamClientService,
            IUnitOfWork<BaseSourceDbContext> unitOfWork
            , IConfiguration configuration
            , ILogger<BotHub> logger
            )
        {
            _managerBOTService = managerBOTService;
            _streamClientService = streamClientService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Lấy "BotId" từ header của HTTP request gắn với connection hiện tại (do client gửi lúc kết nối).
        /// </summary>
        private string? GetBotIdFromHeader()
        {
            if (this.Context.GetHttpContext()?.Request?.Headers?.TryGetValue("BotId", out StringValues strings) == true)
                return strings.ToString();
            return null;
        }

        public async Task PingAsync(ClientInfo clientInfo)
        {
            string? botId = GetBotIdFromHeader();
            if (string.IsNullOrWhiteSpace(botId))
            {
                _logger.LogWarning("PingAsync BỎ QUA: thiếu header 'BotId'. ConnectionId=[{ConnectionId}] → bot sẽ không bao giờ lên Connected.", Context.ConnectionId);
                return;
            }

            _logger.LogInformation(
                "PingAsync nhận từ BotId=[{BotId}] ConnectionId=[{ConnectionId}] Cpu={Cpu} Ram={Ram} Bandwidth={Bandwidth}KBps DiskInfo=[{DiskInfo}]",
                botId, Context.ConnectionId, clientInfo?.CpuUsage, clientInfo?.RamUsage, clientInfo?.Bandwidth,
                clientInfo?.DiskInfo is null ? "null" : $"{clientInfo.DiskInfo.Used}/{clientInfo.DiskInfo.Total}");

            if (clientInfo is null)
            {
                _logger.LogWarning("PingAsync: clientInfo null từ BotId=[{BotId}] — bỏ qua CreateBOT.", botId);
                return;
            }

            var result = await _managerBOTService.CreateBOTAsync(new ManagerBotCreateDto
            {
                BotId = botId,
                ConnectionId = Context.ConnectionId,
                Bandwidth = clientInfo.Bandwidth,
                CpuUsage = clientInfo.CpuUsage,
                RamUsage = clientInfo.RamUsage,
                DiskInfo = clientInfo.DiskInfo,
            });

            if (result.Key)
                _logger.LogInformation("CreateBOTAsync OK cho BotId=[{BotId}] → Status=Connected.", botId);
            else
                _logger.LogWarning("CreateBOTAsync THẤT BẠI cho BotId=[{BotId}]: {Message}", botId, result.Value);
        }

        public async Task RenderUpdateAsync(RenderMessage renderMessage)
        {
            await _streamClientService.UpdateStreamAsync(renderMessage);
        }
        public override Task OnConnectedAsync()
        {
            string? botId = GetBotIdFromHeader();
            if (string.IsNullOrWhiteSpace(botId))
                _logger.LogWarning("BOT kết nối nhưng THIẾU header 'BotId'. ConnectionId=[{ConnectionId}]", Context.ConnectionId);
            else
                _logger.LogInformation("BOT kết nối: BotId=[{BotId}] ConnectionId=[{ConnectionId}]", botId, Context.ConnectionId);

            _ = base.Clients.Caller.SetSettingAsync(new SettingInfo()
            {
                PingInterval = 5000,
                ScheduleTimeOnDownloadFailed = TimeSpan.FromMinutes(10),
                NetworkName = null,
            });
            return Task.CompletedTask;
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (exception is null)
                _logger.LogInformation("BOT disconnected (bình thường): ConnectionId=[{ConnectionId}]", Context.ConnectionId);
            else
                _logger.LogWarning(exception, "BOT disconnected (kèm lỗi): ConnectionId=[{ConnectionId}]", Context.ConnectionId);
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
                _logger.LogError(ex, "OnDisconnectedAsync xử lý thất bại cho ConnectionId=[{ConnectionId}]", Context.ConnectionId);
            }
            // _ = _managerBOTService.DisconnectBotAsync(Context.ConnectionId);
            //await base.OnDisconnectedAsync(exception);
        }
    }
}
