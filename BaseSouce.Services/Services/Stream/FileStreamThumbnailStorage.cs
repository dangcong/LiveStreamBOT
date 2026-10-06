using Microsoft.Extensions.Configuration;

namespace BaseSouce.Services.Services.Stream
{
    public sealed class FileStreamThumbnailStorage : IStreamThumbnailStorage
    {
        private readonly string _storageDirectory;

        public FileStreamThumbnailStorage(IConfiguration configuration)
        {
            var configuredPath = configuration["StreamThumbnailStoragePath"];
            var dataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrWhiteSpace(dataDirectory))
                dataDirectory = AppContext.BaseDirectory;

            _storageDirectory = Path.GetFullPath(string.IsNullOrWhiteSpace(configuredPath)
                ? Path.Combine(dataDirectory, "LiveStreamBOT", "stream-thumbnails")
                : configuredPath);
        }

        public async Task SaveAsync(int streamId, byte[] content, CancellationToken cancellationToken = default)
        {
            if (streamId <= 0) throw new ArgumentOutOfRangeException(nameof(streamId));
            if (content is null || content.Length == 0) throw new ArgumentException("Thumbnail is empty.", nameof(content));

            Directory.CreateDirectory(_storageDirectory);
            var destinationPath = GetPath(streamId);
            var temporaryPath = $"{destinationPath}.{Guid.NewGuid():N}.tmp";
            try
            {
                await File.WriteAllBytesAsync(temporaryPath, content, cancellationToken);
                File.Move(temporaryPath, destinationPath, overwrite: true);
            }
            finally
            {
                if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
            }
        }

        public async Task<byte[]?> GetAsync(int streamId, CancellationToken cancellationToken = default)
        {
            var path = GetPath(streamId);
            return File.Exists(path) ? await File.ReadAllBytesAsync(path, cancellationToken) : null;
        }

        public Task DeleteAsync(int streamId, CancellationToken cancellationToken = default)
        {
            var path = GetPath(streamId);
            if (File.Exists(path)) File.Delete(path);
            return Task.CompletedTask;
        }

        private string GetPath(int streamId)
        {
            if (streamId <= 0) throw new ArgumentOutOfRangeException(nameof(streamId));
            return Path.Combine(_storageDirectory, $"{streamId}.jpg");
        }
    }
}
