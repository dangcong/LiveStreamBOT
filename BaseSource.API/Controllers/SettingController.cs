using BaseSouce.Services.Services.Setting;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Setting;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.Controllers
{
    public class SettingController : BaseApiController
    {
        private readonly IConfigSettingService _configSettingService;
        public SettingController(IConfigSettingService configSettingService)
        {
            _configSettingService = configSettingService;
        }
        [HttpGet]
        [Route("/api/setting")]
        public async Task<IActionResult>  GetSetting()
        {
            var result =await _configSettingService.GetSettingAsync();
            return Ok(new ApiSuccessResult<ConfigSettingVm>(result));
        }

        
    }
}
