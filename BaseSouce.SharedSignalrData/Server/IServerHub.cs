using BaseSouce.SharedSignalrData.BotClient;

namespace BaseSouce.SharedSignalrData.Server
{
    public interface IServerHub
    {
        Task RenderUpdateAsync(RenderMessage renderMessage);
        Task PingAsync(ClientInfo clientInfo);
    }
}
