using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseSouce.Shared.Enums;
using BaseSouce.ViewModels.Common;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.ServicePackage;

namespace BaseSource.ApiIntegration.WebApi.SerivePackage
{
    public interface IServicePackageApiClient
    {
        Task<ApiResult<List<ServicePackDto>>> GetServicePackByTypeAsync(ServicePackType type);
        Task<ApiResult<string>> CreateAsync(ServicePackCreateDto model);
        Task<ApiResult<string>> UpdateAsync(int id, ServicePackUpdateDto model);
        Task<ApiResult<string>> DeleteAsync(int id);
        Task<ApiResult<ServicePackDto>> GetByIdAsync(int id);
        Task<ApiResult<InvoiceCodeInfoDto>> GetInvoiceInfoAsync(string live);
    }
}
