using AutoMapper;
using BaseSouce.Data.EF;
using BaseSource.ViewModels.ServicePackage;
using EFCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace BaseSouce.Services.Services.ServicePackageByMonth
{
    public class ServicePackByMonthService : IServicePackByMonthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServicePackByMonthService> _logger;
        private readonly IMapper _mapper;

        public ServicePackByMonthService(IUnitOfWork<BaseSourceDbContext> unitOfWork, ILogger<ServicePackByMonthService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<KeyValuePair<bool, string>> CreateAsync(ServicePackByMonthCreateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<Data.Entities.ServicePackByMonth>();

                var existsEntity = await _repository.ExistsAsync(x => x.ServicePackId == model.ServicePackId && x.Month == model.Month);
                if (existsEntity)
                {
                    return new KeyValuePair<bool, string>(false, "Gói tháng đã tồn tại!");
                }

                _repository.Insert(new Data.Entities.ServicePackByMonth
                {
                    Month = model.Month,
                    Discount = model.Discount,
                    Percent = model.Percent,
                    Price = model.Price,
                    ServicePackId = model.ServicePackId,
                });
                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create ServicePackByMonth by error: [{ex}]");
                return new KeyValuePair<bool, string>(false, $"Tạo gói tháng không thành công!:[{ex.Message}]");
            }
        }

        public async Task<KeyValuePair<bool, string>> DeleteAsync(int id)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<Data.Entities.ServicePackByMonth>();
                var entity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
                if (entity != null)
                {
                    _repository.Delete(entity);
                    await _unitOfWork.SaveChangesAsync();
                }
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Delete ServicePackByMonth failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Xóa gói tháng không thành công:[{ex.Message}]");
            }
        }

        public async Task<ServicePackByMonthDto> GetByIdAsync(int id)
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ServicePackByMonth>();
            return await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == id,
                selector: x => _mapper.Map<ServicePackByMonthDto>(x));
        }

        public async Task<List<ServicePackByMonthDto>> GetByNameAsync(string name)
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ServicePackByMonth>();
            return await _repository.Queryable().AsNoTracking()
                .Include(x => x.ServicePack)
                .Where(x => x.ServicePack.Name == name)
                .Select(x => _mapper.Map<ServicePackByMonthDto>(x)).ToListAsync();
        }

        public async Task<List<ServicePackByMonthDto>> GetByServiceAsync(int serviceId)
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ServicePackByMonth>();
            return await _repository.Queryable().AsNoTracking()
                .Where(x => x.ServicePackId == serviceId)
                .Select(x => _mapper.Map<ServicePackByMonthDto>(x)).ToListAsync();
        }

        public async Task<KeyValuePair<bool, string>> UpdateAsync(int id, ServicePackByMonthUpdateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<Data.Entities.ServicePackByMonth>();
                var entity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
                if (entity != null)
                {
                    entity.Percent = model.Percent;
                    entity.Discount = model.Discount;
                    entity.Price = model.Price;
                    entity.Percent = model.Percent;
                    _repository.Update(entity);
                    await _unitOfWork.SaveChangesAsync();
                    return new KeyValuePair<bool, string>(true, string.Empty);
                }
                return new KeyValuePair<bool, string>(true, $"Gói tháng không tồn tại!");
            }
            catch (Exception ex)
            {

                _logger.LogError($"Update ServicePackByMonth failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Cập nhật gói tháng không thành công:[{ex.Message}]");
            }
        }
    }
}
