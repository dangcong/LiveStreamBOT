using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Queues.TaskQueues;
using FFmpegArgs.Executes;
using System.Threading;
using System.IO;
using BaseSouce.SharedSignalrData.Enums;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Interfaces;
using LiveStreamBOT.Helpers;
using System.Data.Common;
using BaseSource.Shared.Helpers;
using BaseSource.Shared.Enums;
using TqkLibrary.Net.CloudStorage.GoogleDrive;
using FFmpegArgs.Executes.Exceptions;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace LiveStreamBOT.Queues
{
    internal class WorkQueue : IQueue
    {
        public RenderCommand RenderCommand { get; }
        readonly IServerHub _serverHub;
        readonly CancellationTokenSource _cancellationTokenSource_cancel = new CancellationTokenSource();
        readonly CancellationTokenSource _cancellationTokenSource_endTime = new CancellationTokenSource();
        readonly string _workingDir;
        CancellationToken CancellationToken { get { return _cancellationTokenSource_cancel.Token; } }
        public WorkQueue(RenderCommand renderCommand, IServerHub serverHub)
        {
            this.RenderCommand = renderCommand ?? throw new ArgumentNullException(nameof(renderCommand));
            this._serverHub = serverHub ?? throw new ArgumentNullException(nameof(serverHub));

            _workingDir = Path.Combine(Singleton.BaseWorkingDir, $"{RenderCommand.ItemId}_{RenderCommand.WorkId}");
        }


        public bool IsPrioritize => false;

        public void Cancel()
        {
            _cancellationTokenSource_cancel.Cancel();
        }

        public void Dispose()
        {
            _cancellationTokenSource_cancel.Dispose();
            _cancellationTokenSource_endTime.Dispose();
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
                            DeleteDownloaded();
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
                if (_cancellationTokenSource_endTime.IsCancellationRequested)
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
                if (ex is AggregateException ae) ex = ae.InnerException ?? ex;
                await SendErrorAsync(ex);
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
            while (true)
            {
                try
                {
                    using DownloadHelper downloadHelper = new DownloadHelper();

                    foreach (var fileData in RenderCommand.FileDatas.Where(x => !x.Size.HasValue || x.Size == 0))
                    {
                        fileData.Size = await downloadHelper.GetFileSizeAsync(fileData, CancellationToken);
                    }

                    double totalSize = RenderCommand.FileDatas.Where(x => x.Size.HasValue).Sum(x => x.Size.Value);
                    DateTime dateTime = DateTime.Now;
                    double sizeDownloaded = 0;

                    foreach (var file in RenderCommand.FileDatas)
                    {
                        await downloadHelper.DownloadAsync(file, _workingDir, (x) =>
                        {
                            sizeDownloaded += x;

                            if (dateTime > DateTime.Now)
                            {
                                dateTime = DateTime.Now.AddSeconds(1);

                                _ = RenderUpdateAsync(new RenderMessage()
                                {
                                    ItemId = RenderCommand.ItemId,
                                    WorkId = RenderCommand.WorkId,
                                    RenderStatus = RenderStatus.Downloading,
                                    DownloadPercentage = sizeDownloaded * 100.0 / totalSize,
                                });
                            }
                        }, CancellationToken);
                        CheckEndTime();
                        _cancellationTokenSource_endTime.Token.ThrowIfCancellationRequested();
                    }

                    await RenderUpdateAsync(new RenderMessage()
                    {
                        ItemId = RenderCommand.ItemId,
                        WorkId = RenderCommand.WorkId,
                        RenderStatus = RenderStatus.Downloading,
                        DownloadPercentage = sizeDownloaded * 100.0 / totalSize,
                    });
                    break;
                }
                catch (HttpRequestException hre)
                {
                    if (!string.IsNullOrWhiteSpace(hre.Message))
                    {
                        Match match = Regex.Match(hre.Message, "\\d{3}");
                        if (match.Success)
                        {
                            int code = int.Parse(match.Value);
                            if (code >= 500)
                            {
                                DeleteDownloaded();
                                await RenderUpdateAsync(new RenderMessage()
                                {
                                    ItemId = RenderCommand.ItemId,
                                    WorkId = RenderCommand.WorkId,
                                    RenderStatus = RenderStatus.Waitting,
                                    DownloadPercentage = 0,
                                });
                                await Task.Delay(Singleton.Setting.Setting.ScheduleTimeOnDownloadFailed, CancellationToken);
                                continue;
                            }
                        }
                    }
                    throw;
                }
            }
        }

        void DeleteDownloaded()
        {
            foreach (var file in RenderCommand.FileDatas)
            {
                string path = Path.Combine(_workingDir, file.Name);
                if (File.Exists(path))
                    File.Delete(path);
            }
        }

        Task<Stream> GetLivestreamAsync()
        {
            throw new NotSupportedException();
        }

        async Task PreRenderAsync()
        {
            if (!string.IsNullOrWhiteSpace(RenderCommand.RenderArguments))
            {
                LogHelper.WriteLog($"{nameof(PreRenderAsync)}: {RenderCommand.RenderArguments}");
                FFmpegRender preRender = FFmpegRender.FromArguments(RenderCommand.RenderArguments, new FFmpegRenderConfig()
                {
                    WorkingDirectory = _workingDir,
                });
                preRender.OnEncodingProgress += PreRender_OnEncodingProgress;

                using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                using var register_cancel = _cancellationTokenSource_cancel.Token.Register(() => cancellationTokenSource.Cancel());
                using var register_endTime = _cancellationTokenSource_endTime.Token.Register(() => cancellationTokenSource.Cancel());

                var renderResult = await preRender.ExecuteAsync(cancellationTokenSource.Token);
                renderResult.EnsureSuccess();
            }
        }

        void PreRender_OnEncodingProgress(RenderProgress obj)
        {
            if (obj is not null)
            {
                _ = RenderUpdateAsync(new RenderMessage()
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
                        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                        using var register_cancel = _cancellationTokenSource_cancel.Token.Register(() => cancellationTokenSource.Cancel());
                        using var register_endTime = _cancellationTokenSource_endTime.Token.Register(() => cancellationTokenSource.Cancel());

                        CheckEndTime();

                        await Task.Delay(5000, cancellationTokenSource.Token);
                        _ = RenderUpdateAsync(new RenderMessage()
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
            while (!(_cancellationTokenSource_cancel.IsCancellationRequested || _cancellationTokenSource_endTime.IsCancellationRequested))
            {
                LogHelper.WriteLog($"{nameof(LiveStreamAsync)}: {RenderCommand.LiveStreamArguments}");
                FFmpegRender liveStream = FFmpegRender.FromArguments(RenderCommand.LiveStreamArguments, new FFmpegRenderConfig()
                {
                    WorkingDirectory = _workingDir,
                });
                liveStream.OnEncodingProgress += LiveStream_OnEncodingProgress;
                if (stream != null) liveStream.WithStdInStream(stream);
                _ = RenderUpdateAsync(new RenderMessage()
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

                using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                using var register_cancel = _cancellationTokenSource_cancel.Token.Register(() => cancellationTokenSource.Cancel());
                using var register_endTime = _cancellationTokenSource_endTime.Token.Register(() => cancellationTokenSource.Cancel());

                var result = await liveStream.ExecuteAsync(cancellationTokenSource.Token);
                LogHelper.WriteLog($"[{RenderCommand.ItemId} - {RenderCommand.WorkId}] ffmpeg exit code {result.ExitCode}\r\n\r\b{string.Join("\r\n", result.ErrorDatas)}");
            }
        }

        void LiveStream_OnEncodingProgress(RenderProgress obj)
        {
            if (obj is not null)
            {
                _ = RenderUpdateAsync(new RenderMessage()
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
                LogHelper.WriteLog($"[{RenderCommand.ItemId} - {RenderCommand.WorkId}] UtcEndTime: {RenderCommand.UtcEndTime}");
                try { _cancellationTokenSource_endTime.Cancel(); } catch { }
            }
        }

        Task SendErrorAsync(Exception exception)
        {
            FFmpegRenderException fFmpegRenderException = exception as FFmpegRenderException;
            bool isFFmpegRenderException = fFmpegRenderException is not null;
            return RenderUpdateAsync(new RenderMessage()
            {
                ItemId = RenderCommand.ItemId,
                WorkId = RenderCommand.WorkId,
                RenderStatus = RenderStatus.Error,
                ExceptionInfo = new ExceptionInfo()
                {
                    Message = isFFmpegRenderException ? string.Join("\r\n", fFmpegRenderException.OutputDatas) : exception.Message,
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
                    log += $" {renderMessage.DownloadPercentage}%";
                    break;

                case RenderStatus.Error:
                    log += $" {renderMessage.ExceptionInfo?.Message}\r\n{renderMessage.ExceptionInfo?.StackTrace}";
                    break;
            }
            LogHelper.WriteLog(log);
            return _serverHub.RenderUpdateAsync(renderMessage);
        }
    }
}
