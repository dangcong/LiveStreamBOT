using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Services.HubServices;
using BaseSouce.Services.Services.User;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.Enums;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Interfaces;
using BaseSource.Data.Entities;
using BaseSource.Shared.Helpers;
using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BaseSouce.Services.Services.ValidateLink;
using BaseSource.ViewModels.ManagerBOT;
using Microsoft.Extensions.Configuration;
using BaseSouce.Services.BotChatServices;
using BaseSource.Shared.Interfaces;

namespace BaseSouce.Services.Services.Stream
{
    public class StreamClientService : IStreamClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StreamClientService> _logger;
        private readonly IMapper _mapper;
        private readonly IHubContext<BotHub, IBotClientHub> _botContextHub;
        private readonly IUserService _userService;
        private readonly IBotChatService _botMessage;
        private readonly IValidateLinkService _validateLinkService;
        private readonly IConfiguration _configuration;
        public StreamClientService(
            IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<StreamClientService> logger,
            IMapper mapper,
            IHubContext<BotHub, IBotClientHub> botContextHub,
            IUserService userService,
            IBotChatService botMessage,
            IValidateLinkService validateLinkService,
            IConfiguration configuration
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _botContextHub = botContextHub;
            _userService = userService;
            _botMessage = botMessage;
            _validateLinkService = validateLinkService;
            _configuration = configuration;
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
                //model.RTMP = "";
                Uri uri = new Uri($"{model.RTMP.ToString().TrimEnd('/')}/{model.StreamKey.Trim()}");

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


                var videoUrlType = _validateLinkService.GetUrlType(model.Video);
                if (!videoUrlType.HasValue)
                    return $"Không hỗ trợ kiểu link '{model.Video}'";

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
                            UrlType = videoUrlType.Value,
                        }
                    },
                    FileType = BaseSouce.SharedSignalrData.Enums.FileType.File,
                };

                if (!string.IsNullOrWhiteSpace(model.Audio))
                {
                    var audioUrlType = _validateLinkService.GetUrlType(model.Audio);
                    if (!audioUrlType.HasValue)
                        return $"Không hỗ trợ kiểu link '{model.Audio}'";

                    renderCommand.FileDatas.Add(new FileData()
                    {
                        Name = "sound",
                        Url = model.Audio,
                        UrlType = audioUrlType.Value,
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


        public async Task<BoolStringKeyValuePair> SendDataBackupToBOTAsync(StreamHistory model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.StreamKey))
                {
                    return $"Chưa nhập {nameof(model.StreamKey)}";
                }

                //if (!Uri.TryCreate(model.RTMP, UriKind.Absolute, out Uri? rtmp))
                //{
                //    return $"{nameof(model.RTMP)} sai định dạng";
                //}
                model.RTMP = "rtmp://y.rtmp.youtube.com/live2?backup=1";
                Uri uri = new Uri($"{model.RTMP.ToString().TrimEnd('/')}/{model.StreamKey.Trim()}");

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


                var videoUrlType = _validateLinkService.GetUrlType(model.Video);
                if (!videoUrlType.HasValue)
                    return $"Không hỗ trợ kiểu link '{model.Video}'";

                RenderCommand renderCommand = new RenderCommand()
                {
                    RenderArguments = string.Join(" ", renderArguments),
                    LiveStreamArguments = string.Join(" ", liveStreamArguments),
                    Command = Command.Start,
                    UtcEndTime = model.EndTimeLive?.ToUniversalTime(),
                    UtcStartTime = DateTime.Now.ToUniversalTime(),
                    WorkId = 0,//quan trọng, cần để cancel/update/tạo thư mục
                    ItemId = model.Id,//quan trọng, cần để cancel/update/tạo thư mục( stream ID)
                    FileDatas = new List<FileData>()
                    {
                        new FileData()
                        {
                            Name = "video",
                            Url = model.Video,
                            UrlType = videoUrlType.Value,
                        }
                    },
                    FileType = BaseSouce.SharedSignalrData.Enums.FileType.File,
                };

                if (!string.IsNullOrWhiteSpace(model.Audio))
                {
                    var audioUrlType = _validateLinkService.GetUrlType(model.Audio);
                    if (!audioUrlType.HasValue)
                        return $"Không hỗ trợ kiểu link '{model.Audio}'";

                    renderCommand.FileDatas.Add(new FileData()
                    {
                        Name = "sound",
                        Url = model.Audio,
                        UrlType = audioUrlType.Value,
                    });
                }
                _logger.LogInformation($"[Bot ConnectionId:[{model.ManagerBOTBackup.ConnectionId}]]");
                //await _botContextHub.Clients.Client(model.ManagerBOT.ConnectionId).RenderAsync(renderCommand);
                await _botContextHub.Clients.Client(model.ManagerBOTBackup.ConnectionId).RenderAsync(renderCommand);
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
            var result = await _validateLinkService.ValidateLinkAsync(video, "video");
            if (!result.Key)
                return result.Value;

            if (!string.IsNullOrWhiteSpace(audio))
            {
                result = await _validateLinkService.ValidateLinkAsync(audio, "audio");
                if (!result.Key)
                    return result.Value;
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

                var userBots = await _repositoryUserBot.Queryable()
                    .Include(x => x.ManagerBOT).ThenInclude(i => i.StreamHistorys)
                    .Where(x => x.UserId == userId && x.LiveType == model.LiveType && x.ManagerBOT.Status == ManagerBOTStatus.Connected)
                    .Select(x => new
                    {
                        BotId = x.ManagerBOTId,
                        BotIdString = x.ManagerBOT.BotId,
                        NumberOfThreads = x.NumberOfThreads,
                        NumberOfThreadsInRun = x.ManagerBOT.StreamHistorys.Where(i => i.UserId == userId && i.Status == StreamStatus.Streaming).Count()
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
                    RTMP = "rtmp://x.rtmp.youtube.com/live2",
                    StartTimeLive = model.StartTimeLive,
                    EndTimeLive = model.EndTimeLive,
                    Status = StreamStatus.Stop,
                    StreamKey = model.StreamKey,
                    UserId = userId,
                    LiveType = model.LiveType,
                    ManagerBOTId = model.ManagerBOTId > 0 ? model.ManagerBOTId : null,
                    LiveOrder = model.LiveOrder,
                    TimeBackup = model.TimeBackup,
                    ManagerBOTBackupId = model.ManagerBotBackupId.GetValueOrDefault() > 0 ? model.ManagerBotBackupId : null,
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
                include: x => x.Include(i => i.AppUser)
                .Include(i => i.ManagerBOT)
                .Include(i => i.ManagerBOTBackup),
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


                var streamEntity = await _repositoryStream.Queryable()
                    .Include(x => x.ManagerBOT)
                    .Where(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync();

                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }

                //if (streamEntity.StreamBackupId.GetValueOrDefault() > 0)
                //{
                //    //stop live backup
                //    var streamBackup = await _repositoryStream.GetFirstOrDefaultAsync(
                //        predicate: x => x.Id == streamEntity.StreamBackupId.GetValueOrDefault());
                //    if (streamBackup != null)
                //    {
                //        // call BOT cancel
                //        await _botContextHub.Clients.All.RenderAsync(new RenderCommand
                //        {
                //            ItemId = streamBackup.Id,
                //            Command = Command.Cancel
                //        });
                //        _repositoryStream.Delete(streamBackup);
                //        await _unitOfWork.SaveChangesAsync();
                //    }
                //}

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
                if (streamEntity.ManagerBOTId != null)
                {
                    foreach (var item in userBots)
                    {
                        if (item.BotId == streamEntity.ManagerBOTId && item.NumberOfThreadsInRun < item.NumberOfThreads)
                        {
                            botId = item.BotId;
                            botIdString = item.BotIdString;
                            break;
                        }
                    }
                    if (botId == 0)
                    {
                        return new KeyValuePair<bool, string>(false, "BOT không hoạt động hoặc chưa kết nối!");
                    }
                }
                else
                {
                    foreach (var item in userBots)
                    {
                        if (item.NumberOfThreadsInRun < item.NumberOfThreads)
                        {
                            botId = item.BotId;
                            botIdString = item.BotIdString;
                            break;
                        }
                    }
                    if (botId == 0)
                    {
                        return new KeyValuePair<bool, string>(false, "BOT không hoạt động hoặc chưa kết nối!");
                    }
                }

                streamEntity.Status = StreamStatus.Streaming;
                streamEntity.BotId = botIdString;
                streamEntity.ManagerBOTId = botId;
                streamEntity.DisconnectedTime = null;
                streamEntity.IsLiveBackup = false;
                streamEntity.RenderStatus = RenderStatus.Waitting;
                streamEntity.Updatedtime = DateTime.Now;
                //streamEntity.StreamBackupId = null;

                _repositoryStream.Update(streamEntity);
                await _unitOfWork.SaveChangesAsync();

                streamEntity = await _repositoryStream.Queryable()
                    .Include(x => x.ManagerBOT).Include(i => i.ManagerBOTBackup)
                    .Where(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync();

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

                List<IBotChatId> botChatIds = new List<IBotChatId>();
                botChatIds.Add(userCurrent);
                if (botChatIds.Any())
                {
                    var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                    var timer = streamEntity.StartTimeLive != null ?
                        streamEntity.StartTimeLive?.ToString("dd/MM/yyyy HH:mm:ss")
                        : "Ngay bây giờ";
                    var textTimer = timer == "Ngay bây giờ" ? "Chạy live" : "Hẹn lịch";
                    string message = $"User: {userCurrent.UserName}{Environment.NewLine}"
                       + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                       + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                       + $"Kiểu live: {liveType}{Environment.NewLine}"
                       + $"{textTimer}:{timer}";

                    _ = _botMessage.SendMessageAsync(botChatIds, message);
                }


                //check bot backup
                if (streamEntity.ManagerBOTBackupId.GetValueOrDefault() > 0
                    && streamEntity.EndTimeLive != null
                    && streamEntity.LiveType != LiveType._Backup)
                {
                    var backupId = streamEntity.StreamBackupId;
                    var streamBackupEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                        predicate: x => x.Id == backupId && x.Status == StreamStatus.Streaming);
                    if (streamBackupEntity == null)
                    {
                        //create backupStream

                        var botBackup = streamEntity.ManagerBOTBackup;
                        if (botBackup.Status != ManagerBOTStatus.Connected)
                        {
                            botBackup = await _repositoryBOT.GetFirstOrDefaultAsync(
                                predicate: x => x.DeletedTime == null
                                && x.LiveType == LiveType._Backup
                                && x.Status == ManagerBOTStatus.Connected,
                                orderBy: x => x.OrderByDescending(i => i.NumberOfThreads));

                        }
                        if (botBackup != null && botBackup.Status == ManagerBOTStatus.Connected)
                        {

                            var streamClone = new StreamHistory
                            {
                                IsLiveBackup = true,
                                RTMP = "rtmp://y.rtmp.youtube.com/live2?backup=1",
                                LiveType = LiveType._Backup,
                                Audio = streamEntity.Audio,
                                ManagerBOTId = botBackup.Id,
                                PlatformType = streamEntity.PlatformType,
                                RenderStatus = RenderStatus.Created,
                                StartTimeLive = streamEntity.StartTimeLive,
                                Status = StreamStatus.Streaming,
                                LiveOrder = streamEntity.LiveOrder,
                                IsLiveNow = true,
                                BotId = botBackup.BotId,
                                IsForever = streamEntity.IsForever,
                                TimeBackup = streamEntity.TimeBackup,
                                StreamKey = streamEntity.StreamKey,
                                EndTimeLive = streamEntity.EndTimeLive,
                                UserId = streamEntity.UserId,
                                StreamName = $"{streamEntity.StreamName}_backup11h",
                                CreatedTime = DateTime.Now,
                                Video = streamEntity.Video

                            };

                            _repositoryStream.Insert(streamClone);
                            await _unitOfWork.SaveChangesAsync();
                            //get streamClone
                            var streamCloneEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                                predicate: x => x.Id == streamClone.Id,
                                include: x => x.Include(i => i.ManagerBOT));

                            var resultSendBotBackup = await SendDataToBOTAsync(streamCloneEntity);

                            if (!resultSendBotBackup)
                            {
                                _logger.LogError($"Send data to BOT failed by error:[{resultSendBotBackup.Value}]");

                                streamCloneEntity.Status = StreamStatus.Stop;
                                _repositoryStream.Update(streamCloneEntity);
                                await _unitOfWork.SaveChangesAsync();

                            }
                            else
                            {
                                IBotChatId botChatId = userCurrent;
                                //send message telegram
                                if (botChatId.TelegramUserNames.Any() || botChatId.DiscordUserIds.Any())
                                {
                                    var liveType = streamCloneEntity.EndTimeLive != null ? "11h" : "24/7";
                                    var timer = streamCloneEntity.StartTimeLive != null ?
                                        streamCloneEntity.StartTimeLive?.ToString("dd/MM/yyyy HH:mm:ss")
                                        : "Ngay bây giờ";
                                    var textTimer = timer == "Ngay bây giờ" ? "Chạy live" : "Hẹn lịch";
                                    string textSend = $"User: {userCurrent.UserName}{Environment.NewLine}"
                                        + $"Tên luồng:{streamCloneEntity.StreamName}{Environment.NewLine}"
                                        + $"Live:{streamCloneEntity.LiveType} {Environment.NewLine}"
                                        + $"Kiểu live: {liveType}{Environment.NewLine}"
                                        + $"{textTimer}:{timer}";

                                    _ = _botMessage.SendMessageAsync(userCurrent, textSend);
                                }
                            }

                            streamEntity.DisconnectedTime = null;
                            streamEntity.IsLiveBackup = true;
                            streamEntity.StreamBackupId = streamCloneEntity.Id;
                            _logger.LogInformation($"streamCloneEntity.Id:[{streamCloneEntity.Id}]");
                            _repositoryStream.Update(streamEntity);
                            await _unitOfWork.SaveChangesAsync();

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
                streamEntity.RTMP = "rtmp://x.rtmp.youtube.com/live2";//model.RTMP;
                streamEntity.StartTimeLive = model.StartTimeLive;
                streamEntity.EndTimeLive = model.EndTimeLive;
                streamEntity.StreamKey = model.StreamKey;
                streamEntity.LiveOrder = model.LiveOrder;
                streamEntity.ManagerBOTId = model.ManagerBOTId > 0 ? model.ManagerBOTId : null;
                streamEntity.TimeBackup = model.TimeBackup;
                streamEntity.ManagerBOTBackupId = model.ManagerBotBackupId.GetValueOrDefault() > 0 ? model.ManagerBotBackupId : null;
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
                streamEntity.IsSendNotification = false;

                if (model.RenderStatus == RenderStatus.Error)
                {
                    _logger.LogError($"Error when render :[{model.ExceptionInfo}]");
                    streamEntity.Status = StreamStatus.Stop;
                    streamEntity.ErrorMessage = model.ExceptionInfo.Message;
                }
                else if (model.RenderStatus == RenderStatus.Ended)
                {
                    streamEntity.Status = StreamStatus.Stop;

                }
                else if (model.RenderStatus < RenderStatus.Ended)
                {
                    var timeDelay = 0;
                    int.TryParse(_configuration["TimeDelayBot"], out timeDelay);
                    if (timeDelay == 0)
                    {
                        timeDelay = 180;
                    }
                    if ((streamEntity.Updatedtime < DateTime.Now.AddSeconds(-timeDelay) && statusOld == RenderStatus.LiveStreaming)
                        || ((streamEntity.Status == StreamStatus.Disconnet || streamEntity.Status == StreamStatus.Stop)
                        && streamEntity.DisconnectedTime < DateTime.Now.AddSeconds(-timeDelay))
                        )
                    {
                        //send message telegram
                        List<IBotChatId> botChatIds = new List<IBotChatId>();
                        botChatIds.Add(streamEntity.AppUser);
                        if (botChatIds.Any())
                        {
                            var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                            var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            string textSend = $"User: {streamEntity.AppUser.UserName}{Environment.NewLine}"
                                 + $"Tên luồng:{streamEntity.StreamName}{Environment.NewLine}"
                                 + $"Live:{streamEntity.LiveType} {Environment.NewLine}"
                                 + $"Kiểu live: {liveType}{Environment.NewLine}"
                                 + $"Tiếp tục live:{timer}";
                            _ = _botMessage.SendMessageAsync(botChatIds, textSend);
                        }
                    }
                    streamEntity.Status = StreamStatus.Streaming;
                    streamEntity.DisconnectedTime = null;
                }
                // _logger.LogInformation($"Stream:[{streamEntity.Id}] - [Backup:{streamEntity.StreamBackupId}]");
                //remove live backup

                if (streamEntity.StreamBackupId.GetValueOrDefault() > 0 && model.RenderStatus == RenderStatus.LiveStreaming
                    && streamEntity.Status == StreamStatus.Streaming
                    && streamEntity.EndTimeLive == null)
                {
                    //_logger.LogInformation($"Close Backup:[{streamEntity.StreamBackupId}]");
                    var liveType = "Backup";
                    var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                    //stop live backup
                    var streamBackup = await _repositoryStream.GetFirstOrDefaultAsync(
                        predicate: x => x.Id == streamEntity.StreamBackupId.GetValueOrDefault());
                    if (streamBackup != null)
                    {

                        // call BOT cancel
                        _logger.LogInformation($"Send Cancel command to BOT: ItemId=[{streamBackup.Id}] (UpdateStreamAsync-closeBackup)");
                        await _botContextHub.Clients.All.RenderAsync(new RenderCommand
                        {
                            ItemId = streamBackup.Id,
                            Command = Command.Cancel
                        });
                        List<IBotChatId> botChatIds = new List<IBotChatId>();
                        botChatIds.Add(streamEntity.AppUser);
                        if (botChatIds.Count > 0)
                        {
                            string[] textSends =
                            [
                                $"User: {streamEntity.AppUser.UserName}",
                                $"Tên luồng:{streamBackup.StreamName}",
                                $"Live:{streamEntity.LiveType}",
                                $"Kiểu live: {liveType}",
                                $"Dừng:{timer}"
                            ];
                            _ = _botMessage.SendMessageAsync(botChatIds, string.Join(Environment.NewLine, textSends));
                        }

                        _repositoryStream.Delete(streamBackup);
                        await _unitOfWork.SaveChangesAsync();
                        //_logger.LogInformation($"End close Backup:[{streamEntity.StreamBackupId}]");
                        //streamEntity.StreamBackupId = null;
                        streamEntity.IsLiveBackup = false;
                    }
                }

                if (statusOld != RenderStatus.LiveStreaming && streamEntity.RenderStatus == RenderStatus.LiveStreaming)
                {
                    //send message telegram
                    List<IBotChatId> botChatIds = new List<IBotChatId>();
                    botChatIds.Add(streamEntity.AppUser);
                    if (botChatIds.Any())
                    {
                        var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                        var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        string[] textSends =
                        {
                             $"User: {streamEntity.AppUser.UserName}",
                             $"Tên luồng:{streamEntity.StreamName}",
                             $"Live:{streamEntity.LiveType}",
                             $"Kiểu live: {liveType}",
                             $"Bắt đầu:{timer}"
                        };
                        _ = _botMessage.SendMessageAsync(botChatIds, textSends);
                    }
                }
                else if (streamEntity.RenderStatus == RenderStatus.Error)
                {
                    List<IBotChatId> botChatIds = new List<IBotChatId>();
                    botChatIds.Add(streamEntity.AppUser);
                    if (botChatIds.Count > 0)
                    {
                        var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                        var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        string[] textSends =
                        {
                            $"User: {streamEntity.AppUser.UserName}",
                            $"Tên luồng:{streamEntity.StreamName}",
                            $"Live:{streamEntity.LiveType}",
                            $"Kiểu live: {liveType}",
                            $"Lỗi:{timer}"
                        };
                        _ = _botMessage.SendMessageAsync(botChatIds, textSends);
                    }

                }
                else if (streamEntity.RenderStatus == RenderStatus.Ended)
                {
                    streamEntity.Status = StreamStatus.Stop;


                    List<IBotChatId> botChatIds = new List<IBotChatId>();
                    botChatIds.Add(streamEntity.AppUser);
                    if (botChatIds.Any())
                    {
                        var liveType = streamEntity.EndTimeLive != null ? "11h" : "24/7";
                        var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        string[] textSends =
                        {
                            $"User: {streamEntity.AppUser.UserName}",
                            $"Tên luồng:{streamEntity.StreamName}",
                            $"Live:{streamEntity.LiveType}",
                            $"Kiểu live: {liveType}",
                            $"Dừng:{timer}"
                        };
                        _ = _botMessage.SendMessageAsync(botChatIds, textSends);
                    }
                    if (streamEntity.EndTimeLive != null && streamEntity.LiveType == LiveType._Backup)
                    {
                        _repositoryStream.Delete(streamEntity);
                        await _unitOfWork.SaveChangesAsync();
                        return new KeyValuePair<bool, string>(true, string.Empty);
                    }
                }

                streamEntity.Updatedtime = DateTime.Now;
                _repositoryStream.Update(streamEntity);
                await _unitOfWork.SaveChangesAsync();


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

                // call BOT cancel
                _logger.LogInformation($"Send Cancel command to BOT: ItemId=[{streamEntity.Id}] (CancelStreamAsync)");
                await _botContextHub.Clients.All.RenderAsync(new RenderCommand
                {
                    ItemId = streamEntity.Id,
                    Command = Command.Cancel
                });

                if (streamEntity.LiveType == LiveType._Backup)
                {
                    _repositoryStream.Delete(streamEntity);
                }
                else
                {
                    streamEntity.Status = StreamStatus.Stop;
                    if (streamEntity.StreamBackupId != null)
                    {
                        var streamBackup = await _repositoryStream.GetFirstOrDefaultAsync(
                            predicate: x => x.Id == streamEntity.StreamBackupId, disableTracking: false);
                        if (streamBackup != null)
                        {
                            // call BOT cancel
                            _logger.LogInformation($"Send Cancel command to BOT: ItemId=[{streamBackup.Id}] (CancelStreamAsync-backup)");
                            await _botContextHub.Clients.All.RenderAsync(new RenderCommand
                            {
                                ItemId = streamBackup.Id,
                                Command = Command.Cancel
                            });
                            _repositoryStream.Delete(streamBackup);
                        }

                    }
                    // streamEntity.StreamBackupId = null;
                    streamEntity.DisconnectedTime = null;
                    streamEntity.IsLiveBackup = false;

                    _repositoryStream.Update(streamEntity);
                }
                //

                await _unitOfWork.SaveChangesAsync();

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
                include: x => x.Include(i => i.AppUser).Include(i => i.ManagerBOT),
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

        public Task<KeyValuePair<bool, string>> ValidateLinkAsync(string link)
        {
            return _validateLinkService.ValidateLinkAsync(link);
        }

        public async Task<List<UserBotDto>> GetBotOfUserAsync(string userId)
        {
            var _repository = _unitOfWork.GetRepository<UserManagerBOT>();
            var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

            var data = await _repository.Queryable().AsNoTracking().Include(x => x.ManagerBOT)
                .Where(x => x.UserId == userId && x.ManagerBOT.Status == ManagerBOTStatus.Connected)
                .Select(x => new UserBotDto
                {
                    BotId = x.ManagerBOTId,
                    BotName = x.ManagerBOT.Name,
                    NumberOfThreads = x.NumberOfThreads,
                    NumberOfThreadsInRun = x.ManagerBOT.StreamHistorys.Where(i => i.Status == StreamStatus.Streaming && i.UserId == userId).Count(),
                    Type = x.ManagerBOT.LiveType,
                    Group = x.ManagerBOT.Group
                }).ToListAsync();

            if (data.Any())
            {
                var streamData = await _repositoryStream.Queryable().AsNoTracking()
                    .Where(x => x.UserId == userId && x.LiveType != LiveType._Backup
                    && x.ManagerBOTBackupId.GetValueOrDefault() > 0).Select(x => x.ManagerBOTBackupId).ToListAsync();
                foreach (var item in data)
                {
                    item.NumberOfThreadsBackup = streamData.Where(x => x.GetValueOrDefault() == item.BotId).Count();
                }
            }
            return data;
        }

        public async Task CheckLiveBackupAsync()
        {
            try
            {
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
                var _repositoryBot = _unitOfWork.GetRepository<ManagerBOT>();

                var liveDisconnects = await _repositoryStream.Queryable()
                    .Include(i => i.ManagerBOTBackup).Include(i => i.AppUser)
                    .Where(x => x.DisconnectedTime != null
                    && x.Status == StreamStatus.Disconnet
                    && x.ManagerBOTBackupId.GetValueOrDefault() > 0 && x.TimeBackup > 0 && x.IsLiveBackup == false
                    && x.EndTimeLive == null
                    && x.AppUser.DeletedTime == null).ToListAsync();
                if (liveDisconnects.Any())
                {
                    _logger.LogDebug($"Total Backup Pending:[{liveDisconnects.Count}]");
                    foreach (var item in liveDisconnects)
                    {
                        _logger.LogDebug($"Backup:[{item.Id}]");
                        var botId = 0;
                        var botIdString = string.Empty;
                        var timeCheckDisconnect = DateTime.Now.AddMinutes(-item.TimeBackup);

                        var botBackup = item.ManagerBOTBackup;
                        if (botBackup.Status != ManagerBOTStatus.Connected)
                        {
                            botBackup = await _repositoryBot.GetFirstOrDefaultAsync(
                                predicate: x => x.DeletedTime == null
                                && x.LiveType == LiveType._Backup
                                && x.Status == ManagerBOTStatus.Connected,
                                orderBy: x => x.OrderByDescending(i => i.NumberOfThreads));

                        }
                        if (timeCheckDisconnect > item.DisconnectedTime
                            && botBackup != null && botBackup.Status == ManagerBOTStatus.Connected)
                        {
                            ///send data to BOT
                            _logger.LogDebug($"Start Backup:[{item.Id}]");
                            var streamClone = new StreamHistory
                            {
                                IsLiveBackup = true,
                                RTMP = "rtmp://y.rtmp.youtube.com/live2?backup=1",
                                LiveType = LiveType._Backup,
                                Audio = item.Audio,
                                ManagerBOTId = botBackup.Id,
                                PlatformType = item.PlatformType,
                                RenderStatus = RenderStatus.Created,
                                StartTimeLive = DateTime.Now,
                                Status = StreamStatus.Streaming,
                                LiveOrder = item.LiveOrder,
                                IsLiveNow = true,
                                BotId = botBackup.BotId,
                                IsForever = item.IsForever,
                                TimeBackup = item.TimeBackup,
                                StreamKey = item.StreamKey,
                                EndTimeLive = item.EndTimeLive,
                                UserId = item.UserId,
                                StreamName = $"{item.StreamName}_backup247",
                                CreatedTime = DateTime.Now,
                                Video = item.Video

                            };

                            _repositoryStream.Insert(streamClone);
                            await _unitOfWork.SaveChangesAsync();
                            //get streamClone
                            var streamCloneEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                                predicate: x => x.Id == streamClone.Id,
                                include: x => x.Include(i => i.ManagerBOT));

                            var result = await SendDataToBOTAsync(streamCloneEntity);
                            if (!result)
                            {
                                _logger.LogError($"Send data to BOT failed by error:[{result.Value}]");

                                streamCloneEntity.Status = StreamStatus.Stop;
                                _repositoryStream.Update(streamCloneEntity);
                                await _unitOfWork.SaveChangesAsync();

                            }
                            else
                            {
                                List<IBotChatId> botChatIds = new List<IBotChatId>();
                                botChatIds.Add(item.AppUser);
                                if (botChatIds.Any())
                                {
                                    var liveType = streamCloneEntity.EndTimeLive != null ? "11h" : "24/7";
                                    var timer = streamCloneEntity.StartTimeLive != null ?
                                        streamCloneEntity.StartTimeLive?.ToString("dd/MM/yyyy HH:mm:ss")
                                        : "Ngay bây giờ";
                                    var textTimer = timer == "Ngay bây giờ" ? "Chạy live" : "Hẹn lịch";
                                    string textSend = $"User: {item.AppUser.UserName}{Environment.NewLine}"
                                       + $"Tên luồng:{streamCloneEntity.StreamName}{Environment.NewLine}"
                                       + $"Live:{streamCloneEntity.LiveType} {Environment.NewLine}"
                                       + $"Kiểu live: {liveType}{Environment.NewLine}"
                                       + $"{textTimer}:{timer}";
                                    _ = _botMessage.SendMessageAsync(botChatIds, textSend);
                                }
                            }
                            //item.DisconnectedTime = null;
                            item.IsLiveBackup = true;
                            item.StreamBackupId = streamCloneEntity.Id;
                            _repositoryStream.Update(item);
                            await _unitOfWork.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Check live backup failed by error:[{ex}]");
            }
        }


        public async Task<KeyValuePair<bool, string>> UpdateBotBackupAsync(string userId, StreamBackupUpdateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<StreamHistory>();

                var streamCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == model.Id && x.UserId == userId, disableTracking: false);
                if (streamCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Dữ liệu không tồn tại!");
                }
                streamCurrent.StreamName = model.StreamName;
                streamCurrent.ManagerBOTBackupId = model.ManagerBOTBackupId.GetValueOrDefault() > 0 ? model.ManagerBOTBackupId : null;
                streamCurrent.TimeBackup = model.TimeBackup;
                _repository.Update(streamCurrent);
                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update botBackup failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Cập nhật không thành công:[{ex.Message}]");
            }
        }

        public async Task CheckConnectionBotAsync()
        {
            try
            {
                _logger.LogInformation($"Begin check connection");
                var _repositoryBot = _unitOfWork.GetRepository<ManagerBOT>();
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
                var _repositorySetting = _unitOfWork.GetRepository<ConfigSystem>();
                var bots = await _repositoryBot.Queryable().Include(x => x.Manager).ToListAsync();

                var timeDelay = 0;
                int.TryParse(_configuration["TimeDelayBot"], out timeDelay);
                if (timeDelay == 0)
                {
                    timeDelay = 180;
                }
                var timeCurrent = DateTime.Now.AddSeconds(-timeDelay);
                var timeDisconnectCurrent = DateTime.Now.AddSeconds(-timeDelay);
                var setting = await _repositorySetting.GetFirstOrDefaultAsync();
                foreach (var bot in bots)
                {
                    if ((bot.UpdatedTime == null || timeCurrent > bot.UpdatedTime))
                    {
                        bot.Status = ManagerBOTStatus.Disconnected;
                        var streamsCurrents = _repositoryStream.Queryable()
                            .Where(x => x.ManagerBOTId == bot.Id && x.Status == StreamStatus.Streaming).ToList();
                        if (streamsCurrents != null && streamsCurrents.Any())
                        {
                            foreach (var item in streamsCurrents)
                            {
                                item.DisconnectedTime = DateTime.Now;
                            }
                            _repositoryStream.Update(streamsCurrents);
                        }
                        List<IBotChatId> botChatIds = new List<IBotChatId>();

                        if (timeDisconnectCurrent > bot.DisconnectedTime && !bot.IsSendNotification)
                        {
                            bot.IsSendNotification = true;
                            //bot.DisconnectedTime = DateTime.Now;
                            _repositoryBot.Update(bot);
                            if (setting is not null)
                                botChatIds.Add(setting);
                            if (bot.Manager is not null)
                            {
                                botChatIds.Add(bot.Manager.GetBotChatIdForStaff());
                            }
                        }

                        await _unitOfWork.SaveChangesAsync();

                        //send notification
                        if (botChatIds.Any())
                        {
                            var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            string textSend = $"BotName: {bot.Name}{Environment.NewLine}"
                                + $"Group:{bot.Group}{Environment.NewLine}"
                                + $"Manager:{bot.Manager?.UserName} {Environment.NewLine}"
                                + $"Live: {bot.LiveType}{Environment.NewLine}"
                                + $"Disconnected:{timer}";
                            _ = _botMessage.SendMessageAsync(botChatIds, textSend, BotChatService.SendType.Bot);
                        }
                    }
                }

                var streamDisconnect = await _repositoryStream.Queryable().Include(x => x.AppUser)
                    .Where(x => x.Status == StreamStatus.Disconnet
                    && x.DisconnectedTime < timeDisconnectCurrent
                    && x.DisconnectedTime != null
                    && !x.IsSendNotification).ToListAsync();
                _logger.LogInformation($"Total stream disconneted:[{streamDisconnect.Count}]");
                if (streamDisconnect.Any())
                {
                    foreach (var item in streamDisconnect)
                    {
                        item.IsSendNotification = true;

                        List<IBotChatId> botChatIds = new List<IBotChatId>();
                        botChatIds.Add(item.AppUser);
                        if (botChatIds.Any())
                        {
                            var liveType = item.EndTimeLive != null ? "11h" : "24/7";
                            var timer = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                            string textSend = $"User: {item.AppUser.UserName}{Environment.NewLine}"
                               + $"Tên luồng:{item.StreamName}{Environment.NewLine}"
                               + $"Live:{item.LiveType} {Environment.NewLine}"
                               + $"Kiểu live: {liveType}{Environment.NewLine}"
                               + $"Disconnected:{timer}";
                            _ = _botMessage.SendMessageAsync(botChatIds, textSend);
                        }
                    }
                    _repositoryStream.Update(streamDisconnect);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Check connected bot failed: {ex.Message}");
            }

        }
    }
}