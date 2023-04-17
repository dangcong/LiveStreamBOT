using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Services.HubServices;
using BaseSouce.Services.Services.User;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.BotClient;
using BaseSouce.SharedSignalrData.Server;
using BaseSource.Data.Entities;
using BaseSource.Shared.Helpers;
using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using FFmpegArgs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseSouce.Services.Services.Stream
{
    public class StreamClientService : IStreamClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StreamClientService> _logger;
        private readonly IMapper _mapper;
        private readonly IRenderArgumentGenerateHelper _argumentGenerateHelper;
        private readonly IHubContext<BotHub, IBotClientHub> _botContextHub;
        private readonly IUserService _userService;
        public StreamClientService(
            IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<StreamClientService> logger,
            IMapper mapper,
            IRenderArgumentGenerateHelper argumentGenerateHelper,
            IHubContext<BotHub, IBotClientHub> botContextHub,
            IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _argumentGenerateHelper = argumentGenerateHelper;
            _botContextHub = botContextHub;
            _userService = userService;

        }

        public async Task<KeyValuePair<bool, string>> CreateAsync(string userId, StreamCreateDto model)
        {
            try
            {

                //add database
                var _repositoryUser = _unitOfWork.GetRepository<AppUser>();
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                var profileUser = await _userService.GetProfileInfoAsync(userId);

                var packageCurrent = model.LiveType == LiveType._1080 ? profileUser.PackageLive1080 : profileUser.PackageLive4k;

                if (packageCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Bạn chưa đăng ký gói cước để Livestream, vui lòng đăng ký để sử dụng!");
                }

                else if (packageCurrent.NumberOfThreadsCreatedInRun >= packageCurrent.NumberOfThreadsCreated)
                {
                    return new KeyValuePair<bool, string>(false, "Số lượng luồng của bạn đã vượt giới hạn của gói hiện tại, vui lòng thử lại!");
                }
                var streamEntityNew = new StreamHistory
                {
                    Audio = model.Audio,
                    Video = model.Video,
                    StreamName = model.StreamName,
                    BotId = string.Empty,
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

                //send data to BOT

                RenderData renderData = new RenderData()
                {
                    VideoName = "video.mp4"
                };
                if (!string.IsNullOrWhiteSpace(model.Audio)) renderData.AudioName = "sound.mp3";

                Uri uri = model.PlatformType switch
                {
                    PlatformType.Youtube => new Uri($"rtmp://a.rtmp.youtube.com/live2/{model.StreamKey}"),
                    _ => throw new NotSupportedException(model.PlatformType.ToString())
                };
                renderData.StreamOutput = uri;


                PackageConfig packageConfig = model.LiveType switch
                {
                    LiveType._1080 => new PackageConfig()
                    {
                        GlobalOption = "-y",
                        OutputOption = "-s 1080x720",
                    },
                    LiveType._4K => new PackageConfig()
                    {
                        GlobalOption = "-y",
                        OutputOption = "-s 4096×2160",
                    },
                    _ => throw new NotSupportedException(model.LiveType.ToString())
                };

                FFmpegArg fFmpegArg = _argumentGenerateHelper.Generate(renderData, packageConfig);
                RenderCommand renderCommand = new RenderCommand()
                {
                    Arguments = fFmpegArg.GetFullCommandline(),
                    Command = Command.Start,
                    WorkId = 0,
                    ItemId = 0,
                    FileDatas = new List<FileData>()
                    {
                        new FileData()
                        {
                            Name = renderData.VideoName,
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
                        Name = renderData.AudioName,
                        Url = model.Audio,
                        UrlType = UrlType.GoogleDrive
                    });
                }

                await _botContextHub.Clients.All.RenderAsync(renderCommand);

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {

                throw;
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

                var streamEntity = await _repositoryStream.GetFirstOrDefaultAsync(
                   predicate: x => x.UserId == userId && x.Id == id,
                   disableTracking: false);

                if (streamEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Luồng không tồn tại, vui lòng thử lại!");
                }
                streamEntity.Status = StreamStatus.Streaming;

                _repositoryStream.Update(streamEntity);
                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
                //send to BOT
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
    }
}
