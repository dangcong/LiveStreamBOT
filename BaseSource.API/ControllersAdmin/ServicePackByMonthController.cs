using BaseSouce.Services.ServicePackage;
using BaseSouce.Services.Services.ServicePackageByMonth;
using BaseSouce.ViewModels.Common;
using BaseSource.ViewModels.ServicePackage;
using Microsoft.AspNetCore.Mvc;

namespace BaseSource.API.ControllersAdmin
{
     [Route("api/admin/[controller]")]
    public class ServicePackByMonthController : BaseAdminApiController
    {
        private readonly IServicePackByMonthService _servicePackByMonthService;
        public ServicePackByMonthController(IServicePackByMonthService servicePackByMonthService)
        {
            _servicePackByMonthService = servicePackByMonthService;
        }

        [HttpGet]
        [Route("package/{id:int:min(1)}")]
        public async Task<IActionResult> GetServicePackByType(int id)
        {
            var result = await _servicePackByMonthService.GetByServiceAsync(id);
            return Ok(new ApiSuccessResult<List<ServicePackByMonthDto>>(result));
        }

        [HttpGet]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> GetServicePackById(int id)
        {
            var result = await _servicePackByMonthService.GetByIdAsync(id);
            return Ok(new ApiSuccessResult<ServicePackByMonthDto>(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServicePackByMonthCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }

            var result = await _servicePackByMonthService.CreateAsync(model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            // AddErrors(result.Value, string.Empty);
            return BadRequest(new ApiErrorResult<string>(result.Value));
        }

        [HttpPut]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ServicePackByMonthUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResult<string>(ModelState.GetListErrors()));
            }
            var result = await _servicePackByMonthService.UpdateAsync(id, model);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            //AddErrors(result.Value, string.Empty);
            return BadRequest(new ApiErrorResult<string>(result.Value));
        }

        [HttpDelete]
        [Route("{id:int:min(1)}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var result = await _servicePackByMonthService.DeleteAsync(id);
            if (result.Key)
                return Ok(new ApiSuccessResult<string>(string.Empty));

            // AddErrors(result.Value, string.Empty);
            return BadRequest(new ApiErrorResult<string>(result.Value));
        }
    }
}
