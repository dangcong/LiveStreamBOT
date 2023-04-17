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

        public async Task<ApiResult<string>> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.DeleteAsync<ApiResult<string>>($"/api/stream/{id}");
        }

        public async Task<ApiResult<PagedResult<StreamClientInfoDto>>> GetByFilterAsync(StreamClientRequestDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<PagedResult<StreamClientInfoDto>>>("/api/streams", model);
        }

        public async Task<ApiResult<string>> StartStreamAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>($"/api/stream/{id},start");
        }

        public async Task<ApiResult<string>> UpdateAsync(int id, StreamUpdateDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PutAsync<ApiResult<string>>($"/api/stream/{id}", model);
        }
    }
}
