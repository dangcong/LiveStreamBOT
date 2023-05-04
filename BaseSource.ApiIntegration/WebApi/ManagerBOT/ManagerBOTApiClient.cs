using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ManagerBOT;
using BaseSouce.Shared.Extensions;
using BaseSource.Shared.Constants;

namespace BaseSource.ApiIntegration.WebApi.ManagerBOT
{
    public class ManagerBOTApiClient : IManagerBOTApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ManagerBOTApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<string>> ChangeUsedAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PatchAsync<ApiResult<string>>($"/api/admin/bot/{id}/ChangeUsed");
        }

        public async Task<ApiResult<string>> CreateBOTAsync(ManagerBotCreateDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>("/api/bot");
        }

        public async Task<ApiResult<string>> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>($"/api/admin/bot/{id}/delete");
        }

        public async Task<ApiResult<PagedResult<ManagerBotInfoDto>>> GetBOTByFilterAsync(ManagerBOTRequestDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<PagedResult<ManagerBotInfoDto>>>("/api/admin/bots", model);
        }

        public async Task<ApiResult<ManagerBotInfoDto>> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<ManagerBotInfoDto>>($"/api/admin/bot/{id}");
        }

        public async Task<ApiResult<int>> GetTotalBOTInRunAsync()
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<int>>("/api/admin/bot/total/run");
        }

        public async Task<ApiResult<List<UserBotDto>>> GetUserBot(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<List<UserBotDto>>>($"/api/admin/bot/group/{id}");
        }

        public async Task<ApiResult<string>> UpdateBOTAsync(int id, ManagerBotUpdateDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PutAsync<ApiResult<string>>($"/api/admin/bot/{id}", model);
        }
        
    }
}
