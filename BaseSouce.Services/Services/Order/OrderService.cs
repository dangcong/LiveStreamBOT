using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Shared.Enums;
using BaseSource.ViewModels.Order;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Services.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork<BaseSourceDbContext> unitOfWork, ILogger<OrderService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<KeyValuePair<bool, string>> CreateAsync(string userId, OrderCreateDto model)
        {
            try
            {
                var _repositoryOrder = _unitOfWork.GetRepository<Data.Entities.Order>();

                var _repositoryPack = _unitOfWork.GetRepository<Data.Entities.ServicePack>();

                var anyBill = await _repositoryOrder.GetFirstOrDefaultAsync(predicate: x => x.OrderId == model.CodeBill);
                if (anyBill != null)
                {
                    return new KeyValuePair<bool, string>(false, "Đơn hàng đã tồn tại, vui lòng thử lại");
                }

                var packageCurrent = await _repositoryPack.GetFirstOrDefaultAsync(
                    predicate: x => x.Name.ToLower() == model.PackageName
                    && x.IsPublished
                    && x.ServicePackByMonths.Any(i => i.Month == model.Month),
                    selector: x => new
                    {
                        Id = x.Id,
                        PriceMonth = x.ServicePackByMonths.Where(i => i.Month == model.Month).Select(i => i.Price).FirstOrDefault(),
                        PriceDiscount = x.ServicePackByMonths.Where(i => i.Month == model.Month).Select(i => i.Discount).FirstOrDefault(),
                        NumberOfThreads = x.NumberOfThreads

                    });
                if (packageCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Gói dịch vụ không tồn tại, vui lòng thử lại");
                }

                _repositoryOrder.Insert(
                    new Data.Entities.Order()
                    {
                        OrderId = model.CodeBill,
                        CreatedTime = DateTime.Now,
                        ExpiredTime = DateTime.Now.AddMonths(model.Month),
                        IsActive = false,
                        NumberOfThreads = packageCurrent.NumberOfThreads,
                        Price = packageCurrent.PriceDiscount > 0 ? packageCurrent.PriceDiscount : packageCurrent.PriceMonth,
                        Month = model.Month,
                        ServicePackId = packageCurrent.Id,
                        UserId = userId,
                        Status = OrderStatus.Waiting
                    });
                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create Order failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Không thể tạo đơn hàng, vui lòng thử lại!");
            }
        }

        public async Task<IPagedList<OrderInfoDto>> GetOrderByFillerAsync(string userId, OrderRequestDto model)
        {
            var _repositoryOrder = _unitOfWork.GetRepository<Data.Entities.Order>();
            var query = _repositoryOrder.Queryable().AsNoTracking();
            query = query.Where(x => x.UserId == userId);

            return await _repositoryOrder.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<OrderInfoDto>(x),
                include: x => x.Include(i => i.User).Include(i => i.ServicePack),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: model.Page, pageSize: model.PageSize);

        }
    }
}
