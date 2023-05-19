using AutoMapper;
using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Shared.Enums;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Services.Services.StreamAdmin
{
    public class StreamAdminService : IStreamAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StreamAdminService> _logger;
        private readonly IMapper _mapper;
        public StreamAdminService(
            IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<StreamAdminService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

        }

        public async Task<List<StreamAdminInfoDto>> GetAllByUserIdAsync(string userId)
        {
            var _repository = _unitOfWork.GetRepository<StreamHistory>();

            var query = _repository.Queryable().AsNoTracking();
            query = query.Where(x => x.UserId == userId);

            var data = await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<StreamAdminInfoDto>(x),
                include: x => x.Include(i => i.AppUser),
               orderBy: x => x.OrderByDescending(i => i.CreatedTime),
               pageIndex: 1, pageSize: 500);
            return data.Items.ToList();
        }

        public async Task<IPagedList<StreamAdminInfoDto>> GetByBotIdAsync(int botId)
        {
            var _repository = _unitOfWork.GetRepository<StreamHistory>();

            var query = _repository.Queryable().AsNoTracking();
            query = query.Where(x => x.ManagerBOTId == botId);

            return await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<StreamAdminInfoDto>(x),
                include: x => x.Include(i => i.AppUser),
               orderBy: x => x.OrderByDescending(i => i.CreatedTime),
               pageIndex: 1, pageSize: 1000);
        }

        public async Task<IPagedList<StreamAdminInfoDto>> GetByFilterAsync(StreamAdminRequestDto model)
        {
            var _repository = _unitOfWork.GetRepository<StreamHistory>();



            var query = _repository.Queryable().AsNoTracking();

            query = query.Where(x => x.Status == StreamStatus.Streaming);
            return await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<StreamAdminInfoDto>(x),
                include: x => x.Include(i => i.AppUser).Include(i=>i.ManagerBOT),
               orderBy: x => x.OrderByDescending(i => i.CreatedTime),
               pageIndex: model.Page, pageSize: model.PageSize
                );
        }

        public async Task<StreamReportAdminDto> GetReportStreamAsync()
        {
            var _repositoryUser = _unitOfWork.GetRepository<AppUser>();
            var _repositoryBOT = _unitOfWork.GetRepository<ManagerBOT>();
            var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
            var _repositoryUserBot = _unitOfWork.GetRepository<UserManagerBOT>();
            var data = new StreamReportAdminDto();

            data.TotalBOT = await _repositoryBOT.Queryable().AsNoTracking()
                .Where(x => x.DeletedTime == null).CountAsync();

            var streamData = await _repositoryStream.Queryable().AsNoTracking()
                .Select(x => new
                {
                    Type = x.LiveType,
                    Status = x.Status
                }).ToListAsync();

            data.NumberOfThreads1080 = await _repositoryUserBot.Queryable()
                .Include(x => x.ManagerBOT).Include(x=>x.User)
                .Where(x =>x.User.DeletedTime == null && x.ManagerBOT.LiveType == LiveType._1080)
                .SumAsync(x => x.NumberOfThreads);

            data.NumberOfThreads1080InRun = streamData
                .Where(x => x.Type == LiveType._1080 && x.Status == StreamStatus.Streaming).Count();

            data.NumberOfThreads4K = await _repositoryUserBot.Queryable()
                .Include(x => x.ManagerBOT).Include(x=>x.User)
                .Where(x => x.User.DeletedTime == null && x.ManagerBOT.LiveType == LiveType._4K)
                .SumAsync(x => x.NumberOfThreads);

            data.NumberOfThreads4KInRun = streamData
                .Where(x => x.Type == LiveType._4K && x.Status == StreamStatus.Streaming).Count();

            data.TotalUser = await _repositoryUser.Queryable().AsNoTracking()
                .Where(x => !x.UserRoles.Any() && x.DeletedTime == null).CountAsync();
            data.TotalManager = await _repositoryUser.Queryable().AsNoTracking()
                .Where(x => x.DeletedTime == null && x.UserRoles.Any(x => x.RoleId == "baea9405-c706-46ea-9f52-6cfa8c83d65f"))
                .CountAsync();
           // _logger.LogInformation($"NumberOfThreads4K:[{data.NumberOfThreads4K}]");
            return data;
        }

        public async Task<StreamClientInfoDto> GetStreamByIdAsync(int id)
        {
            var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

            return await _repositoryStream.GetFirstOrDefaultAsync(
                predicate: x => x.Id == id,
                selector: x => _mapper.Map<StreamClientInfoDto>(x),
                include: x => x.Include(i => i.AppUser),
                disableTracking: false);
        }
    }
}
