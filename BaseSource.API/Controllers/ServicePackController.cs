using BaseSouce.Services.ServicePackage;
using BaseSouce.Shared.Enums;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ServicePackage;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.Controllers
{
    [Route("api/[controller]")]
    public class ServicePackController : BaseApiController
    {
        private readonly IServicePackService _servicePackService;
        public ServicePackController(IServicePackService servicePackService)
        {
            _servicePackService=servicePackService;
        }

        [HttpGet]
        [Route("type/{type:int:min(1)}")]
        public async Task<IActionResult>  GetServicePackByType(ServicePackType type)
        {
            var result = await _servicePackService.GetServicePackByTypeAsync(type);
            return Ok(new ApiSuccessResult<List<ServicePackDto>>(result));
        }

        [HttpGet]
        [Route("invoice")]
        public async Task<IActionResult> GetInvoiceInfo(string live)
        {
            var result= await _servicePackService.GetInvoiceInfoAsync(UserId, live);
            return Ok(new ApiSuccessResult<InvoiceCodeInfoDto>(result));
        }


    }
}
