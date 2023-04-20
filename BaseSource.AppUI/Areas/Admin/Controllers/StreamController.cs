using BaseSource.ApiIntegration.WebApi.StreamAdmin;
using BaseSource.ViewModels.Stream;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class StreamController : BaseAdminController
    {
        private readonly IStreamAdminApiClient _streamAdminApiClient;
        public StreamController(IStreamAdminApiClient streamAdminApiClient)
        {
            _streamAdminApiClient = streamAdminApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _streamAdminApiClient.GetByFilterAsync(new StreamAdminRequestDto
            {
                Page = 1,
                PageSize = 2000
            });
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            return View(result.ResultObj.Items);
        }


        [HttpGet]
        [Route("/admin/stream/user")]
        public async Task<IActionResult> GetStreamByUser(string userId)
        {
            var result = await _streamAdminApiClient.GetAllByUserIdAsync(userId);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            return View(result.ResultObj);
        }
    }
}
