using BaseSource.ViewModels.Order;
using EFCore.UnitOfWork.PageList;

namespace BaseSouce.Services.Services.OrderAdmin
{
    public interface IOrderAdminService
    {
        Task<IPagedList<OrderInfoDto>> GetAllOrderByFillerAsync( OrderRequestDto model);
        Task<KeyValuePair<bool, string>> AcceptAsync(int id);
        Task<KeyValuePair<bool, string>> CancelAsync(int id);
        Task<OrderInfoDto> GetByIdAsync(int id);
        Task<List<OrderInfoDto>> GetOrderByUserIdAsync(string userId);
        
    }
}
