using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Stream;
using BaseSouce.Shared.Extensions;
using BaseSource.Shared.Constants;

namespace BaseSource.ApiIntegration.WebApi.StreamAdmin
{
    public class StreamAdminApiClient : IStreamAdminApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StreamAdminApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<List<StreamAdminInfoDto>>> GetAllByUserIdAsync(string userId)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<List<StreamAdminInfoDto>>>($"/api/admin/stream/user?userId={userId}");
        }

        public async Task<ApiResult<PagedResult<StreamAdminInfoDto>>> GetByBotIdAsync(int botId)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<PagedResult<StreamAdminInfoDto>>>($"/api/admin/stream/bot/{botId}");
        }

        public async Task<ApiResult<PagedResult<StreamAdminInfoDto>>> GetByFilterAsync(StreamAdminRequestDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<PagedResult<StreamAdminInfoDto>>>("/api/admin/streams", model);
        }
    }
}
