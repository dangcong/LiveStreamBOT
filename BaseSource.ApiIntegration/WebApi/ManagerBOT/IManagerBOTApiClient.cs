using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ManagerBOT;


namespace BaseSource.ApiIntegration.WebApi.ManagerBOT
{
    public interface IManagerBOTApiClient
    {
        Task<ApiResult<string>> CreateBOTAsync(ManagerBotCreateDto model);
        Task<ApiResult<PagedResult<ManagerBotInfoDto>>> GetBOTByFilterAsync(ManagerBOTRequestDto model);
        Task<ApiResult<int>> GetTotalBOTInRunAsync();
        Task<ApiResult<string>> UpdateBOTAsync(int id, ManagerBotUpdateDto model);
        Task<ApiResult<string>> DeleteAsync(int id);
    }
}
