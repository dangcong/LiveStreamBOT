using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Services.HubServices;
using BaseSouce.Services.Services.Stream;
using BaseSouce.Shared.Enums;
using BaseSouce.Shared.Helpers;
using BaseSouce.SharedSignalrData.Classes;
using BaseSouce.SharedSignalrData.Enums;
using BaseSouce.SharedSignalrData.Interfaces;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.UserAdmin;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseSouce.Services.Services.UserAdmin
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<UserAdminService> _logger;
        private readonly IStreamClientService _streamClientService;
        private readonly IHubContext<BotHub, IBotClientHub> _botContextHub;
        public UserAdminService(IUnitOfWork<BaseSourceDbContext> unitOfWork,
             RoleManager<AppRole> roleManager, ILogger<UserAdminService> logger
            , IStreamClientService streamClientService
            , IHubContext<BotHub, IBotClientHub> botContextHub)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _logger = logger;
            _streamClientService = streamClientService;
            _botContextHub = botContextHub;


        }

        public async Task<KeyValuePair<bool, string>> AddUserToBotAsync(UserAddBotAdminDto model)
        {
            try
            {
                if (model.NumberOfThreads < 1)
                {
                    return new KeyValuePair<bool, string>(false, "Số luồng không hợp lệ!");
                }

                var _repository = _unitOfWork.GetRepository<UserManagerBOT>();
                var _repositoryBOT = _unitOfWork.GetRepository<ManagerBOT>();
                var anyData = await _repository.ExistsAsync(
                    x => x.UserId == model.UserId && x.ManagerBOTId == model.BotId);
                if (anyData)
                {
                    return new KeyValuePair<bool, string>(false, "BOT đã tồn tại");
                }


                var botCurrent = await _repositoryBOT.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == model.BotId);
                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT Không tồn tại!");
                }

                _repository.Insert(new UserManagerBOT
                {
                    UserId = model.UserId,
                    ManagerBOTId = model.BotId,
                    LiveType = botCurrent.LiveType.GetValueOrDefault(),
                    NumberOfThreads = model.NumberOfThreads,
                    CreatedTime = DateTime.Now,
                });

                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Inser BOT User failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Thêm BOT không thành công![{ex.Message}]");
            }
        }

        public async Task<KeyValuePair<bool, string>> CreateUserAsync(UserCreateAdminDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<AppUser>();

                var existUser = await _repository.ExistsAsync(x => x.UserName.ToLower() == model.UserName.ToLower());
                if (existUser)
                {
                    return new KeyValuePair<bool, string>(false, "Tài khoản đã tồn tại!");
                }


                var userIdParent = string.Empty;

                var hasher = new PasswordHasher<AppUser>();

                var user = new AppUser
                {
                    Email = model.UserName,
                    UserName = model.UserName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PhoneNumber = "123456789",
                    PasswordHash = hasher.HashPassword(null, model.Password),
                    Password = model.Password,
                    CreatedTime = DateTime.Now,
                    NormalizedEmail = model.UserName,
                    NormalizedUserName = model.UserName,
                    LinkFB = string.Empty,
                    TelegramAPI = model.LinkTelegram,
                    UserManager = !string.IsNullOrWhiteSpace(model.UserManager) ? model.UserManager : null,
                    //NumberOfThreads1080 = model.NumberOfThreads1080,
                    //NumberOfThreads4K = model.NumberOfThreads4K

                };
                await _unitOfWork.GetRepository<AppUser>().InsertAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create account failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(true, "Create account failed");
            }
        }


        public async Task<KeyValuePair<bool, string>> DeleteAsync(string userId)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<AppUser>();
                var _repositoryUserBOT = _unitOfWork.GetRepository<UserManagerBOT>();

                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();

                // var _repositoryUserBOT = _unitOfWork.GetRepository<UserManagerBOT>();

                var userCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == userId, disableTracking: false);
                if (userCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại!");
                }
                userCurrent.DeletedTime = DateTime.Now;
                //remove usermanagerBot

                var userBots = await _repositoryUserBOT.Queryable().Where(x => x.UserId == userId).ToListAsync();
                if (userBots.Any())
                {
                    _repositoryUserBOT.Delete(userBots);
                }
                var streams = await _repositoryStream.Queryable()
                    .Where(x => x.UserId == userId && x.Status == StreamStatus.Streaming).ToListAsync();
                if (streams.Any())
                {
                    foreach (var item in streams)
                    {
                        await _streamClientService.CancelStreamAsync(userId, item.Id);
                    }
                }

                // userCurrent.UserManagerBOTs = null;
                //userCurrent.NumberOfThreads4K = 0;
                //userCurrent.UserManagerBOTs = null;
                _repository.Update(userCurrent);

                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete User failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Xóa người dùng không thành công:[{ex.Message}]");
            }
        }

        public async Task<KeyValuePair<bool, string>> DeleteUserManagerBot(int id)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<UserManagerBOT>();
                var userBotEntity = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == id, disableTracking: false);
                if (userBotEntity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Dữ liệu không tồn tại, vui lòng thử lại!");
                }
                //check stream by bot
                var anyStream = await _unitOfWork.GetRepository<StreamHistory>()
                    .Queryable().Where(x => x.UserId == userBotEntity.UserId
                    && x.ManagerBOTId == userBotEntity.ManagerBOTId
                    && x.Status == StreamStatus.Streaming).ToListAsync();
                if (anyStream != null && anyStream.Any())
                {
                    foreach (var item in anyStream)
                    {
                        await _streamClientService.CancelStreamAsync(userBotEntity.UserId, item.Id);
                    }
                    //return new KeyValuePair<bool, string>(false, "Vui lòng tắt Stream trước khi xóa BOT");
                }
                _repository.Delete(userBotEntity);
                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete UserBot failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Lỗi xóa BOT:[{ex.Message}]");
            }
        }

        public async Task<List<UserAdminInfoDto>> GetAllAsync(string userId, bool isAdmin = false, string managers = "")
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            var query = _repository.Queryable().AsNoTracking();
            if (!isAdmin)
            {
                query = query.Where(x => x.UserManager == userId || x.Id == userId);
            }
            var lstManagers = new List<string>();
            if (!string.IsNullOrWhiteSpace(managers))
            {
                lstManagers = managers.Split(',').ToList();
            }
            if (lstManagers.Any())
            {
                query = query.Where(x => lstManagers.Any(i => i == x.UserManager));
            }
            return await query.Where(x => x.DeletedTime == null)
                .Select(x => new UserAdminInfoDto
                {
                    Id = x.Id,
                    UserName = x.UserName
                }).ToListAsync();
        }

        public async Task<List<UserGroupDto>> GetAllUserOfManagerAsync(string userId)
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            var query = _repository.Queryable().AsNoTracking();


            return await _repository.Queryable().AsNoTracking()
                .Select(x => new UserGroupDto
                {
                    UserId = x.Id,
                    UserManagerId = userId,
                    UserName = x.UserName
                }).ToListAsync();


        }

        public async Task<List<UserManagerBotInfoDto>> GetUserBotAsync(string userId)
        {
            var _repository = _unitOfWork.GetRepository<UserManagerBOT>();

            return await _repository.Queryable().AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.ManagerBOT).ThenInclude(i => i.StreamHistorys)
                .Select(x => new UserManagerBotInfoDto
                {
                    Id = x.Id,
                    BotName = x.ManagerBOT.Name,
                    NumberOfThreads = x.NumberOfThreads,
                    UserName = x.User.UserName,
                    LiveType = x.LiveType,
                    BotId = x.ManagerBOTId,
                    Group = x.ManagerBOT.Group,
                    NumberOfThreadsInRun = x.ManagerBOT.StreamHistorys.Where(x => x.UserId == userId && x.Status == StreamStatus.Streaming).Count()
                }).ToListAsync();
        }

        public async Task<IPagedList<UserAdminInfoDto>> GetUserByFilterAsync(string userId, UserAdminRequestDto model, bool isAdmin = false)
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            var query = _repository.Queryable().AsNoTracking();
            if (isAdmin)
            {
                if (!string.IsNullOrWhiteSpace(model.Manager))
                {
                    query = query.Where(x => x.UserManager == model.Manager);
                }
                var lstManagers = new List<string>();
                if (!string.IsNullOrWhiteSpace(model.Managers))
                {
                    lstManagers = model.Managers.Split(',').ToList();
                }
                if (lstManagers.Any())
                {
                    query = query.Where(x => lstManagers.Any(i => i == x.UserManager));
                }
            }
            else
            {
                query = query.Where(x => x.UserManager == userId || x.Id == userId);
            }
            query = query.Where(x => x.DeletedTime == null);
            if (!string.IsNullOrEmpty(model.UserName))
            {
                query = query.Where(x => x.UserName.Contains(model.UserName) || x.Email.Contains(model.UserName));
            }

            return await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => new UserAdminInfoDto
                {
                    CreatedTime = x.CreatedTime,
                    Email = x.Email,
                    Id = x.Id,
                    LinkFB = x.LinkFB,
                    LinkTelegram = x.LinkTelegram,
                    Password = x.Password,
                    NumberOfThreads1080 = x.UserManagerBOTs.Where(i => i.ManagerBOT.LiveType == LiveType._1080).Sum(i => i.NumberOfThreads),
                    NumberOfThreads4K = x.UserManagerBOTs.Where(i => i.ManagerBOT.LiveType == LiveType._4K).Sum(i => i.NumberOfThreads),
                    NumberOfThreadsBackup = x.UserManagerBOTs.Where(i => i.ManagerBOT.LiveType == LiveType._Backup).Sum(i => i.NumberOfThreads),
                    UserName = x.UserName,
                    TelegramAPI = x.TelegramAPI,
                    NumberOfThreads4KInRun = x.StreamHistorys.Where(i => i.LiveType == LiveType._4K && i.Status == StreamStatus.Streaming).Count(),
                    NumberOfThreads1080InRun = x.StreamHistorys.Where(i => i.LiveType == LiveType._1080 && i.Status == StreamStatus.Streaming).Count(),
                    NumberOfThreadsBackupInRun = x.StreamHistorys.Where(i => i.LiveType == LiveType._Backup && i.Status == StreamStatus.Streaming).Count(),
                    TotalBOT = x.UserManagerBOTs.Count(),
                    TotalBot1080 = x.UserManagerBOTs.Where(x => x.LiveType == LiveType._1080).Count(),
                    TotalBot4k = x.UserManagerBOTs.Where(x => x.LiveType == LiveType._4K).Count(),
                    TotalBotBackup = x.UserManagerBOTs.Where(x => x.LiveType == LiveType._Backup).Count(),
                    NumberOfThreadsBackupPending = x.StreamHistorys.Where(i => i.ManagerBOTBackupId.GetValueOrDefault() > 0).Count(),
                    UpdatedTime = x.UpdatedTime,
                    UserUpdate = x.UserUpdate,
                    UserManager = x.UserManager,
                    Manager = x.Manager.UserName,
                    Roles = x.UserRoles.Select(x => x.Role.Name).ToList()
                }, include: x => x.Include(i => i.StreamHistorys).Include(i => i.UserManagerBOTs).ThenInclude(o => o.ManagerBOT),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: model.Page, pageSize: model.PageSize);
        }

        public async Task<List<UserAdminInfoDto>> GetUserManagerAsync()
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();
            var roleManager = await _roleManager.FindByNameAsync("Manager");

            var query = _repository.Queryable().AsNoTracking();

            query = query.Where(x => x.DeletedTime == null &&
            x.UserRoles.Any(i => i.RoleId == roleManager.Id));

            var data = await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => new UserAdminInfoDto
                {
                    CreatedTime = x.CreatedTime,
                    Email = x.Email,
                    Id = x.Id,
                    LinkFB = x.LinkFB,
                    LinkTelegram = x.LinkTelegram,
                    NumberOfThreads1080 = x.UserManagerBOTs.Where(i => i.ManagerBOT.LiveType == LiveType._1080).Sum(i => i.NumberOfThreads),
                    NumberOfThreads4K = x.UserManagerBOTs.Where(i => i.ManagerBOT.LiveType == LiveType._4K).Sum(i => i.NumberOfThreads),
                    UserName = x.UserName,
                    TelegramAPI = x.TelegramAPI
                }, include: x => x.Include(i => i.StreamHistorys).Include(i => i.UserManagerBOTs).ThenInclude(o => o.ManagerBOT),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: 1, pageSize: 1000);
            return data.Items.ToList();
        }
        public async Task<List<UserAdminInfoDto>> GetUserAdminAsync()
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();
            var roleManager = await _roleManager.FindByNameAsync("Admin");

            var query = _repository.Queryable().AsNoTracking();

            query = query.Where(x => x.DeletedTime == null &&
            x.UserRoles.Any(i => i.RoleId == roleManager.Id));

            var data = await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => new UserAdminInfoDto
                {
                    CreatedTime = x.CreatedTime,
                    Email = x.Email,
                    Id = x.Id,
                    LinkFB = x.LinkFB,
                    LinkTelegram = x.LinkTelegram,
                    UserName = x.UserName,
                    TelegramAPI = x.TelegramAPI
                }, include: x => x.Include(i => i.StreamHistorys).Include(i => i.UserManagerBOTs).ThenInclude(o => o.ManagerBOT),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: 1, pageSize: 1000);
            return data.Items.ToList();
        }

        public async Task<UserManagerBotDto> GetUserManagerBotAsync(string userId)
        {
            var _reposiory = _unitOfWork.GetRepository<AppUser>();

            return await _reposiory.Queryable().AsNoTracking()
                 .Where(x => x.Id == userId)
                 .Include(x => x.UserManagerBOTs).ThenInclude(o => o.ManagerBOT)
                 .Select(x => new UserManagerBotDto
                 {
                     NumberOfThreads1080 = x.UserManagerBOTs.Where(i => i.ManagerBOT.LiveType == LiveType._1080).Sum(i => i.NumberOfThreads),
                     NumberOfThreads4K = x.UserManagerBOTs.Where(i => i.ManagerBOT.LiveType == LiveType._4K).Sum(i => i.NumberOfThreads),
                     Bot1080 = x.UserManagerBOTs.Where(i => i.LiveType == LiveType._1080)
                         .Select(i => i.ManagerBOTId).FirstOrDefault(),
                     Bot4K = x.UserManagerBOTs.Where(i => i.LiveType == LiveType._4K)
                         .Select(i => i.ManagerBOTId).FirstOrDefault(),
                     UserName = x.UserName
                 }).FirstOrDefaultAsync();
        }

        public async Task<KeyValuePair<bool, string>> InsertBOTOfUserAsync(UserAddBOTAdminDto model)
        {
            try
            {
                if (model.NumberOfThreads < 1)
                {
                    return new KeyValuePair<bool, string>(false, "Số luồng không hợp lệ!");
                }
                var _repositoryBOT = _unitOfWork.GetRepository<ManagerBOT>();
                var botId = model.Bot1080.GetValueOrDefault() > 0 ? model.Bot1080 : model.Bot4k.GetValueOrDefault() > 0 ? model.Bot4k : model.BotBackup;
                if (botId == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT Không tồn tại!");
                }
                var _repository = _unitOfWork.GetRepository<UserManagerBOT>();
                var anyData = await _repository.ExistsAsync(x => x.UserId == model.UserId && x.ManagerBOTId == botId);
                if (anyData)
                {
                    return new KeyValuePair<bool, string>(false, "Dữ liệu đã tồn tại!");
                }
                var botCurrent = await _repositoryBOT.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == botId);
                if (botCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "BOT Không tồn tại!");
                }

                _repository.Insert(new UserManagerBOT
                {
                    UserId = model.UserId,
                    ManagerBOTId = botId.GetValueOrDefault(),
                    LiveType = botCurrent.LiveType.GetValueOrDefault(),
                    NumberOfThreads = model.NumberOfThreads,
                    CreatedTime = DateTime.Now,
                });
                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Inser BOT User failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Thêm BOT không thành công![{ex.Message}]");
            }
        }

        public async Task<KeyValuePair<bool, string>> ResetPassowrdAsync(ResetPasswordAdminDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<AppUser>();

                var userCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == model.UserId, disableTracking: false);
                if (userCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại!");
                }

                if (string.IsNullOrEmpty(model.Password))
                {
                    model.Password = RandomHelper.RandomString(6);
                }
                var hasher = new PasswordHasher<AppUser>();

                var passwordHash = hasher.HashPassword(null, model.Password);

                userCurrent.Password = model.Password;
                userCurrent.PasswordHash = passwordHash;
                userCurrent.UpdatedTime = DateTime.Now;
                userCurrent.UserUpdate = model.UserUpdate;

                _repository.Update(userCurrent);

                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Resetpassword failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Reset password không thành công, vui lòng thử lại!");
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateManagerAsync(string userName)
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            var roleManager = await _roleManager.FindByNameAsync("Manager");

            var userCurrent = await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.UserName == userName,
                include: x => x.Include(i => i.UserRoles),
                disableTracking: false);

            if (userCurrent != null)
            {
                var isRoleCurrent = userCurrent.UserRoles.FirstOrDefault(x => x.RoleId == roleManager.Id);
                if (isRoleCurrent != null)
                {
                    userCurrent.UserRoles.Remove(isRoleCurrent);
                }
                else
                {
                    userCurrent.UserRoles.Add(new AppUserRole
                    {
                        RoleId = roleManager.Id,
                        UserId = userCurrent.Id,
                    });
                }

                _repository.Update(userCurrent);

                await _unitOfWork.SaveChangesAsync();
            }
            return new KeyValuePair<bool, string>(true, string.Empty);
        }

        public async Task<KeyValuePair<bool, string>> UpdateAdminAsync(string userName)
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            var roleManager = await _roleManager.FindByNameAsync("Admin");

            var userCurrent = await _repository.GetFirstOrDefaultAsync(
                predicate: x => x.UserName == userName,
                include: x => x.Include(i => i.UserRoles),
                disableTracking: false);

            if (userCurrent != null)
            {
                var isRoleCurrent = userCurrent.UserRoles.FirstOrDefault(x => x.RoleId == roleManager.Id);
                if (isRoleCurrent != null)
                {
                    userCurrent.UserRoles.Remove(isRoleCurrent);
                }
                else
                {
                    userCurrent.UserRoles.Add(new AppUserRole
                    {
                        RoleId = roleManager.Id,
                        UserId = userCurrent.Id,
                    });
                }

                _repository.Update(userCurrent);

                await _unitOfWork.SaveChangesAsync();
            }
            return new KeyValuePair<bool, string>(true, string.Empty);
        }

        public async Task<KeyValuePair<bool, string>> UpdateTelegramUserAsync(UpdateTelegramDto model, bool isAdmin = false)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<AppUser>();
                var _repositoryUserBot = _unitOfWork.GetRepository<UserManagerBOT>();
                var _repositoryStream = _unitOfWork.GetRepository<StreamHistory>();
                var userCurrent = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == model.UserId, disableTracking: false);

                if (userCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Tài khoản không tồn tại!");
                }
                userCurrent.LinkTelegram = model.Telegram;

                if (isAdmin && !string.IsNullOrWhiteSpace(model.UserManager) && userCurrent.UserManager != model.UserManager)
                {

                    //stop all streams
                    var userOfBots = await _repositoryUserBot.Queryable()
                        .Where(x => x.UserId == model.UserId).ToListAsync();

                    if (userOfBots.Any())
                    {
                        var streamsCurrents = await _repositoryStream.Queryable()
                            .Where(x => x.UserId == model.UserId && x.Status != StreamStatus.Stop).ToListAsync();

                        if (streamsCurrents.Any())
                        {
                            foreach (var item in streamsCurrents)
                            {
                                item.Status = StreamStatus.Stop;
                                item.ManagerBOTId = null;
                            }
                            _repositoryStream.Update(streamsCurrents);
                            await _unitOfWork.SaveChangesAsync();

                            foreach (var item in streamsCurrents)
                            {
                                await _botContextHub.Clients.All.RenderAsync(new RenderCommand
                                {
                                    ItemId = item.Id,
                                    Command = Command.Cancel
                                });
                            }
                        }
                        _repositoryUserBot.Delete(userOfBots);
                    }
                    userCurrent.UserManager = model.UserManager;
                }
                _repository.Update(userCurrent);
                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Update Telegram failed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật không thành công!");
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateUserGroupAsync(UserGroupAddDto model)
        {
            var _repositoryUser = _unitOfWork.GetRepository<AppUser>();
            var _repositoryUserGroup = _unitOfWork.GetRepository<ManagerGroup>();

            var userGroup = await _repositoryUserGroup.GetFirstOrDefaultAsync(
                predicate: x => x.User.UserName == model.UserName && x.UserIdManager == model.UserManagerId);
            if (userGroup != null)
            {
                _repositoryUserGroup.Delete(userGroup);
            }
            else
            {
                var userCurrent = await _repositoryUser.GetFirstOrDefaultAsync(predicate: x => x.UserName == model.UserName);
                if (userCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại!");
                }

                _repositoryUserGroup.Insert(new ManagerGroup
                {
                    Note = "Manager Group",
                    UserId = userCurrent.Id,
                    UserIdManager = model.UserManagerId,
                });
            }
            await _unitOfWork.SaveChangesAsync();
            return new KeyValuePair<bool, string>(true, string.Empty);
        }

        public async Task<KeyValuePair<bool, string>> UpdateUserManagerBotAsync(UserManagerBotDto model)
        {
            try
            {
                var _repositoryUser = _unitOfWork.GetRepository<AppUser>();

                var _repositoryBot = _unitOfWork.GetRepository<ManagerBOT>();
                var _repositoryUserBot = _unitOfWork.GetRepository<UserManagerBOT>();

                var userCurrent = await _repositoryUser.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == model.UserId, disableTracking: false);

                if (userCurrent == null)
                {
                    return new KeyValuePair<bool, string>(false, "Người dùng không tồn tại!");
                }
                //userCurrent.NumberOfThreads1080 = model.NumberOfThreads1080;
                //userCurrent.NumberOfThreads4K = model.NumberOfThreads4K;


                _repositoryUser.Update(userCurrent);

                if (model.Bot1080 != null && model.Bot1080 > 0)
                {
                    var bot1080Current = await _repositoryUserBot.GetFirstOrDefaultAsync(
                        predicate: x => x.LiveType == LiveType._1080 && x.UserId == model.UserId, disableTracking: false);

                    if (bot1080Current != null)
                    {
                        bot1080Current.ManagerBOTId = model.Bot1080.GetValueOrDefault();
                        _repositoryUserBot.Update(bot1080Current);
                    }
                    else
                    {
                        _repositoryUserBot.Insert(new UserManagerBOT
                        {
                            CreatedTime = DateTime.Now,
                            LiveType = LiveType._1080,
                            ManagerBOTId = model.Bot1080.GetValueOrDefault(),
                            UserId = model.UserId
                        });
                    }
                }
                else
                {
                    var bot1080Current = await _repositoryUserBot.GetFirstOrDefaultAsync(
                        predicate: x => x.LiveType == LiveType._1080 && x.UserId == model.UserId, disableTracking: false);

                    if (bot1080Current != null)
                    {
                        _repositoryUserBot.Delete(bot1080Current);
                    }

                }
                if (model.Bot4K != null && model.Bot4K > 0)
                {
                    var bot4kCurrent = await _repositoryUserBot.GetFirstOrDefaultAsync(
                        predicate: x => x.LiveType == LiveType._4K && x.UserId == model.UserId, disableTracking: false);

                    if (bot4kCurrent != null)
                    {
                        bot4kCurrent.ManagerBOTId = model.Bot4K.GetValueOrDefault();
                        _repositoryUserBot.Update(bot4kCurrent);
                    }
                    else
                    {
                        _repositoryUserBot.Insert(new UserManagerBOT
                        {
                            CreatedTime = DateTime.Now,
                            LiveType = LiveType._4K,
                            ManagerBOTId = model.Bot4K.GetValueOrDefault(),
                            UserId = model.UserId
                        });
                    }
                }
                else
                {
                    var bot4kCurrent = await _repositoryUserBot.GetFirstOrDefaultAsync(
                        predicate: x => x.LiveType == LiveType._4K && x.UserId == model.UserId, disableTracking: false);

                    if (bot4kCurrent != null)
                    {
                        _repositoryUserBot.Delete(bot4kCurrent);
                    }

                }

                if (model.BotBackup != null && model.BotBackup > 0)
                {
                    var botBackupCurrent = await _repositoryUserBot.GetFirstOrDefaultAsync(
                        predicate: x => x.UserId == model.UserId && (x.LiveType == LiveType._Backup), disableTracking: false);

                    if (botBackupCurrent != null)
                    {
                        botBackupCurrent.ManagerBOTId = model.BotBackup.GetValueOrDefault();
                        _repositoryUserBot.Update(botBackupCurrent);
                    }
                    else
                    {
                        _repositoryUserBot.Insert(new UserManagerBOT
                        {
                            CreatedTime = DateTime.Now,
                            LiveType = LiveType._4K,
                            ManagerBOTId = model.BotBackup.GetValueOrDefault(),
                            UserId = model.UserId
                        });
                    }
                }
                else
                {
                    var botBackupCurrent = await _repositoryUserBot.GetFirstOrDefaultAsync(
                        predicate: x => x.UserId == model.UserId && (x.LiveType == LiveType._Backup), disableTracking: false);

                    if (botBackupCurrent != null)
                    {
                        _repositoryUserBot.Delete(botBackupCurrent);
                    }

                }
                await _unitOfWork.SaveChangesAsync();

                return new KeyValuePair<bool, string>(true, string.Empty);

            }
            catch (Exception ex)
            {
                _logger.LogError($"Update UserManagerBot failedd by error: [{ex}]");
                return new KeyValuePair<bool, string>(false, "Cập nhật không thành công!");
            }
        }

        public async Task<KeyValuePair<bool, string>> UpdateUserManagetBotAsync(UserManagerBotUpdateDto model)
        {
            try
            {
                var _repository = _unitOfWork.GetRepository<UserManagerBOT>();
                var entity = await _repository.GetFirstOrDefaultAsync(
                    predicate: x => x.Id == model.Id, disableTracking: false);
                if (entity == null)
                {
                    return new KeyValuePair<bool, string>(false, "Dữ liệu không tồn tại, vui lòng thử lại!");
                }
                entity.NumberOfThreads = model.NumberOfThreads;
                _repository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return new KeyValuePair<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {

                _logger.LogError($"Update UserManagerBotFailed by error:[{ex}]");
                return new KeyValuePair<bool, string>(false, $"Cập nhật không thành công:[{ex.Message}]");
            }
        }
    }
}
