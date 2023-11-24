using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork.PageList;


namespace BaseSouce.Services.Services.StreamAdmin
{
    public interface IStreamAdminService
    {
        Task<IPagedList<StreamAdminInfoDto>> GetByFilterAsync(string userId, StreamAdminRequestDto model, bool isAdmin = false);
        Task<IPagedList<StreamAdminInfoDto>> GetByBotIdAsync(int botId);
        Task<List<StreamAdminInfoDto>> GetAllByUserIdAsync(string userId);
        Task<StreamReportAdminDto> GetReportStreamAsync(string userId, bool isAdmin,string managers);
        Task<StreamClientInfoDto> GetStreamByIdAsync(int id);
    }
}
