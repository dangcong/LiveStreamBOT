using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ManagerBOT;
using BaseSource.ViewModels.Stream;
namespace BaseSource.ApiIntegration.WebApi.Stream
{
    public interface IStreamApiClient
    {
        Task<ApiResult<string>> Create(StreamCreateDto model);
        Task<ApiResult<string>> UpdateAsync(int id, StreamUpdateDto model);
        Task<ApiResult<string>> DeleteAsync(int id);
        Task<ApiResult<string>> StartStreamAsync(int id);
        Task<ApiResult<string>> StopStreamAsync(int id);
        Task<ApiResult<PagedResult<StreamClientInfoDto>>> GetByFilterAsync(StreamClientRequestDto model);
        Task<ApiResult<StreamClientInfoDto>> GetById(int id);
        Task<ApiResult<StreamReportDto>> GetReport();
        Task<ApiResult<string>> ValidateLink(StreamLinkDto model);
        Task<ApiResult<List<UserBotDto>>> GetBotOfUserAsync();
        
    }
}
