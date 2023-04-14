using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Services.HubServices;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.BotClient;
using BaseSouce.SharedSignalrData.Server;
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
            catch (Exception)
            {

                throw;
            }
        }
    }
}
