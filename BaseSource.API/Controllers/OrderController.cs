using BaseSouce.Services.Services.Order;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.Order;
using EFCore.UnitOfWork.PageList;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        [Route("/api/orders")]
        public async Task<IActionResult> GetAll([FromQuery] OrderRequestDto model)
        {
            var result = await _orderService.GetOrderByFillerAsync(UserId, model);
            return Ok(new ApiSuccessResult<IPagedList<OrderInfoDto>>(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto model)
        {
            var result = await _orderService.CreateAsync(UserId, model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            //AddErrors(result.Value, string.Empty);
            return Ok(new ApiErrorResult<string>(result.Value));
        }
    }
}
