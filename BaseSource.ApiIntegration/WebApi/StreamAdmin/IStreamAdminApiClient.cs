using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Stream;

namespace BaseSource.ApiIntegration.WebApi.StreamAdmin
{
    public interface IStreamAdminApiClient
    {
       Task<ApiResult<PagedResult<StreamAdminInfoDto>>> GetByFilterAsync(StreamAdminRequestDto model);
        Task<ApiResult<PagedResult<StreamAdminInfoDto>>> GetByBotIdAsync(int botId);
        Task<ApiResult<List<StreamAdminInfoDto>>> GetAllByUserIdAsync(string userId);
        Task<ApiResult<StreamReportAdminDto>> GetReportStream();
        Task<ApiResult<StreamClientInfoDto>> GetById(int id);
        
    }
}
