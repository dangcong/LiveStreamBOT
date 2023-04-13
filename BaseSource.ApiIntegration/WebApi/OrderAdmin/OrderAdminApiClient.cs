using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Order;
using BaseSouce.Shared.Extensions;
using BaseSource.Shared.Constants;

namespace BaseSource.ApiIntegration.WebApi.OrderAdmin
{
    public class OrderAdminApiClient : IOrderAdminApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderAdminApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<string>> Accept(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>($"/api/admin/order/accept/{id}");
        }

        public async Task<ApiResult<string>> Cancel(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>($"/api/admin/order/cancel/{id}");
        }

        public async Task<ApiResult<PagedResult<OrderInfoDto>>> GetAllAsync(OrderRequestDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<PagedResult<OrderInfoDto>>>($"/api/admin/orders", model);
        }

        public async Task<ApiResult<OrderInfoDto>> GetById(int id)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<OrderInfoDto>>($"/api/admin/order/{id}");
        }
    }
}
