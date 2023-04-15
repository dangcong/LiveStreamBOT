using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Services.HubServices;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.BotClient;
using BaseSouce.SharedSignalrData.Server;
using BaseSource.Shared.Enums;
using BaseSource.Shared.Helpers;
using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork;
using FFmpegArgs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Net.CloudStorage.GoogleDrive;

namespace BaseSouce.Services.Services.Stream
{
    public class StreamClientService : IStreamClientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StreamClientService> _logger;
        private readonly IMapper _mapper;
        private readonly IRenderArgumentGenerateHelper _argumentGenerateHelper;
        private readonly IHubContext<BotHub, IBotClientHub> _botContextHub;
        public StreamClientService(
            IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<StreamClientService> logger,
            IMapper mapper,
            IRenderArgumentGenerateHelper argumentGenerateHelper,
            IHubContext<BotHub, IBotClientHub> botContextHub)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _argumentGenerateHelper = argumentGenerateHelper;
            _botContextHub = botContextHub;
        }

        public async Task<KeyValuePair<bool, string>> CreateAsync(string userId, StreamCreateDto model)
        {
            try
            {
                var driveVideoItem = GoogleDriveHelper.TryParse(model.Video);
                if (driveVideoItem is null ||
                    driveVideoItem.LinkType != GoogleDriveLinkType.File)
                    return new KeyValuePair<bool, string>(false, "Link video không hỗ trợ hoặc không hợp lệ");

                var driveAudioItem = GoogleDriveHelper.TryParse(model.Audio);
                if (!string.IsNullOrWhiteSpace(model.Audio) &&
                    (driveAudioItem is null || driveAudioItem.LinkType != GoogleDriveLinkType.File))
                    return new KeyValuePair<bool, string>(false, "Link audio không hỗ trợ hoặc không hợp lệ");


                using (DriveApiNonLogin driveApiNonLogin = new DriveApiNonLogin())
                {
                    try
                    {
                        var videoMetadata = await driveApiNonLogin.GetMetadataAsync(driveVideoItem.ID);
                    }
                    catch (Exception)
                    {
                        return new KeyValuePair<bool, string>(false, "Link video không truy cập được");
                    }

                    if (driveAudioItem is not null)
                    {
                        try
                        {
                            var videoMetadata = await driveApiNonLogin.GetMetadataAsync(driveAudioItem.ID);
                        }
                        catch (Exception)
                        {
                            return new KeyValuePair<bool, string>(false, "Link audio không truy cập được");
                        }
                    }
                }


                Uri uri = model.PlatformType switch
                {
                    PlatformType.Youtube => new Uri($"rtmp://a.rtmp.youtube.com/live2/{model.StreamKey}"),
                    _ => throw new NotSupportedException($"{model.PlatformType.GetType().Name}.{model.PlatformType}")
                };

                PackageConfig packageConfig = new PackageConfig();
                packageConfig.GlobalOptions.Add("-y");
                if (model.IsForever)
                {
                    packageConfig.VideoInputOptions.Add($"-stream_loop -1");
                    packageConfig.AudioInputOptions.Add($"-stream_loop -1");
                }
                switch (model.LiveType)
                {
                    case LiveType._4K:
                        {
                            packageConfig.OutputOptions.Add("-s 4096x2160");
                            packageConfig.OutputOptions.Add("-r 24");
                            packageConfig.OutputOptions.Add("-maxrate 35000k");
                            packageConfig.OutputOptions.Add($"-x264-params keyint={24 * 4}:scenecut=0");
                            break;
                        }
                    case LiveType._1080:
                        {
                            packageConfig.OutputOptions.Add("-s 1920x1080");
                            packageConfig.OutputOptions.Add("-r 24");
                            packageConfig.OutputOptions.Add("-maxrate 8000k");
                            packageConfig.OutputOptions.Add($"-x264-params keyint={24 * 4}:scenecut=0");
                            break;
                        }

                    default:
                        throw new NotSupportedException($"{model.LiveType.GetType().Name}.{model.LiveType}");
                }

                RenderData renderData = new RenderData()
                {
                    VideoName = "video.mp4"
                };
                if (!string.IsNullOrWhiteSpace(model.Audio)) renderData.AudioName = "sound.mp3";
                renderData.StreamOutput = uri;


                FFmpegArg fFmpegArg = _argumentGenerateHelper.Generate(renderData, packageConfig);
                RenderCommand renderCommand = new RenderCommand()
                {
                    Arguments = fFmpegArg.GetFullCommandline(),
                    Command = Command.Start,
                    WorkId = 0,//quan trọng, cần để cancel/update/tạo thư mục
                    ItemId = 0,//quan trọng, cần để cancel/update/tạo thư mục
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


                if (model.IsLiveNow)
                {
                    await _botContextHub.Clients.All.RenderAsync(renderCommand);
                    return new KeyValuePair<bool, string>(true, string.Empty);
                }
                else
                {
                    //hẹn giờ


                    return new KeyValuePair<bool, string>(true, string.Empty);
                }
            }
            catch (NotSupportedException nse)
            {
                return new KeyValuePair<bool, string>(false, $"{nse.Message} chưa hỗ trợ");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
