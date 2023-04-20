using BaseSouce.Services.Services.OrderAdmin;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Order;
using EFCore.UnitOfWork.PageList;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
    [Route("/api/admin/[controller]")]
    public class OrderController : BaseAdminApiController
    {
        private readonly IOrderAdminService _orderAdminService;
        public OrderController(IOrderAdminService orderAdminService)
        {
            _orderAdminService = orderAdminService;
        }

        [HttpGet]
        [Route("/api/admin/orders")]
        public async Task<IActionResult> GetAll([FromQuery] OrderRequestDto model)
        {
            var result = await _orderAdminService.GetAllOrderByFillerAsync(model);
            return Ok(new ApiSuccessResult<IPagedList<OrderInfoDto>>(result));
        }

        [HttpGet]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _orderAdminService.GetByIdAsync(id);
            if (result == null)
            {
                return Ok(new ApiErrorResult<string>("Not found!"));
            }
            return Ok(new ApiSuccessResult<OrderInfoDto>(result));
        }


        [HttpPost]
        [Route("accept/{id:int:min(1)}")]
        public async Task<IActionResult> Accept(int id)
        {
            var result = await _orderAdminService.AcceptAsync(id);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            // AddErrors(result.Value, string.Empty);
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpPost]
        [Route("cancel/{id:int:min(1)}")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _orderAdminService.CancelAsync(id);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            // AddErrors(result.Value, string.Empty);
            return Ok(new ApiErrorResult<string>(result.Value));
        }

        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetOrderByUserId(string userId)
        {
            var result = await _orderAdminService.GetOrderByUserIdAsync(userId);
            return Ok(new ApiSuccessResult<object>(result));
        }
    }
}
