using BaseSouce.Shared.Extensions;
using BaseSouce.ViewModels.Common;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.ServicePackage;

namespace BaseSource.ApiIntegration.WebApi.SerivePackage
{
    public class ServicePackageByMonthApiClient : IServicePackageByMonthApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ServicePackageByMonthApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<string>> CreateAsync(ServicePackByMonthCreateDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>($"/api/admin/ServicePackByMonth", model);
        }

        public async Task<ApiResult<string>> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.DeleteAsync<ApiResult<string>>($"/api/admin/ServicePackByMonth/${id}");
        }

        public async Task<ApiResult<ServicePackByMonthDto>> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<ServicePackByMonthDto>>($"/api/ServicePackByMonth/{id}");
        }

        public async Task<ApiResult<List<ServicePackByMonthDto>>> GetServicePackByNameAsync(string name)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<List<ServicePackByMonthDto>>>($"/api/ServicePackByMonth/name/{name}");
        }

        public async Task<ApiResult<List<ServicePackByMonthDto>>> GetServicePackByTypeAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<List<ServicePackByMonthDto>>>($"/api/ServicePackByMonth/type/{id}");
        }

        public async Task<ApiResult<string>> UpdateAsync(ServicePackByMonthDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PutAsync<ApiResult<string>>($"/api/admin/ServicePackByMonth/{model.Id}", model);
        }
    }
}
