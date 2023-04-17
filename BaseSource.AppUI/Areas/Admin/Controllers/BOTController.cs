using BaseSource.ApiIntegration.WebApi.ManagerBOT;
using BaseSource.ViewModels.ManagerBOT;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class BOTController : BaseAdminController
    {
        private readonly IManagerBOTApiClient _managerBOTApiClient;
        public BOTController(IManagerBOTApiClient managerBOTApiClient)
        {
            _managerBOTApiClient = managerBOTApiClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            return View();
        }
        public async Task<IActionResult> Edit(ManagerBotUpdateDto model)
        {
            var result = await _managerBOTApiClient.UpdateBOTAsync(model.Id, model);
            if (result == null || !result.IsSuccessed)
            {
                ModelState.AddModelError("", result?.Message ?? "Cập nhật không thành công!");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Deleate(int id)
        {
            var result = await _managerBOTApiClient.DeleteAsync(id);
            if (result == null || !result.IsSuccessed)
            {
                return Json(new { });
            }
            return Json(new { });
        }
    }
}
