using BaseSouce.SharedSignalrData.Classes;

namespace BaseSouce.SharedSignalrData.Interfaces
{
    public interface IServerHub
    {
        Task RenderUpdateAsync(RenderMessage renderMessage);
        Task UploadThumbnailAsync(long itemId, byte[] thumbnail);
        Task PingAsync(ClientInfo clientInfo);
    }
}
