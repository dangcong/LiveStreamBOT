using BaseSouce.ViewModels.Common;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.Setting;
using BaseSouce.Shared.Extensions;

namespace BaseSource.ApiIntegration.WebApi.Setting
{
    public class ConfigSettingApiClient : IConfigSettingApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ConfigSettingApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<ApiResult<ConfigSettingVm>> GetSetting()
        {
            var client = _httpClientFactory.CreateClient(SystemConstants.BackendApiClient);
            return await client.GetAsync<ApiResult<ConfigSettingVm>>("/api/setting");
        }
    }
}
