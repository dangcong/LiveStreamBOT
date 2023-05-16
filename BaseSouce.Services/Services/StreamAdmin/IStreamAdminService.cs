using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork.PageList;


namespace BaseSouce.Services.Services.StreamAdmin
{
    public interface IStreamAdminService
    {
        Task<IPagedList<StreamAdminInfoDto>> GetByFilterAsync(StreamAdminRequestDto model);
        Task<IPagedList<StreamAdminInfoDto>> GetByBotIdAsync(int botId);
        Task<List<StreamAdminInfoDto>> GetAllByUserIdAsync(string userId);
        Task<StreamReportAdminDto> GetReportStreamAsync();
        Task<StreamClientInfoDto> GetStreamByIdAsync(int id);
    }
}
