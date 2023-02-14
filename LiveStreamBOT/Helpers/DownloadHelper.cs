using BaseSouce.SharedSignalrData.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LiveStreamBOT.Helpers
{
    internal static class DownloadHelper
    {
        static readonly HttpClient httpClient = new HttpClient();
        //https://drive.google.com/file/d/1I0zTP9I5XIcDitHa_4hKug5FM8pCV-EY/view?usp=share_link
        static readonly Regex regex_driveFile = new Regex("(?<=\\/d\\/)[a-zA-Z0-9_-]+(?=\\/)");
        public static async Task DownloadAsync(this FileData fileData, string workingDir, Action<int> dataTransfer, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(fileData?.Url)) throw new ArgumentNullException($"{nameof(FileData)}.{nameof(FileData.Url)}");
            if (string.IsNullOrWhiteSpace(workingDir)) throw new ArgumentNullException($"{nameof(workingDir)}");
            Directory.CreateDirectory(workingDir);

            switch (fileData.UrlType)
            {
                case UrlType.Normal:
                    {
                        await DownloadRaw(fileData.Url, Path.Combine(workingDir, fileData.Name), dataTransfer, cancellationToken);
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
                            await DownloadRaw(
                                $"https://drive.google.com/uc?export=download&id={fileId}",
                                Path.Combine(workingDir, fileData.Name),
                                dataTransfer,
                                cancellationToken);
                        }
                        break;
                    }

                default:
                    throw new NotSupportedException($"{fileData.UrlType}");
            }

        }

        static async Task DownloadRaw(string url, string filePath, Action<int> dataTransfer, CancellationToken cancellationToken = default)
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
