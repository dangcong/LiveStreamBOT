using BaseSouce.Services.Services.ServicePackageByMonth;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ServicePackage;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.Controllers
{
    [Route("api/[controller]")]
    public class ServicePackByMonthController : BaseApiController
    {
        private readonly IServicePackByMonthService _servicePackService;
        public ServicePackByMonthController(IServicePackByMonthService servicePackService)
        {
            _servicePackService = servicePackService;
        }

        [HttpGet]
        [Route("type/{id:int:min(1)}")]
        public async Task<IActionResult> GetServicePackByType(int id)
        {
            var result = await _servicePackService.GetByServiceAsync(id);
            return Ok(new ApiSuccessResult<List<ServicePackByMonthDto>>(result));
        }

        [HttpGet]
        [Route("name/{name}")]
        public async Task<IActionResult> GetServicePackByName(string name)
        {
            var result = await _servicePackService.GetByNameAsync(name);
            return Ok(new ApiSuccessResult<List<ServicePackByMonthDto>>(result));
        }

        [HttpGet]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _servicePackService.GetByIdAsync(id);
            return Ok(new ApiSuccessResult<ServicePackByMonthDto>(result));
        }

    }
}
