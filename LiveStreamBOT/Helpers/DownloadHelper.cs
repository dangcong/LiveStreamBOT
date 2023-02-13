using BaseSouce.SharedSignalrData.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveStreamBOT.Helpers
{
    internal static class DownloadHelper
    {
        static readonly HttpClient httpClient = new HttpClient();
        public static async Task DownloadAsync(this FileData fileData, string workingDir, Action<int> dataTransfer, CancellationToken cancellationToken = default)
        {

            switch (fileData.UrlType)
            {
                case UrlType.Normal:
                    {
                        using HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, fileData.Url);
                        using HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);
                        using Stream stream = await httpResponseMessage.EnsureSuccessStatusCode().Content.ReadAsStreamAsync(cancellationToken);
                        using TrackStream trackStream = new TrackStream(stream, dataTransfer);
                        using FileStream fileStream = new FileStream(Path.Combine(workingDir, fileData.Name), FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                        await trackStream.CopyToAsync(fileStream, cancellationToken);
                        break;
                    }


                case UrlType.GoogleDrive:
                    {

                        break;
                    }
            }

        }
    }
}
