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

        [HttpPost]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) / pageSize : 0;
                int recordsTotal = 0;
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var result = await _streamApiClient.GetByFilterAsync(new StreamClientRequestDto
                {
                    Page = skip + 1,
                    PageSize = pageSize,
                });

                recordsTotal = result.ResultObj.TotalCount;


                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = result.ResultObj.Items };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return Json(new { countData = 0, datas = new List<StreamClientInfoDto>() });
            }
        }
    }
}
