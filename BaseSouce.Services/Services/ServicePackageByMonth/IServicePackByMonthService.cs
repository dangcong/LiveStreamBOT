using BaseSource.ViewModels.ServicePackage;

namespace BaseSouce.Services.Services.ServicePackageByMonth
{
    public interface IServicePackByMonthService
    {
        Task<List<ServicePackByMonthDto>> GetByServiceAsync(int serviceId);
        Task<ServicePackByMonthDto> GetByIdAsync(int id);
        Task<List<ServicePackByMonthDto>> GetByNameAsync(string name);
        Task<KeyValuePair<bool, string>> CreateAsync(ServicePackByMonthCreateDto model);
        Task<KeyValuePair<bool, string>> UpdateAsync(int id, ServicePackByMonthUpdateDto model);
        Task<KeyValuePair<bool, string>> DeleteAsync(int id);
    }
}
