using BaseSouce.Services.Services.Setting;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Setting;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
     [Route("api/admin/[controller]")]
    public class SettingController : BaseAdminApiController
    {
        private readonly IConfigSettingService _configSettingService;
        public SettingController(IConfigSettingService configSettingService)
        {
            _configSettingService = configSettingService;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] ConfigSettingVm model)
        {
            var result = await _configSettingService.UpdateAsync(model);
          
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            // AddErrors(result.Value, string.Empty);
            return BadRequest(new ApiErrorResult<string>(result.Value));
        }
    }
}
