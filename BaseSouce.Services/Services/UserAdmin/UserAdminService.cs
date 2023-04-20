using BaseSouce.Data.EF;
using BaseSouce.Data.Entities;
using BaseSouce.Shared.Enums;
using BaseSouce.Shared.Helpers;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.UserAdmin;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BaseSouce.Services.Services.UserAdmin
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ILogger<UserAdminService> _logger;
        public UserAdminService(IUnitOfWork<BaseSourceDbContext> unitOfWork,
             RoleManager<AppRole> roleManager, ILogger<UserAdminService> logger)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _logger = logger;


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

        public async Task<IPagedList<UserAdminInfoDto>> GetUserByFilterAsync(UserAdminRequestDto model)
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();

            var query = _repository.Queryable().AsNoTracking();
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
                    NumberOfThreads1080 = x.NumberOfThreads1080,
                    NumberOfThreads4K = x.NumberOfThreads4K,
                    UserName = x.UserName,
                    TelegramAPI = x.TelegramAPI,
                    NumberOfThreads4KInRun = x.StreamHistorys.Where(i => i.LiveType == LiveType._4K && i.Status == StreamStatus.Streaming).Count(),
                    NumberOfThreads1080InRun = x.StreamHistorys.Where(i => i.LiveType == LiveType._1080 && i.Status == StreamStatus.Streaming).Count(),
                }, include: x => x.Include(i => i.StreamHistorys),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: model.Page, pageSize: model.PageSize);
        }

        public async Task<List<UserAdminInfoDto>> GetUserManagerAsync()
        {
            var _repository = _unitOfWork.GetRepository<AppUser>();
            var roleManager = await _roleManager.FindByNameAsync("Manager");

            var query = _repository.Queryable().AsNoTracking();

            query = query.Where(x => x.UserRoles.Any(i => i.RoleId == roleManager.Id));

            var data = await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => new UserAdminInfoDto
                {
                    CreatedTime = x.CreatedTime,
                    Email = x.Email,
                    Id = x.Id,
                    LinkFB = x.LinkFB,
                    LinkTelegram = x.LinkTelegram,
                    NumberOfThreads1080 = x.NumberOfThreads1080,
                    NumberOfThreads4K = x.NumberOfThreads4K,
                    UserName = x.UserName,
                    TelegramAPI = x.TelegramAPI
                }, include: x => x.Include(i => i.StreamHistorys),
                orderBy: x => x.OrderByDescending(i => i.CreatedTime),
                pageIndex: 1, pageSize: 1000);
            return data.Items.ToList();
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
    }
}
