namespace BaseSouce.Services.Services.Stream
{
    public interface IStreamThumbnailStorage
    {
        Task SaveAsync(int streamId, byte[] content, CancellationToken cancellationToken = default);
        Task<byte[]?> GetAsync(int streamId, CancellationToken cancellationToken = default);
        Task DeleteAsync(int streamId, CancellationToken cancellationToken = default);
    }
}
