using BaseSouce.ViewModels.Common;
using BaseSouce.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.WebApi
{
    public interface IUserApiClient
    {
        Task<ApiResult<string>> Register(RegisterRequestVm model);
        Task<ApiResult<string>> Authenticate(LoginRequestVm model);
        Task<ApiResult<string>> ConfirmEmail(ConfirmEmailVm model);
        Task<ApiResult<string>> ForgotPassword(ForgotPasswordVm model);
        Task<ApiResult<string>> ResetPassword(ResetPasswordVm model);
        Task<ApiResult<UserInfoResponse>> GetUserInfo();
        //Task<ApiResult<string>> EditProfile(EditProfileVm model);
        Task<ApiResult<string>> ChangePassword(ChangePasswordVm model);

        Task<ApiResult<ProfifleInfoDto>> GetProfileInfo();
        Task<ApiResult<PackageLiveUserDto>> GetPackageInfo();
        Task<ApiResult<string>> Update(UserUpdateDto model);

        //Task<ApiResult<string>> AuthenticateExternalAsync(UserClaimRequest model);
        //Task<ApiResult<PagedResult<WalletTransactionVm>>> GetWalletTransactions(GetWalletTransactionRequestVm model);
        //Task<ApiResult<WalletTransactionVm>> GetWalletTransactionInfo(int id);
    }
}
