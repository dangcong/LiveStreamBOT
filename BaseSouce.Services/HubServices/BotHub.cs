using BaseSouce.Services.Services.BOT;
using BaseSouce.Services.Services.Stream;
using BaseSouce.SharedSignalrData.BotClient;
using BaseSouce.SharedSignalrData.Server;
using BaseSource.ViewModels.ManagerBOT;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace BaseSouce.Services.HubServices
{
    public class BotHub : Hub<IBotClientHub>, IServerHub
    {
        private readonly IBotDataCache _botDataCache;
        private readonly IManagerBOTService _managerBOTService;
        private readonly IStreamClientService _streamClientService;

        public BotHub(IBotDataCache botDataCache, IManagerBOTService managerBOTService,
            IStreamClientService streamClientService)
        {
            _botDataCache = botDataCache;
            _managerBOTService = managerBOTService;
            _streamClientService = streamClientService;

        }

        public async Task PingAsync(ClientInfo clientInfo)
        {
            if (this.Context.GetHttpContext()?.Request?.Headers?.TryGetValue("BotId", out StringValues strings) == true)
            {
                string key = strings.ToString();
                if (!string.IsNullOrWhiteSpace(key))
                {
                    _botDataCache[key] = clientInfo;
                }
                await _managerBOTService.CreateBOTAsync(new ManagerBotCreateDto
                {
                    BotId = key,
                    ConnectionId = Context.ConnectionId,
                    Bandwidth = clientInfo.Bandwidth,
                    CpuUsage = clientInfo.CpuUsage,
                    RamUsage = clientInfo.RamUsage
                });
            }
        }

        public async Task RenderUpdateAsync(RenderMessage renderMessage)
        {
            await _streamClientService.UpdateStreamAsync(renderMessage);
        }
    }
}
