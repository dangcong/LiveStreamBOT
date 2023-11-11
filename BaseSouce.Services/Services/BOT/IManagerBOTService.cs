using BaseSouce.Shared.Enums;
using BaseSource.ViewModels.ManagerBOT;
using EFCore.UnitOfWork.PageList;

namespace BaseSouce.Services.Services.BOT
{
    public interface IManagerBOTService
    {
        Task<KeyValuePair<bool, string>> CreateBOTAsync(ManagerBotCreateDto model);
        Task<KeyValuePair<bool, string>> UpdateBOTAsync(int id, ManagerBotUpdateDto model, bool isAdmin = false);

        Task<KeyValuePair<bool, string>> DeleteAsync(int id);
        Task<KeyValuePair<bool, string>> UpdateStatusAsync(int id, ManagerBOTStatus Status);
        Task<IPagedList<ManagerBotInfoDto>> GetBOTByFilterAsync(string userId, ManagerBOTRequestDto model, bool isAdmin = false);
        Task<int> GetTotalBOTInRunAsync();

        Task<KeyValuePair<bool, string>> ChangeUsedAsync(int id);
        Task<ManagerBotInfoDto> GetBOTByIdAsync(int id);
        Task<List<UserBotDto>> GetUserConnectBot(int id);
        Task<KeyValuePair<bool, string>> DisconnectBotAsync(string connectionId);
    }
}
