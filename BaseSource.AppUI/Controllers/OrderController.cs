using BaseSource.ApiIntegration.WebApi.Order;
using BaseSource.Shared.Constants;
using BaseSource.ViewModels.Order;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.AppUI.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderApiClient _orderApiClient;
        public OrderController(IOrderApiClient orderApiClient)
        {
            _orderApiClient = orderApiClient;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Create(OrderCreateDto model)
        {
            var result = await _orderApiClient.CreateAsync(model);
            if (result.IsSuccessed)
            {
                return Json(new { code = ErrorCodeJson.Success });
            }
            return Json(new { code = ErrorCodeJson.ErrorMess, err = result.Message });
        }
    }
}
