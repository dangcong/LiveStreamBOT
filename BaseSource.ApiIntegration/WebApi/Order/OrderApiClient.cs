using BaseSouce.ViewModels.Common;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.Order;
using EFCore.UnitOfWork.PageList;
using BaseSouce.Shared.Extensions;

namespace BaseSource.ApiIntegration.WebApi.Order
{
    public class OrderApiClient : IOrderApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<string>> CreateAsync(OrderCreateDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.PostAsync<ApiResult<string>>($"/api/order", model);
        }

        public async Task<ApiResult<PagedResult<OrderInfoDto>>> GetAllAsync(OrderRequestDto model)
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<PagedResult<OrderInfoDto>>>($"/api/orders",model);
        }
    }
}
