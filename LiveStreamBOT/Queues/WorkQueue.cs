using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Queues.TaskQueues;
using FFmpegArgs.Executes;
using System.Threading;
using System.IO;
using BaseSouce.SharedSignalrData.Server;
using BaseSouce.SharedSignalrData.BotClient;
using LiveStreamBOT.Helpers;
using System.Data.Common;

namespace LiveStreamBOT.Queues
{
    internal class WorkQueue : IQueue
    {
        public RenderCommand RenderCommand { get; }
        readonly IServerHub _serverHub;
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        readonly string _workingDir;
        bool _isCancelledByEndTime = false;
        CancellationToken CancellationToken { get { return _cancellationTokenSource.Token; } }
        public WorkQueue(RenderCommand renderCommand, IServerHub serverHub)
        {
            this.RenderCommand = renderCommand ?? throw new ArgumentNullException(nameof(renderCommand));
            this._serverHub = serverHub ?? throw new ArgumentNullException(nameof(serverHub));

            _workingDir = Path.Combine(Singleton.BaseWorkingDir, $"{RenderCommand.ItemId}_{RenderCommand.WorkId}");
        }


        public bool IsPrioritize => false;

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        public async Task DoWork()
        {
            try
            {
                if (
                    RenderCommand.UtcStartTime.HasValue &&
                    RenderCommand.UtcEndTime.HasValue &&
                    RenderCommand.UtcEndTime.Value <= RenderCommand.UtcStartTime.Value
                    )
                {
                    await RenderUpdateAsync(new RenderMessage()
                    {
                        ItemId = RenderCommand.ItemId,
                        WorkId = RenderCommand.WorkId,
                        RenderStatus = RenderStatus.Ended,
                    });
                    return;
                }
                switch (RenderCommand.FileType)
                {
                    case FileType.File:
                        {
                            await DownloadFileAsync();
                            await PreRenderAsync();
                            await WaitTimeAsync();
                            await LiveStreamAsync();

                            await RenderUpdateAsync(new RenderMessage()
                            {
                                ItemId = RenderCommand.ItemId,
                                WorkId = RenderCommand.WorkId,
                                RenderStatus = RenderStatus.Ended,
                            });
                            break;
                        }

                    //case FileType.LiveStream:
                    //    {
                    //        using Stream stream = await GetLivestreamAsync();

                    //        break;
                    //    }
                    default:
                        throw new NotSupportedException(RenderCommand.FileType.ToString());
                }
            }
            catch (OperationCanceledException)
            {
                if (_isCancelledByEndTime)
                {
                    await RenderUpdateAsync(new RenderMessage()
                    {
                        ItemId = RenderCommand.ItemId,
                        WorkId = RenderCommand.WorkId,
                        RenderStatus = RenderStatus.Ended,
                    });
                }
                else
                {
                    await RenderUpdateAsync(new RenderMessage()
                    {
                        ItemId = RenderCommand.ItemId,
                        WorkId = RenderCommand.WorkId,
                        RenderStatus = RenderStatus.Cancelled,
                    });
                }
            }
            catch (Exception ex)
            {
                if (ex is AggregateException ae) await SendErrorAsync(ae.InnerException);
                else await SendErrorAsync(ex.InnerException);
            }
            finally
            {
                if (Directory.Exists(_workingDir))
                {
                    Directory.Delete(_workingDir, true);
                }
            }
        }


        async Task DownloadFileAsync()
        {
            double sizeDownloaded = 0;
            double totalSize = RenderCommand.FileDatas.Sum(x => x.Size);
            DateTime dateTime = DateTime.Now;

            using DownloadHelper downloadHelper = new DownloadHelper();
            foreach (var file in RenderCommand.FileDatas)
            {
                await downloadHelper.DownloadAsync(file, _workingDir, async (x) =>
                {
                    sizeDownloaded += x;

                    if (dateTime > DateTime.Now)
                    {
                        dateTime = DateTime.Now.AddSeconds(1);

                        await RenderUpdateAsync(new RenderMessage()
                        {
                            ItemId = RenderCommand.ItemId,
                            WorkId = RenderCommand.WorkId,
                            RenderStatus = RenderStatus.Downloading,
                            DownloadPercentage = sizeDownloaded,
                        });
                    }
                }, CancellationToken);
                CheckEndTime();
            }

            await RenderUpdateAsync(new RenderMessage()
            {
                ItemId = RenderCommand.ItemId,
                WorkId = RenderCommand.WorkId,
                RenderStatus = RenderStatus.Downloading,
                DownloadPercentage = sizeDownloaded,
            });
        }

        Task<Stream> GetLivestreamAsync()
        {
            throw new NotSupportedException();
        }

