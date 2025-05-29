using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Services.Services.Stream;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.Enums;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Interfaces;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.ManagerBOT;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using BaseSouce.Services.HubServices;
using Microsoft.Extensions.Configuration;
using BaseSouce.Services.BotChatServices;
using BaseSource.Shared.Interfaces;

namespace BaseSouce.Services.Services.BOT
{
    public class ManagerBOTService : IManagerBOTService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ManagerBOTService> _logger;
        private readonly IMapper _mapper;
        private readonly IStreamClientService _streamService;
        private readonly IHubContext<BotHub, IBotClientHub> _botContextHub;
        private readonly IBotChatService _botMessage;
        private readonly IConfiguration _configuration;
        public ManagerBOTService(IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<ManagerBOTService> logger,
            IMapper mapper,
            IStreamClientService streamService
            , IHubContext<BotHub, IBotClientHub> botContextHub
            , IBotChatService botMessage
            , IConfiguration configuration
            //,IBotTeleClient2 botTeleClient2
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _streamService = streamService;
            _botMessage = botMessage;
            _configuration = configuration;
            //_botTeleClient2 = botTeleClient2;
        }

        public async Task<KeyValuePair<bool, string>> ChangeUsedAsync(int id)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<ManagerBOT>();
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id, disableTracking: false);

                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không tồn tại!");
                }
                botCurrent.IsUsed = !botCurrent.IsUsed;
                _repository.Update(botCurrent);

                await _unitOfWork.SaveChangesAsync();

                if (!botCurrent.IsUsed)
                {
                    //send data to BOT
                    var streams = await _repositoryStream.Queryable()
                        .AsNoTracking()
                        .Where(x => x.ManagerBOTId == botCurrent.Id)
                        .ToListAsync();
                    if (streams.Any())
                    {
                        foreach (var stream in streams)
                        {
                            await _streamService.CancelStreamAsync(stream.UserId, stream.Id);
                            Thread.Sleep(70);
                        }
                    }
                }

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Change Used BOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật BOT không thành công vui lòng thử lại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> CreateBOTAsync(ManagerBotCreateDto model)
        {
            try
            {
                var _repositorySetting = _unitOfWork.GetRepository<ConfigSystem>();
                var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.BotId == model.BotId,
                    disableTracking: false,
                    include: x => x.Include(i => i.Manager));

                var statusOld = ManagerBOTStatus.Connected;
                var timeUpdateOld = botCurrent?.UpdatedTime ?? DateTime.Now;
                DateTime? timeDisconnected = DateTime.Now;
                var bandwidth = model.Bandwidth != null && model.Bandwidth >= 0 ? model.Bandwidth : 0;
                if (botCurrent == null)
                {
                    botCurrent = new ManagerBOT
                    {
                        BotId = model.BotId,
                        CreatedTime = DateTime.Now,
                        Group = model.Group,
                        Name = model.Name,
                        Status = ManagerBOTStatus.Connected,
                        UpdatedTime = DateTime.Now,
                        Bandwidth = Math.Round(bandwidth, 2),
                        RAM = model.RamUsage,
                        DiskTotal = model.DiskInfo.Total,
                        DiskUsed = model.DiskInfo.Used,
                        IsUsed = true

                    };
                    _repository.Insert(botCurrent);
                    statusOld = ManagerBOTStatus.Disconnected;
                    timeDisconnected = DateTime.Now.AddSeconds(-240);
                }
                else
                {
                    statusOld = botCurrent.Status;
                    timeDisconnected = botCurrent.DisconnectedTime;
                    botCurrent.ConnectionId = model.ConnectionId;
                    botCurrent.Bandwidth = Math.Round(bandwidth, 2);
                    botCurrent.RAM = model.RamUsage;
                    botCurrent.CPU = model.CpuUsage;
                    botCurrent.Status = ManagerBOTStatus.Connected;
                    botCurrent.UpdatedTime = DateTime.Now;
                    botCurrent.RAM = model.RamUsage;
                    //botCurrent.Bandwidth = model.Bandwidth;
                    botCurrent.DiskTotal = model.DiskInfo?.Total ?? 0;
                    botCurrent.DiskUsed = model.DiskInfo?.Used ?? 0;
                    botCurrent.DisconnectedTime = null;
                    botCurrent.IsSendNotification = false;
                    botCurrent.IsUsed = true;
                    _repository.Update(botCurrent);

                    //reset live backup
                }

                await _unitOfWork.SaveChangesAsync();

                var setting = await _repositorySetting.GetFirstOrDefaultAsync();

                List<IBotChatId> botChatIds = new List<IBotChatId>();
                if (setting is not null)
                    botChatIds.Add(setting);
                if (botCurrent.Manager is not null)
                {
                    botChatIds.Add(botCurrent.Manager.GetBotChatIdForStaff());
                }

                _logger.LogInformation($"{timeDisconnected}");
                var timeDelay = 0;
                int.TryParse(_configuration["TimeDelayBot"], out timeDelay);
                if (timeDelay == 0)
                {
                    timeDelay = 180;
                }

                if (timeUpdateOld < DateTime.Now.AddSeconds(-timeDelay)
                    || (statusOld == ManagerBOTStatus.Disconnected && (timeDisconnected < DateTime.Now.AddSeconds(-timeDelay) || timeDisconnected == null))
                    )
                {
                    if (botChatIds.Any())
                    {
                        var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        string textSend = $"BotName: {botCurrent.Name}{Environment.NewLine}"
                            + $"Group:{botCurrent.Group}{Environment.NewLine}"
                            + $"Manager:{botCurrent.Manager?.UserName} {Environment.NewLine}"
                            + $"Live: {botCurrent.LiveType}{Environment.NewLine}"
                            + $"Connected:{timer}";

                        _ = _botMessage.SendMessageAsync(botChatIds, textSend, BotChatService.SendType.Bot);
                    }

                }

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create BOT failed by error:[{ex}] " +
                    $"-[{model.BotId}- {model.Bandwidth} - {model.RamUsage} -{model.CpuUsage} - {model.ConnectionId}" +
                    $"{model.DiskInfo?.Total} - {model.DiskInfo?.Used}]");
                return new KeyValuePair<bool, string>(false, "Create BOT failed");
            }
        }

        public async Task<KeyValuePair<bool, string>> DeleteAsync(int id)
        {
            try
            {
                var _repositoryUser = _unitOfWork.GetRepository<AppUser>();

                var _repositoryUserBot = _unitOfWork.GetRepository<UserManagerBOT>();

                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
                var existStream = await _repositoryStream.ExistsAsync(
                    x => x.ManagerBOTId.GetValueOrDefault() == id && x.Status == StreamStatus.Streaming);
                if (existStream)
                {
                    return new KeyValuePair<bool, string>(false, "Đang có luồng Stream, không thể xóa!");
                }

                var _repository = _unitOfWork.GetRepository<ManagerBOT>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id, disableTracking: false);

                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không tồn tại!");
                }
                botCurrent.DeletedTime = DateTime.Now;
                botCurrent.Status = ManagerBOTStatus.Disconnected;
                botCurrent.UserManager = null;
                _repository.Update(botCurrent);

                //remove thread of users

                var userOfBots = await _repositoryUserBot.Queryable()
                    .Where(x => x.ManagerBOTId == botCurrent.Id).ToListAsync();

                if (userOfBots.Any())
                {

                    _repositoryUserBot.Delete(userOfBots);
                    //foreach (var item in userOfBots)
                    //{
                    //    switch (botCurrent.LiveType)
                    //    {
                    //        case LiveType._1080:
                    //            item.NumberOfThreads1080 = 0;
                    //            break;
                    //        case LiveType._4K:
                    //            item.NumberOfThreads4K = 0;
                    //            break;
                    //    }
                    //}
                    //_repositoryUser.Update(userOfBots);
                }

                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Delete BOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Xóa BOT không thành công vui lòng thử lại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> DisconnectBotAsync(string connectionId)
        {
            try
            {
                var _repositoryBot = _unitOfWork.GetRepository<ManagerBOT>();
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var botCurrent = await _repositoryBot.GetFirstOrDefaultAsync(
                    predicate: x => x.ConnectionId == connectionId, disableTracking: false);
                if (botCurrent != null)
                {
                    botCurrent.Status = ManagerBOTStatus.Disconnected;

                    var streamsCurrents = await _repositoryStream.Queryable()
                        .Where(x => x.ManagerBOTId == botCurrent.Id && x.Status == StreamStatus.Streaming).ToListAsync();

                    if (streamsCurrents.Any())
                    {
                        foreach (var item in streamsCurrents)
                        {
                            item.Status = StreamStatus.Stop;
                        }
                        _repositoryStream.Update(streamsCurrents);

                    }
                    _repositoryBot.Update(botCurrent);
                    await _unitOfWork.SaveChangesAsync();
                }
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Disconnect BOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, ex.Message);
            }
        }

        public async Task<IPagedList<ManagerBotInfoDto>> GetBOTByFilterAsync(string userId, ManagerBOTRequestDto model, bool isAdmin = false)
        {
            var _repositoryUserBot = _unitOfWork.GetRepository<UserManagerBOT>();

            var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();
            var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
            var query = _repository.Queryable().AsNoTracking();
            query = query.Where(x => x.DeletedTime == null);
            if (!isAdmin)
            {
                query = query.Where(x => x.UserManager == userId);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(model.Manager))
                {
                    query = query.Where(x => x.UserManager == model.Manager);
                }
                var lstManagers = new List<string>();
                if (!string.IsNullOrWhiteSpace(model.Managers))
                {
                    lstManagers = model.Managers.Split(',').ToList();
                }
                if (lstManagers.Any())
                {
                    query = query.Where(x => lstManagers.Any(i => i == x.UserManager));
                }
            }

            var data = await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<ManagerBotInfoDto>(x),
                include: x => x.Include(i => i.StreamHistorys).Include(x => x.Manager).Include(i => i.UserManagerBOTs),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: model.Page, pageSize: model.PageSize);

            if (data.Items.Any())
            {
                foreach (var item in data.Items)
                {

                    var totalThread = await _repositoryUserBot.Queryable()
                        .AsNoTracking()
                        .Include(x => x.User)
                        .Where(x => x.ManagerBOTId == item.Id)
                        .SumAsync(x => x.NumberOfThreads);
                    item.NumberOfThreads = totalThread;

                    if (item.LiveType == LiveType._Backup)
                    {
                        item.NumberOfThreadsBackupPending = await _repositoryStream.Queryable()
                            .Where(x => x.ManagerBOTBackupId.GetValueOrDefault() > 0
                            && x.Status == StreamStatus.Streaming
                            && x.ManagerBOTBackupId == item.Id).CountAsync();
                    }
                    //switch (item.LiveType)
                    //{
                    //    case LiveType._1080:
                    //        item.NumberOfThreads = dataThread.Sum(x => x.thread1080);
                    //        break;
                    //    case LiveType._4K:
                    //        item.NumberOfThreads = dataThread.Sum(x => x.thread4k);
                    //        break;
                    //}
                }
            }
            return data;
        }

        public async Task<ManagerBotInfoDto> GetBOTByIdAsync(int id)
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

            return await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == id,
                selector: x => _mapper.Map<ManagerBotInfoDto>(x));
        }

        public async Task<int> GetTotalBOTInRunAsync()
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

            return await _repository.Queryable()
                 .Where(x => x.StreamHistorys.Any(i => i.Status == StreamStatus.Streaming)).CountAsync();
        }

        public async Task<List<UserBotDto>> GetUserConnectBot(int id)
        {
            var _repository = _unitOfWork.GetRepository<UserManagerBOT>();

            var _repositoryUser = _unitOfWork.GetRepository<AppUser>();
            var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

            var data = await _repository.Queryable().AsNoTracking()
                .Where(x => x.ManagerBOTId == id).Include(x => x.ManagerBOT).Include(i => i.User)
                .Select(x => new UserBotDto
                {
                    BotName = x.ManagerBOT.Name,
                    UserName = x.User.UserName,
                    Type = x.LiveType,
                    UserId = x.UserId,
                    BotId = x.ManagerBOTId,

                }).ToListAsync();
            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    var userCurrent = await _repositoryUser.GetFirstOrDefaultAsync(
                        predicate: x => x.Id == item.UserId,
                        include: x => x.Include(x => x.StreamHistorys).Include(x => x.UserManagerBOTs),
                        selector: x => new
                        {
                            NumberOfThreads = x.UserManagerBOTs.Where(x => x.ManagerBOTId == item.BotId).Select(x => x.NumberOfThreads).FirstOrDefault(),
                            NumberOfThreadsInRun = x.StreamHistorys
                            .Where(i => i.Status == StreamStatus.Streaming && i.ManagerBOTId == item.BotId).Count()
                        });
                    if (userCurrent != null)
                    {
                        item.NumberOfThreads = userCurrent.NumberOfThreads;
                        item.NumberOfThreadsInRun = userCurrent.NumberOfThreadsInRun;
                    }
                }
            }
            ;
            return data;

        }

        public async Task<KeyValuePair<bool, string>> UpdateBOTAsync(int id, ManagerBotUpdateDto model, bool isAdmin = false)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<ManagerBOT>();

                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
                var _repositoryUserBot = _unitOfWork.GetRepository<UserManagerBOT>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id, disableTracking: false);
                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không tồn tại!");
                }
                var checkNameBot = await _repository.ExistsAsync(x => x.Name == model.Name && x.Id != id);

                if (checkNameBot)
                {
                    return new KeyValuePair<bool, string>(false, "Tên BOT đã tồn tại!");
                }

                botCurrent.Name = model.Name;
                botCurrent.Group = model.Group;
                botCurrent.NumberOfThreads = model.NumberOfThreads;
                botCurrent.UpdatedTime = DateTime.Now;
                botCurrent.LiveType = model.LiveType;

                if (isAdmin)
                {
                    if (string.IsNullOrWhiteSpace(botCurrent.UserManager))
                    {
                        botCurrent.UserManager = model.UserManager;
                    }
                    else if (botCurrent.UserManager != model.UserManager)
                    {
                        botCurrent.UserManager = model.UserManager;
                        //stop all streams
                        var userOfBots = await _repositoryUserBot.Queryable()
                            .Where(x => x.ManagerBOTId == botCurrent.Id).ToListAsync();

                        if (userOfBots.Any())
                        {
                            var streamsCurrents = await _repositoryStream.Queryable()
                                .Where(x => x.ManagerBOTId == botCurrent.Id
                                && x.RenderStatus != RenderStatus.Cancelled
                                && x.RenderStatus != RenderStatus.Error
                                && x.RenderStatus != RenderStatus.Ended
                                ).ToListAsync();

                            if (streamsCurrents.Any())
                            {
                                foreach (var item in streamsCurrents)
                                {
                                    item.Status = StreamStatus.Stop;
                                    item.ManagerBOTId = null;
                                }
                                _repositoryStream.Update(streamsCurrents);
                                await _unitOfWork.SaveChangesAsync();
                                foreach (var item in streamsCurrents)
                                {
                                    await _botContextHub.Clients.All.RenderAsync(new RenderCommand
                                    {
                                        ItemId = item.Id,
                                        Command = Command.Cancel
                                    });
                                }
                            }
                            _repositoryUserBot.Delete(userOfBots);
                        }
                    }
                }
                _repository.Update(botCurrent);


                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update ManagetBOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật BOT không thành công!");
            }
        }


        public async Task<KeyValuePair<bool, string>> UpdateStatusAsync(int id, ManagerBOTStatus status)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<ManagerBOT>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id, disableTracking: false);
                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không tồn tại!");
                }
                botCurrent.Status = status;
                _repository.Update(botCurrent);

                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update status ManagetBOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật trạng thái BOT không thành công!");
            }
        }
    }
}
