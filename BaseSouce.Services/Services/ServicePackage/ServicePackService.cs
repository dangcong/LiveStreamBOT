using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Services.ServicePackage;
using BaseSouce.Shared.Enums;
using BaseSource.ViewModels.ServicePackage;
using EFCore.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace BaseSouce.Services.Services.ServicePackage
{
    public class ServicePackService : IServicePackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ServicePackService> _logger;
        private readonly IMapper _mapper;

        public ServicePackService(IUnitOfWork<BaseSourceDbContext> unitOfWork, ILogger<ServicePackService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<KeyValuePair<bool, string>> CreateAsync(ServicePackCreateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<ServicePack>();

                var existsPackage = await _repository.ExistsAsync(x => x.Name.ToLower() == model.Name);
                if (existsPackage)
                {
                    return new KeyValuePair<bool, string>(false, "Tên gói đã tồn tại");
                }

                _repository.Insert(new ServicePack
                {
                    Discount = model.Discount,
                    Name = model.Name,
                    NumberOfThreads = model.NumberOfThreads,
                    Price = model.Price,
                    Type = model.Type,
                    IsPublished = true,
                    CreatedTime = DateTime.Now
                });
                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Create ServicePack failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Tạo gói không thành công : [{ex.Message}]");
            }
        }

        public async Task<KeyValuePair<bool, string>> DeleteAsync(int id)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<ServicePack>();

                var servicePackEntity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
                if (servicePackEntity != null)
                {
                    servicePackEntity.DeletedTime = DateTime.Now;
                    servicePackEntity.UpdatedTime = DateTime.Now;
                    _repository.Update(servicePackEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new KeyValuePair<bool, string>(true, string.Empty);
                }
                return new KeyValuePair<bool, string>(false, "Gói không tồn tại!");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete ServicePack failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Xóa không thành công : [{ex}]");
            }
        }

        public async Task<ServicePackDto> GetByIdAsync(int id)
        {
            var _repository = _unitOfWork.GetRepository<ServicePack>();

            return await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == id && x.IsPublished,
                selector: x => _mapper.Map<ServicePackDto>(x));
        }

        public async Task<InvoiceCodeInfoDto> GetInvoiceInfoAsync(string userId, string live)
        {
            var data = new InvoiceCodeInfoDto()
            {
                IsSuccessed = true,
                PackageName = live.ToUpper()
            };
            var _repositoryUser = _unitOfWork.GetRepository<AppUser>();
            var _repositoryOrder = _unitOfWork.GetRepository<Data.Entities.Order>();

            var userCurrent = await _repositoryUser.GetFirstOrDefaultAsync(predicate: x => x.Id == userId);

            var servicePackCurrent = await _unitOfWork.GetRepository<ServicePack>()
                .GetFirstOrDefaultAsync(predicate: x => x.Name.ToLower() == live.ToLower());
            if (servicePackCurrent == null)
            {
                data.IsSuccessed = false;
            }
            else
            {
                var order = await _repositoryOrder.GetFirstOrDefaultAsync(predicate: x => x.ServicePackId == servicePackCurrent.Id,
                selector: x => x.Id,
                orderBy: x => x.OrderByDescending(i => i.Id));

                var orderId = order != null ? order += 1 : 1;


                data.UserName = userCurrent.UserName;
                data.LinkFB = userCurrent.LinkFB;
                data.CodeBill = $"{userCurrent.UserName.ToUpper()}-{live.ToUpper()}-{orderId}";
            }
            return data;

        }

        public async Task<List<ServicePackDto>> GetServicePackAllAsync()
        {
            var _repository = _unitOfWork.GetRepository<ServicePack>();

            return await _repository.Queryable()
                .AsNoTracking().Select(x => _mapper.Map<ServicePack, ServicePackDto>(x)).ToListAsync();
        }

        public async Task<List<ServicePackDto>> GetServicePackByTypeAsync(ServicePackType type)
        {
            var _repository = _unitOfWork.GetRepository<ServicePack>();

            return await _repository.Queryable()
                .AsNoTracking().Where(x => x.Type == type)
                .Select(x => _mapper.Map<ServicePack, ServicePackDto>(x)).ToListAsync();
        }

        public async Task<KeyValuePair<bool, string>> UpdateAsync(int id, ServicePackUpdateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<ServicePack>();

                var existsPackage = await _repository.ExistsAsync(x => x.Name.ToLower() == model.Name && x.Id != id);
                if (existsPackage)
                {
                    return new KeyValuePair<bool, string>(false, "Tên gói đã tồn tại");
                }

                var servicePackEntity = await _repository.GetFirstOrDefaultAsync(predicate: x => x.Id == id);
                if (servicePackEntity != null)
                {
                    servicePackEntity.Name = model.Name;
                    servicePackEntity.Price = model.Price;
                    servicePackEntity.NumberOfThreads = model.NumberOfThreads;
                    servicePackEntity.UpdatedTime = DateTime.Now;
                    servicePackEntity.Discount = model.Discount;
                    _repository.Update(servicePackEntity);
                    await _unitOfWork.SaveChangesAsync();
                    return new KeyValuePair<bool, string>(true, string.Empty);
                }
                return new KeyValuePair<bool, string>(false, "Gói không tồn tại1");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update ServicePack failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Cập nhật gói không thành công : [{ex.Message}]");
            }
        }
    }
}
