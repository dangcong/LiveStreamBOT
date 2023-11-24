using BaseSouce.SharedSignalrData.Classes;
using BaseSource.Shared.Helpers;
using BaseSource.ViewModels.ManagerBOT;
using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork.PageList;

namespace BaseSouce.Services.Services.Stream
{
    public interface IStreamClientService
    {
        Task<BoolStringKeyValuePair> CreateAsync(string userId, StreamCreateDto model);
        Task<KeyValuePair<bool, string>> UpdateAsync(string userId, int id, StreamUpdateDto model);
        Task<KeyValuePair<bool, string>> DeleteAsync(string userId, int id);
        Task<KeyValuePair<bool, string>> StartStreamAsync(string userId, int id);
        Task<IPagedList<StreamClientInfoDto>> GetByFilterAsync(string userId, StreamClientRequestDto model);
        Task<KeyValuePair<bool, string>> UpdateStreamAsync(RenderMessage model);
        Task<KeyValuePair<bool, string>> CancelStreamAsync(string userId, int id);
        Task<StreamClientInfoDto> GetStreamByIdAsync(string userId, int id);
        Task<StreamReportDto> GetStreamReportAsync(string userId);
        Task<KeyValuePair<bool, string>> ValidateLinkAsync(string link);
        Task<List<UserBotDto>> GetBotOfUserAsync(string userId);
        Task<KeyValuePair<bool,string>> UpdateBotBackupAsync(string userId, StreamBackupUpdateDto model);
        Task CheckLiveBackupAsync();

    }
}
