using BaseSource.ApiIntegration.WebApi.ManagerBOT;
using BaseSource.ApiIntegration.WebApi.StreamAdmin;
using BaseSource.ApiIntegration.WebApi.UserAdmin;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.ManagerBOT;
using BaseSource.ViewModels.UserAdmin;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        private readonly IUserAdminApiClient _userAdminApiClient;
        private readonly IManagerBOTApiClient _managerBOTApiClient;
        private readonly IStreamAdminApiClient _streamAdminApiClient;
        public UserController(IUserAdminApiClient userAdminApiClient, IManagerBOTApiClient managerBOTApiClient
            , IStreamAdminApiClient streamAdminApiClient)
        {
            _userAdminApiClient = userAdminApiClient;
            _managerBOTApiClient = managerBOTApiClient;
            _streamAdminApiClient = streamAdminApiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _userAdminApiClient.GetUserByFilter(new UserAdminRequestDto
            {
                Page = 1,
                PageSize = 800,
                //UserName = searchValue
            });
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

                var result = await _userAdminApiClient.GetUserByFilter(new UserAdminRequestDto
                {
                    Page = skip + 1,
                    PageSize = pageSize,
                    UserName = searchValue
                });

                recordsTotal = result.ResultObj.TotalCount;


                var jsonData = new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = result.ResultObj.Items };
                return Ok(jsonData);
            }
            catch (Exception ex)
            {
                return Json(new { countData = 0, datas = new List<UserAdminInfoDto>() });
            }
        }

        public async Task<IActionResult> Manager()
        {
            var result = await _userAdminApiClient.GetAllManager();
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            return View(result.ResultObj);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoleManager(string userName)
        {
            var result = await _userAdminApiClient.UpdateRoleManager(userName);
            if (result == null || !result.IsSuccessed)
            {
                return RedirectToAction("Manager");
                //return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Cập nhật không thành công!" });
            }
            return RedirectToAction("Manager");
            // return Json(new { code = ErrorCodeJson.Success });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordAdminDto model)
        {

            var result = await _userAdminApiClient.ResetPassword(model);
            if (result == null || !result.IsSuccessed)
            {
                return RedirectToAction("Index");
                //return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Cập nhật không thành công!" });
            }
            //return Json(new { code = ErrorCodeJson.Success, value=result.ResultObj });
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> ManagerBot(string userId, string username)
        {
            var result = await _userAdminApiClient.GetUserManagerBot(userId);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }

            var bots = await _managerBOTApiClient.GetBOTByFilterAsync(new ManagerBOTRequestDto
            {
                Page = 1,
                PageSize = 800
            });
            ViewBag.UserName = username;
            ViewBag.UserId = userId;
            ViewBag.Bots = bots.ResultObj.Items.ToList();
            return View(result.ResultObj);
        }

        [HttpPost]
        public async Task<IActionResult> AddBOT(UserAddBOTAdminDto model)
        {
            if (model.Bot1080 == null && model.Bot4k == null)
            {
                return RedirectToAction("ManagerBot", new { userId = model.UserId, username = model.UserName });
            }
            if (model.TypeBot.GetValueOrDefault() < 1)
            {
                return RedirectToAction("ManagerBot", new { userId = model.UserId, username = model.UserName });
            }
            else
            {
                switch (model.TypeBot)
                {
                    case 1:
                        model.Bot4k = 0;
                        break;
                    case 2:
                        model.Bot1080 = 0;
                        break;
                    default:
                        break;
                }
            }
            var result = await _userAdminApiClient.InserBOTUser(model);
            return RedirectToAction("ManagerBot", new { userId = model.UserId, username = model.UserName });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBOT(int id, string userId, string username)
        {
            var result = await _userAdminApiClient.DeleteUserManagerBot(id);
            return RedirectToAction("ManagerBot", new { userId = userId, username = username });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBOtUser(UserManagerBotUpdateDto model)
        {
            var result = await _userAdminApiClient.UpdateUserManagetBotAsync(model);
            return RedirectToAction("ManagerBot", new { userId = model.UserId, username = model.UserName });
        }

        [HttpPost]
        public async Task<IActionResult> ManagerBot(UserManagerBotDto model)
        {
            if (model.Bot1080 == null && model.NumberOfThreads1080 > 0)
            {
                ModelState.AddModelError("", "Chưa chọn BOT 1080");
                return View(model);
            }
            if (model.Bot4K == null && model.NumberOfThreads4K > 0)
            {
                ModelState.AddModelError("", "Chưa chọn BOT 4k");
                return View(model);
            }

            var result = await _userAdminApiClient.UpdateUserManagerBot(model);
            if (result == null || !result.IsSuccessed)
            {
                var bots = await _managerBOTApiClient.GetBOTByFilterAsync(new ManagerBOTRequestDto
                {
                    Page = 1,
                    PageSize = 1000
                });

                ViewBag.Bots = bots.ResultObj.Items.ToList();
                ModelState.AddModelError("", result?.ResultObj ?? "Cập nhật không thành công!");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            var result = await _userAdminApiClient.Delete(userId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var managers = await _userAdminApiClient.GetAllManager();
            if (managers == null || !managers.IsSuccessed)
            {
                return NotFound();

            }

            ViewBag.Managers = managers.ResultObj;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateAdminDto model)
        {
            if (!User.IsInRole("Admin"))
            {
                model.UserManager = UserId;
            }
            var result = await _userAdminApiClient.Create(model);
            if (result == null || !result.IsSuccessed)
            {
                var managers = await _userAdminApiClient.GetAllManager();
                if (managers == null || !managers.IsSuccessed)
                {
                    return NotFound();

                }

                ViewBag.Managers = managers.ResultObj;
                ModelState.AddModelError("", result?.Message ?? "Tạo mới tài khoản không thành công");
                return View(model);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTelegram(UpdateTelegramDto model)
        {
            var result = await _userAdminApiClient.UpdateTelegram(model);
            if (result == null || !result.IsSuccessed)
            {

                // ModelState.AddModelError("", result?.ResultObj ?? "Tạo mới tài khoản không thành công");
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


    }
}
