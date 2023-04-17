using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Shared.Enums;
using BaseSource.ViewModels.ManagerBOT;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseSouce.Services.Services.BOT
{
    public class ManagerBOTService : IManagerBOTService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ManagerBOTService> _logger;
        private readonly IMapper _mapper;

        public ManagerBOTService(IUnitOfWork<BaseSourceDbContext> unitOfWork, ILogger<ManagerBOTService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<KeyValuePair<bool, string>> CreateBOTAsync(ManagerBotCreateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

                _repository.Insert(new Data.Entities.ManagerBOT
                {
                    BotId = model.BotId,
                    CreatedTime = DateTime.Now,
                    Group = model.Group,
                    Name = model.Name,
                    Status = ManagerBOTStatus.Connected,
                });


                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create BOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Create BOT failed");
            }
        }

        public async Task<KeyValuePair<bool, string>> DeleteAsync(int id)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id, disableTracking: false);

                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không tồn tại!");
                }
                botCurrent.DeletedTime = DateTime.Now;
                botCurrent.Status = ManagerBOTStatus.Disconnected;
                _repository.Update(botCurrent);

                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Delete BOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Xóa BOT không thành công vui lòng thử lại!");
            }
        }

        public async Task<IPagedList<ManagerBotInfoDto>> GetBOTByFilterAsync(ManagerBOTRequestDto model)
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();
            var query = _repository.Queryable().AsNoTracking();

            return await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<ManagerBotInfoDto>(x),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: model.Page, pageSize: model.PageSize);
        }

        public async Task<int> GetTotalBOTInRunAsync()
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

            return await _repository.Queryable()
                 .Where(x => x.StreamHistorys.Any(i => i.Status == StreamStatus.Streaming)).CountAsync();
        }

        public async Task<KeyValuePair<bool, string>> UpdateBOTAsync(int id, ManagerBotUpdateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id, disableTracking: false);
                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không tồn tại!");
                }
                botCurrent.Name = model.Name;
                botCurrent.Group = model.Group;
                botCurrent.NumberOfThreads = model.NumberOfThreads;
                _repository.Update(botCurrent);

                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update ManagetBOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật BOT không thành công!");
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateStatusAsync(int id, ManagerBOTStatus status)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id, disableTracking: false);
                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT không tồn tại!");
                }
                botCurrent.Status = status;
                _repository.Update(botCurrent);

                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Update status ManagetBOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật trạng thái BOT không thành công!");
            }
        }
    }
}
