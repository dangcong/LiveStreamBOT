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

        public async Task<List<StreamAdminInfoDto>> GetAllByUserIdAsync(string userId, bool isAdmin = false)
        {
            var _repository = _unitOfWork.GetRepository<StreamHistory>();

            var query = _repository.Queryable().AsNoTracking();
            if (!isAdmin)
            {
                query = query.Where(x => x.UserId == userId || x.ManagerBOT.UserManager == userId);
            }


            var data = await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<StreamAdminInfoDto>(x),
                include: x => x.Include(i => i.AppUser).Include(i => i.ManagerBOT),
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

        public async Task<IPagedList<StreamAdminInfoDto>> GetByFilterAsync(string userId, StreamAdminRequestDto model, bool isAdmin = false)
        {
            var _repository = _unitOfWork.GetRepository<StreamHistory>();
            var query = _repository.Queryable().AsNoTracking();
            if (isAdmin)
            {
                if (!string.IsNullOrWhiteSpace(model.Manager))
                {
                    query = query.Where(x => x.ManagerBOT.UserManager == model.Manager);
                }
                var lstManagers = new List<string>();
                if (!string.IsNullOrWhiteSpace(model.Managers))
                {
                    lstManagers = model.Managers.Split(',').ToList();
                }
                if (lstManagers.Any())
                {
                    query = query.Where(x => lstManagers.Any(i => i == x.AppUser.UserManager));
                }
            }
            else
            {
                query = query.Where(x => x.AppUser.UserManager == userId || x.UserId == userId);
            }


            query = query
                .Where(x => x.Status == StreamStatus.Streaming);


            return await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<StreamAdminInfoDto>(x),
                include: x => x.Include(i => i.AppUser).ThenInclude(o => o.Manager)
                .Include(i => i.ManagerBOT)
                .Include(i => i.ManagerBOTBackup),
               orderBy: x => x.OrderByDescending(i => i.CreatedTime),
               pageIndex: model.Page, pageSize: model.PageSize
                );
        }

        public async Task<StreamReportAdminDto> GetReportStreamAsync(string userId, bool isAdmin, string managers)
        {
            var _repositoryUser = _unitOfWork.GetRepository<AppUser>();
            var _repositoryBOT = _unitOfWork.GetRepository<ManagerBOT>();
            var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
            var _repositoryUserBot = _unitOfWork.GetRepository<UserManagerBOT>();
            var data = new StreamReportAdminDto();

            if (isAdmin)
            {
                var lstManagers = new List<string>();
                if (!string.IsNullOrWhiteSpace(managers))
                {
                    lstManagers = managers.Split(',').ToList();
                }
                if (lstManagers.Any())
                {
                    data.TotalBOTConnected = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && lstManagers.Any(i => i == x.UserManager) && x.Status == ManagerBOTStatus.Connected).CountAsync();
                    data.TotalBOT = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && lstManagers.Any(i => i == x.UserManager)).CountAsync();

                    var streamData = await _repositoryStream.Queryable().AsNoTracking()
                        .Where(x => lstManagers.Any(i => i == x.AppUser.UserManager))
                        .Select(x => new
                        {
                            Type = x.LiveType,
                            Status = x.Status
                        }).ToListAsync();

                    data.NumberOfThreads1080 = await _repositoryUserBot.Queryable()
                        .Include(x => x.ManagerBOT)
                        .Where(x => x.User.DeletedTime == null
                        && x.ManagerBOT.LiveType == LiveType._1080
                        && lstManagers.Any(i => i == x.User.UserManager)
                        ).SumAsync(x => x.NumberOfThreads);

                    data.NumberOfThreads1080InRun = streamData
                        .Where(x => x.Type == LiveType._1080 && x.Status == StreamStatus.Streaming
                        ).Count();

                    data.NumberOfThreads4K = await _repositoryUserBot.Queryable()
                        .Include(x => x.ManagerBOT).Include(x => x.User)
                        .Where(x => x.User.DeletedTime == null
                        && x.ManagerBOT.LiveType == LiveType._4K
                        && lstManagers.Any(i => i == x.User.UserManager)
                        ).SumAsync(x => x.NumberOfThreads);

                    data.NumberOfThreads4KInRun = streamData
                        .Where(x => x.Type == LiveType._4K && x.Status == StreamStatus.Streaming).Count();

                    data.NumberOfThreadsBackup = await _repositoryUserBot.Queryable()
                        .Include(x => x.ManagerBOT).Include(x => x.User)
                        .Where(x => x.User.DeletedTime == null
                        && x.ManagerBOT.LiveType == LiveType._4K
                        && lstManagers.Any(i => i == x.User.UserManager)
                        ).SumAsync(x => x.NumberOfThreads);


                    data.NumberOfThreadsBackupInRun = streamData
                        .Where(x => x.Type == LiveType._Backup && x.Status == StreamStatus.Streaming).Count();

                    data.NumberOfThreadsBackupPending = await _repositoryStream.Queryable().AsNoTracking()
                        .Where(x => lstManagers.Any(i => i == x.AppUser.UserManager) && x.ManagerBOTBackupId.GetValueOrDefault() > 0).CountAsync();



                    data.TotalBOT1080 = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.LiveType == LiveType._1080 && lstManagers.Any(i => i == x.UserManager)).CountAsync();

                    data.TotalBOT4k = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.LiveType == LiveType._4K && lstManagers.Any(i => i == x.UserManager)).CountAsync();

                    data.TotalBOTBackup = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.LiveType == LiveType._Backup && lstManagers.Any(i => i == x.UserManager)).CountAsync();


                    data.TotalUser = await _repositoryUser.Queryable().AsNoTracking()
                        .Where(x => !x.UserRoles.Any() && x.DeletedTime == null
                        && lstManagers.Any(i => i == x.UserManager)).CountAsync();
                    data.TotalManager = lstManagers.Count();
                }
                else
                {
                    data.TotalBOTConnected = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.Status == ManagerBOTStatus.Connected).CountAsync();
                    data.TotalBOT = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null).CountAsync();

                    var streamData = await _repositoryStream.Queryable().AsNoTracking()
                        .Select(x => new
                        {
                            Type = x.LiveType,
                            Status = x.Status
                        }).ToListAsync();

                    data.NumberOfThreads1080 = await _repositoryUserBot.Queryable()
                        .Include(x => x.ManagerBOT).Include(x => x.User)
                        .Where(x => x.User.DeletedTime == null && x.ManagerBOT.LiveType == LiveType._1080)
                        .SumAsync(x => x.NumberOfThreads);

                    data.NumberOfThreads1080InRun = streamData
                        .Where(x => x.Type == LiveType._1080 && x.Status == StreamStatus.Streaming).Count();

                    data.NumberOfThreads4K = await _repositoryUserBot.Queryable()
                        .Include(x => x.ManagerBOT).Include(x => x.User)
                        .Where(x => x.User.DeletedTime == null && x.ManagerBOT.LiveType == LiveType._4K)
                        .SumAsync(x => x.NumberOfThreads);

                    data.NumberOfThreads4KInRun = streamData
                        .Where(x => x.Type == LiveType._4K && x.Status == StreamStatus.Streaming).Count();

                    data.TotalUser = await _repositoryUser.Queryable().AsNoTracking()
                        .Where(x => !x.UserRoles.Any() && x.DeletedTime == null).CountAsync();
                    //data.TotalManager = await _repositoryUser.Queryable().AsNoTracking()
                    //    .Where(x => x.DeletedTime == null && x.UserRoles.Any(x => x.RoleId == "baea9405-c706-46ea-9f52-6cfa8c83d65f"))
                    //    .CountAsync();

                    data.NumberOfThreadsBackup = await _repositoryUserBot.Queryable()
                    .Include(x => x.ManagerBOT).Include(x => x.User)
                    .Where(x => x.User.DeletedTime == null
                    && x.ManagerBOT.LiveType == LiveType._4K
                    ).SumAsync(x => x.NumberOfThreads);

                    data.NumberOfThreadsBackupInRun = streamData
                    .Where(x => x.Type == LiveType._Backup && x.Status == StreamStatus.Streaming).Count();

                    data.NumberOfThreadsBackupPending = await _repositoryStream.Queryable().AsNoTracking()
                        .Where(x => x.ManagerBOTBackupId.GetValueOrDefault() > 0).CountAsync();



                    data.TotalBOT1080 = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.LiveType == LiveType._1080).CountAsync();

                    data.TotalBOT4k = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.LiveType == LiveType._4K).CountAsync();

                    data.TotalBOTBackup = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.LiveType == LiveType._Backup).CountAsync();
                }

            }
            else
            {
                data.TotalBOTConnected = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.UserManager == userId && x.Status == ManagerBOTStatus.Connected).CountAsync();
                data.TotalBOT = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.UserManager == userId).CountAsync();

                var streamData = await _repositoryStream.Queryable().AsNoTracking()
                    .Where(x => x.AppUser.UserManager == userId || x.UserId == userId)
                    .Select(x => new
                    {
                        Type = x.LiveType,
                        Status = x.Status
                    }).ToListAsync();

                data.NumberOfThreads1080 = await _repositoryUserBot.Queryable()
                    .Include(x => x.ManagerBOT).Include(x => x.User)
                    .Where(x => x.User.DeletedTime == null
                    && x.ManagerBOT.LiveType == LiveType._1080
                    && (x.User.UserManager == userId || x.UserId == userId))
                    .SumAsync(x => x.NumberOfThreads);

                data.NumberOfThreads1080InRun = streamData
                    .Where(x => x.Type == LiveType._1080 && x.Status == StreamStatus.Streaming).Count();

                data.NumberOfThreads4K = await _repositoryUserBot.Queryable()
                    .Include(x => x.ManagerBOT).Include(x => x.User)
                    .Where(x => x.User.DeletedTime == null && x.ManagerBOT.LiveType == LiveType._4K
                    && (x.User.UserManager == userId || x.UserId == userId))
                    .SumAsync(x => x.NumberOfThreads);

                data.NumberOfThreads4KInRun = streamData
                    .Where(x => x.Type == LiveType._4K && x.Status == StreamStatus.Streaming).Count();

                data.TotalUser = await _repositoryUser.Queryable().AsNoTracking()
                    .Where(x => x.UserManager == userId && !x.UserRoles.Any() && x.DeletedTime == null).CountAsync();
                data.TotalManager = 1;

                data.NumberOfThreadsBackup = await _repositoryUserBot.Queryable()
                    .Include(x => x.ManagerBOT).Include(x => x.User)
                    .Where(x => x.User.DeletedTime == null
                    && x.ManagerBOT.LiveType == LiveType._Backup
                    && (x.User.UserManager == userId || x.UserId == userId))
                    .SumAsync(x => x.NumberOfThreads);

                data.NumberOfThreadsBackupInRun = streamData
                .Where(x => x.Type == LiveType._Backup && x.Status == StreamStatus.Streaming).Count();

                data.NumberOfThreadsBackupPending = await _repositoryStream.Queryable().AsNoTracking()
                    .Where(x => x.ManagerBOTBackupId.GetValueOrDefault() > 0).CountAsync();



                data.TotalBOT1080 = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.UserManager == userId && x.LiveType == LiveType._1080).CountAsync();

                data.TotalBOT4k = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.UserManager == userId && x.LiveType == LiveType._4K).CountAsync();

                data.TotalBOTBackup = await _repositoryBOT.Queryable().AsNoTracking()
                    .Where(x => x.DeletedTime == null && x.UserManager == userId && x.LiveType == LiveType._Backup).CountAsync();


                //data.TotalManager = await _repositoryUser.Queryable().AsNoTracking()
                //    .Where(x => x.UserManager == userId && x.DeletedTime == null && x.UserRoles.Any(x => x.RoleId == "baea9405-c706-46ea-9f52-6cfa8c83d65f"))
                //    .CountAsync();
            }

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