        async Task PreRenderAsync()
        {
            if (!string.IsNullOrWhiteSpace(RenderCommand.RenderArguments))
            {
                FFmpegRender preRender = FFmpegRender.FromArguments(RenderCommand.RenderArguments, new FFmpegRenderConfig()
                {
                    WorkingDirectory = _workingDir,
                });
                preRender.OnEncodingProgress += PreRender_OnEncodingProgress;
                var renderResult = await preRender.ExecuteAsync(CancellationToken);
                renderResult.EnsureSuccess();
            }
        }

        async void PreRender_OnEncodingProgress(RenderProgress obj)
        {
            if (obj is not null)
            {
                await RenderUpdateAsync(new RenderMessage()
                {
                    ItemId = RenderCommand.ItemId,
                    WorkId = RenderCommand.WorkId,
                    RenderStatus = RenderStatus.Rendering,
                    RenderInfo = new RenderInfo()
                    {
                        Bitrate = obj.Bitrate,
                        Fps = obj.Fps,
                        Size = obj.Size,
                        Time = obj.Time
                    }
                });
                CheckEndTime();
            }
        }

        async Task WaitTimeAsync()
        {
            if (RenderCommand.UtcStartTime.HasValue)
            {
                while (true)
                {
                    var utc_current = DateTime.UtcNow;
                    TimeSpan diff = RenderCommand.UtcStartTime.Value - utc_current;
                    if (diff > TimeSpan.Zero)
                    {
                        CheckEndTime();
                        await Task.Delay(5000, CancellationToken);
                        await RenderUpdateAsync(new RenderMessage()
                        {
                            ItemId = RenderCommand.ItemId,
                            WorkId = RenderCommand.WorkId,
                            RenderStatus = RenderStatus.Waitting,
                        });
                    }
                    else
                    {
                        break;
                    }
                }

            }
        }

        async Task LiveStreamAsync(Stream stream = null)
        {
            FFmpegRender liveStream = FFmpegRender.FromArguments(RenderCommand.LiveStreamArguments, new FFmpegRenderConfig()
            {
                WorkingDirectory = _workingDir,
            });
            liveStream.OnEncodingProgress += LiveStream_OnEncodingProgress;
            if (stream != null) liveStream.WithStdInStream(stream);
            await RenderUpdateAsync(new RenderMessage()
            {
                ItemId = RenderCommand.ItemId,
                WorkId = RenderCommand.WorkId,
                RenderStatus = RenderStatus.LiveStreaming,
                RenderInfo = new RenderInfo()
                {
                    Bitrate = 0,
                    Fps = 0,
                    Size = 0,
                    Time = TimeSpan.Zero
                }
            });
            await liveStream.ExecuteAsync(CancellationToken);
        }

        async void LiveStream_OnEncodingProgress(RenderProgress obj)
        {
            if (obj is not null)
            {
                await RenderUpdateAsync(new RenderMessage()
                {
                    ItemId = RenderCommand.ItemId,
                    WorkId = RenderCommand.WorkId,
                    RenderStatus = RenderStatus.LiveStreaming,
                    RenderInfo = new RenderInfo()
                    {
                        Bitrate = obj.Bitrate,
                        Fps = obj.Fps,
                        Size = obj.Size,
                        Time = obj.Time
                    }
                });
                CheckEndTime();
            }
        }




        void CheckEndTime()
        {
            if (
                RenderCommand?.UtcEndTime.HasValue == true &&
                DateTime.UtcNow >= RenderCommand.UtcEndTime.Value
                )
            {
                Console.WriteLine($"[{RenderCommand.ItemId} - {RenderCommand.WorkId}] UtcEndTime: {RenderCommand.UtcEndTime}");
                try { _cancellationTokenSource.Cancel(); _isCancelledByEndTime = true; } catch { }
            }
        }

        Task SendErrorAsync(Exception exception)
        {
            return RenderUpdateAsync(new RenderMessage()
            {
                ItemId = RenderCommand.ItemId,
                WorkId = RenderCommand.WorkId,
                RenderStatus = RenderStatus.Error,
                ExceptionInfo = new ExceptionInfo()
                {
                    Message = exception.Message,
                    Name = exception.GetType().FullName,
                    StackTrace = exception.StackTrace,
                }
            });
        }
        Task RenderUpdateAsync(RenderMessage renderMessage)
        {
            string log = $"[{renderMessage.ItemId} - {renderMessage.WorkId}] {renderMessage.RenderStatus}";
            switch (renderMessage?.RenderStatus)
            {
                case RenderStatus.LiveStreaming:
                    log += $" {renderMessage.RenderInfo?.Time}";
                    break;

                case RenderStatus.Downloading:
                    log += $" {renderMessage.DownloadPercentage}";
                    break;
            }
            Console.WriteLine(log);
            return _serverHub.RenderUpdateAsync(renderMessage);
        }
    }
}
