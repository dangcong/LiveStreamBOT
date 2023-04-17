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
        public async Task<ApiResult<PagedResult<StreamAdminInfoDto>>> GetByFilterAsync(StreamAdminRequestDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<PagedResult<StreamAdminInfoDto>>>("/api/admin/streams", model);
        }
    }
}
