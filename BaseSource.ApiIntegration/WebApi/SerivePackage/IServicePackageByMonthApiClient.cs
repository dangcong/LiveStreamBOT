using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ServicePackage;

namespace BaseSource.ApiIntegration.WebApi.SerivePackage
{
    public interface IServicePackageByMonthApiClient
    {
        Task<ApiResult<List<ServicePackByMonthDto>>> GetServicePackByTypeAsync(int id);
        Task<ApiResult<List<ServicePackByMonthDto>>> GetServicePackByNameAsync(string name);
        Task<ApiResult<ServicePackByMonthDto>> GetByIdAsync(int id);
        Task<ApiResult<string>> CreateAsync(ServicePackByMonthCreateDto model);
        Task<ApiResult<string>> UpdateAsync(ServicePackByMonthDto model);
        Task<ApiResult<string>> DeleteAsync(int id);
    }
}
