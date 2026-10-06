using BaseSource.ApiIntegration.WebApi;
using BaseSource.ApiIntegration.WebApi.Stream;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.Stream;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Controllers
{
    public class StreamController : BaseController
    {
        private readonly IStreamApiClient _streamApiClient;
        private readonly IUserApiClient _userApiClient;
        public StreamController(IStreamApiClient streamApiClient, IUserApiClient userApiClient)
        {
            _streamApiClient = streamApiClient;
            _userApiClient = userApiClient;
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var result = await _userApiClient.GetPackageInfo();
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            ViewBag.PackageInfo = result.ResultObj;

            var streamData = await _streamApiClient.GetByFilterAsync(new StreamClientRequestDto
            {
                Page = 1,
                PageSize = 500
            });
            ViewBag.StreamData = streamData.ResultObj.Items.ToList();

            var report = await _streamApiClient.GetReport();
            ViewBag.Report = report.ResultObj;

            var bots = await _streamApiClient.GetBotOfUserAsync();
            if (bots == null || !bots.IsSuccessed)
            {
                return NotFound();
            }
            ViewBag.Bots = bots.ResultObj;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StreamCreateDto model)
        {

            if (!ModelState.IsValid)
            {
                var errorList = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                var report = await _streamApiClient.GetReport();
                ViewBag.Report = report.ResultObj;

                var packageInfo = await _userApiClient.GetPackageInfo();
                if (packageInfo == null || !packageInfo.IsSuccessed)
                {
                    return NotFound();
                }
                ViewBag.PackageInfo = packageInfo.ResultObj;
                var bots = await _streamApiClient.GetBotOfUserAsync();
                if (bots == null || !bots.IsSuccessed)
                {
                    return NotFound();
                }
                ViewBag.Bots = bots.ResultObj;
                return View(model);

            }


            var result = await _streamApiClient.Create(model);
            if (result == null || !result.IsSuccessed)
            {
                var report = await _streamApiClient.GetReport();
                ViewBag.Report = report.ResultObj;

                var packageInfo = await _userApiClient.GetPackageInfo();
                if (packageInfo == null || !packageInfo.IsSuccessed)
                {
                    return NotFound();
                }
                ViewBag.PackageInfo = packageInfo.ResultObj;
                var bots = await _streamApiClient.GetBotOfUserAsync();
                if (bots == null || !bots.IsSuccessed)
                {
                    return NotFound();
                }
                ViewBag.Bots = bots.ResultObj;
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

        [HttpPost]
        public async Task<IActionResult> StartStream(int id)
        {
            var result = await _streamApiClient.StartStreamAsync(id);
            if (result == null || !result.IsSuccessed)
            {
                return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Cập nhật không thành công!" });
            }
            return Json(new { code = ErrorCodeJson.Success });
        }

        [HttpPost]
        public async Task<IActionResult> StopStream(int id)
        {
            var result = await _streamApiClient.StopStreamAsync(id);
            if (result == null || !result.IsSuccessed)
            {
                return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Cập nhật không thành công!" });
            }
            return Json(new { code = ErrorCodeJson.Success });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _streamApiClient.DeleteAsync(id);
            if (result == null || !result.IsSuccessed)
            {
                return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Cập nhật không thành công!" });
            }
            return Json(new { code = ErrorCodeJson.Success });
        }

        [HttpGet]
        public async Task<IActionResult> Thumbnail(int id)
        {
            var thumbnail = await _streamApiClient.GetThumbnailAsync(id);
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
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _streamApiClient.GetById(id);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            var bots = await _streamApiClient.GetBotOfUserAsync();
            if (bots == null || !bots.IsSuccessed)
            {
                return NotFound();
            }
            ViewBag.Bots = bots.ResultObj;
            return View(result.ResultObj);
        }

        [HttpGet]
        public async Task<IActionResult> Info(int id)
        {
            var bots = await _streamApiClient.GetBotOfUserAsync();
            if (bots == null || !bots.IsSuccessed)
            {
                return NotFound();
            }
            ViewBag.Bots = bots.ResultObj;

            var result = await _streamApiClient.GetById(id);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            return View(result.ResultObj);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBotBackup(StreamBackupUpdateDto model)
        {
            var result = await _streamApiClient.UpdateBotBackupAsync(model);

            return RedirectToAction("Info", new { id = model.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, StreamUpdateDto model)
        {
            var result = await _streamApiClient.UpdateAsync(id, model);
            if (result == null || !result.IsSuccessed)
            {
                var bots = await _streamApiClient.GetBotOfUserAsync();
                if (bots == null || !bots.IsSuccessed)
                {
                    return NotFound();
                }
                ViewBag.Bots = bots.ResultObj;
                ModelState.AddModelError("", result?.Message ?? "Không thể sửa luồng stream!");
                return View(model);
            }
            return RedirectToAction("Create");
        }


        [HttpPost]
        public async Task<IActionResult> ValidateLink(StreamLinkDto model)
        {
            var result = await _streamApiClient.ValidateLink(model);
            if (result == null || !result.IsSuccessed)
            {
                return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Link không hoạt động!" });
            }
            return Json(new { code = ErrorCodeJson.Success });
        }
    }
}
