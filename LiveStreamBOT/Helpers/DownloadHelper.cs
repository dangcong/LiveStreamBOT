using BaseSouce.SharedSignalrData.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TqkLibrary.Net.CloudStorage.GoogleDrive;

namespace LiveStreamBOT.Helpers
{
    internal class DownloadHelper : IDisposable
    {
        //https://drive.google.com/file/d/1I0zTP9I5XIcDitHa_4hKug5FM8pCV-EY/view?usp=share_link
        readonly Regex regex_driveFile = new Regex("(?<=\\/d\\/)[a-zA-Z0-9_-]+(?=\\/)");

        readonly HttpClient httpClient;
        readonly DriveApiNonLogin driveApiNonLogin;
        public DownloadHelper()
        {
            httpClient = new HttpClient(new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                AllowAutoRedirect = true,
            });
            driveApiNonLogin = new DriveApiNonLogin(httpClient);
        }

        public void Dispose()
        {
            driveApiNonLogin.Dispose();
        }

        public async Task DownloadAsync(FileData fileData, string workingDir, Action<int> dataTransfer, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileData?.Url)) throw new ArgumentNullException($"{nameof(FileData)}.{nameof(FileData.Url)}");
            if (string.IsNullOrWhiteSpace(workingDir)) throw new ArgumentNullException($"{nameof(workingDir)}");
            Directory.CreateDirectory(workingDir);
            string savePath = Path.Combine(workingDir, fileData.Name);
            switch (fileData.UrlType)
            {
                case UrlType.Normal:
                    {
                        await DownloadRaw(fileData.Url, savePath, dataTransfer, cancellationToken);
                        break;
                    }

                case UrlType.GoogleDrive:
                    {
                        string fileId = string.Empty;

                        Match match = regex_driveFile.Match(fileData.Url);
                        if (match.Success)
                            fileId = match.Value;


                        if (string.IsNullOrWhiteSpace(fileId))
                        {
                            throw new NotSupportedException($"Not support link type: {fileData.Url}");
                        }
                        else
                        {
                            using Stream stream = await driveApiNonLogin.DownloadFileAsync(fileId, cancellationToken);
                            using TrackStream trackStream = new TrackStream(stream, dataTransfer);
                            using FileStream fileStream = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                            await trackStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                        }
                        break;
                    }

                default:
                    throw new NotSupportedException($"{fileData.UrlType}");
            }

        }

        async Task DownloadRaw(string url, string filePath, Action<int> dataTransfer, CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
            using Stream stream = await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadAsStreamAsync(cancellationToken);
            using TrackStream trackStream = new TrackStream(stream, dataTransfer);
            using FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            await trackStream.CopyToAsync(fileStream, cancellationToken);
        }
    }
}
