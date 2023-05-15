using BaseSource.ApiIntegration.WebApi.ManagerBOT;
using BaseSource.ApiIntegration.WebApi.StreamAdmin;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.ManagerBOT;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks.Dataflow;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class BOTController : BaseAdminController
    {
        private readonly IManagerBOTApiClient _managerBOTApiClient;
        private readonly IStreamAdminApiClient _streamAdminApiClient;
        public BOTController(IManagerBOTApiClient managerBOTApiClient, IStreamAdminApiClient streamAdminApiClient)
        {
            _managerBOTApiClient = managerBOTApiClient;
            _streamAdminApiClient = streamAdminApiClient;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _managerBOTApiClient.GetBOTByFilterAsync(new ManagerBOTRequestDto
            {
                Page = 1,
                PageSize = 1000
            });
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }

            var report = await _streamAdminApiClient.GetReportStream();

            if (report == null || !report.IsSuccessed)
            {
                return NotFound();

            }

            ViewBag.Report = report.ResultObj;
            return View(result.ResultObj);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var result = await _managerBOTApiClient.GetById(id);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            return View(result.ResultObj);
        }
        [HttpPost]
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

        [HttpPost]
        [Route("/admin/bot/delete")]
        public async Task<IActionResult> Deleate(int id)
        {
            var result = await _managerBOTApiClient.DeleteAsync(id);
            return RedirectToAction("Index");
            //if (result == null || !result.IsSuccessed)
            //{
            //    return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Xóa không thành công!" });
            //}
            //return Json(new { code = ErrorCodeJson.Success });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUsed(int id)
        {
            var result = await _managerBOTApiClient.ChangeUsedAsync(id);
            if (result == null || !result.IsSuccessed)
            {
                return RedirectToAction("Index");
                // return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Cập nhật không thành công!" });
            }
            return RedirectToAction("Index");
            // return Json(new { code = ErrorCodeJson.Success });
        }

        [HttpGet]
        [Route("/admin/bot/stream")]
        public async Task<IActionResult> GetStreamByIdBot(int id)
        {
            var result = await _streamAdminApiClient.GetByBotIdAsync(id);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            return View(result.ResultObj.Items);
        }

        [HttpGet]
        [Route("/admin/bot/group")]
        public async Task<IActionResult> GroupBot(int id)
        {
            var result = await _managerBOTApiClient.GetUserBot(id);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            return View(result.ResultObj);
        }
    }
}
