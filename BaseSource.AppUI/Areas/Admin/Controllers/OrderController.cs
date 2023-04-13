using BaseSource.ApiIntegration.WebApi.OrderAdmin;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Areas.Admin.Controllers
{
    public class OrderController : BaseAdminController
    {
        private readonly IOrderAdminApiClient _orderAdminApiClient;
        public OrderController(IOrderAdminApiClient orderAdminApiClient)
        {
            _orderAdminApiClient = orderAdminApiClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Info(int id)
        {
            var result = await _orderAdminApiClient.GetById(id);
            if (result == null || !result.IsSuccessed)
            {
                return NotFound();
            }
            return View(result.ResultObj);
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

                var result = await _orderAdminApiClient.GetAllAsync(new ViewModels.Order.OrderRequestDto
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
                return Json(new { countData = 0, datas = new List<OrderInfoDto>() });
            }
        }



        [HttpPost]
        public async Task<IActionResult> Accept(int id)
        {
            var result = await _orderAdminApiClient.Accept(id);
            if (result == null || !result.IsSuccessed)
            {
                return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Cập nhật không thành công!" });
            }
            return Json(new { code = ErrorCodeJson.Success });
        }
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _orderAdminApiClient.Cancel(id);
            if (result == null || !result.IsSuccessed)
            {
                return Json(new { code = ErrorCodeJson.ErrorMess, err = result?.Message ?? "Cập nhật không thành công!" });
            }
            return Json(new { code = ErrorCodeJson.Success });
        }
    }
}
