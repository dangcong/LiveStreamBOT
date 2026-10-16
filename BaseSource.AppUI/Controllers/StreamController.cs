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

        private async Task<bool> LoadCreateViewDataAsync()
        {
            var packageInfo = await _userApiClient.GetPackageInfo();
            if (packageInfo == null || !packageInfo.IsSuccessed)
            {
                return false;
            }
            ViewBag.PackageInfo = packageInfo.ResultObj;

            var streamData = await _streamApiClient.GetByFilterAsync(new StreamClientRequestDto
            {
                Page = 1,
                PageSize = 500
            });
            ViewBag.StreamData = streamData?.ResultObj?.Items?.ToList() ?? new List<StreamClientInfoDto>();

            var report = await _streamApiClient.GetReport();
            ViewBag.Report = report?.ResultObj;

            var bots = await _streamApiClient.GetBotOfUserAsync();
            if (bots == null || !bots.IsSuccessed)
            {
                return false;
            }
            ViewBag.Bots = bots.ResultObj;
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!await LoadCreateViewDataAsync())
            {
                return NotFound();
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StreamCreateDto model)
        {

            if (!ModelState.IsValid)
            {
                if (!ModelState.TryGetValue(string.Empty, out var modelErrors) || modelErrors.Errors.Count == 0)
                {
                    ModelState.AddModelError(string.Empty, "Thông tin chưa hợp lệ. Vui lòng kiểm tra các thông báo dưới trường nhập rồi thử lại.");
                }

                if (!await LoadCreateViewDataAsync())
                {
                    return NotFound();
                }
                return View(model);

            }


            var result = await _streamApiClient.Create(model);
            if (result == null || !result.IsSuccessed)
            {
                if (!await LoadCreateViewDataAsync())
                {
                    return NotFound();
                }
                ModelState.AddModelError("", result?.Message ?? "Không thể tạo luồng stream!");
                return View(model);
            }

            TempData["Success"] = "Tạo luồng thành công.";
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
            if (!ModelState.IsValid)
            {
                var bots = await _streamApiClient.GetBotOfUserAsync();
                if (bots == null || !bots.IsSuccessed)
                {
                    return NotFound();
                }
                ViewBag.Bots = bots.ResultObj;
                ModelState.AddModelError(string.Empty, "Thông tin chưa hợp lệ. Vui lòng kiểm tra các thông báo dưới trường nhập rồi thử lại.");
                return View(BuildEditViewModel(id, model));
            }

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
                return View(BuildEditViewModel(id, model));
            }
            return RedirectToAction("Create");
        }

        private static StreamClientInfoDto BuildEditViewModel(int id, StreamUpdateDto model)
        {
            return new StreamClientInfoDto
            {
                Id = id,
                StreamName = model.StreamName,
                StreamKey = model.StreamKey,
                Video = model.Video,
                Audio = model.Audio,
                LiveType = model.LiveType,
                PlatformType = model.PlatformType,
                ManagerBOTId = model.ManagerBOTId,
                ManagerBotBackupId = model.ManagerBotBackupId,
                TimeBackup = model.TimeBackup,
                StartTimeLive = model.StartTimeLive,
                EndTimeLive = model.EndTimeLive,
                IsForever = model.IsForever,
                IsLiveNow = model.IsLiveNow,
                RTMP = "rtmp://x.rtmp.youtube.com/live2"
            };
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
