using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using BaseSource.Shared.Bot;

namespace LiveStreamBOT.Services
{
    /// <summary>
    /// Client gọn gọi API cập nhật bot (BotUpdateController). Tự chứa, BaseAddress lấy từ URL API
    /// (args[0] mà bot dùng cho SignalR); bỏ qua kiểm tra SSL giống SignalrClient.
    /// </summary>
    internal sealed class BotUpdateApiClient
    {
        /// <summary>Kết quả API server (khớp shape ApiResult&lt;T&gt;; server trả camelCase).</summary>
        sealed class BotApiResult<T>
        {
            public bool IsSuccessed { get; set; }
            public string? Message { get; set; }
            public T? ResultObj { get; set; }
        }

        static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        static readonly HttpClientHandler _handler = new HttpClientHandler
        {
            // always trust the SSL certificate (giống SignalrClient)
            ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
        };

        readonly Uri _baseAddress;

        public BotUpdateApiClient(Uri baseAddress)
        {
            this._baseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
        }

        HttpClient CreateClient()
        {
            return new HttpClient(_handler, disposeHandler: false)
            {
                BaseAddress = _baseAddress,
                Timeout = TimeSpan.FromMinutes(10),
            };
        }

        public async Task<BotVersionVM?> GetLatestVersionAsync(string rid, CancellationToken cancellationToken = default)
        {
            using HttpClient client = CreateClient();
            using HttpResponseMessage response = await client.GetAsync($"api/BotUpdate/GetLatestVersion?rid={Uri.EscapeDataString(rid)}", cancellationToken);
            string json = await response.Content.ReadAsStringAsync(cancellationToken);
            BotApiResult<BotVersionVM>? result = JsonSerializer.Deserialize<BotApiResult<BotVersionVM>>(json, _jsonOptions);
            if (result is null || !result.IsSuccessed)
                return null;
            return result.ResultObj;
        }

        /// <summary>Tải file (url tương đối so với BaseAddress). Trả về stream đã đọc toàn bộ vào memory.</summary>
        public async Task<Stream> DownloadFileAsync(string relativeUrl, CancellationToken cancellationToken = default)
        {
            using HttpClient client = CreateClient();
            using HttpResponseMessage response = await client.GetAsync(relativeUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            MemoryStream memoryStream = new MemoryStream();
            using (Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken))
            {
                await stream.CopyToAsync(memoryStream, cancellationToken);
            }
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}
