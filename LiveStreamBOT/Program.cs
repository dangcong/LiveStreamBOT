using BaseSouce.SharedSignalrData.Server;
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
using TqkLibrary.Queues.TaskQueues;
Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

TaskQueue<WorkQueue> taskQueue;
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
taskQueue = new TaskQueue<WorkQueue>();
taskQueue.MaxRun = 999;
try
{
    await using SignalrClient signalrClient = new SignalrClient(uri, Singleton.Setting.Setting.BotId);
    ServerHub = signalrClient.ServerHub;
    signalrClient.OnConnectionStateChange += SignalrClient_OnConnectionStateChange;
    signalrClient.OnRenderCommand += SignalrClient_OnRenderCommand;

    await signalrClient.StartAsync();

    Singleton.Setting.Save();
    using ISystemInfoReader systemInfoReader = GetSystemInfoReader();

    LogHelper.WriteLog($"Started {Singleton.Setting.Setting.BotId}");
    while (true)
    {
        await Task.Delay(Singleton.Setting.Setting.PingInterval);

        ClientInfo clientInfo = null;
        try
        {
            clientInfo = systemInfoReader?.GetClientInfo();
        }
        catch { }

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
    taskQueue.ShutDown();
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
                        taskQueue.Add(new WorkQueue(obj, ServerHub));
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