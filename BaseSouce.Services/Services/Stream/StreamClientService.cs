using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Services.HubServices;
using BaseSouce.Services.Services.User;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.BotClient;
using BaseSouce.SharedSignalrData.Server;
using BaseSource.Shared.Enums;
using BaseSource.Data.Entities;
using BaseSource.Shared.Helpers;
using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TqkLibrary.Net.CloudStorage.GoogleDrive;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BaseSouce.Services.Services.Stream
{


    public class StreamClientService : IStreamClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StreamClientService> _logger;
        private readonly IMapper _mapper;
        private readonly IHubContext<BotHub, IBotClientHub> _botContextHub;
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _telegramBotClient;
        public StreamClientService(
            IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<StreamClientService> logger,
            IMapper mapper,
            IHubContext<BotHub, IBotClientHub> botContextHub,
            IUserService userService,
            ITelegramBotClient telegramBotClient)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _botContextHub = botContextHub;
            _userService = userService;
            _telegramBotClient = telegramBotClient;

        }


        public async Task<BoolStringKeyValuePair> SendDataToBOTAsync(StreamHistory model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.StreamKey))
                {
                    return $"Chưa nhập {nameof(model.StreamKey)}";
                }

                if (!Uri.TryCreate(model.RTMP, UriKind.Absolute, out Uri? rtmp))
                {
                    return $"{nameof(model.RTMP)} sai định dạng";
                }
                string url = $"{rtmp.Scheme}://{rtmp.Host}{rtmp.AbsolutePath}/{model.StreamKey}";
                if (!string.IsNullOrWhiteSpace(rtmp.Query)) url += rtmp.Query;
                Uri uri = new Uri(url);

                List<string> renderArguments = new List<string>();
                {//global
                    renderArguments.Add("-y");
                }
                {//video
                    if (!string.IsNullOrWhiteSpace(model.Audio))
                    {
                        renderArguments.Add("-an");
                        renderArguments.Add($"-stream_loop -1");
                    }
                    renderArguments.Add("-i video");
                }
                if (!string.IsNullOrWhiteSpace(model.Audio))
                {
                    renderArguments.Add("-vn");
                    renderArguments.Add("-i sound");
                }
                {//output
                    renderArguments.Add("-c copy");
                    renderArguments.Add("-shortest");
                    renderArguments.Add("-f flv");
                    //renderArguments.Add("-flvflags no_duration_filesize");
                    renderArguments.Add("rendered");
                }



                List<string> liveStreamArguments = new List<string>();
                {//input
                    liveStreamArguments.Add("-re");
                    //liveStreamArguments.Add($"-stream_loop -1");
                    liveStreamArguments.Add("-f flv");
                    liveStreamArguments.Add("-i rendered");
                }

                {//output
                    liveStreamArguments.Add("-c copy");
                    liveStreamArguments.Add("-f flv");
                    liveStreamArguments.Add("-flvflags no_duration_filesize");
                    liveStreamArguments.Add($"\"{uri}\"");
                }


                RenderCommand renderCommand = new RenderCommand()
                {
                    RenderArguments = string.Join(" ", renderArguments),
                    LiveStreamArguments = string.Join(" ", liveStreamArguments),
                    Command = Command.Start,
                    UtcEndTime = model.EndTimeLive?.ToUniversalTime(),
                    UtcStartTime = model.StartTimeLive?.ToUniversalTime(),
                    WorkId = 0,//quan trọng, cần để cancel/update/tạo thư mục
                    ItemId = model.Id,//quan trọng, cần để cancel/update/tạo thư mục( stream ID)
                    FileDatas = new List<FileData>()
                    {
                        new FileData()
                        {
                            Name = "video",
                            Url = model.Video,
                            UrlType = UrlType.GoogleDrive
                        }
                    },
                    FileType = SharedSignalrData.Server.FileType.File,
                };
                if (!string.IsNullOrWhiteSpace(model.Audio))
                {
                    renderCommand.FileDatas.Add(new FileData()
                    {
                        Name = "sound",
                        Url = model.Audio,
                        UrlType = UrlType.GoogleDrive
                    });
                }
                _logger.LogInformation($"[Bot ConnectionId:[{model.ManagerBOT.ConnectionId}]]");
                //await _botContextHub.Clients.Client(model.ManagerBOT.ConnectionId).RenderAsync(renderCommand);
                await _botContextHub.Clients.Client(model.ManagerBOT.ConnectionId).RenderAsync(renderCommand);
                return true;

                //if (model.IsLiveNow)
                //{

                //}
                //else
                //{
                //    //hẹn giờ


                //    return true;
                //}
            }
            catch (NotSupportedException nse)
            {
                _logger.LogError($"Send data to BOT failed by error:[{nse}]");
                return $"{nse.Message} chưa hỗ trợ";
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send data to BOT failed by error:[{ex}]");
                return $"Lỗi {ex.Message}";
            }
        }


        private async Task<BoolStringKeyValuePair> ValidateLink(string video, string audio)
        {
            var driveVideoItem = GoogleDriveHelper.TryParse(video);
            if (driveVideoItem is null ||
                driveVideoItem.LinkType != GoogleDriveLinkType.File)
                return "Link video không hỗ trợ hoặc không hợp lệ";

            var driveAudioItem = GoogleDriveHelper.TryParse(audio);
            if (!string.IsNullOrWhiteSpace(audio) &&
                (driveAudioItem is null || driveAudioItem.LinkType != GoogleDriveLinkType.File))
                return new BoolStringKeyValuePair(false, "Link audio không hỗ trợ hoặc không hợp lệ");


            using (DriveApiNonLogin driveApiNonLogin = new DriveApiNonLogin())
            {
                try
                {
                    var videoMetadata = await driveApiNonLogin.GetMetadataAsync(driveVideoItem.ID);
                }
                catch (Exception)
                {
                    return "Link video không truy cập được";
                }

                if (driveAudioItem is not null)
                {
                    try
                    {
                        var videoMetadata = await driveApiNonLogin.GetMetadataAsync(driveAudioItem.ID);
                    }
                    catch (Exception)
                    {
                        return "Link audio không truy cập được";
                    }
                }
            }

            return true;
        }


        public async Task<BoolStringKeyValuePair> CreateAsync(string userId, StreamCreateDto model)
        {
            try
            {
                //check link audio
                var result = await ValidateLink(model.Video, model.Audio);
                if (!result)
                {
                    return result;
                }

                //add database
                var _repositoryUser = _unitOfWork.GetRepository<AppUser>();
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
                var _repositoryUserBot = _unitOfWork.GetRepository<UserManagerBOT>();

                //var userCurrent = await _repositoryUser.GetFirstOrDefaultAsync(
                //    predicate: x => x.Id == userId,
                //    selector: x => new
                //    {
                //        NumberOfThreads1080 = x.UserManagerBOTs.Where(x => x.ManagerBOT.LiveType == LiveType._1080).Sum(x => x.NumberOfThreads),
                //        NumberOfThreads1080InRun = x.StreamHistorys.Where(i => i.LiveType == LiveType._1080).Count(),
                //        NumberOfThreads4k = x.UserManagerBOTs.Where(x => x.ManagerBOT.LiveType == LiveType._4K).Sum(x => x.NumberOfThreads),
                //        NumberOfThreads4kInRun = x.StreamHistorys.Where(i => i.LiveType == LiveType._4K).Count(),
                //        Bot1080Id = x.UserManagerBOTs.Where(i => i.LiveType == LiveType._1080).Select(i => new
                //        {
                //            Id = i.ManagerBOTId,
                //            BotId = i.ManagerBOT.BotId
                //        }).FirstOrDefault(),
                //        Bot4k0Id = x.UserManagerBOTs.Where(i => i.LiveType == LiveType._4K).Select(i => new
                //        {
                //            Id = i.ManagerBOTId,
                //            BotId = i.ManagerBOT.BotId
                //        }).FirstOrDefault()
                //    },
                //    include: x => x.Include(i => i.UserManagerBOTs).ThenInclude(o => o.ManagerBOT));

                var userBots = await _repositoryUserBot.Queryable()
                    .Include(x => x.ManagerBOT).ThenInclude(i => i.StreamHistorys)
                    .Where(x => x.UserId == userId && x.LiveType == model.LiveType && x.ManagerBOT.Status == ManagerBOTStatus.Connected)
                    .Select(x => new
                    {
                        BotId = x.ManagerBOTId,
                        BotIdString = x.ManagerBOT.BotId,
                        NumberOfThreads = x.NumberOfThreads,
                        NumberOfThreadsInRun = x.ManagerBOT.StreamHistorys.Where(i => i.UserId == userId && i.Status == StreamStatus.Streaming).Count(),
                    })
                    .OrderBy(x => x.NumberOfThreadsInRun)
                    .ToListAsync();

                //var profileUser = await _userService.GetProfileInfoAsync(userId);

                var botConnectCurrent = string.Empty;
                var botIdCurrent = 0;

                //if (userBots == null || !userBots.Any())
                //{
                //    return "BOT không hoạt động hoặc chưa kết nối!";
                //}

                //foreach (var item in userBots)
                //{
                //    if (item.NumberOfThreadsInRun < item.NumberOfThreads)
                //    {
                //        botIdCurrent = item.BotId;
                //        botConnectCurrent = item.BotIdString;
                //        break;
                //    }
                //}
                //if (botIdCurrent == 0)
                //{
                //    return "BOT không hoạt động hoặc chưa kết nối!";
                //}

                //if (model.LiveType == LiveType._1080)
                //{

                //    if (userCurrent.NumberOfThreads1080 < userCurrent.NumberOfThreads1080InRun
                //        || userCurrent.NumberOfThreads1080 == 0)
                //    {
                //        return "Số luồng 1080 hiện tại đã đạt tối đa!";
                //    }
                //    if (userCurrent.Bot1080Id == null)
                //    {
                //        return "BOT không hoạt động hoặc chưa kết nối!";
                //    }
                //    botConnectCurrent = userCurrent.Bot1080Id?.BotId;
                //    botIdCurrent = userCurrent.Bot1080Id.Id;
                //}
                //if (model.LiveType == LiveType._4K)
                //{
                //    if (userCurrent.NumberOfThreads4k < userCurrent.NumberOfThreads4kInRun
                //        || userCurrent.NumberOfThreads4k == 0)
                //    {
                //        return "Số luồng 4k hiện tại đã đạt tối đa!";
                //    }
                //    if (userCurrent.Bot4k0Id == null)
                //    {
                //        return "BOT không hoạt động hoặc chưa kết nối!";
                //    }
                //    botConnectCurrent = userCurrent.Bot4k0Id?.BotId;
                //    botIdCurrent = userCurrent.Bot4k0Id.Id;
                //}

                //if (string.IsNullOrEmpty(botConnectCurrent))
                //{
                //    return "BOT không hoạt động hoặc chưa kết nối!";
                //}

                //check role live

                if (model.IsLiveNow)
                {
                    model.StartTimeLive = null;
                    model.EndTimeLive = null;
                }
                else if (model.StartTimeLive != null)
                {
                    model.IsLiveNow = false;
                }

                //var packageCurrent = model.LiveType == LiveType._1080 ? profileUser.PackageLive1080 : profileUser.PackageLive4k;

                //if (packageCurrent == null)
                //{
                //    return "Bạn chưa đăng ký gói cước để Livestream, vui lòng đăng ký để sử dụng!";
                //}

                //else if (packageCurrent.NumberOfThreadsCreatedInRun >= packageCurrent.NumberOfThreadsCreated)
                //{
                //    return "Số lượng luồng của bạn đã vượt giới hạn của gói hiện tại, vui lòng thử lại!";
                //}

                //get bot 

                var _repositoryBOT = _unitOfWork.GetRepository<ManagerBOT>();

                //var botCurrent = await _repositoryBOT.GetFirstOrDefaultAsync(
                //    predicate: x => x.NumberOfThreads > 0 && x.Status == ManagerBOTStatus.Connected && x.IsUsed);

                //if (botCurrent == null)
                //{
                //    return "Không có BOT, vui lòng thử lại";
                //}

                var streamEntityNew = new StreamHistory
                {
                    Audio = model.Audio,
                    Video = model.Video,
                    StreamName = model.StreamName,
                    //BotId = botConnectCurrent,
                    //ManagerBOTId = botIdCurrent,
                    PlatformType = model.PlatformType,
                    RenderStatus = RenderStatus.Created,
                    CreatedTime = DateTime.Now,
                    IsForever = model.IsForever,
                    IsLiveNow = model.IsLiveNow,
                    RTMP = model.RTMP,
                    StartTimeLive = model.StartTimeLive,
                    EndTimeLive = model.EndTimeLive,
                    Status = StreamStatus.Stop,
                    StreamKey = model.StreamKey,
                    UserId = userId,
                    LiveType = model.LiveType,
                    LiveOrder = model.LiveOrder,
                };

                _repositoryStream.Insert(streamEntityNew);

                await _unitOfWork.SaveChangesAsync();

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Send data to BOT failed by error:[{ex}]");
                return $"Lỗi {ex.Message}";
            }
        }

        public async Task<KeyValuePair<bool, string>> DeleteAsync(string userId, int id)
        {
            try
            {
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
                var streamEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == userId && x.Id == id, disableTracking: false);
                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }
                _repositoryStream.Delete(streamEntity);

                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete Stream failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Xóa luồng không thành công, vui lòng thử lại!");
            }
        }

        public async Task<IPagedList<StreamClientInfoDto>> GetByFilterAsync(string userId, StreamClientRequestDto model)
        {
            var _repository = _unitOfWork.GetRepository<StreamHistory>();

            var query = _repository.Queryable().AsNoTracking();
            query = query.Where(x => x.UserId == userId);

            return await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<StreamClientInfoDto>(x),
                include: x => x.Include(i => i.AppUser).Include(i => i.ManagerBOT),
               orderBy: x => x.OrderByDescending(i => i.CreatedTime),
               pageIndex: model.Page, pageSize: model.PageSize
                );
        }

        public async Task<KeyValuePair<bool, string>> StartStreamAsync(string userId, int id)
        {
            try
            {
                var _repositoryUser = _unitOfWork.GetRepository<AppUser>();

                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var _repositoryBOT = _unitOfWork.GetRepository<ManagerBOT>();

                var _repositoryUserBOT = _unitOfWork.GetRepository<UserManagerBOT>();


                var userCurrent = await _repositoryUser.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == userId, disableTracking: true);

                //var streamEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                //   predicate: x => x.UserId == userId && x.Id == id,
                //   include: x => x.Include(i => i.ManagerBOT),
                //   disableTracking: false);

                var streamEntity = await _repositoryStream.Queryable()
                    .Include(x => x.ManagerBOT)
                    .Where(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync();



                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }

                //_logger.LogInformation($"Stream Info [streamName:[{streamEntity.StreamName}] " +
                //  $"- BotName:[{streamEntity.ManagerBOT?.Name}] " +
                //  $"- Connection:[{streamEntity.ManagerBOT?.ConnectionId}]]");

                var userBots = await _repositoryUserBOT.Queryable()
                    .Include(x => x.ManagerBOT).ThenInclude(i => i.StreamHistorys)
                    .Where(x => x.UserId == userId
                    && x.LiveType == streamEntity.LiveType
                    && x.ManagerBOT.Status == ManagerBOTStatus.Connected)
                    .Select(x => new
                    {
                        BotId = x.ManagerBOTId,
                        BotIdString = x.ManagerBOT.BotId,
                        NumberOfThreads = x.NumberOfThreads,
                        NumberOfThreadsInRun = x.ManagerBOT.StreamHistorys.Where(i => i.UserId == userId && i.Status == StreamStatus.Streaming).Count(),
                    })
                    .OrderBy(x => x.NumberOfThreadsInRun)
                    .ToListAsync();

                if (userBots == null || !userBots.Any())
                {
                    return new KeyValuePair<bool, string>(false, "BOT không có hoặc không tồn tại, vui lòng thử lại!");
                }

                var botId = 0;
                var botIdString = string.Empty;
                foreach (var item in userBots)
                {
                    if (item.NumberOfThreadsInRun < item.NumberOfThreads)
                    {
                        botId = item.BotId;
                        botIdString = item.BotIdString;
                        break;
                    }
                }

                //_logger.LogInformation($"BOT New [botId:[{botId}]- BotIdString:[{botIdString}]]");

                if (botId == 0)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không hoạt động hoặc chưa kết nối!");
                }

                //check connection bot

                //get bot connect 
                //var botConnect = await _repositoryUserBOT.GetFirstOrDefaultAsync(
                //    predicate: x => x.UserId == userId && x.ManagerBOT.Status == ManagerBOTStatus.Connected,
                //    include: x => x.Include(i => i.ManagerBOT),
                //    selector: x => new
                //    {
                //        Status = x.ManagerBOT.Status,
                //        BotId = x.ManagerBOTId,
                //        BotStringId = x.ManagerBOT.BotId,
                //        //ConnectionId = x.ManagerBOT.ConnectionId
                //    });

                //if (botConnect == null)
                //{
                //    return new KeyValuePair<bool, string>(false, "BOT không có hoặc không tồn tại, vui lòng thử lại!");
                //}


                //var botCurrent = await _repositoryBOT.GetFirstOrDefaultAsync(
                //    predicate: x => x.Id == streamEntity.ManagerBOTId);

                //if (botCurrent == null)
                //{
                //    return new KeyValuePair<bool, string>(false, "BOT không tồn tại, vui lòng thử lại!");
                //}

                //if (botConnect.Status == ManagerBOTStatus.Disconnected)
                //{
                //    return new KeyValuePair<bool, string>(false, "BOT đang Disconnect, vui lòng thử lại!");
                //}

                streamEntity.Status = StreamStatus.Streaming;
                streamEntity.BotId = botIdString;
                streamEntity.ManagerBOTId = botId;

                _repositoryStream.Update(streamEntity);
                await _unitOfWork.SaveChangesAsync();

                streamEntity = await _repositoryStream.Queryable()
                    .Include(x => x.ManagerBOT)
                    .Where(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync();

                //_logger.LogInformation($"Stream Info [streamName:[{streamEntity.StreamName}] " +
                //    $"- BotName:[{streamEntity.ManagerBOT?.Name}] " +
                //    $"- Connection:[{streamEntity.ManagerBOT?.ConnectionId}]]");


                ///send data to BOT
                var result = await SendDataToBOTAsync(streamEntity);
                if (!result)
                {
                    _logger.LogError($"Send data to BOT failed by error:[{result.Value}]");

                    streamEntity.Status = StreamStatus.Stop;
                    _repositoryStream.Update(streamEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new KeyValuePair<bool, string>(false, result.Value);

                }
                //send message telegram
                if (!string.IsNullOrEmpty(userCurrent.TelegramAPI))
                {
                    var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                    var timer = streamEntity.StartTimeLive != null ?
                        streamEntity.StartTimeLive?.ToString("dd/MM/yyyy HH:mm:ss")
                        : "Ngay bây giờ";
                    var textTimer = timer == "Ngay bây giờ" ? "Chạy live" : "Hẹn lịch";
                    _ = _telegramBotClient
                    .SendTextMessageAsync(new ChatId(userCurrent.TelegramAPI),
                    $"User: {userCurrent.UserName}{Environment.NewLine}"
                    + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                    + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                    + $"Kiểu live: {liveType}{Environment.NewLine}"
                    + $"{textTimer}:{timer}", ParseMode.Default, true);
                }

                if (!string.IsNullOrEmpty(userCurrent.LinkTelegram))
                {
                    var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                    var timer = streamEntity.StartTimeLive != null ?
                        streamEntity.StartTimeLive?.ToString("dd/MM/yyyy HH:mm:ss")
                        : "Ngay bây giờ";
                    var textTimer = timer == "Ngay bây giờ" ? "Chạy live" : "Hẹn lịch";
                    var idTeles = userCurrent.LinkTelegram.Split(',');
                    if (idTeles.Length > 0)
                    {
                        foreach (var item in idTeles)
                        {
                            _ = _telegramBotClient
                           .SendTextMessageAsync(new ChatId(item.Trim()),
                           $"User: {userCurrent.UserName}{Environment.NewLine}"
                           + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                           + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                           + $"Kiểu live: {liveType}{Environment.NewLine}"
                           + $"{textTimer}:{timer}", ParseMode.Default, true);
                        }
                    }
                }


                return new KeyValuePair<bool, string>(true, string.Empty);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Start livestream failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Live không thành công vui lòng thử lại");
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateAsync(string userId, int id, StreamUpdateDto model)
        {
            try
            {
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var streamEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                    predicate: x => x.UserId == userId && x.Id == id,
                    disableTracking: false);

                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }
                if (model.IsLiveNow)
                {
                    model.StartTimeLive = null;
                    model.EndTimeLive = null;
                }
                else if (model.StartTimeLive != null)
                {
                    model.IsLiveNow = false;
                }
                //update data
                streamEntity.StreamName = model.StreamName;
                streamEntity.Video = model.Video;
                streamEntity.Audio = model.Audio;
                streamEntity.IsForever = model.IsForever;
                streamEntity.IsLiveNow = model.IsLiveNow;
                streamEntity.RTMP = model.RTMP;
                streamEntity.StartTimeLive = model.StartTimeLive;
                streamEntity.EndTimeLive = model.EndTimeLive;
                streamEntity.StreamKey = model.StreamKey;
                streamEntity.LiveOrder = model.LiveOrder;

                _repositoryStream.Update(streamEntity);


                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update Stream [{id}] failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật luồng không thành công, vui lòng thử lại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateStreamAsync(RenderMessage model)
        {
            try
            {
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var streamEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == model.ItemId,
                    disableTracking: false,
                    include: x => x.Include(i => i.AppUser));

                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }

                var statusOld = streamEntity.RenderStatus;


                //
                streamEntity.RenderStatus = model.RenderStatus;
                if (model.RenderStatus == RenderStatus.Error)
                {
                    _logger.LogError($"Error when render :[{model.ExceptionInfo}]");
                    streamEntity.Status = StreamStatus.Stop;
                    streamEntity.ErrorMessage = model.ExceptionInfo.Message;
                }
                if (model.RenderStatus == RenderStatus.Ended)
                {
                    streamEntity.Status = StreamStatus.Stop;

                }
                _repositoryStream.Update(streamEntity);
                await _unitOfWork.SaveChangesAsync();

                if (statusOld != RenderStatus.LiveStreaming && streamEntity.RenderStatus == RenderStatus.LiveStreaming)
                {

                    //send message telegram
                    var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                    var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    if (!string.IsNullOrEmpty(streamEntity.AppUser.TelegramAPI))
                    {
                        _ = _telegramBotClient
                    .SendTextMessageAsync(new ChatId(streamEntity.AppUser.TelegramAPI),
                    $"User: {streamEntity.AppUser.UserName}{Environment.NewLine}"
                    + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                    + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                    + $"Kiểu live: {liveType}{Environment.NewLine}"
                    + $"Bắt đầu:{timer}", ParseMode.Default, true);
                    }

                    if (!string.IsNullOrEmpty(streamEntity.AppUser.LinkTelegram))
                    {
                        var idTeles = streamEntity.AppUser.LinkTelegram.Split(',');
                        if (idTeles.Length > 0)
                        {
                            foreach (var item in idTeles)
                            {
                                _ = _telegramBotClient
                               .SendTextMessageAsync(new ChatId(item.Trim()),
                               $"User: {streamEntity.AppUser.UserName}{Environment.NewLine}"
                               + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                               + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                               + $"Kiểu live: {liveType}{Environment.NewLine}"
                               + $"Bắt đầu:{timer}", ParseMode.Default, true);
                            }
                        }
                    }
                }
                else if (streamEntity.RenderStatus == RenderStatus.Error)
                {
                    var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                    var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    if (!string.IsNullOrEmpty(streamEntity.AppUser.TelegramAPI))
                    {
                        _ = _telegramBotClient
                    .SendTextMessageAsync(new ChatId(streamEntity.AppUser.TelegramAPI),
                    $"User: {streamEntity.AppUser.UserName}{Environment.NewLine}"
                    + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                    + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                    + $"Kiểu live: {liveType}{Environment.NewLine}"
                    + $"Lỗi:{streamEntity.ErrorMessage}", ParseMode.Default, true);
                    }
                    if (!string.IsNullOrEmpty(streamEntity.AppUser.LinkTelegram))
                    {
                        var idTeles = streamEntity.AppUser.LinkTelegram.Split(',');
                        if (idTeles.Length > 0)
                        {
                            foreach (var item in idTeles)
                            {
                                _ = _telegramBotClient
                               .SendTextMessageAsync(new ChatId(item.Trim()),
                               $"User: {streamEntity.AppUser.UserName}{Environment.NewLine}"
                               + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                               + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                               + $"Kiểu live: {liveType}{Environment.NewLine}"
                               + $"Lỗi:{timer}", ParseMode.Default, true);
                            }
                        }
                    }

                }
                else if (streamEntity.RenderStatus == RenderStatus.Ended)
                {
                    streamEntity.Status = StreamStatus.Stop;

                    var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                    var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                    if (!string.IsNullOrEmpty(streamEntity.AppUser.TelegramAPI))
                    {
                        _ = _telegramBotClient
                       .SendTextMessageAsync(new ChatId(streamEntity.AppUser.TelegramAPI),
                       $"User: {streamEntity.AppUser.UserName}{Environment.NewLine}"
                       + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                       + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                       + $"Kiểu live: {liveType}{Environment.NewLine}"
                       + $"Dừng:{timer}", ParseMode.Default, true);
                    }
                    if (!string.IsNullOrEmpty(streamEntity.AppUser.LinkTelegram))
                    {
                        var idTeles = streamEntity.AppUser.LinkTelegram.Split(',');
                        if (idTeles.Length > 0)
                        {
                            foreach (var item in idTeles)
                            {
                                _ = _telegramBotClient
                               .SendTextMessageAsync(new ChatId(item.Trim()),
                               $"User: {streamEntity.AppUser.UserName}{Environment.NewLine}"
                               + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                               + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                               + $"Kiểu live: {liveType}{Environment.NewLine}"
                               + $"Dừng:{timer}", ParseMode.Default, true);
                            }
                        }
                    }
                }


                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update Stream [{model.ItemId}] failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật luồng không thành công, vui lòng thử lại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> CancelStreamAsync(string userId, int id)
        {
            try
            {
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var streamEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id && x.UserId == userId,
                    disableTracking: false);

                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }

                //
                streamEntity.Status = StreamStatus.Stop;

                _repositoryStream.Update(streamEntity);
                await _unitOfWork.SaveChangesAsync();

                // call BOT cancel

                await _botContextHub.Clients.All.RenderAsync(new RenderCommand
                {
                    ItemId = streamEntity.Id,
                    Command = Command.Cancel
                });



                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cancel Stream [{id}] failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Dừng luồng không thành công, vui lòng thử lại!");
            }
        }

        public async Task<StreamClientInfoDto> GetStreamByIdAsync(string userId, int id)
        {
            var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

            return await _repositoryStream.GetFirstOrDefaultAsync(
                predicate: x => x.Id == id && x.UserId == userId,
                selector: x => _mapper.Map<StreamClientInfoDto>(x),
                include: x => x.Include(i => i.AppUser),
                disableTracking: false);
        }

        public async Task<StreamReportDto> GetStreamReportAsync(string userId)
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            return await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == userId,
                include: x => x.Include(i => i.StreamHistorys),
                selector: x => new StreamReportDto
                {
                    Total = x.StreamHistorys.Count(),
                    TotalStop = x.StreamHistorys.Where(i => i.Status == StreamStatus.Stop).Count(),
                    TotalStreaming = x.StreamHistorys.Where(i => i.Status == StreamStatus.Streaming).Count(),
                    TotalTimer = x.StreamHistorys.Where(x => x.StartTimeLive != null && x.StartTimeLive > DateTime.Now).Count()
                });
        }

        public async Task<KeyValuePair<bool, string>> ValidateLinkAsync(string link)
        {
            var driveVideoItem = GoogleDriveHelper.TryParse(link);
            if (driveVideoItem is null ||
                driveVideoItem.LinkType != GoogleDriveLinkType.File)
                return new KeyValuePair<bool, string>(false, "Link  không hỗ trợ hoặc không hợp lệ");

            using (DriveApiNonLogin driveApiNonLogin = new DriveApiNonLogin())
            {
                try
                {
                    var videoMetadata = await driveApiNonLogin.GetMetadataAsync(driveVideoItem.ID);
                }
                catch (Exception)
                {
                    return new KeyValuePair<bool, string>(false, "Link không truy cập được");
                }
            }

            return new KeyValuePair<bool, string>(true, string.Empty);
        }
    }
}
