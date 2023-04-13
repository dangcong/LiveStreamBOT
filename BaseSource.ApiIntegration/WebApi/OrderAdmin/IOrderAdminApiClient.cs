using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.WebApi.OrderAdmin
{
    public interface IOrderAdminApiClient
    {
        Task<ApiResult<PagedResult<OrderInfoDto>>> GetAllAsync(OrderRequestDto model);
        Task<ApiResult<string>> Accept(int id);
        Task<ApiResult<string>> Cancel(int id);
        Task<ApiResult<OrderInfoDto>> GetById(int id);
    }
}
