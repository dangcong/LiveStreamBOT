using BaseSouce.SharedSignalrData.Server;
using LiveStreamBOT;
using LiveStreamBOT.Enums;
using LiveStreamBOT.Helpers;
using LiveStreamBOT.Queues;
using LiveStreamBOT.Services;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TqkLibrary.Queues.TaskQueues;

TaskQueue<WorkQueue> taskQueue;
IServerHub ServerHub;
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
        Console.WriteLine("Started");
        while (true)
        {
            await Task.Delay(Singleton.Setting.Setting.PingInterval);

            ClientInfo clientInfo = null;
            try
            {
                clientInfo = ClientInfoHelper.GetClientInfo();
            }
            catch { }

            if (clientInfo is not null) await ServerHub.PingAsync(clientInfo);
        }
    }
    finally
    {
        taskQueue.ShutDown();
    }
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