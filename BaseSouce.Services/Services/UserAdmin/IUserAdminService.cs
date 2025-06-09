using BaseSource.ViewModels.UserAdmin;
using EFCore.UnitOfWork.PageList;

namespace BaseSouce.Services.Services.UserAdmin
{
    public interface IUserAdminService
    {
        Task<IPagedList<UserAdminInfoDto>> GetUserByFilterAsync(string userId, UserAdminRequestDto model, bool isAdmin = false);
        Task<KeyValuePair<bool, string>> UpdateManagerAsync(string userName);
        Task<KeyValuePair<bool, string>> UpdateAdminAsync(string userName);
        Task<List<UserAdminInfoDto>> GetUserManagerAsync();
        Task<List<UserAdminInfoDto>> GetUserAdminAsync();
        Task<KeyValuePair<bool, string>> UpdateUserGroupAsync(UserGroupAddDto model);
        Task<List<UserGroupDto>> GetAllUserOfManagerAsync(string userId);
        Task<KeyValuePair<bool, string>> ResetPassowrdAsync(ResetPasswordAdminDto model);
        Task<UserManagerBotDto> GetUserManagerBotAsync(string userId);
        Task<KeyValuePair<bool, string>> UpdateUserManagerBotAsync(UserManagerBotDto model);
        Task<KeyValuePair<bool, string>> DeleteAsync(string userId);
        Task<KeyValuePair<bool, string>> CreateUserAsync(UserCreateAdminDto model);

        Task<List<UserManagerBotInfoDto>> GetUserBotAsync(string userId);
        Task<KeyValuePair<bool, string>> DeleteUserManagerBot(int id);
        Task<KeyValuePair<bool, string>> UpdateUserManagetBotAsync(UserManagerBotUpdateDto model);
        Task<KeyValuePair<bool, string>> InsertBOTOfUserAsync(UserAddBOTAdminDto model);
        Task<KeyValuePair<bool, string>> UpdateBotChatTargetUserAsync(UpdateBotChatTargetDto model, bool isAdmin = false);
        Task<List<UserAdminInfoDto>> GetAllAsync(string userId, bool isAdmin = false, string managers = "");
        Task<KeyValuePair<bool, string>> AddUserToBotAsync(UserAddBotAdminDto model);

    }
}
