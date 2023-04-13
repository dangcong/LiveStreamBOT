using BaseSource.ViewModels.Order;
using EFCore.UnitOfWork.PageList;

namespace BaseSouce.Services.Services.Order
{
    public interface IOrderService
    {
        Task<KeyValuePair<bool, string>> CreateAsync(string userId, OrderCreateDto model);
        Task<IPagedList<OrderInfoDto>> GetOrderByFillerAsync(string userId, OrderRequestDto model);
        
    }
}
