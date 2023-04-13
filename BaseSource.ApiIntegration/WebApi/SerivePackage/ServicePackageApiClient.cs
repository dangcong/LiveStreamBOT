using BaseSouce.Shared.Enums;
using BaseSouce.Shared.Extensions;
using BaseSouce.ViewModels.Common;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.ServicePackage;

namespace BaseSource.ApiIntegration.WebApi.SerivePackage
{
    public class ServicePackageApiClient : IServicePackageApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ServicePackageApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResult<string>> CreateAsync(ServicePackCreateDto model)
        {

            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>($"/api/admin/ServicePack", model);
        }

        public async Task<ApiResult<string>> DeleteAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.DeleteAsync<ApiResult<string>>($"/api/admin/ServicePack/{id}");
        }

        public async Task<ApiResult<ServicePackDto>> GetByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<ServicePackDto>>($"/api/admin/ServicePack/id/{id}");
        }

        public async Task<ApiResult<InvoiceCodeInfoDto>> GetInvoiceInfoAsync(string live)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<InvoiceCodeInfoDto>>($"/api/ServicePack/invoice?live={live}");
        }

        public async Task<ApiResult<List<ServicePackDto>>> GetServicePackByTypeAsync(ServicePackType type)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<List<ServicePackDto>>>($"/api/ServicePack/type/{(int)type}");
        }

        public async Task<ApiResult<string>> UpdateAsync(int id, ServicePackUpdateDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PutAsync<ApiResult<string>>($"/api/admin/ServicePack/{id}", model);
        }
    }
}
