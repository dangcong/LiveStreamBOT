using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Shared.Enums;
using BaseSource.ViewModels.Order;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseSouce.Services.Services.OrderAdmin
{
    public class OrderAdminService : IOrderAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderAdminService> _logger;
        private readonly IMapper _mapper;

        public OrderAdminService(IUnitOfWork<BaseSourceDbContext> unitOfWork, ILogger<OrderAdminService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<KeyValuePair<bool, string>> AcceptAsync(int id)
        {
            try
            {
                var _repositoryUser = _unitOfWork.GetRepository<AppUser>();

                var _repository = _unitOfWork.GetRepository<Data.Entities.Order>();
                var orderEntity = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id,
                    selector: x => x,
                    disableTracking: false,
                    include: x => x.Include(i => i.ServicePack));
                if (orderEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Đơn hàng không tồn tại!");
                }
                orderEntity.Status = OrderStatus.Success;
                orderEntity.ExpiredTime = DateTime.Now.AddMonths(orderEntity.Month);
                orderEntity.IsActive = true;

                _repository.Update(orderEntity);

                //disable order


                var userCurrent = await _repositoryUser.Queryable()
                    .Where(x => x.Id == orderEntity.UserId).FirstOrDefaultAsync();

                if (userCurrent != null)
                {
                    switch (orderEntity.ServicePack.Type)
                    {
                        case ServicePackType._1080:
                            userCurrent.NumberOfThreads1080 = orderEntity.NumberOfThreads;
                            userCurrent.NumberOfThreads1080Create = orderEntity.ServicePack.NumberOfThreadsCreate;

                            var order1080Disable = _repository.Queryable().Where(x => x.IsActive
                            && x.ServicePack.Type == ServicePackType._4K).FirstOrDefault();
                            if (order1080Disable != null)
                            {
                                order1080Disable.IsActive = false;
                                _repository.Update(order1080Disable);
                            }

                            break;
                        case ServicePackType._4K:
                            userCurrent.NumberOfThreads4K = orderEntity.NumberOfThreads;
                            userCurrent.NumberOfThreads4KCreate = orderEntity.ServicePack.NumberOfThreadsCreate;

                            var order4KDisable = _repository.Queryable().Where(x => x.IsActive
                            && x.ServicePack.Type == ServicePackType._4K).FirstOrDefault();
                            if (order4KDisable != null)
                            {
                                order4KDisable.IsActive = false;
                                _repository.Update(order4KDisable);
                            }
                            break;
                    }

                    _repositoryUser.Update(userCurrent);
                }
                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Accept Order failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Cập nhật không thành công:[{ex.Message}]");
            }
        }

        public async Task<KeyValuePair<bool, string>> CancelAsync(int id)
        {
            try
            {
                var _repositoryUser = _unitOfWork.GetRepository<AppUser>();

                var _repository = _unitOfWork.GetRepository<Data.Entities.Order>();
                var orderEntity = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id,
                    selector: x => x,
                    disableTracking: false,
                    include: x => x.Include(i => i.ServicePack));
                if (orderEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Đơn hàng không tồn tại!");
                }
                orderEntity.Status = OrderStatus.Cancel;
               

                _repository.Update(orderEntity);

               
                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Accept Order failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Cập nhật không thành công:[{ex.Message}]");
            }
        }

        public async Task<IPagedList<OrderInfoDto>> GetAllOrderByFillerAsync(OrderRequestDto model)
        {
            var _repositoryOrder = _unitOfWork.GetRepository<Data.Entities.Order>();
            var query = _repositoryOrder.Queryable().AsNoTracking();

            try
            {
                return await _repositoryOrder.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<OrderInfoDto>(x),
                include: x => x.Include(i => i.User).Include(i => i.ServicePack),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: model.Page, pageSize: model.PageSize);
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task<OrderInfoDto> GetByIdAsync(int id)
        {
            var _reposiotry = _unitOfWork.GetRepository<Data.Entities.Order>();
            return await _reposiotry.GetFirstOrDefaultAsync(
                predicate: x => x.Id == id,
                selector: x => _mapper.Map<OrderInfoDto>(x),
                include: x => x.Include(i => i.User).Include(i => i.ServicePack));
        }
    }
}
