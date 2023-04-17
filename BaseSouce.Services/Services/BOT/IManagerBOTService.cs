using AutoMapper.Configuration.Conventions;
using BaseSouce.Shared.Enums;
using BaseSouce.SharedSignalrData.BotClient;
using BaseSource.ViewModels.ManagerBOT;
using EFCore.UnitOfWork.PageList;

namespace BaseSouce.Services.Services.BOT
{
    public interface IManagerBOTService
    {
        Task<KeyValuePair<bool, string>> CreateBOTAsync(ManagerBotCreateDto model);
        Task<KeyValuePair<bool, string>> UpdateBOTAsync(int id, ManagerBotUpdateDto model);
        
        Task<KeyValuePair<bool, string>> DeleteAsync(int id);
        Task<KeyValuePair<bool, string>> UpdateStatusAsync(int id, ManagerBOTStatus Status);
        Task<IPagedList<ManagerBotInfoDto>> GetBOTByFilterAsync(ManagerBOTRequestDto model);
        Task<int> GetTotalBOTInRunAsync();
    }
}
