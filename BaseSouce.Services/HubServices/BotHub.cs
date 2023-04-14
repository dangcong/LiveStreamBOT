using BaseSouce.SharedSignalrData.BotClient;
using BaseSouce.SharedSignalrData.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace BaseSouce.Services.HubServices
{
    public class BotHub : Hub<IBotClientHub>, IServerHub
    {
        private readonly IBotDataCache _botDataCache;
        public BotHub(
             IBotDataCache botDataCache
            )
        {
            _botDataCache = botDataCache;
        }

        public Task PingAsync(ClientInfo clientInfo)
        {
            if (this.Context.GetHttpContext()?.Request?.Headers?.TryGetValue("BotId", out StringValues strings) == true)
            {
                string key = strings.ToString();
                if (!string.IsNullOrWhiteSpace(key))
                {
                    _botDataCache[key] = clientInfo;
                }
            }
            return Task.CompletedTask;
        }

        public Task RenderUpdateAsync(RenderMessage renderMessage)
        {
            return Task.CompletedTask;
        }
    }
}
