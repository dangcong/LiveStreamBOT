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

TaskQueue<WorkQueue> taskQueue;
IServerHub ServerHub;
bool isDebug = "debug".Equals(args.Skip(1).FirstOrDefault(), StringComparison.OrdinalIgnoreCase);
if (Uri.TryCreate(args.FirstOrDefault(), UriKind.Absolute, out Uri uri))
{
    Console.WriteLine("Starting");
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

        Console.WriteLine("Started");
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
                if(isDebug)
                {
                    Console.WriteLine($"PingAsync: CpuUsage:{clientInfo.CpuUsage}, RamUsage:{clientInfo.RamUsage}, Bandwidth:{clientInfo.Bandwidth}KBps");
                }
                await ServerHub.PingAsync(clientInfo);
            }
        }
    }
    finally
    {
        taskQueue.ShutDown();
    }
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
    //if (string.IsNullOrWhiteSpace(obj.ItemId) || string.IsNullOrWhiteSpace(obj.WorkId)) return;
    switch (obj.Command)
    {
        case Command.Start:
            {
                if (!taskQueue.Runnings.Any(x => x.RenderCommand.WorkId == obj.WorkId))
                {
                    taskQueue.Add(new WorkQueue(obj, ServerHub));
                }
                break;
            }

        case Command.Cancel:
            {
                taskQueue.Cancel(x => x.RenderCommand.WorkId.Equals(obj.WorkId));
                break;
            }
    }
}

void SignalrClient_OnConnectionStateChange(SignalRConnectionState obj)
{
    Console.WriteLine($"{nameof(SignalRConnectionState)}.{obj}");
}