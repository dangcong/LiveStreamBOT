using BaseSource.ApiIntegration.WebApi.Stream;
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
        private readonly IStreamApiClient _streamApiClient;
        public StreamController(
            IStreamAdminApiClient streamAdminApiClient
            , IUserAdminApiClient userAdminApiClient
            ,IStreamApiClient streamApiClient
            )
        {
            _streamAdminApiClient = streamAdminApiClient;
            _userAdminApiClient = userAdminApiClient;
            _streamApiClient = streamApiClient;
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

            var report = await _streamAdminApiClient.GetReportStream(managerSelect ?? string.Empty);

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

            if (result==null)
            {
                return NotFound();
            }
          
            return View(result.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> Thumbnail(int id)
        {
            var thumbnail = await _streamAdminApiClient.GetThumbnailAsync(id);
            if (thumbnail != null)
            {
                Response.Headers.CacheControl = "private, no-store";
                return File(thumbnail, "image/jpeg");
            }

            const string placeholder = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"160\" height=\"90\" viewBox=\"0 0 160 90\"><rect width=\"160\" height=\"90\" rx=\"8\" fill=\"#eef1f5\"/><path d=\"M68 30v30l27-15z\" fill=\"#9aa4b2\"/><text x=\"80\" y=\"78\" text-anchor=\"middle\" font-family=\"Arial,sans-serif\" font-size=\"10\" fill=\"#667085\">Chưa có ảnh</text></svg>";
            Response.Headers.CacheControl = "private, no-store";
            return Content(placeholder, "image/svg+xml");
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
