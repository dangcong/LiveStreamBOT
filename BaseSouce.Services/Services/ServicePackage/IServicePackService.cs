using BaseSouce.Shared.Enums;
using BaseSource.ViewModels.ServicePackage;

namespace BaseSouce.Services.ServicePackage
{
    public interface IServicePackService
    {
        Task<List<ServicePackDto>> GetServicePackByTypeAsync(ServicePackType type);
        Task<KeyValuePair<bool, string>> CreateAsync(ServicePackCreateDto model);
        Task<KeyValuePair<bool, string>> UpdateAsync(int id, ServicePackUpdateDto model);
        Task<KeyValuePair<bool, string>> DeleteAsync(int id);
        Task<ServicePackDto> GetByIdAsync(int id);
        Task<List<ServicePackDto>> GetServicePackAllAsync();
        Task<InvoiceCodeInfoDto> GetInvoiceInfoAsync(string userId,string live);
    }
}
