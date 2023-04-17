using BaseSource.ApiIntegration.WebApi.Stream;
using BaseSource.ViewModels.Stream;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Controllers
{
    public class StreamController : BaseController
    {
        private readonly IStreamApiClient _streamApiClient;
        public StreamController(IStreamApiClient streamApiClient)
        {
            _streamApiClient = streamApiClient;
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StreamCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);

            }

            var result = await _streamApiClient.Create(model);
            if (result == null || !result.IsSuccessed)
            {
                ModelState.AddModelError("", result?.Message ?? "Không thể tạo luồng stream!");
                return View(model);
            }
            return RedirectToAction("Create");
        }
    }
}
