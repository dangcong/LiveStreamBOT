using BaseSource.ApiIntegration.WebApi.Setting;
using BaseSource.ViewModels.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class SettingController : BaseAdminController
    {
        private readonly IConfigSettingApiClient _configSettingApiClient;
        public SettingController(IConfigSettingApiClient configSettingApiClient)
        {
            _configSettingApiClient = configSettingApiClient;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit()
        {
            var result = await _configSettingApiClient.GetSetting();

            return View(result.ResultObj);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ConfigSettingVm model)
        {
            var result = await _configSettingApiClient.Update(model);
            if (result == null || !result.IsSuccessed)
            {
                ModelState.AddModelError("", result?.Message ?? "Cập nhật không thành công!");
                return View(model);
            }
            return RedirectToAction("Edit");
        }
    }
}
