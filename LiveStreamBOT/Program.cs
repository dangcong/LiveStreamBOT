using BaseSouce.SharedSignalrData.Enums;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Interfaces;
using LiveStreamBOT;
using LiveStreamBOT.Enums;
using LiveStreamBOT.Helpers;
using LiveStreamBOT.Interfaces;
using LiveStreamBOT.Queues;
using LiveStreamBOT.Services;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Concurrency.TaskQueues;
Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

WorkQueue<LiveStreamWork> taskQueue;
IServerHub ServerHub;
Uri uri = null;
bool isDebug = "debug".Equals(args.Skip(1).FirstOrDefault(), StringComparison.OrdinalIgnoreCase);

Uri.TryCreate(args.FirstOrDefault(), UriKind.Absolute, out uri);
while (uri is null)
{
    Console.Write("Nhập Url Api (https://api.shinemusicllc.com/):");
    string url = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(url))
        url = "https://api.shinemusicllc.com/";
    Uri.TryCreate(args.FirstOrDefault(), UriKind.Absolute, out uri);
}

LogHelper.WriteLog("Starting");
taskQueue = new WorkQueue<LiveStreamWork>();
taskQueue.MaxRun = 999;
AutoUpdateService autoUpdateService = null;
try
{
    await using SignalrClient signalrClient = new SignalrClient(uri, Singleton.Setting.Setting.BotId);
    ServerHub = signalrClient.ServerHub;
    signalrClient.OnConnectionStateChange += SignalrClient_OnConnectionStateChange;
    signalrClient.OnRenderCommand += SignalrClient_OnRenderCommand;
    signalrClient.OnUpdateSettingCommand += SignalrClient_OnUpdateSettingCommand;
    await signalrClient.StartAsync();

    Singleton.Setting.Save();

    // Tự động kiểm tra & áp dụng cập nhật (chỉ khi bot rảnh), rồi restart bằng cùng command-line (URL API).
    autoUpdateService = new AutoUpdateService(new BotUpdateApiClient(uri), taskQueue);
    autoUpdateService.Start();
    using ISystemInfoReader systemInfoReader = GetSystemInfoReader();

    LogHelper.WriteLog($"Started {Singleton.Setting.Setting.BotId}");
    while (true)
    {
        await Task.Delay(Singleton.Setting.Setting.PingInterval);

        try
        {
            ClearLog();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{nameof(ClearLog)}: {ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
        }


        ClientInfo clientInfo = null;
        try
        {
            clientInfo = systemInfoReader?.GetClientInfo();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{nameof(ClearLog)}: {ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
        }

        if (clientInfo is not null)
        {
            if (isDebug)
            {
                LogHelper.WriteLog($"PingAsync: CpuUsage:{clientInfo.CpuUsage}, RamUsage:{clientInfo.RamUsage}, Bandwidth:{clientInfo.Bandwidth}KBps");
            }
            await ServerHub.PingAsync(clientInfo);
        }
    }
}
finally
{
    autoUpdateService?.Stop();
    taskQueue.ShutDown();
}

void SignalrClient_OnUpdateSettingCommand(SettingInfo obj)
{
    Singleton.Setting.Setting.ScheduleTimeOnDownloadFailed = obj.ScheduleTimeOnDownloadFailed;
    Singleton.Setting.Setting.PingInterval = obj.PingInterval;
    if(!string.IsNullOrWhiteSpace(obj.NetworkName)) Singleton.Setting.Setting.NetworkName = obj.NetworkName;
    Singleton.Setting.Save();
}

ISystemInfoReader GetSystemInfoReader()
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    {
        return new LinuxSystemInfoReader();
    }
    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        return new WindowSystemInfoReader();
    }
    return null;
}


void SignalrClient_OnRenderCommand(RenderCommand obj)
{
    try
    {
        LogHelper.WriteLog($"{nameof(RenderCommand)}: [{obj.ItemId} - {obj.WorkId}] {obj?.Command}");
        switch (obj.Command)
        {
            case Command.Start:
                {
                    if (!taskQueue.Runnings.Any(x => x.RenderCommand.ItemId == obj.ItemId))
                    {
                        taskQueue.Add(new LiveStreamWork(obj, ServerHub));
                    }
                    break;
                }

            case Command.Cancel:
                {
                    taskQueue.Cancel(x => x.RenderCommand.ItemId.Equals(obj.ItemId));
                    break;
                }
        }
    }
    catch (Exception ex)
    {
        LogHelper.WriteLog($"{ex.GetType().FullName}: {ex.Message}, {ex.StackTrace}");
    }
}

void SignalrClient_OnConnectionStateChange(SignalRConnectionState obj)
{
    LogHelper.WriteLog($"{nameof(SignalRConnectionState)}.{obj}");
}


void ClearLog()
{
    var date = DateTime.Now.AddDays(-7);
    var fileInfos = Directory.GetFiles(LogHelper.LogsDir).Select(x => new FileInfo(x)).ToArray();
    foreach (var fileInfo in fileInfos)
    {
        if (fileInfo.CreationTime < date)
        {
            try
            {
                fileInfo.Delete();
            }
            catch
            {

            }
        }
    }
}