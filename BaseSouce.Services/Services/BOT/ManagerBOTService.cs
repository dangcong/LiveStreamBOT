using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.BotClient;
using BaseSource.Data.Entities;
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

        public async Task<KeyValuePair<bool, string>> ChangeUsedAsync(int id)
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
                botCurrent.IsUsed = !botCurrent.IsUsed;
                _repository.Update(botCurrent);

                await _unitOfWork.SaveChangesAsync();

                if (!botCurrent.IsUsed)
                {
                    //send data to BOT
                }

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Change Used BOT failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật BOT không thành công vui lòng thử lại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> CreateBOTAsync(ManagerBotCreateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

                var botCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.BotId == model.BotId,
                    disableTracking: false);

                if (botCurrent == null)
                {
                    _repository.Insert(new Data.Entities.ManagerBOT
                    {
                        BotId = model.BotId,
                        CreatedTime = DateTime.Now,
                        Group = model.Group,
                        Name = model.Name,
                        Status = ManagerBOTStatus.Connected,
                        UpdatedTime = DateTime.Now,
                    });
                }
                else
                {
                    botCurrent.ConnectionId = model.ConnectionId;
                    botCurrent.Bandwidth = model.Bandwidth;
                    botCurrent.RAM = model.RamUsage;
                    botCurrent.CPU = model.CpuUsage;
                    botCurrent.Status = ManagerBOTStatus.Connected;
                    botCurrent.UpdatedTime = DateTime.Now;
                    _repository.Update(botCurrent);

                }

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
                include: x => x.Include(i => i.StreamHistorys),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: model.Page, pageSize: model.PageSize);
        }

        public async Task<ManagerBotInfoDto> GetBOTByIdAsync(int id)
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

            return await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.Id == id,
                selector: x => _mapper.Map<ManagerBotInfoDto>(x));
        }

        public async Task<int> GetTotalBOTInRunAsync()
        {
            var _repository = _unitOfWork.GetRepository<Data.Entities.ManagerBOT>();

            return await _repository.Queryable()
                 .Where(x => x.StreamHistorys.Any(i => i.Status == StreamStatus.Streaming)).CountAsync();
        }

        public async Task<List<UserBotDto>> GetUserConnectBot(int id)
        {
            var _repository = _unitOfWork.GetRepository<UserManagerBOT>();

            return await _repository.Queryable().AsNoTracking()
                .Where(x => x.Id == id).Include(x => x.ManagerBOT).Include(i => i.User)
                .Select(x => new UserBotDto
                {
                    BotName = x.ManagerBOT.Name,
                    UserName = x.User.UserName,
                    Type = x.LiveType
                }).ToListAsync();
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
                var checkNameBot = await _repository.ExistsAsync(x => x.Name == model.Name && x.Id != id);

                if (checkNameBot)
                {
                    return new KeyValuePair<bool, string>(false, "Tên BOT đã tồn tại!");
                }

                botCurrent.Name = model.Name;
                botCurrent.Group = model.Group;
                botCurrent.NumberOfThreads = model.NumberOfThreads;
                botCurrent.UpdatedTime = DateTime.Now;

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
