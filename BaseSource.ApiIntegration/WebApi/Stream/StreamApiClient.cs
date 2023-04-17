using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Stream;
using BaseSouce.Shared.Extensions;
using BaseSource.Shared.Constants;

namespace BaseSource.ApiIntegration.WebApi.Stream
{
    public class StreamApiClient : IStreamApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StreamApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<string>> Create(StreamCreateDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/stream/create", model);
        }
    }
}
