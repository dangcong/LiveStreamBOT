using BaseSource.ApiIntegration.WebApi.StreamAdmin;
using BaseSource.ApiIntegration.WebApi.UserAdmin;
using BaseSource.ViewModels.Stream;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class StreamController : BaseAdminController
    {
        private readonly IStreamAdminApiClient _streamAdminApiClient;
        private readonly IUserAdminApiClient _userAdminApiClient;
        public StreamController(
            IStreamAdminApiClient streamAdminApiClient
            , IUserAdminApiClient userAdminApiClient
            )
        {
            _streamAdminApiClient = streamAdminApiClient;
            _userAdminApiClient = userAdminApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var managerChooses = new List<string>();
            string managerSelect = HttpContext.Session.GetString("Manangers");
            if (!string.IsNullOrWhiteSpace(managerSelect))
            {
                managerChooses = managerSelect.Split(",").ToList();
            }

            ViewBag.ManagerChooses = managerChooses;

            var result = await _streamAdminApiClient.GetByFilterAsync(new StreamAdminRequestDto
            {
                Page = 1,
                PageSize = 800,
                Managers = User.IsInRole("Admin") ? managerSelect : string.Empty
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
            var managers = await _userAdminApiClient.GetAllManager();
            if (managers == null || !managers.IsSuccessed)
            {
                return NotFound();

            }

            ViewBag.Managers = managers.ResultObj;

            ViewBag.Report = report.ResultObj;

            return View(result.ResultObj.Items);
        }

        [HttpGet]
        public async Task<IActionResult> Info(int id)
        {
            var result = await _streamAdminApiClient.GetById(id);
            return View(result.ResultObj);
        }
        [HttpGet]
        [Route("/admin/stream/user")]
        public async Task<IActionResult> GetStreamByUser(string userId, string username)
        {
            var result = await _streamAdminApiClient.GetAllByUserIdAsync(userId);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            ViewBag.UserName = username;
            return View(result.ResultObj);
        }

    }
}
