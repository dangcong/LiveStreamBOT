using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork.PageList;


namespace BaseSouce.Services.Services.StreamAdmin
{
    public interface IStreamAdminService
    {
        Task<IPagedList<StreamAdminInfoDto>> GetByFilterAsync(StreamAdminRequestDto model);
    }
}
