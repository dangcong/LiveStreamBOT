using BaseSouce.SharedSignalrData.Classes;

namespace BaseSouce.SharedSignalrData.Interfaces
{
    public interface IServerHub
    {
        Task RenderUpdateAsync(RenderMessage renderMessage);
        Task PingAsync(ClientInfo clientInfo);
    }
}
