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

namespace BaseSouce.Services.Services.Stream
{


    public class StreamClientService : IStreamClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StreamClientService> _logger;
        private readonly IMapper _mapper;
        private readonly IHubContext<BotHub, IBotClientHub> _botContextHub;
        private readonly IUserService _userService;
        public StreamClientService(
            IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<StreamClientService> logger,
            IMapper mapper,
            IHubContext<BotHub, IBotClientHub> botContextHub,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _botContextHub = botContextHub;
            _userService = userService;

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
                    if (!string.IsNullOrWhiteSpace(model.Audio)) renderArguments.Add("-an");
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
                    if (model.IsForever) liveStreamArguments.Add($"-stream_loop -1");
                    renderArguments.Add("-f flv");
                    liveStreamArguments.Add("-i rendered");
                }

                {//output
                    liveStreamArguments.Add("-c copy");
                    liveStreamArguments.Add("-f flv");
                    liveStreamArguments.Add("-flvflags no_duration_filesize");
                    liveStreamArguments.Add($"\"{uri}\"");
                }


                List<string> arguments_oldBot = new List<string>();
                arguments_oldBot.Add("-y");

                {//video
                    arguments_oldBot.Add("-re");
                    if (model.IsForever) arguments_oldBot.Add($"-stream_loop -1");
                    if (!string.IsNullOrWhiteSpace(model.Audio)) arguments_oldBot.Add("-an");
                    arguments_oldBot.Add("-i video");
                }

                if (!string.IsNullOrWhiteSpace(model.Audio))
                {
                    arguments_oldBot.Add("-re");
                    if (model.IsForever) arguments_oldBot.Add($"-stream_loop -1");
                    arguments_oldBot.Add("-vn");
                    arguments_oldBot.Add("-i sound");
                }

                {//output
                    arguments_oldBot.Add("-c copy");
                    if (!model.IsForever) arguments_oldBot.Add("-shortest");
                    arguments_oldBot.Add("-f flv");
                    arguments_oldBot.Add("-flvflags no_duration_filesize");
                    arguments_oldBot.Add($"\"{uri}\"");
                }


                RenderCommand renderCommand = new RenderCommand()
                {
                    Arguments = string.Join("", arguments_oldBot),
                    RenderArguments = string.Join("", renderArguments),
                    LiveStreamArguments = string.Join(" ", liveStreamArguments),
                    Command = Command.Start,
                    UtcEndTime = model.EndTime?.ToUniversalTime(),
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
                    FileType = FileType.File,
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

                var profileUser = await _userService.GetProfileInfoAsync(userId);

                var packageCurrent = model.LiveType == LiveType._1080 ? profileUser.PackageLive1080 : profileUser.PackageLive4k;

                if (packageCurrent == null)
                {
                    return "Bạn chưa đăng ký gói cước để Livestream, vui lòng đăng ký để sử dụng!";
                }

                //else if (packageCurrent.NumberOfThreadsCreatedInRun >= packageCurrent.NumberOfThreadsCreated)
                //{
                //    return "Số lượng luồng của bạn đã vượt giới hạn của gói hiện tại, vui lòng thử lại!";
                //}

                //get bot 

                var _repositoryBOT = _unitOfWork.GetRepository<ManagerBOT>();

                var botCurrent = await _repositoryBOT.GetFirstOrDefaultAsync(
                    predicate: x => x.NumberOfThreads > 0 && x.Status == ManagerBOTStatus.Connected && x.IsUsed);

                if (botCurrent == null)
                {
                    return "Không có BOT, vui lòng thử lại";
                }

                var streamEntityNew = new StreamHistory
                {
                    Audio = model.Audio,
                    Video = model.Video,
                    StreamName = model.StreamName,
                    BotId = botCurrent.BotId,
                    ManagerBOTId = botCurrent.Id,
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
                include: x => x.Include(i => i.AppUser),
               orderBy: x => x.OrderByDescending(i => i.CreatedTime),
               pageIndex: model.Page, pageSize: model.PageSize
                );
        }

        public async Task<KeyValuePair<bool, string>> StartStreamAsync(string userId, int id)
        {
            try
            {
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var _repositoryBOT = _unitOfWork.GetRepository<ManagerBOT>();

                var streamEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                   predicate: x => x.UserId == userId && x.Id == id,
                   include: x => x.Include(i => i.ManagerBOT),
                   disableTracking: false);

                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }


                //check connection bot

                var botCurrent = await _repositoryBOT.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == streamEntity.ManagerBOTId);

                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không tồn tại, vui lòng thử lại!");
                }

                if (botCurrent.Status == ManagerBOTStatus.Disconnected)
                {
                    return new KeyValuePair<bool, string>(false, "BOT đang Disconnect, vui lòng thử lại!");
                }

                streamEntity.Status = StreamStatus.Streaming;

                _repositoryStream.Update(streamEntity);
                await _unitOfWork.SaveChangesAsync();


                ///send data to BOT
                var result = await SendDataToBOTAsync(streamEntity);
                if (!result)
                {
                    return new KeyValuePair<bool, string>(false, result.Value);
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
                    disableTracking: false);

                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }

                //
                streamEntity.RenderStatus = model.RenderStatus;
                if (model.RenderStatus == RenderStatus.Error)
                {
                    _logger.LogError($"Error when render :[{model.ExceptionInfo}]");

                    streamEntity.ErrorMessage = model.ExceptionInfo.Message;
                }
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
                selector: x => new StreamReportDto
                {
                    Total = x.StreamHistorys.Count(),
                    TotalStop = x.StreamHistorys.Where(i => i.Status == StreamStatus.Stop).Count(),
                    TotalStreaning = x.StreamHistorys.Where(i => i.Status == StreamStatus.Streaming).Count()
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
