using BaseSource.ViewModels.UserAdmin;
using EFCore.UnitOfWork.PageList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Services.Services.UserAdmin
{
    public interface IUserAdminService
    {
        Task<IPagedList<UserAdminInfoDto>> GetUserByFilterAsync(UserAdminRequestDto model);
        Task<KeyValuePair<bool, string>> UpdateManagerAsync(string userName);
        Task<List<UserAdminInfoDto>> GetUserManagerAsync();
        Task<KeyValuePair<bool, string>> UpdateUserGroupAsync(UserGroupAddDto model);
        Task<List<UserGroupDto>> GetAllUserOfManagerAsync(string userId);
        Task<KeyValuePair<bool, string>> ResetPassowrdAsync(ResetPasswordAdminDto model);

    }
}
