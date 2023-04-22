using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSource.ApiIntegration.WebApi.Setting
{
    public interface IConfigSettingApiClient
    {
        Task<ApiResult<ConfigSettingVm>> GetSetting();
        Task<ApiResult<string>> Update(ConfigSettingVm model); 
    }
}
