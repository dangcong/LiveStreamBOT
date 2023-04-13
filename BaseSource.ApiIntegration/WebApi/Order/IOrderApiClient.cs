using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Order;
using EFCore.UnitOfWork.PageList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.WebApi.Order
{
    public interface IOrderApiClient
    {
        Task<ApiResult<PagedResult<OrderInfoDto>>> GetAllAsync(OrderRequestDto model);
        Task<ApiResult<string>> CreateAsync(OrderCreateDto model);

    }
}
